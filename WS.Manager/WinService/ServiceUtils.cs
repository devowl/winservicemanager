using System;
using System.Runtime.InteropServices;
using System.ServiceProcess;

using WS.Manager.WinService.WinApi;

using static WS.Manager.WinService.WinApi.NativeMethods;

namespace WS.Manager.WinService
{
    /// <summary>
    /// Service work utilities.
    /// </summary>
    public static class ServiceUtils
    {
        /// <summary>
        /// Convert string to command line arguments.
        /// </summary>
        /// <param name="commandLine">Arguments string.</param>
        /// <returns>Arguments array.</returns>
        public static string[] CommandLineToArgs(string commandLine)
        {
            int argc;
            var argv = CommandLineToArgvW(commandLine, out argc);
            if (argv == IntPtr.Zero)
            {
                throw new System.ComponentModel.Win32Exception();
            }
            try
            {
                var args = new string[argc];
                for (var i = 0; i < args.Length; i++)
                {
                    var p = Marshal.ReadIntPtr(argv, i * IntPtr.Size);
                    args[i] = Marshal.PtrToStringUni(p);
                }

                return args;
            }
            finally
            {
                Marshal.FreeHGlobal(argv);
            }
        }

        /// <summary>
        /// Конвертация <see cref="ServiceStartMode"/> to <see cref="ServiceBootFlag"/>.
        /// </summary>
        /// <param name="startMode"><see cref="ServiceStartMode"/> mode.</param>
        /// <returns><see cref="ServiceBootFlag"/> flag.</returns>
        public static ServiceBootFlag ToServiceBootFlag(this ServiceStartMode startMode)
        {
            switch (startMode)
            {
                case ServiceStartMode.Manual:
                    return ServiceBootFlag.DemandStart;
                case ServiceStartMode.Automatic:
                    return ServiceBootFlag.AutoStart;
                case ServiceStartMode.Disabled:
                    return ServiceBootFlag.Disabled;
                default:
                    throw new ArgumentOutOfRangeException(nameof(startMode), startMode, null);
            }
        }
    }
}