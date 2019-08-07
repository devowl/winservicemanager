using System.Windows;

using Microsoft.Win32;

using WS.Manager.Prism;

namespace WS.Manager.Presentation.Controls.ViewModels
{
    /// <summary>
    /// View model for <see cref="ServiceFilePath"/>.
    /// </summary>
    public class ServiceFilePathViewModel : NotificationObject
    {
        private string _serviceFilePath;

        /// <summary>
        /// Constructor for <see cref="ServiceFilePathViewModel"/>.
        /// </summary>
        public ServiceFilePathViewModel()
        {
            OpenCommand = new DelegateCommand(OpenFilePath);
        }

        /// <summary>
        /// Constructor for <see cref="ServiceFilePathViewModel"/>.
        /// </summary>
        public ServiceFilePathViewModel(string filePath) : this()
        {
            _serviceFilePath = filePath;
        }

        /// <summary>
        /// Open command.
        /// </summary>
        public DelegateCommand OpenCommand { get; }

        private void OpenFilePath(object obj)
        {
            var dialog = new OpenFileDialog()
            {
                CheckFileExists = true,
                CheckPathExists = true,
                Multiselect = false,
                Filter = "Executable files|*.exe",
                Title = "Choose executable file"
            };

            if (dialog.ShowDialog(Application.Current.MainWindow).GetValueOrDefault())
            {
                ServiceFilePath = $@"""{dialog.FileName}""";
            }
        }

        /// <summary>
        /// Service executable file path.
        /// </summary>
        public string ServiceFilePath
        {
            get
            {
                return _serviceFilePath;
            }

            set
            {
                _serviceFilePath = value;
                RaisePropertyChanged(() => ServiceFilePath);
            }
        }
    }
}
