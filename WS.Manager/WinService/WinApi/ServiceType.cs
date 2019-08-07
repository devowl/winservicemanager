namespace WS.Manager.WinService.WinApi
{
    public enum ServiceType
    {
        NoChanges = -1,

        SERVICE_FILE_SYSTEM_DRIVER = 0x00000002,

        SERVICE_KERNEL_DRIVER = 0x00000001,

        SERVICE_WIN32_OWN_PROCESS = 0x00000010,

        SERVICE_WIN32_SHARE_PROCESS = 0x00000020
    }
}