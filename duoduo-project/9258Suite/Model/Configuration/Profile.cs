using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;

namespace YoYoStudio.Model.Configuration
{
    [Serializable]
	public class Profile
	{
		public List<LoginEntry> LoginEntries { get; set; }
		public LoginEntry LastLoginEntry { get; set; }
		public bool AutoLogin { get; set; }
        public AudioConfiguration AudioConfiguration { get; set; }
        public VideoConfiguration VideoConfiguration { get; set; }
        public PersonalInfoConfiguration PersonalInfoConfiguration { get; set; }
        public PhotoSelectorConfiguration PhotoSelectorConfiguration { get; set; }
        public SecurityConfiguration SecurityConfiguration { get; set; }
	}
    [Serializable]
    public abstract class Config
    {
        public abstract string Name { get; }
    }
    [Serializable]
	public class LoginEntry : Config
	{
        public LoginEntry()
        {
            UserId = string.Empty;
            Password = string.Empty;
            Remember = false;
            Icon = Path.Combine(Environment.CurrentDirectory, @"Images\Logo128.png");
        }
		public string UserId { get; set; }
		public string Password { get; set; }
        public bool Remember { get; set; }
        public string Icon { get; set; }

        public override string Name
        {
            get { return "LoginEntry"; }
        }
    }
    
    [Serializable]
    public class AudioConfiguration : Config
    {
        public int SoundVolume { get; set; }
        public int MicrophoneVolume { get; set; }
        public bool LoopbackRecording { get; set; }
        public string MicNameLabel { get; set; }
        public int MicDeviceIndex { get; set; }
        public string MicDeviceName { get; set; }
        public ObservableCollection<string> MicDevices { get; set; }
        public string AudioNameLabel { get; set; }
        public int AudioDeviceIndex { get; set; }
        public string AudioDeviceName { get; set; }
        public ObservableCollection<string> AudioDevices { get; set; }
        public string AudioRTMP { get; set; }

        public override string Name
        {
            get { return "AudioConfiguration"; }
        }

        public AudioConfiguration()
        {
            MicNameLabel = "麦克风设备";
            AudioNameLabel = "音频设备";
        }
    }
    [Serializable]
    public class VideoConfiguration : Config
    {
        public int CameraIndex { get; set; }
        public bool Mirror { get; set; }

        public override string Name
        {
            get { return "VideoConfiguration"; }
        }
    }
    
    public class PersonalInfoConfiguration : Config
    {
        public override string Name
        {
            get { return "PersonalInfoConfiguration"; }
        }
    }

    public class PhotoSelectorConfiguration : Config
    {
        public override string Name
        {
            get { return "PhotoSelectorConfiguration"; }
        }
    }

    public class SecurityConfiguration : Config
    {
        public string OldPassword { get; set; }
        public string NewPasswrod { get; set; }
        public string ConfirmPassword { get; set; }
        public bool AutoLogin { get; set; }

        public override string Name
        {
            get { return "SecurityConfiguration"; }
        }
    }
}
