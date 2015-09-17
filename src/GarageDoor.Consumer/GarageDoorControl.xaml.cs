using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace GarageDoor.Consumer
{
    public sealed partial class GarageDoorControl : UserControl
    {
        private GarageDoorControlViewModel _model;

        public GarageDoorControl()
        {
            this.InitializeComponent();
            _model = new GarageDoorControlViewModel();
            this.DataContext = _model;

            this.Loaded += GarageDoorControl_Loaded;
        }

        private void GarageDoorControl_Loaded(object sender, RoutedEventArgs e)
        {
            _model.Start();
        }

        private void Open_Click(object sender, RoutedEventArgs e)
        {
            _model.Open();
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            _model.Close();
        }
    }
}
