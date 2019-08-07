using System.ServiceProcess;

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

        private bool _hasChanges = false;

        /// <summary>
        /// Constructor for <see cref="EditServiceWindowViewModel"/>.
        /// </summary>
        public EditServiceWindowViewModel(ServiceModel serviceModel)
        {
            _serviceModel = serviceModel;
            Description = serviceModel.Description;
            DisplayName = serviceModel.DisplayName;
            
            ServiceUser = new ServiceUserDetailsViewModel(serviceModel.UserName);
            ServiceUser.PropertyChanged += (sender, args) => Changed();

            ServiceFilePath = new ServiceFilePathViewModel(serviceModel.BinaryPathName);
            ServiceStartupType = new ServiceStartupTypeViewModel(serviceModel.StartupType);
            OkCommand = new DelegateCommand(OkPressed, o => !string.IsNullOrEmpty(DisplayName));
        }

        /// <summary>
        /// Service description.
        /// </summary>
        public string Description
        {
            get
            {
                return _description; 
            }

            set
            {
                Changed();
                _description = value;
                RaisePropertyChanged(() => Description);
            }
        }

        /// <summary>
        /// Service display name.
        /// </summary>
        public string DisplayName
        {
            get
            {
                return _displayName; 
            }

            set
            {
                Changed();
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
            get
            {
                return _dialogResult;
            }

            set
            {
                _dialogResult = value;
                RaisePropertyChanged(() => DialogResult);
            }
        }

        /// <summary>
        /// OK command.
        /// </summary>
        public DelegateCommand OkCommand { get; }

        private void Changed()
        {
            _hasChanges = true;
        }

        private void OkPressed(object obj)
        {
            if (Description != _serviceModel.Description)
            {
                WinServiceUtils.ChangeServiceDescription(_serviceModel.ServiceName, Description);
            }

            if (_hasChanges)
            {
                //WinServiceUtils.ChangeServiceDisplayName(_serviceModel.ServiceName, DisplayName,);
            }

            DialogResult = _hasChanges;
        }
    }
}