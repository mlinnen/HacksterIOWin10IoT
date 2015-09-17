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
    public sealed partial class TemperatureControl : UserControl
    {
        private TemperatureControlViewModel _model;
        public TemperatureControl()
        {
            this.InitializeComponent();
            _model = new TemperatureControlViewModel();
            this.DataContext = _model;

            this.Loaded += TemperatureControl_Loaded;
        }

        private void TemperatureControl_Loaded(object sender, RoutedEventArgs e)
        {
            _model.Start();
        }
    }
}
