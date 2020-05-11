namespace YoYoStudio.Client.ViewModel
{
    public enum HallWindowAction
    {
        
		RoomNotExist,
        AlreadyInRoom,
		EnterRoomSucceeded,
		EnterRoomFailed,
        OpenConfigurationWindow,
        SwitchUser,
        
        CloseRoomWindow,
        ApplicationShutdown,
		TestData,
        OpenAgentPortal
    }

    public enum WebWindowAction
    {
        InitHall,
        InitHallWithLogin,
        HallPageLoaded,
        InitRoom,
        RoomPageLoaded
    }

	public enum NoAction
	{
		None
	}

    public enum RoomWindowAction
    {
        ShowConfigWindow,
        RecordAudio,
        PlayMusic,
        ManageMusic,
        PublishWarning,
        PublishExit,
        PlayError,
        PlayExit
    }

    public enum PlayMusicWindowAction
    { 
        PlayMusic,
        LoadMusicComplete
    }

    public enum AudioWindowAction
    {
        
    }

    public enum ManageMusicWindowAction
    {
        UploadMusic,
        DeleteMusic,
        LoadMusicComplete
    }

    public enum AgentPortalWindowAction
    { }

	public enum ConfigurationWindowAction
	{
        ConfigurationStateChanged,
        CameraIndexChanged,
        LocalPhotoSelect,
        CameraPhotoSelect,
        EndScreenCapture,
        StartScreenCapture,
        PasswordInvalid,
        VideoRefresh
	}

    public enum RegisterWindowAction
    {
    }

    public enum VideoWindowAction
    {
    }

    public enum LoginWindowAction
    {
        Register,
        RegisterSuccess,
        RegisterCancel,
        RegisterUserIdNotAvailable,
        LoginSuccess,
        InvalidToken,
        UserBlocked,
        CacheLoaded
    }

    public enum CameraWindowAction
    { 
        TakePicture,
        Save
    }
}