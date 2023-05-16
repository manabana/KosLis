using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace KosLis//MessageBox.Show((SystemInformation.PowerStatus.BatteryLifePercent *100).ToString()+"%" , "Заряд батареи %"
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public bool stop = false;
        public MainWindow()
        {
            InitializeComponent();
            MainFrame.Content = new LoginPage();
        }

        private void Grid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        private void CloseWindow(object sender, RoutedEventArgs e)
        {
            Environment.Exit(0);
        }

        private void MinimizeWindow(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void ExpandWindow(object sender, RoutedEventArgs e)
        {
            if (WindowState == WindowState.Normal)
            {
                WindowState = WindowState.Maximized;
            } 
            else
            {
                WindowState = WindowState.Normal;
            }
        }
        private void Window_Activated(object sender, EventArgs e)
        {
            SolidColorBrush brush = new SolidColorBrush(Color.FromRgb(255, 255, 255));
            isActive.Fill = brush;

        }

        private void Window_Deactivated(object sender, EventArgs e)
        {
            SolidColorBrush brush = new SolidColorBrush(Color.FromRgb(38, 38, 38));
            isActive.Fill = brush;

        }
        public void LogOut()
        {
            stop = true;
            LoginPage loginPage = new LoginPage();
            MainFrame.Content = loginPage;
        }
    }
}
