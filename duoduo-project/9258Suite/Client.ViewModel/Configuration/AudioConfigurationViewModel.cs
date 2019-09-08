using Snippets;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Linq;
using System.Text;
using YoYoStudio.Model.Configuration;
using YoYoStudio.Resource;

namespace YoYoStudio.Client.ViewModel
{
    [SnippetPropertyINPC(field = "soundVolume", property = "SoundVolume", type = "int", defaultValue = "0")]
    [SnippetPropertyINPC(field = "loopbackRecording", property = "LoopbackRecording", type = "bool", defaultValue = "false")]
    [SnippetPropertyINPC(field = "microphoneVolume", property = "MicrophoneVolume", type = "int", defaultValue = "0")]
    [SnippetPropertyINPC(field = "micNameLabel", property = "MicNameLabel", type = "string", defaultValue = "")]
    [SnippetPropertyINPC(field = "micDevices", property = "MicDevices", type = "ObservableCollection<string>", defaultValue = "new ObservableCollection<string>()")]
    [SnippetPropertyINPC(field = "micDeviceIndex", property = "MicDeviceIndex", type = "int", defaultValue = "0")]
    [SnippetPropertyINPC(field = "micDeviceName", property = "MicDeviceName", type = "string", defaultValue = "")]
    [SnippetPropertyINPC(field = "audioNameLabel", property = "AudioNameLabel", type = "string", defaultValue = "")]
    [SnippetPropertyINPC(field = "audioDevices", property = "AudioDevices", type = "ObservableCollection<string>", defaultValue = "new ObservableCollection<string>()")]
    [SnippetPropertyINPC(field = "audioDeviceIndex", property = "AudioDeviceIndex", type = "int", defaultValue = "0")]
    [SnippetPropertyINPC(field = "audioDeviceName", property = "AudioDeviceName", type = "string", defaultValue = "")]
    [SnippetPropertyINPC(field = "audioRTMP", property = "AudioRTMP", type = "string", defaultValue = "")]
    public partial class AudioConfigurationViewModel : ConfigurationViewModel
    {
        public AudioConfigurationViewModel(AudioConfiguration config)
            : base(config)
        {
            LoopbackRecording = config.LoopbackRecording;
            MicrophoneVolume = config.MicrophoneVolume;
            SoundVolume = config.SoundVolume;
            MicNameLabel = config.MicNameLabel;
            AudioNameLabel = config.AudioNameLabel;
        }

        public override void Save(out bool result)
        {
            result = true;
            if (Dirty)
            {
                var config = GetConcreteConfiguration<AudioConfiguration>();
                config.SoundVolume = SoundVolume;
                config.MicrophoneVolume = MicrophoneVolume;
                config.LoopbackRecording = LoopbackRecording;
                config.AudioDeviceIndex = AudioDeviceIndex;
                config.AudioDevices = AudioDevices;
                config.AudioDeviceName = AudioDeviceName;
                config.MicDevices = MicDevices;
                config.MicDeviceIndex = MicDeviceIndex;
                config.MicDeviceName = MicDeviceName;
                config.AudioRTMP = AudioRTMP;
                base.Save(out result);
            }
        }

        public override void Reset()
        {
            var config = GetConcreteConfiguration<AudioConfiguration>();
            loopbackRecording.SetValue(config.LoopbackRecording);
            soundVolume.SetValue(config.SoundVolume);
            microphoneVolume.SetValue(config.MicrophoneVolume);
            base.Reset();
        }

        protected override void InitializeResource()
        {
            title = Text.AudioConfiguration;
            MicDevices = new ObservableCollection<string>();
            NAudio.CoreAudioApi.MMDeviceEnumerator MMDE = new NAudio.CoreAudioApi.MMDeviceEnumerator();
            NAudio.CoreAudioApi.MMDeviceCollection capDevCol = MMDE.EnumerateAudioEndPoints(NAudio.CoreAudioApi.DataFlow.Capture, NAudio.CoreAudioApi.DeviceState.Active);
            foreach (var item in capDevCol)
            {
                MicDevices.Add(item.FriendlyName);
            }
            MicDeviceName = MicDevices.Count > 0 ? MicDevices[0] : "Microphone Array (Realtek High Definition Audio)";
            AudioDevices = new ObservableCollection<string>() { "virtual-audio-capturer" };
            NAudio.CoreAudioApi.MMDeviceCollection renderDevCol = MMDE.EnumerateAudioEndPoints(NAudio.CoreAudioApi.DataFlow.Render, NAudio.CoreAudioApi.DeviceState.Active);
            foreach (var item in renderDevCol)
            {
                AudioDevices.Add(item.FriendlyName);
            }
            AudioDeviceName = AudioDevices[0];
            AudioRTMP = ConfigurationManager.AppSettings["AudioRtmpPath"] + "/" + this.ApplicationVM.RoomWindowVM.RoomVM.Id;
            base.InitializeResource();
        }
    }
}
