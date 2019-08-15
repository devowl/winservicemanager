using System.ComponentModel;
using System.Linq;
using System.ServiceProcess;

using WS.Manager.Presentation.Controls.ViewModels;
using WS.Manager.Presentation.Models;
using WS.Manager.Presentation.Windows;
using WS.Manager.Prism;
using WS.Manager.WinService;

namespace WS.Manager.Presentation.ViewModels
{
    /// <summary>
    /// View model for <see cref="CreateOrEditServiceWindow"/>.
    /// </summary>
    public class CreateOrEditServiceWindowViewModel : NotificationObject
    {
        private readonly ServiceModel _serviceModel;
        
        private bool? _dialogResult;

        private string _description;

        private string _displayName;

        private string _serviceName;

        /// <summary>
        /// Constructor for <see cref="CreateOrEditServiceWindowViewModel"/>.
        /// </summary>
        public CreateOrEditServiceWindowViewModel(ServiceModel serviceModel = null)
        {
            Creation = serviceModel == null;
            _serviceModel = serviceModel;
            _description = serviceModel?.Description;
            _displayName = serviceModel?.DisplayName;
            _serviceName = serviceModel?.ServiceName;

            ServiceUser = Creation
                ? new ServiceUserDetailsViewModel()
                : new ServiceUserDetailsViewModel(serviceModel.UserName);

            var startupType = Creation ? ServiceStartMode.Automatic : serviceModel?.StartupType;
            ServiceFilePath = new ServiceFilePathViewModel(serviceModel?.BinaryPathName);
            ServiceStartupType = new ServiceStartupTypeViewModel(startupType);
            CreateOrChangeCommand = new DelegateCommand(CreateOrChangePressed, CanCreateOrChangeExecute);
            CancelCommand = new DelegateCommand(CancelPressed);

            var viewModels = new INotifyPropertyChanged[]
            {
                this,
                ServiceFilePath,
                ServiceStartupType
            };

            foreach (var viewModel in viewModels)
            {
                viewModel.PropertyChanged += PropertyChangedHandler;
            }
        }

        private void PropertyChangedHandler(object sender, PropertyChangedEventArgs e)
        {
            CreateOrChangeCommand.RaiseCanExecuteChanged();
        }

        /// <summary>
        /// Открыто в режиме создания.
        /// </summary>
        public bool Creation { get; }

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
        public string ServiceName
        {
            get => _serviceName;
            set
            {
                _serviceName = value;
                RaisePropertyChanged(() => ServiceName);
            }
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
        /// Create or change command.
        /// </summary>
        public DelegateCommand CreateOrChangeCommand { get; }

        /// <summary>
        /// Cancel command.
        /// </summary>
        public DelegateCommand CancelCommand { get; }

        private void CancelPressed(object obj)
        {
            DialogResult = false;
        }

        private bool CanCreateOrChangeExecute(object obj)
        {
            var requiredFields = new[]
            {
                ServiceName,
                DisplayName,
                ServiceFilePath.ServiceFilePath
            };

            return requiredFields.All(field => !string.IsNullOrEmpty(field));
        }

        private void CreateOrChangePressed(object obj)
        {
            var password = ServiceUser.PasswordChanged ? ServiceUser.UserPassword : null;

            if (Creation)
            {
                WinServiceUtils.Install(
                    ServiceName,
                    DisplayName,
                    ServiceStartupType.StartMode.ToServiceBootFlag(),
                    ServiceFilePath.ServiceFilePath,
                    ServiceUser.UserName,
                    password);

                WinServiceUtils.ChangeServiceDescription(ServiceName, Description);
            }
            else
            {
                WinServiceUtils.ChangeServiceDescription(_serviceModel.ServiceName, Description);

                WinServiceUtils.ChangeServiceProperties(
                    ServiceName,
                    DisplayName,
                    ServiceStartupType.StartMode.ToServiceBootFlag(),
                    ServiceFilePath.ServiceFilePath,
                    ServiceUser.UserName,
                    password);
            }

            DialogResult = true;
        }
    }
}