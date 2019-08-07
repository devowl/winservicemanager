using System;

using WS.Manager.Presentation.Controls.Views;
using WS.Manager.Prism;

namespace WS.Manager.Presentation.Controls.ViewModels
{
    /// <summary>
    /// View model for <see cref="ServiceUserDetails"/>.
    /// </summary>
    public class ServiceUserDetailsViewModel : NotificationObject
    {
        private bool _isLocalSystemAccount;

        private string _userName;

        private string _userPassword;

        private const string LocalSystem = "LocalSystem";

        /// <summary>
        /// Constructor for <see cref="ServiceUserDetailsViewModel"/>.
        /// </summary>
        public ServiceUserDetailsViewModel()
        {
            _isLocalSystemAccount = true;
        }

        /// <summary>
        /// Constructor for <see cref="ServiceUserDetailsViewModel"/>.
        /// </summary>
        public ServiceUserDetailsViewModel(string userName)
        {
            if (LocalSystem.Equals(userName, StringComparison.OrdinalIgnoreCase))
            {
                _isLocalSystemAccount = true;
            }
            else
            {
                _isLocalSystemAccount = string.IsNullOrEmpty(userName);
                _userName = userName;
            }
            
            if (!_isLocalSystemAccount)
            {
                _userPassword = "********";
            }
        }

        /// <summary>
        /// Use local system account.
        /// </summary>
        public bool IsLocalSystemAccount
        {
            get
            {
                return _isLocalSystemAccount;
            }

            set
            {
                _isLocalSystemAccount = value;
                RaisePropertyChanged(() => IsLocalSystemAccount);
            }
        }

        /// <summary>
        /// User name.
        /// </summary>
        public string UserName
        {
            get
            {
                return _userName;
            }
            set
            {
                _userName = value;
                RaisePropertyChanged(() => UserName);
            }
        }

        /// <summary>
        /// User password.
        /// </summary>
        public string UserPassword
        {
            get
            {
                return _userPassword;
            }
            set
            {
                _userPassword = value;
                RaisePropertyChanged(() => UserPassword);
            }
        }
    }
}
