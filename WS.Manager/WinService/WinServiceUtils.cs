using System;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.ServiceProcess;
using System.Threading;

using Microsoft.Win32;

using WS.Manager.WinService.WinApi;

using static WS.Manager.WinService.WinApi.NativeMethods;

using ServiceType = WS.Manager.WinService.WinApi.ServiceType;

namespace WS.Manager.WinService
{
    /// <summary>
    /// Windows Service utilities.
    /// </summary>
    public static class WinServiceUtils
    {
        internal const int ERROR_INVALID_NAME = 0x7B;

        internal const int ERROR_SERVICE_DOES_NOT_EXIST = 0X424;

        private const int SERVICE_CONFIG_DESCRIPTION = 0x01;
        
        private const int ERROR_INSUFFICIENT_BUFFER = 0x7a;

        private const int SC_STATUS_PROCESS_INFO = 0;
        
        private const int ERROR_ACCESS_DENIED = 5;

        private const int ERROR_INVALID_HANDLE = 0x6;
        
        /// <summary>
        /// Try get service configuration info.
        /// </summary>
        /// <param name="serviceName">Service name.</param>
        /// <param name="serviceConfig">Service configuration.</param>
        /// <returns></returns>
        public static bool TryGetServiceConfig(string serviceName, out QueryServiceConfig? serviceConfig)
        {
            serviceConfig = null;
            using (
                var serviceObject = GetServiceObject(
                    serviceName,
                    ScmAccessRights.AllAccess,
                    ServiceAccessRights.AllAccess,
                    false))
            {
                int neededBytes = 0;

                QueryServiceConfig(serviceObject.Service, IntPtr.Zero, 0, out neededBytes);
                if (Marshal.GetLastWin32Error() == ERROR_INSUFFICIENT_BUFFER)
                {
                    IntPtr ptr = IntPtr.Zero;
                    try
                    {
                        ptr = Marshal.AllocCoTaskMem(neededBytes);
                        var result = QueryServiceConfig(serviceObject.Service, ptr, neededBytes, out neededBytes);
                        if (result)
                        {
                            serviceConfig = (QueryServiceConfig)Marshal.PtrToStructure(ptr, typeof(QueryServiceConfig));
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }
                    finally
                    {
                        Marshal.FreeCoTaskMem(ptr);
                    }
                }

                return false;
            }
        }

        /// <summary>
        /// Get service description.
        /// </summary>
        /// <param name="serviceName">Service name.</param>
        public static string GetServiceDescription(string serviceName)
        {
            using (
                var serviceObject = GetServiceObject(
                    serviceName,
                    ScmAccessRights.AllAccess,
                    ServiceAccessRights.AllAccess,
                    false))
            {
                var service = serviceObject.Service;
                if (service == IntPtr.Zero)
                {
                    return GetErrorDescription(GetLastError());
                }

                // Determine the buffer size needed
                QueryServiceConfig2(service, SERVICE_CONFIG_DESCRIPTION, IntPtr.Zero, 0, out var dwBytesNeeded);

                var ptr = Marshal.AllocHGlobal((int)dwBytesNeeded);
                QueryServiceConfig2(service, SERVICE_CONFIG_DESCRIPTION, ptr, dwBytesNeeded, out dwBytesNeeded);
                var descriptionStruct = new ServiceDescriptionClass();
                Marshal.PtrToStructure(ptr, descriptionStruct);
                Marshal.FreeHGlobal(ptr);

                return descriptionStruct.lpDescription;
            }
        }

        /// <summary>
        /// Change service properties.
        /// </summary>
        /// <param name="serviceName">Service name.</param>
        /// <param name="displayName">Service display name.</param>
        /// <param name="bootFlag">Тип загрузки.</param>
        /// <param name="binaryPathName">Путь к исполняемому файлу.</param>
        /// <param name="userName">Имя пользователя.</param>
        /// <param name="userPassword">Пароль пользователя.</param>
        public static void ChangeServiceProperties(
            string serviceName,
            string displayName,
            ServiceBootFlag bootFlag,
            string binaryPathName,
            string userName,
            string userPassword)
        {
            using (
                var serviceObject = GetServiceObject(
                    serviceName,
                    ScmAccessRights.AllAccess,
                    ServiceAccessRights.AllAccess))
            {
                ChangeServiceConfig(
                    serviceObject.Service,
                    ServiceType.NoChanges, 
                    bootFlag,
                    ServiceError.NoChanges,
                    binaryPathName,
                    null,
                    0,
                    null,
                    userName,
                    userPassword,
                    displayName);
            }
        }

        /// <summary>
        /// Change service description.
        /// </summary>
        /// <param name="serviceName">Service name.</param>
        /// <param name="description">Service Description.</param>
        public static bool ChangeServiceDescription(string serviceName, string description)
        {
            var scm = GetScManagerPtr(ScmAccessRights.AllAccess);
            var service = OpenService(scm, serviceName, ServiceAccessRights.AllAccess);
            var pinfo = new ServiceDescriptionStruct { lpDescription = description };

            return ChangeServiceConfig2(service, SERVICE_CONFIG_DESCRIPTION, ref pinfo);
        }

        /// <summary>
        /// Get service status.
        /// </summary>
        /// <param name="serviceName">Service name.</param>
        /// <returns>Service status.</returns>
        public static ServiceState GetServiceStatus(string serviceName)
        {
            using (
                var serviceObject = GetServiceObject(
                    serviceName,
                    ScmAccessRights.Connect,
                    ServiceAccessRights.QueryStatus))
            {
                var service = serviceObject.Service;
                if (service == IntPtr.Zero)
                {
                    return ServiceState.NotFound;
                }

                return GetServiceStatus(service);
            }
        }

        /// <summary>
        /// Install service.
        /// </summary>
        /// <param name="serviceName">Service name.</param>
        /// <param name="displayName">Display name.</param>
        /// <param name="bootFlag">Service boot flag value.</param>
        /// <param name="fileName">Executable file path.</param>
        /// <param name="login">Service account login.</param>
        /// <param name="password">Service account password.</param>
        public static void Install(
            string serviceName,
            string displayName,
            ServiceBootFlag bootFlag,
            string fileName,
            string login,
            string password)
        {
            using (
                var serviceObject = GetServiceObject(
                    serviceName,
                    ScmAccessRights.AllAccess,
                    ServiceAccessRights.AllAccess,
                    false))
            {
                var service = serviceObject.Service;
                if (service == IntPtr.Zero)
                {
                    service = CreateService(
                        serviceObject.ScManager,
                        serviceName,
                        displayName,
                        ServiceAccessRights.AllAccess,
                        ServiceType.SERVICE_WIN32_OWN_PROCESS,
                        bootFlag,
                        ServiceError.Normal,
                        fileName,
                        null,
                        IntPtr.Zero,
                        null,
                        login,
                        password);
                }

                if (service == IntPtr.Zero)
                {
                    throw new ApplicationException("Failed to install service.");
                }
            }
        }

        /// <summary>
        /// Check service is installed.
        /// </summary>
        /// <param name="serviceName">Service name.</param>
        /// <returns>Installation flag.</returns>
        public static bool ServiceIsInstalled(string serviceName)
        {
            using (
                var serviceObject = GetServiceObject(
                    serviceName,
                    ScmAccessRights.Connect,
                    ServiceAccessRights.QueryStatus))
            {
                return serviceObject.Service != IntPtr.Zero;
            }
        }

        /// <summary>
        /// Start service.
        /// </summary>
        /// <param name="serviceName">Service name.</param>
        public static void ServiceStart(string serviceName)
        {
            using (
                var serviceObject = GetServiceObject(
                    serviceName,
                    ScmAccessRights.Connect,
                    ServiceAccessRights.QueryStatus | ServiceAccessRights.Start))
            {
                InternalStartService(serviceObject.Service);
            }
        }

        /// <summary>
        /// Stop service.
        /// </summary>
        /// <param name="serviceName">Service name.</param>
        public static void StopService(string serviceName)
        {
            using (
                var serviceObject = GetServiceObject(
                    serviceName,
                    ScmAccessRights.Connect,
                    ServiceAccessRights.QueryStatus | ServiceAccessRights.Stop))
            {
                if (GetServiceStatus(serviceObject.Service) == ServiceState.Running)
                {
                    StopService(serviceObject.Service);
                }
            }
        }

        /// <summary>
        /// Uninstall service.
        /// </summary>
        /// <param name="serviceName">Service name.</param>
        public static void Uninstall(string serviceName)
        {
            using(var serviceObject = GetServiceObject(serviceName, ScmAccessRights.AllAccess, ServiceAccessRights.AllAccess))
            {
                if (GetServiceStatus(serviceObject.Service) == ServiceState.Running)
                {
                    TerminateService(serviceName);
                }

                if (!DeleteService(serviceObject.Service))
                {
                    throw new ApplicationException("Could not delete service " + Marshal.GetLastWin32Error());
                }
            }
        }

        /// <summary>
        /// Terminate service by name.
        /// </summary>
        /// <param name="serviceName">Service name.</param>
        public static void TerminateService(string serviceName)
        {
            var service =
                ServiceController.GetServices()
                    .FirstOrDefault(s => string.Equals(s.ServiceName, serviceName, StringComparison.OrdinalIgnoreCase));

            if (service == null)
            {
                throw new ArgumentNullException($"Service [{serviceName}] not founded");
            }

            var processId = GetServiceProcessId(service);
            if (processId > 0)
            {
                KillProcessById(processId);
            }
        }

        private static ServiceObject GetServiceObject(string serviceName, ScmAccessRights scmRights, ServiceAccessRights serviceRights, bool throwException = true)
        {
            var scm = GetScManagerPtr(scmRights);
            var service = OpenService(scm, serviceName, serviceRights);
            if (service == IntPtr.Zero)
            {
                if (throwException)
                {
                    throw new ApplicationException("Service not installed.");
                }
            }

            return new ServiceObject(scm, service);
        }

        private static string GetErrorDescription(int errorCode)
        {
            switch (errorCode)
            {
                case ERROR_ACCESS_DENIED:
                    return "[Access denied]";
                case ERROR_INVALID_HANDLE:
                    return "[Invalid handle]";
                case ERROR_INVALID_NAME:
                    return "[Invalid name]";
                case ERROR_SERVICE_DOES_NOT_EXIST:
                    return "[Service does not exist]";
                default:
                    return $"[Unknown error: {errorCode}]";
            }
        }

        private static void KillProcessById(int pid)
        {
            foreach (var process in Process.GetProcesses())
            {
                if (process.Id == pid)
                {
                    process.Kill();
                    break;
                }
            }
        }

        private static int GetServiceProcessId(ServiceController sc)
        {
            IntPtr zero = IntPtr.Zero;

            try
            {
                UInt32 dwBytesNeeded;

                // Call once to figure the size of the output buffer.
                QueryServiceStatusEx(sc.ServiceHandle, SC_STATUS_PROCESS_INFO, zero, 0, out dwBytesNeeded);
                if (Marshal.GetLastWin32Error() == ERROR_INSUFFICIENT_BUFFER)
                {
                    // Allocate required buffer and call again.
                    zero = Marshal.AllocHGlobal((int)dwBytesNeeded);

                    if (QueryServiceStatusEx(
                        sc.ServiceHandle,
                        SC_STATUS_PROCESS_INFO,
                        zero,
                        dwBytesNeeded,
                        out dwBytesNeeded))
                    {
                        var ssp = new ServiceStatusProcess();
                        Marshal.PtrToStructure(zero, ssp);
                        return (int)ssp.dwProcessId;
                    }
                }
            }
            finally
            {
                if (zero != IntPtr.Zero)
                {
                    Marshal.FreeHGlobal(zero);
                }
            }
            return -1;
        }

        private static ServiceState GetServiceStatus(IntPtr service)
        {
            ServiceStatus status = new ServiceStatus();

            if (QueryServiceStatus(service, status) == 0)
            {
                throw new ApplicationException("Failed to query service status.");
            }

            return status.dwCurrentState;
        }

        private static IntPtr GetScManagerPtr(ScmAccessRights rights)
        {
            var scm = OpenSCManager(null, null, rights);
            if (scm == IntPtr.Zero)
            {
                throw new ApplicationException("Could not connect to service control manager.");
            }

            return scm;
        }

        private static void InternalStartService(IntPtr service)
        {
            StartService(service, 0, 0);
            WaitForServiceStatus(service, ServiceState.StartPending, ServiceState.Running);
        }

        private static void StopService(IntPtr service)
        {
            ServiceStatus status = new ServiceStatus();
            ControlService(service, ServiceControl.Stop, status);
            WaitForServiceStatus(service, ServiceState.StopPending, ServiceState.Stopped);
        }

        private static string GetServiceRegistryProperty(string serviceName, string propertyName)
        {
            var serviceRegisrty = Registry.LocalMachine.OpenSubKey($@"SYSTEM\CurrentControlSet\Services\{serviceName}");
            if (serviceRegisrty == null)
            {
                return string.Empty;
            }

            using (serviceRegisrty)
            {
                return serviceRegisrty.GetValue(propertyName)?.ToString();
            }
        }

        private static bool WaitForServiceStatus(IntPtr service, ServiceState waitStatus, ServiceState desiredStatus)
        {
            ServiceStatus status = new ServiceStatus();

            QueryServiceStatus(service, status);
            if (status.dwCurrentState == desiredStatus)
            {
                return true;
            }

            int dwStartTickCount = Environment.TickCount;
            int dwOldCheckPoint = status.dwCheckPoint;

            while (status.dwCurrentState == waitStatus)
            {
                // Do not wait longer than the wait hint. A good interval is
                // one tenth the wait hint, but no less than 1 second and no
                // more than 10 seconds.

                int dwWaitTime = status.dwWaitHint / 10;

                if (dwWaitTime < 1000)
                {
                    dwWaitTime = 1000;
                }
                else if (dwWaitTime > 10000)
                {
                    dwWaitTime = 10000;
                }

                Thread.Sleep(dwWaitTime);

                // Check the status again.

                if (QueryServiceStatus(service, status) == 0)
                {
                    break;
                }

                if (status.dwCheckPoint > dwOldCheckPoint)
                {
                    // The service is making progress.
                    dwStartTickCount = Environment.TickCount;
                    dwOldCheckPoint = status.dwCheckPoint;
                }
                else
                {
                    if (Environment.TickCount - dwStartTickCount > status.dwWaitHint)
                    {
                        // No progress made within the wait hint
                        break;
                    }
                }
            }
            return (status.dwCurrentState == desiredStatus);
        }

        private class ServiceObject : IDisposable
        {
            public ServiceObject()
            {
                
            }

            public ServiceObject(IntPtr scManager, IntPtr service)
            {
                ScManager = scManager;
                Service = service;
            }

            /// <summary>
            /// Service pointer.
            /// </summary>
            public IntPtr Service { get; }

            /// <summary>
            /// ScManager pointer.
            /// </summary>
            public IntPtr ScManager { get; } 

            /// <inheritdoc/>
            public void Dispose()
            {
                CloseServiceHandle(Service);
                CloseServiceHandle(ScManager);
            }
        }
    }
}