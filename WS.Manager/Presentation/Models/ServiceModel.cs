using System;
using System.Linq;
using System.ServiceProcess;

using WS.Manager.Prism;
using WS.Manager.WinService;
using WS.Manager.WinService.WinApi;

namespace WS.Manager.Presentation.Models
{
    /// <summary>
    /// Service model.
    /// </summary>
    public class ServiceModel : NotificationObject, IDisposable
    {
        private string _description = null;

        private QueryServiceConfig? _serviceConfig;

        /// <summary>
        /// Constructor for <see cref="ServiceModel"/>.
        /// </summary>
        public ServiceModel(ServiceController service)
        {
            Controller = service;
            WinServiceUtils.TryGetServiceConfig(Controller.ServiceName, out _serviceConfig);
        }

        /// <summary>
        /// Service account name.
        /// </summary>
        public string UserName
        {
            get
            {
                return _serviceConfig?.lpServiceStartName;
            }
        }

        /// <summary>
        /// Service controller.
        /// </summary>
        public ServiceController Controller { get; private set; }

        /// <summary>
        /// Service description.
        /// </summary>
        public string Description
        {
            get
            {
                return _description ??
                       (_description = WinServiceUtils.GetServiceDescription(Controller.ServiceName) ?? string.Empty);
            }
        }

        /// <summary>
        /// Binary file path.
        /// </summary>
        public string BinaryPathName
        {
            get
            {
                return _serviceConfig?.lpBinaryPathName;
            }
        }

        /// <summary>
        /// Service startup type.
        /// </summary>
        public ServiceStartMode? StartupType
        {
            get
            {
                return _serviceConfig?.dwStartType;
            }
        }

        /// <summary>
        /// Service status.
        /// </summary>
        public string Status
        {
            get
            {
                if (Controller.Status == ServiceControllerStatus.Stopped)
                {
                    return string.Empty;
                }

                return Controller.Status.ToString();
            }
        }
        
        /// <summary>
        /// Service full name.
        /// </summary>
        public string DisplayName => Controller.DisplayName;

        /// <summary>
        /// Service short name.
        /// </summary>
        public string ServiceName => Controller.ServiceName;
        
        /// <inheritdoc/>
        public void Dispose()
        {
            Controller.Dispose();
        }

        /// <summary>
        /// Refresh service data.
        /// </summary>
        public void Refresh()
        {
            Controller.Refresh();
            RaisePropertiesChanged();
        }

        /// <summary>
        /// Entire record refresh.
        /// </summary>
        public void EntireRefresh()
        {
            // Here we have a bug. If i change Display name Controller doesn't refresh it.
            Controller = ServiceController.GetServices().Single(s => s.ServiceName == Controller.ServiceName);
            WinServiceUtils.TryGetServiceConfig(Controller.ServiceName, out _serviceConfig);
            RaisePropertiesChanged();
        }

        private void RaisePropertiesChanged()
        {
            RaisePropertyChanged(() => ServiceName);
            RaisePropertyChanged(() => DisplayName);
            RaisePropertyChanged(() => Status);
            RaisePropertyChanged(() => StartupType);

            _description = null;
            RaisePropertyChanged(() => Description);
        }
    }
}