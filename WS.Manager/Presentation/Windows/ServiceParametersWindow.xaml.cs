using System.Windows;

namespace WS.Manager.Presentation.Windows
{
    /// <summary>
    /// Interaction logic for ServiceParametersWindow.xaml
    /// </summary>
    public partial class ServiceParametersWindow
    {
        /// <summary>
        /// Constructor for <see cref="ServiceParametersWindow"/>.
        /// </summary>
        public ServiceParametersWindow(object dataContext)
        {
            DataContext = dataContext;
            InitializeComponent();
        }
    }
}