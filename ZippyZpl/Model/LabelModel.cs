using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Windows.Media.Imaging;

namespace ZippyZpl.Model {
    public class LabelModel {
    }

    public class Label : INotifyPropertyChanged {
        private BitmapImage labelImage;
        //private string labelImage;

        public BitmapImage LabelImage {
            get {
                return labelImage;
            }

            set {
                if (labelImage != value) {
                    labelImage = value;
                    RaisePropertyChanged("LabelImage");
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void RaisePropertyChanged(string property) {
            if (PropertyChanged != null) {
                PropertyChanged(this, new PropertyChangedEventArgs(property));
            }
        }
    }
}
