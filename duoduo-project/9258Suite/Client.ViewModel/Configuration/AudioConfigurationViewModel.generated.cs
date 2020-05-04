




/// <copyright>
/// Copyright ©  2012 Unisys Corporation. All rights reserved. UNISYS CONFIDENTIAL
/// </copyright>

using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using YoYoStudio.Common.ObjectHistory;
using YoYoStudio.Common.Extensions;
using YoYoStudio.Common.Wpf.ViewModel;
using YoYoStudio.Model.Core;
using YoYoStudio.Model.Chat;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using YoYoStudio.Model.Media;

namespace YoYoStudio.Client.ViewModel
{
	public partial class AudioConfigurationViewModel
	{
	

		/// <summary>
		/// Field which backs the SoundVolume property
		/// </summary>
		private HistoryableProperty<int> soundVolume = new HistoryableProperty<int>(0);

		/// <summary>
		/// Gets / sets the SoundVolume value
		/// </summary>
		[Browsable(false)]
		public  int SoundVolume
		{
			get { return soundVolume.GetValue(); }
			set { ChangeAndNotifyHistory<int>(soundVolume, value, () => SoundVolume ); }
		}


		/// <summary>
		/// Field which backs the LoopbackRecording property
		/// </summary>
		private HistoryableProperty<bool> loopbackRecording = new HistoryableProperty<bool>(false);

		/// <summary>
		/// Gets / sets the LoopbackRecording value
		/// </summary>
		[Browsable(false)]
		public  bool LoopbackRecording
		{
			get { return loopbackRecording.GetValue(); }
			set { ChangeAndNotifyHistory<bool>(loopbackRecording, value, () => LoopbackRecording ); }
		}


		/// <summary>
		/// Field which backs the MicrophoneVolume property
		/// </summary>
		private HistoryableProperty<int> microphoneVolume = new HistoryableProperty<int>(0);

		/// <summary>
		/// Gets / sets the MicrophoneVolume value
		/// </summary>
		[Browsable(false)]
		public  int MicrophoneVolume
		{
			get { return microphoneVolume.GetValue(); }
			set { ChangeAndNotifyHistory<int>(microphoneVolume, value, () => MicrophoneVolume ); }
		}


		/// <summary>
		/// Field which backs the MicNameLabel property
		/// </summary>
		private HistoryableProperty<string> micNameLabel = new HistoryableProperty<string>("");

		/// <summary>
		/// Gets / sets the MicNameLabel value
		/// </summary>
		[Browsable(false)]
		public  string MicNameLabel
		{
			get { return micNameLabel.GetValue(); }
			set { ChangeAndNotifyHistory<string>(micNameLabel, value, () => MicNameLabel ); }
		}


		/// <summary>
		/// Field which backs the MicDevices property
		/// </summary>
		private HistoryableProperty<ObservableCollection<string>> micDevices = new HistoryableProperty<ObservableCollection<string>>(new ObservableCollection<string>());

		/// <summary>
		/// Gets / sets the MicDevices value
		/// </summary>
		[Browsable(false)]
		public  ObservableCollection<string> MicDevices
		{
			get { return micDevices.GetValue(); }
			set { ChangeAndNotifyHistory<ObservableCollection<string>>(micDevices, value, () => MicDevices ); }
		}


		/// <summary>
		/// Field which backs the MicDeviceIndex property
		/// </summary>
		private HistoryableProperty<int> micDeviceIndex = new HistoryableProperty<int>(0);

		/// <summary>
		/// Gets / sets the MicDeviceIndex value
		/// </summary>
		[Browsable(false)]
		public  int MicDeviceIndex
		{
			get { return micDeviceIndex.GetValue(); }
			set { ChangeAndNotifyHistory<int>(micDeviceIndex, value, () => MicDeviceIndex ); }
		}


		/// <summary>
		/// Field which backs the MicDeviceName property
		/// </summary>
		private HistoryableProperty<string> micDeviceName = new HistoryableProperty<string>("");

		/// <summary>
		/// Gets / sets the MicDeviceName value
		/// </summary>
		[Browsable(false)]
		public  string MicDeviceName
		{
			get { return micDeviceName.GetValue(); }
			set { ChangeAndNotifyHistory<string>(micDeviceName, value, () => MicDeviceName ); }
		}


		/// <summary>
		/// Field which backs the AudioNameLabel property
		/// </summary>
		private HistoryableProperty<string> audioNameLabel = new HistoryableProperty<string>("");

		/// <summary>
		/// Gets / sets the AudioNameLabel value
		/// </summary>
		[Browsable(false)]
		public  string AudioNameLabel
		{
			get { return audioNameLabel.GetValue(); }
			set { ChangeAndNotifyHistory<string>(audioNameLabel, value, () => AudioNameLabel ); }
		}


		/// <summary>
		/// Field which backs the AudioDevices property
		/// </summary>
		private HistoryableProperty<ObservableCollection<string>> audioDevices = new HistoryableProperty<ObservableCollection<string>>(new ObservableCollection<string>());

		/// <summary>
		/// Gets / sets the AudioDevices value
		/// </summary>
		[Browsable(false)]
		public  ObservableCollection<string> AudioDevices
		{
			get { return audioDevices.GetValue(); }
			set { ChangeAndNotifyHistory<ObservableCollection<string>>(audioDevices, value, () => AudioDevices ); }
		}


		/// <summary>
		/// Field which backs the AudioDeviceIndex property
		/// </summary>
		private HistoryableProperty<int> audioDeviceIndex = new HistoryableProperty<int>(0);

		/// <summary>
		/// Gets / sets the AudioDeviceIndex value
		/// </summary>
		[Browsable(false)]
		public  int AudioDeviceIndex
		{
			get { return audioDeviceIndex.GetValue(); }
			set { ChangeAndNotifyHistory<int>(audioDeviceIndex, value, () => AudioDeviceIndex ); }
		}


		/// <summary>
		/// Field which backs the AudioDeviceName property
		/// </summary>
		private HistoryableProperty<string> audioDeviceName = new HistoryableProperty<string>("");

		/// <summary>
		/// Gets / sets the AudioDeviceName value
		/// </summary>
		[Browsable(false)]
		public  string AudioDeviceName
		{
			get { return audioDeviceName.GetValue(); }
			set { ChangeAndNotifyHistory<string>(audioDeviceName, value, () => AudioDeviceName ); }
		}


		/// <summary>
		/// Field which backs the AudioSampleLabel property
		/// </summary>
		private HistoryableProperty<string> audioSampleLabel = new HistoryableProperty<string>("");

		/// <summary>
		/// Gets / sets the AudioSampleLabel value
		/// </summary>
		[Browsable(false)]
		public  string AudioSampleLabel
		{
			get { return audioSampleLabel.GetValue(); }
			set { ChangeAndNotifyHistory<string>(audioSampleLabel, value, () => AudioSampleLabel ); }
		}


		/// <summary>
		/// Field which backs the AudioSamples property
		/// </summary>
		private HistoryableProperty<ObservableCollection<string>> audioSamples = new HistoryableProperty<ObservableCollection<string>>(new ObservableCollection<string>());

		/// <summary>
		/// Gets / sets the AudioSamples value
		/// </summary>
		[Browsable(false)]
		public  ObservableCollection<string> AudioSamples
		{
			get { return audioSamples.GetValue(); }
			set { ChangeAndNotifyHistory<ObservableCollection<string>>(audioSamples, value, () => AudioSamples ); }
		}


		/// <summary>
		/// Field which backs the AudioSampleIndex property
		/// </summary>
		private HistoryableProperty<int> audioSampleIndex = new HistoryableProperty<int>(0);

		/// <summary>
		/// Gets / sets the AudioSampleIndex value
		/// </summary>
		[Browsable(false)]
		public  int AudioSampleIndex
		{
			get { return audioSampleIndex.GetValue(); }
			set { ChangeAndNotifyHistory<int>(audioSampleIndex, value, () => AudioSampleIndex ); }
		}


		/// <summary>
		/// Field which backs the AudioSample property
		/// </summary>
		private HistoryableProperty<string> audioSample = new HistoryableProperty<string>("");

		/// <summary>
		/// Gets / sets the AudioSample value
		/// </summary>
		[Browsable(false)]
		public  string AudioSample
		{
			get { return audioSample.GetValue(); }
			set { ChangeAndNotifyHistory<string>(audioSample, value, () => AudioSample ); }
		}


		/// <summary>
		/// Field which backs the AudioSync property
		/// </summary>
		private HistoryableProperty<bool> audioSync = new HistoryableProperty<bool>(false);

		/// <summary>
		/// Gets / sets the AudioSync value
		/// </summary>
		[Browsable(false)]
		public  bool AudioSync
		{
			get { return audioSync.GetValue(); }
			set { ChangeAndNotifyHistory<bool>(audioSync, value, () => AudioSync ); }
		}
	}
}
	