using System.ServiceProcess;

using WS.Manager.Presentation.Controls.Views;
using WS.Manager.Prism;

namespace WS.Manager.Presentation.Controls.ViewModels
{
    /// <summary>
    /// View model for <see cref="ServiceStartupType"/>.
    /// </summary>
    public class ServiceStartupTypeViewModel : NotificationObject
    {
        private ServiceStartMode _startMode;

        /// <summary>
        /// Constructor for <see cref="ServiceFilePathViewModel"/>.
        /// </summary>
        public ServiceStartupTypeViewModel(ServiceStartMode? startMode)
        {
            _startMode = startMode ?? ServiceStartMode.Automatic;
            IsEditable = startMode.HasValue;
        }

        /// <summary>
        /// Can edit start mode.
        /// </summary>
        public bool IsEditable { get; }

        /// <summary>
        /// Service executable file path.
        /// </summary>
        public ServiceStartMode StartMode
        {
            get
            {
                return _startMode;
            }

            set
            {
                _startMode = value;
                RaisePropertyChanged(() => StartMode);
            }
        }
    }
}
