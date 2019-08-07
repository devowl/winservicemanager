using System;
using System.ComponentModel;
using System.Linq;
using System.Windows;

using Microsoft.Win32;

using WS.Manager.Presentation.Controls.ViewModels;
using WS.Manager.Presentation.Windows;
using WS.Manager.Prism;

namespace WS.Manager.Presentation.ViewModels
{
    /// <summary>
    /// <see cref="CreateServiceWindow"/> view model.
    /// </summary>
    public class CreateServiceWindowViewModel : NotificationObject
    {
        private bool? _dialogResult;

        private string _description;

        private string _serviceName;

        private string _displayName;

        private string _filePath;

        /// <summary>
        /// Constructor for <see cref="CreateServiceWindowViewModel"/>.
        /// </summary>
        public CreateServiceWindowViewModel()
        {
            CreateCommand = new DelegateCommand(CreateService, CanCreateService);
            CancelCommand = new DelegateCommand(o => DialogResult = false);
            ServiceUser = new ServiceUserDetailsViewModel();
            ServiceFilePath = new ServiceFilePathViewModel();
            PropertyChanged += OnPropertyChanged;
        }

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
                _description = value;
                RaisePropertyChanged(() => Description);
            }
        }

        /// <summary>
        /// Display name.
        /// </summary>
        public string DisplayName
        {
            get
            {
                return _displayName;
            }
            set
            {
                _displayName = value;
                RaisePropertyChanged(() => DisplayName);
            }
        }

        /// <summary>
        /// Service name.
        /// </summary>
        public string ServiceName
        {
            get
            {
                return _serviceName;
            }
            set
            {
                _serviceName = value;
                RaisePropertyChanged(() => ServiceName);
            }
        }

        /// <summary>
        /// Путь к исполняемому файлу сервиса.
        /// </summary>
        public string FilePath
        {
            get
            {
                return _filePath;
            }
            set
            {
                _filePath = value;
                RaisePropertyChanged(() => FilePath);
            }
        }

        /// <summary>
        /// Create command.
        /// </summary>
        public DelegateCommand CreateCommand { get; }

        /// <summary>
        /// Cancel command.
        /// </summary>
        public DelegateCommand CancelCommand { get; }

        /// <summary>
        /// Service user account view model.
        /// </summary>
        public ServiceUserDetailsViewModel ServiceUser { get; }

        /// <summary>
        /// Service file path view model.
        /// </summary>
        public ServiceFilePathViewModel ServiceFilePath { get; }

        private bool CanCreateService(object obj)
        {
            return new[]
            {
                _displayName,
                _serviceName,
                _filePath,
            }.All(p => !string.IsNullOrEmpty(p));
        }

        private void OnPropertyChanged(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
        {
            CreateCommand.RaiseCanExecuteChanged();
        }

        private void CreateService(object obj)
        {
        }
    }
}