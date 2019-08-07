using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interactivity;

namespace WS.Manager.Presentation.Behaviors
{
    /// <summary>
    /// PasswordBox binding password property.
    /// </summary>
    public class PasswordBehavior : Behavior<PasswordBox>
    {
        /// <summary>
        /// Пароль (свойство зависимости).
        /// </summary>
        public static readonly DependencyProperty PasswordProperty = DependencyProperty.Register(
            "Password",
            typeof(string),
            typeof(PasswordBehavior),
            new PropertyMetadata(default(string)));

        private bool _skipUpdate;

        /// <summary>
        /// Password property.
        /// </summary>
        public string Password
        {
            get
            {
                return (string)GetValue(PasswordProperty);
            }

            set
            {
                SetValue(PasswordProperty, value);
            }
        }

        /// <inheritdoc/>
        protected override void OnAttached()
        {
            AssociatedObject.PasswordChanged += PasswordChanged;
        }

        /// <inheritdoc/>
        protected override void OnDetaching()
        {
            AssociatedObject.PasswordChanged -= PasswordChanged;
        }

        /// <inheritdoc/>
        protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            try
            {
                base.OnPropertyChanged(e);
                if (e.Property != PasswordProperty)
                {
                    return;
                }

                if (_skipUpdate)
                {
                    return;
                }

                _skipUpdate = true;

                AssociatedObject.Password = (string)e.NewValue;
            }
            catch (Exception)
            {
                // catch design time exception
            }
            finally
            {
                _skipUpdate = false;
            }
        }

        private void PasswordChanged(object sender, RoutedEventArgs e)
        {
            _skipUpdate = true;
            try
            {
                Password = AssociatedObject.Password;
            }
            finally
            {
                _skipUpdate = false;
            }
        }
    }
}