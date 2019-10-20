// (c) Copyright Jacob Johnston.
// This source is subject to Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Threading;
using NAudio.Wave;
using WPFSoundVisualizationLib;
using NAudio.Dsp;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using NAudio.CoreAudioApi;

namespace YoYoStudio.Client.Chat
{
    class RealTimePlayback : INotifyPropertyChanged, ISpectrumPlayer
    {
       private WasapiLoopbackCapture _capture;
       private NAudio.CoreAudioApi.MMDevice audioDev;
        private object _lock;
        private int _fftPos;
        private int _fftLength;
        private Complex[] _fftBuffer;
        private float[] _lastFftBuffer;
        private bool _fftBufferAvailable;
        private int _m;
        DispatcherTimer dataTimer;
        private float volumePercent = 1.0f;

        public RealTimePlayback()
        {
            this._lock = new object();

            this._capture = new WasapiLoopbackCapture();
            this._capture.DataAvailable += this.DataAvailable;
            initAudioDev();
            this._m = (int)Math.Log(this._fftLength, 2.0);
            this._fftLength = 1024; // 44.1kHz.
            this._fftBuffer = new Complex[this._fftLength];
            this._lastFftBuffer = new float[this._fftLength];
        }

        public WaveFormat Format
        {
            get
            {
                return this._capture.WaveFormat;
            }
        }

        

        private void initAudioDev()
        { 
                NAudio.CoreAudioApi.MMDeviceEnumerator MMDE = new NAudio.CoreAudioApi.MMDeviceEnumerator();
                //Get all the devices, no matter what condition or status
                NAudio.CoreAudioApi.MMDeviceCollection DevCol = MMDE.EnumerateAudioEndPoints(NAudio.CoreAudioApi.DataFlow.All, NAudio.CoreAudioApi.DeviceState.Active);
                //Loop through all devices
                foreach (NAudio.CoreAudioApi.MMDevice dev in DevCol)
                {
                    try
                    {
                        if (dev.FriendlyName.Contains("Headphone") || dev.FriendlyName.Contains("Speakers"))
                        {
                            //Get its audio volume
                            System.Diagnostics.Debug.Print("Volume of " + dev.FriendlyName + " is " + dev.AudioEndpointVolume.MasterVolumeLevel.ToString());
                            audioDev = dev;
                            return;
                        }
                    }
                    catch (Exception ex)
                    {
                        //Do something with exception when an audio endpoint could not be muted
                        System.Diagnostics.Debug.Print(dev.FriendlyName + " could not be muted" + ex.Message);
                    }
                }
        }

        private float[] ConvertByteToFloat(byte[] array, int length)
        {
            if (audioDev != null)
            {
                if (audioDev.AudioEndpointVolume.Mute)
                    volumePercent = 0;
                else
                    volumePercent = (audioDev.AudioEndpointVolume.MasterVolumeLevel - audioDev.AudioEndpointVolume.VolumeRange.MinDecibels) / (audioDev.AudioEndpointVolume.VolumeRange.MaxDecibels - audioDev.AudioEndpointVolume.VolumeRange.MinDecibels);
            }
            int samplesNeeded = length / 16;
            float[] floatArr = new float[samplesNeeded];

            for (int i = 0; i < samplesNeeded; i++)
            {
                floatArr[i] = BitConverter.ToSingle(array, i * 16) * volumePercent;
            }

            return floatArr;
        }

        private void DataAvailable(object sender, WaveInEventArgs e)
        {
            Task.Factory.StartNew(() =>
            {
                try
                {
                    // Convert byte[] to float[].
                    float[] data = ConvertByteToFloat(e.Buffer, e.BytesRecorded);

                    //System.Diagnostics.Debug.Print("Volume of " + audioDev.FriendlyName + " is " + vol.ToString());
                    // For all data. Skip right channel on stereo (i += this.Format.Channels).
                    for (int i = 0; i < data.Length; i += this.Format.Channels)
                    {
                        if (_fftPos >= _fftBuffer.Length)
                            break;
                        this._fftBuffer[_fftPos].X = (float)(data[i] * FastFourierTransform.HannWindow(_fftPos, _fftLength));
                        this._fftBuffer[_fftPos].Y = 0;
                        this._fftPos++;

                        if (this._fftPos >= this._fftLength)
                        {
                            this._fftPos = 0;

                            // NAudio FFT implementation.
                            FastFourierTransform.FFT(true, this._m, this._fftBuffer);

                            // Copy to buffer.
                            lock (this._lock)
                            {
                                for (int c = 0; c < this._fftLength; c++)
                                {
                                    float amplitude = (float)Math.Sqrt(this._fftBuffer[c].X * this._fftBuffer[c].X + this._fftBuffer[c].Y * this._fftBuffer[c].Y);
                                    this._lastFftBuffer[c] = amplitude;
                                }

                                this._fftBufferAvailable = true;
                            }
                        }
                    }
                }
                catch
                {
                }
            });
        }

        public void Start()
        {
            try
            {
                this._capture.StartRecording();
            }
            catch(Exception ex)
            {

            }
        }

        public void Stop()
        {
            try
            {
                this._capture.StopRecording();
            }
            catch(Exception ex)
            {

            }
        }

        public bool GetFFTData(float[] fftDataBuffer)
        {
            lock (this._lock)
            {
                // Use last available buffer.
                if (this._fftBufferAvailable)
                {
                    this._lastFftBuffer.CopyTo(fftDataBuffer, 0);
                    this._fftBufferAvailable = false;
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        public int GetFFTFrequencyIndex(int frequency)
        {
            int index = (int)(frequency / (this.Format.SampleRate / this._fftLength / this.Format.Channels));
            return index;
        }

        public bool IsPlaying
        {
            get { return true; }
        }

        [DllImport("winmm.dll", EntryPoint = "waveOutSetVolume")]
        public static extern int WaveOutGetVolume(IntPtr hwo, uint dwVolume);

        private float GetVol()
        {

            try
            {
                //Instantiate an Enumerator to find audio devices
                NAudio.CoreAudioApi.MMDeviceEnumerator MMDE = new NAudio.CoreAudioApi.MMDeviceEnumerator();
                //Get all the devices, no matter what condition or status
                NAudio.CoreAudioApi.MMDeviceCollection DevCol = MMDE.EnumerateAudioEndPoints(NAudio.CoreAudioApi.DataFlow.All, NAudio.CoreAudioApi.DeviceState.Active);
                //Loop through all devices
                foreach (NAudio.CoreAudioApi.MMDevice dev in DevCol)
                {
                    try
                    {
                        if (dev.FriendlyName.Contains("Headphone") || dev.FriendlyName.Contains("Speakers"))
                        {
                            //Get its audio volume
                            System.Diagnostics.Debug.Print("Volume of " + dev.FriendlyName + " is " + dev.AudioEndpointVolume.MasterVolumeLevel.ToString());
                            return (dev.AudioEndpointVolume.MasterVolumeLevel - dev.AudioEndpointVolume.VolumeRange.MinDecibels) / (dev.AudioEndpointVolume.VolumeRange.MaxDecibels - dev.AudioEndpointVolume.VolumeRange.MinDecibels);
                        }

                        //Mute it
                        //dev.AudioEndpointVolume.Mute = true;
                        //System.Diagnostics.Debug.Print(dev.FriendlyName + " is muted");

                        //Get its audio volume
                        //System.Diagnostics.Debug.Print(dev.AudioEndpointVolume.MasterVolumeLevel.ToString());
                    }
                    catch (Exception ex)
                    {
                        //Do something with exception when an audio endpoint could not be muted
                        System.Diagnostics.Debug.Print(dev.FriendlyName + " could not be muted");
                    }
                }
            }
            catch (Exception ex)
            {
                //When something happend that prevent us to iterate through the devices
                System.Diagnostics.Debug.Print("Could not enumerate devices due to an excepion: " + ex.Message);
            }
            return 1.0f;
        }

        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
    }
}
