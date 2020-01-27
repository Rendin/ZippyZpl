using System.Windows;
using ZippyZpl.ViewModel;

namespace ZippyZpl {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {
        private LabelViewModel labelViewModelObject = null;
        public MainWindow() {
            InitializeComponent();
        }

        private void LabelViewControl_Loaded(object sender, RoutedEventArgs e) {
            labelViewModelObject = new LabelViewModel();
            labelViewModelObject.LoadLabels();

            LabelViewControl.DataContext = labelViewModelObject;
        }

        private void Listen_Checked(object sender, RoutedEventArgs e) {
            labelViewModelObject.StartListening();
        }

        private void Listen_Unchecked(object sender, RoutedEventArgs e) {
            labelViewModelObject.StartListening();
        }
    }
}
