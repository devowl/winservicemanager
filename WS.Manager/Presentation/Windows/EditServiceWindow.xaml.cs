using WS.Manager.Presentation.Models;
using WS.Manager.Presentation.ViewModels;

namespace WS.Manager.Presentation.Windows
{
    /// <summary>
    /// Interaction logic for EditServiceWindow.xaml
    /// </summary>
    public partial class EditServiceWindow
    {
        /// <summary>
        /// Constructor for <see cref="EditServiceWindow"/>.
        /// </summary>
        public EditServiceWindow(ServiceModel serviceModel)
        {
            DataContext = new EditServiceWindowViewModel(serviceModel);
            InitializeComponent();
        }
    }
}