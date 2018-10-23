using System;
using System.Threading.Tasks;
using System.Windows;

namespace CefSharp.Wpf.Example
{
    /// <summary>
    /// Interaction logic for SimpleMainWindow.xaml
    /// </summary>
    public partial class SimpleMainWindow : Window
    {
        public SimpleMainWindow()
        {
            InitializeComponent();
            this.WindowState = WindowState.Normal;
        }

        private async void btnResize_Click(object sender, RoutedEventArgs e)
        {
            this.Width = 500;
            await Task.Delay(1);
            this.Width = 1000;
            await Task.Delay(1);
            this.Width = 500;
            await Task.Delay(1);
            this.Width = 1000;
        }
    }
}
