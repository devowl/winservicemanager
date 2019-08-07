using System.Runtime.InteropServices;

namespace WS.Manager.WinService.WinApi
{
    [StructLayout(LayoutKind.Sequential)]
    public sealed class ServiceStatusProcess
    {
        [MarshalAs(UnmanagedType.U4)] public uint dwServiceType;

        [MarshalAs(UnmanagedType.U4)] public uint dwCurrentState;

        [MarshalAs(UnmanagedType.U4)] public uint dwControlsAccepted;

        [MarshalAs(UnmanagedType.U4)] public uint dwWin32ExitCode;

        [MarshalAs(UnmanagedType.U4)] public uint dwServiceSpecificExitCode;

        [MarshalAs(UnmanagedType.U4)] public uint dwCheckPoint;

        [MarshalAs(UnmanagedType.U4)] public uint dwWaitHint;

        [MarshalAs(UnmanagedType.U4)] public uint dwProcessId;

        [MarshalAs(UnmanagedType.U4)] public uint dwServiceFlags;
    }
}