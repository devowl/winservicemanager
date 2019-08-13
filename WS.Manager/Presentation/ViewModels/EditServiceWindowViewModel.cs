using WS.Manager.Presentation.Controls.ViewModels;
using WS.Manager.Presentation.Models;
using WS.Manager.Presentation.Windows;
using WS.Manager.Prism;
using WS.Manager.WinService;

namespace WS.Manager.Presentation.ViewModels
{
    /// <summary>
    /// View model for <see cref="EditServiceWindow"/>.
    /// </summary>
    public class EditServiceWindowViewModel : NotificationObject
    {
        private readonly ServiceModel _serviceModel;
        
        private bool? _dialogResult;

        private string _description;

        private string _displayName;

        private readonly string _name; 

        /// <summary>
        /// Constructor for <see cref="EditServiceWindowViewModel"/>.
        /// </summary>
        public EditServiceWindowViewModel(ServiceModel serviceModel)
        {
            _serviceModel = serviceModel;
            _description = serviceModel.Description;
            _displayName = serviceModel.DisplayName;
            _name = serviceModel.ServiceName;

            ServiceUser = new ServiceUserDetailsViewModel(serviceModel.UserName);
            ServiceFilePath = new ServiceFilePathViewModel(serviceModel.BinaryPathName);
            ServiceStartupType = new ServiceStartupTypeViewModel(serviceModel.StartupType);
            ChangeCommand = new DelegateCommand(ChangePressed);
            CancelCommand = new DelegateCommand(CancelPressed);
        }

        /// <summary>
        /// Service description.
        /// </summary>
        public string Description
        {
            get => _description;

            set
            {
                _description = value;
                RaisePropertyChanged(() => Description);
            }
        }

        /// <summary>
        /// Service name.
        /// </summary>
        public string Name
        {
            get => _name;
        }

        /// <summary>
        /// Service display name.
        /// </summary>
        public string DisplayName
        {
            get => _displayName;

            set
            {
                _displayName = value;
                RaisePropertyChanged(() => DisplayName);
            }
        }

        /// <summary>
        /// Service user view model.
        /// </summary>
        public ServiceUserDetailsViewModel ServiceUser { get; }

        /// <summary>
        /// Service file path view model. 
        /// </summary>
        public ServiceFilePathViewModel ServiceFilePath { get; }

        /// <summary>
        /// Service startup type view model. 
        /// </summary>
        public ServiceStartupTypeViewModel ServiceStartupType { get; }
        
        /// <summary>
        /// Window dialog result.
        /// </summary>
        public bool? DialogResult
        {
            get => _dialogResult;

            set
            {
                _dialogResult = value;
                RaisePropertyChanged(() => DialogResult);
            }
        }

        /// <summary>
        /// Change command.
        /// </summary>
        public DelegateCommand ChangeCommand { get; }

        /// <summary>
        /// Cancel command.
        /// </summary>
        public DelegateCommand CancelCommand { get; }

        private void CancelPressed(object obj)
        {
            DialogResult = false;
        }

        private void ChangePressed(object obj)
        {
            WinServiceUtils.ChangeServiceDescription(_serviceModel.ServiceName, Description);

            var password = ServiceUser.PasswordChanged ? ServiceUser.UserPassword : null;
            WinServiceUtils.ChangeServiceProperties(
                _serviceModel.ServiceName,
                DisplayName,
                ServiceStartupType.StartMode.ToServiceBootFlag(),
                ServiceFilePath.ServiceFilePath,
                ServiceUser.UserName,
                password);

            DialogResult = true;
        }
    }
}