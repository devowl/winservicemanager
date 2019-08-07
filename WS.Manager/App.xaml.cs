using System;
using System.Windows;
using System.Windows.Threading;

using WS.Manager.Libraries;

namespace WS.Manager
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App
    {
        private readonly AssemblyResolver _resolver = new AssemblyResolver();

        /// <summary>
        /// Constructor for <see cref="App"/>.
        /// </summary>
        public App()
        {
            AppDomain.CurrentDomain.UnhandledException += CurrentDomainOnUnhandledException;
            Dispatcher.UnhandledException += DispatcherOnUnhandledException;
            _resolver.Activate();
        }

        private static void DispatcherOnUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs args)
        {
            args.Handled = true;
            ShowException(args.Exception);
        }

        private static void CurrentDomainOnUnhandledException(object sender, UnhandledExceptionEventArgs args)
        {
            ShowException((Exception)args.ExceptionObject);
        }

        private static void ShowException(Exception exception)
        {
            MessageBox.Show(exception.ToString(), "Unhandled Error", MessageBoxButton.OK, MessageBoxImage.Asterisk);
        }
    }
}