using System;
using System.Runtime.InteropServices;
using System.Security.AccessControl;

namespace WS.Manager.WinService.WinApi
{
    /// <summary>
    /// Windows native methods.
    /// </summary>
    internal class NativeMethods
    {
        [DllImport("advapi32.dll", EntryPoint = "OpenSCManagerW", ExactSpelling = true, CharSet = CharSet.Unicode,
            SetLastError = true)]
        public static extern IntPtr OpenSCManager(
            string machineName,
            string databaseName,
            ScmAccessRights dwDesiredAccess);

        [DllImport("advapi32.dll", SetLastError = true)]
        public static extern bool QueryServiceStatusEx(
            SafeHandle hService,
            int infoLevel,
            IntPtr lpBuffer,
            uint cbBufSize,
            out uint pcbBytesNeeded);

        [DllImport("kernel32.dll")]
        public static extern int GetLastError();

        [DllImport("advapi32.dll", CharSet = CharSet.Unicode, SetLastError = true, EntryPoint = "QueryServiceConfig2W")]
        public static extern Boolean QueryServiceConfig2(
            IntPtr hService,
            UInt32 dwInfoLevel,
            IntPtr buffer,
            UInt32 cbBufSize,
            out UInt32 pcbBytesNeeded);

        [DllImport("advapi32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool ChangeServiceConfig2(
            IntPtr hService,
            int dwInfoLevel,
            [MarshalAs(UnmanagedType.Struct)] ref ServiceDescriptionStruct lpInfo);

        [DllImport("advapi32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool CloseServiceHandle(IntPtr hScObject);

        [DllImport("advapi32.dll")]
        public static extern int ControlService(
            IntPtr hService,
            ServiceControl dwControl,
            ServiceStatus lpServiceStatus);

        // https://docs.microsoft.com/en-us/windows/desktop/api/winsvc/nf-winsvc-createservicea
        [DllImport("advapi32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern IntPtr CreateService(
            IntPtr hScManager,
            string lpServiceName,
            string lpDisplayName,
            ServiceAccessRights dwDesiredAccess,
            ServiceType dwServiceType,
            ServiceBootFlag dwStartType,
            ServiceError dwErrorControl,
            string lpBinaryPathName,
            string lpLoadOrderGroup,
            IntPtr lpdwTagId,
            string lpDependencies,
            string lpServiceStartName,
            string lpPassword);

        [DllImport("advapi32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool DeleteService(IntPtr hService);

        [DllImport("advapi32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern IntPtr OpenService(
            IntPtr hScManager,
            string lpServiceName,
            ServiceAccessRights dwDesiredAccess);

        [DllImport("advapi32.dll")]
        public static extern int QueryServiceStatus(IntPtr hService, ServiceStatus lpServiceStatus);

        [DllImport("advapi32.dll", SetLastError = true)]
        public static extern int StartService(IntPtr hService, int dwNumServiceArgs, int lpServiceArgVectors);

        [DllImport("advapi32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern bool ChangeServiceConfig(
            IntPtr hService,
            ServiceType nServiceType,
            ServiceBootFlag nStartType,
            ServiceError nErrorControl,
            string lpBinaryPathName,
            string lpLoadOrderGroup,
            int lpdwTagId,
            string lpDependencies,
            string lpServiceStartName,
            string lpPassword,
            string lpDisplayName);

        [DllImport("shell32.dll", SetLastError = true)]
        public static extern IntPtr CommandLineToArgvW(
            [MarshalAs(UnmanagedType.LPWStr)] string lpCmdLine,
            out int pNumArgs);

        [DllImport("advapi32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern Boolean QueryServiceConfig(
            IntPtr hService,
            IntPtr intPtrQueryConfig,
            int cbBufSize,
            out int pcbBytesNeeded);
    }
}