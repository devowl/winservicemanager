using WS.Manager.Presentation.Windows;
using WS.Manager.Prism;

namespace WS.Manager.Presentation.ViewModels
{
    /// <summary>
    /// <see cref="ServiceParametersWindow"/> view model.
    /// </summary>
    public class ServiceParametersWindowViewModel : NotificationObject
    {
        private bool? _dialogResult;

        private string _userInput;

        /// <summary>
        /// Constructor for <see cref="ServiceParametersWindowViewModel"/>.
        /// </summary>
        public ServiceParametersWindowViewModel()
        {
            OkCommand = new DelegateCommand(OkPressed, o => !string.IsNullOrEmpty(UserInput));
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
        /// OK command.
        /// </summary>
        public DelegateCommand OkCommand { get; }

        /// <summary>
        /// User input text.
        /// </summary>
        public string UserInput
        {
            get
            {
                return _userInput;
            }

            set
            {
                _userInput = value;
                OkCommand.RaiseCanExecuteChanged();
            }
        }

        private void OkPressed(object obj)
        {
            DialogResult = true;
        }
    }
}