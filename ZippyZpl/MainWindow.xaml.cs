using System;
using System.Windows;
using ZippyZpl.View;
using ZippyZpl.ViewModel;

namespace ZippyZpl {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {
        private LabelViewModel labelViewModelObject = null;
        public MainWindow() {
            InitializeComponent();

            base.Closing += this.MainWindow_Closing;
        }

        void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e) {
            if (labelViewModelObject != null) {
                labelViewModelObject.Shutdown();
            }
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

        private void LabelSize_Click(object sender, RoutedEventArgs e) {
            // Instantiate the dialog box
            LabelSizeView dlg = new LabelSizeView(labelViewModelObject);

            // Configure the dialog box
            dlg.Owner = this;

            dlg.Show();
        }
    }
}
