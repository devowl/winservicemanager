using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.ServiceProcess;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

using WS.Manager.Presentation.Models;
using WS.Manager.Presentation.Windows;
using WS.Manager.Prism;
using WS.Manager.WinService;

namespace WS.Manager.Presentation.ViewModels
{
    /// <summary>
    /// ViewModel for <see cref="MainWindow"/>.
    /// </summary>
    public class MainWindowViewModel : NotificationObject
    {
        private readonly DispatcherTimer _refreshTimer = new DispatcherTimer() { Interval = TimeSpan.FromSeconds(3) };

        private bool _waiting;

        private ObservableCollection<ServiceModel> _services = new ObservableCollection<ServiceModel>();

        private ServiceModel _selectedService;

        private const string WarrningText = "Warrning";

        private object _refreshSyncObject = new object();

        /// <summary>
        /// Constructor for <see cref="MainWindowViewModel"/>.
        /// </summary>
        public MainWindowViewModel()
        {
            RefreshCommand = new DelegateCommand(async o => await RefreshAsync());
            StartCommand = new DelegateCommand(o => ExecuteServiceOperation(StartService), HasSelectedItem);
            StopCommand = new DelegateCommand(o => ExecuteServiceOperation(StopService), HasSelectedItem);
            EditCommand = new DelegateCommand(o => EditService(), HasSelectedItem);
            DeleteCommand = new DelegateCommand(o => DeleteService(), HasSelectedItem);
            StartWithArgumentsCommand =
                new DelegateCommand(o => ExecuteServiceOperation(StartWithArgumentsService, true), HasSelectedItem);
            TerminateCommand = new DelegateCommand(o => ExecuteServiceOperation(TerminateService), HasSelectedItem);
            Dispatcher.CurrentDispatcher.BeginInvoke(new Action(async () => await RefreshAsync()));
            _refreshTimer.Tick += RefreshTimerTick;
            _refreshTimer.Start();
        }

        /// <summary>
        /// Selected list item.
        /// </summary>
        public ServiceModel SelectedService
        {
            get
            {
                return _selectedService;
            }
            set
            {
                _selectedService = value;
                SelectedItemChanged();
                RaisePropertyChanged(() => SelectedService);
            }
        }

        /// <summary>
        /// Available services.
        /// </summary>
        public ObservableCollection<ServiceModel> Services
        {
            get
            {
                return _services;
            }

            private set
            {
                _services = value;
                RaisePropertyChanged(() => Services);
            }
        }

        /// <summary>
        /// Main windows title text.
        /// </summary>
        public string WindowTitle
        {
            get
            {
                var version = FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location).FileVersion;
                return $"Service Manager. (by Sergey Vaulin, www.devowl.net) ver. {version}";
            }
        }

        /// <summary>
        /// Refresh command.
        /// </summary>
        public DelegateCommand RefreshCommand { get; }

        /// <summary>
        /// Start command.
        /// </summary>
        public DelegateCommand StartCommand { get; }

        /// <summary>
        /// Start with arguments command.
        /// </summary>
        public DelegateCommand StartWithArgumentsCommand { get; }

        /// <summary>
        /// Stop command.
        /// </summary>
        public DelegateCommand StopCommand { get; }

        /// <summary>
        /// Terminate command.
        /// </summary>
        public DelegateCommand TerminateCommand { get; }

        /// <summary>
        /// Edit command.
        /// </summary>
        public DelegateCommand EditCommand { get; }

        /// <summary>
        /// Delete command.
        /// </summary>
        public DelegateCommand DeleteCommand { get; }
        
        /// <summary>
        /// Exit command.
        /// </summary>
        public DelegateCommand ExitCommand { get; } = new DelegateCommand(o => { Environment.Exit(0); });

        /// <summary>
        /// Окно находится в режиме ожидания.
        /// </summary>
        public bool Waiting
        {
            get
            {
                return _waiting;
            }

            set
            {
                _waiting = value;
                RaisePropertyChanged(() => Waiting);
            }
        }

        private void RefreshTimerTick(object sender, EventArgs arg)
        {
            lock (_refreshSyncObject)
            {
                foreach (var service in Services.ToArray())
                {
                    service.Refresh();
                }
            }
        }

        private bool HasSelectedItem(object obj)
        {
            return SelectedService != null;
        }

        private void SelectedItemChanged()
        {
            new List<DelegateCommand> { StartCommand, StopCommand, EditCommand, TerminateCommand, StartWithArgumentsCommand }.ForEach(
                c => c.RaiseCanExecuteChanged());
        }

        private void EditService()
        {
            var editor = new EditServiceWindow(SelectedService) { Owner = Application.Current.MainWindow };
            editor.ShowDialog();
            SelectedService.EntireRefresh();
        }

        private void DeleteService()
        {
            if (MessageBox.Show(
                    $"Are you sure you wanna to delete service?\n\r\"{SelectedService.DisplayName}\"",
                    WarrningText,
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                lock (_refreshSyncObject)
                {
                    WinServiceUtils.Uninstall(SelectedService.ServiceName);
                    Services.Remove(SelectedService);
                }
            }
        }

        private void StartWithArgumentsService()
        {
            var dataContext = new ServiceParametersWindowViewModel();
            var window = new ServiceParametersWindow(dataContext) { Owner = Application.Current.MainWindow };

            if (window.ShowDialog().GetValueOrDefault())
            {
                var userInput = dataContext.UserInput;
                var arguments = ServiceUtils.CommandLineToArgs(userInput);
                SelectedService.Controller.Start(arguments);
            }
        }

        private void TerminateService()
        {
            WinServiceUtils.TerminateService(SelectedService.ServiceName);
        }

        private void StopService()
        {
            SelectedService.Controller.Stop();
        }

        private void StartService()
        {
            SelectedService.Controller.Start();
        }

        private async void ExecuteServiceOperation(Action action, bool sync = false)
        {
            try
            {
                Waiting = true;

                if (sync)
                {
                    action();
                }
                else
                {
                    await Task.Factory.StartNew(action);
                }
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message, WarrningText, MessageBoxButton.OK, MessageBoxImage.Asterisk);
            }
            finally
            {
                Waiting = false;
            }

            await RefreshAsync();
        }

        private async Task RefreshAsync()
        {
            await Task.Factory.StartNew(ServiceRosterRefresh);
        }

        private void ServiceRosterRefresh()
        {
            if (Waiting)
            {
                return;
            }

            Waiting = true;

            var buffer =
                ServiceController.GetServices()
                    .Select(machineService => new ServiceModel(machineService))
                    .ToList();

            var oldItems = Services.ToList();
            Services = new ObservableCollection<ServiceModel>(buffer);
            oldItems.ForEach(i => i.Dispose());

            Waiting = false;
        }
    }
}