using System.Runtime.InteropServices;

namespace WS.Manager.WinService.WinApi
{
    [StructLayout(LayoutKind.Sequential)]
    public class ServiceDescriptionClass
    {
        [MarshalAs(UnmanagedType.LPWStr)]
        public string lpDescription;
    }
}