using System;
using System.Runtime.InteropServices;
using System.ServiceProcess;

namespace WS.Manager.WinService.WinApi
{
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    public struct QueryServiceConfig
    {
        [MarshalAs(UnmanagedType.U4)]
        internal ServiceType dwServiceType;
        [MarshalAs(UnmanagedType.U4)]
        internal ServiceStartMode dwStartType;
        internal int dwErrorControl;
        [MarshalAs(UnmanagedType.LPWStr)]
        internal string lpBinaryPathName;
        [MarshalAs(UnmanagedType.LPWStr)]
        internal string lpLoadOrderGroup;
        internal int dwTagId;
        [MarshalAs(UnmanagedType.LPWStr)]
        internal string lpDependencies;
        [MarshalAs(UnmanagedType.LPWStr)]
        internal string lpServiceStartName;
        [MarshalAs(UnmanagedType.LPWStr)]
        internal string lpDisplayName;
    }
}