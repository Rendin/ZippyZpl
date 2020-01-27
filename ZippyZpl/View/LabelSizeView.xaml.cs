using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using ZippyZpl.ViewModel;

namespace ZippyZpl.View {
    /// <summary>
    /// Interaction logic for LabelSizeView.xaml
    /// </summary>
    public partial class LabelSizeView : Window {
        private LabelViewModel labelViewModelObject;
        public LabelSizeView(LabelViewModel labelViewModel) {
            InitializeComponent();

            labelViewModel = labelViewModelObject;
        }

        void OkButton_Click(object sender, RoutedEventArgs e) {
            this.Close();
        }

        void CloseButton_Click(object sender, RoutedEventArgs e) {
            this.Close();
        }

    }
}
