using QuerySender;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace KosLis
{
    /// <summary>
    /// Логика взаимодействия для OptionsPage.xaml
    /// </summary>
    public partial class OptionsPage : Page
    {
        private int UserId;
        private string Email;
        private string Password;

        bool swtVis = false;

        public OptionsPage(int userId, string email, string password)
        {
            UserId = userId;
            Email = email;
            Password = password;
            InitializeComponent();
            if (File.Exists("login.bin"))
            {
                LoginChecker.IsChecked = true;
            }

        }
        private string ChangePassword(int userid, string pastPassword, string nextPassword)
        {
            return HomeSender.ChangePassword(userid, pastPassword, nextPassword);
        }

        private void RememberData(object sender, RoutedEventArgs e)
        {
            using (BinaryWriter writer = new BinaryWriter(File.Open("login.bin", FileMode.Create)))
            {
                // Запись логина и пароля в файл
                writer.Write(Email);
                writer.Write(Password);
            }

        }

        private void ForgetData(object sender, RoutedEventArgs e)
        {
            if (File.Exists("login.bin"))
            {
                File.Delete("login.bin");
            }
        }

        private void ShowPasswEditor(object sender, RoutedEventArgs e)
        {
            pencil.Visibility = Visibility.Collapsed;
            eye.Visibility = Visibility.Visible;
            pastpwgrid.Visibility = Visibility.Visible;
            nextpwgrid.Visibility = Visibility.Visible;
            ConfirmChanges.Visibility = Visibility.Visible;
        }

        private void TBCleaning(object sender, RoutedEventArgs e)
        {
            TextBox instance = (TextBox)sender;
            instance.Foreground = new SolidColorBrush(Colors.LightGray);
            if (instance.Text == instance.Tag.ToString())
                instance.Text = "";
        }

        private void TBFilling(object sender, RoutedEventArgs e)
        {
            TextBox instance = (TextBox)sender;
            instance.Foreground = new SolidColorBrush(Colors.DarkGray);
            if (string.IsNullOrWhiteSpace(instance.Text))
                instance.Text = instance.Tag.ToString();
        }
        private void PasswordDown(object sender, KeyEventArgs e)
        {
            tbPassword.Text = pbPassword.Password;
        }
        private void TextBoxDown(object sender, KeyEventArgs e)
        {
            pbPassword.Password = "";
            pbPassword.Password += tbPassword.Text;

        }
        private void PasswordDown2(object sender, KeyEventArgs e)
        {
            tbPassword2.Text = pbPassword2.Password;
        }
        private void TextBoxDown2(object sender, KeyEventArgs e)
        {
            pbPassword2.Password = "";
            pbPassword2.Password += tbPassword2.Text;

        }
        private void PWVisibilitySwitch(object sender, RoutedEventArgs e)
        {
            if (swtVis == false)
            {
                var uriSource4 = new Uri(@"IMGs/hide.png", UriKind.Relative);
                PWVis.Source = new BitmapImage(uriSource4);
                tbPassword.Visibility = Visibility.Visible;
                pbPassword.Visibility = Visibility.Collapsed;
                tbPassword2.Visibility = Visibility.Visible;
                pbPassword2.Visibility = Visibility.Collapsed;

                swtVis = !swtVis;
            }
            else
            {
                var uriSource4 = new Uri(@"IMGs/show.png", UriKind.Relative);
                PWVis.Source = new BitmapImage(uriSource4);
                tbPassword.Visibility = Visibility.Collapsed;
                pbPassword.Visibility = Visibility.Visible;
                tbPassword2.Visibility = Visibility.Collapsed;
                pbPassword2.Visibility = Visibility.Visible;
                swtVis = !swtVis;
            }
        }
        private void PWBCleaning(object sender, RoutedEventArgs e)
        {
            //tblPasswordHint.Visibility = pbPassword.Password.Length == 0 ? Visibility.Visible : Visibility.Hidden;
            if (pbPassword.Password.Length == 0)
            {
                tblPasswordHint.Visibility = Visibility.Collapsed;
            }
            else { }

        }

        private void PWBFilling(object sender, RoutedEventArgs e)
        {
            if (pbPassword.Password.Length == 0)
            {
                tblPasswordHint.Visibility = Visibility.Visible;
            }
            else { }
        }
        private void PWBCleaning2(object sender, RoutedEventArgs e)
        {
            //tblPasswordHint.Visibility = pbPassword.Password.Length == 0 ? Visibility.Visible : Visibility.Hidden;
            if (pbPassword2.Password.Length == 0)
            {
                tblPasswordHint2.Visibility = Visibility.Collapsed;
            }
            else { }

        }

        private void PWBFilling2(object sender, RoutedEventArgs e)
        {
            if (pbPassword2.Password.Length == 0)
            {
                tblPasswordHint2.Visibility = Visibility.Visible;
            }
            else { }
        }

        private void SendChanges(object sender, RoutedEventArgs e)
        {
            string Old = pbPassword.Password;
            string New = pbPassword2.Password;
            string req = HomeSender.ChangePassword(UserId, Old, New);
            if(req == "OK")
            {
                MessageFrame("Пароль изменен!", MessageType.Successful);
            }
            else if(req == "ICPassword")
            {
                MessageFrame("Неправильный старый пароль!", MessageType.Error);
            }
            else if(req == "ServerNotResponding")
            {
                MessageFrame("Не удалось подключиться к серверу", MessageType.Error);
            }
            else if (req == "SqlWillNotStarted")
            {
                MessageFrame("Серверу не удалось подключиться к базе данных", MessageType.Error);
            }
        }
        private void ButClicked(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            TopGrid.IsEnabled = true;
            BlurEffect blurEffect = new BlurEffect();
            TopGrid.Effect = blurEffect;
            DoubleAnimation animation = new DoubleAnimation();
            animation.From = 16;
            animation.To = 0;
            animation.Duration = TimeSpan.FromSeconds(0.2);
            MessageGrid.Visibility = Visibility.Collapsed;
            TopGrid.Effect.BeginAnimation(BlurEffect.RadiusProperty, animation);
            TopGrid.Effect = null;

        }
        private void MessageFrame(string message, MessageType type)
        {
            TopGrid.IsEnabled = false;
            BlurEffect blurEffect = new BlurEffect();
            TopGrid.Effect = blurEffect;
            DoubleAnimation animation = new DoubleAnimation();
            animation.From = 0;
            animation.To = 16;
            animation.Duration = TimeSpan.FromSeconds(0.2);

            DoubleAnimation animation2 = new DoubleAnimation();
            animation2.From = 0;
            animation2.To = 1;
            animation2.Duration = TimeSpan.FromSeconds(0.2);

            switch (type)
            {
                case MessageType.Error:
                    Button2.Visibility = Visibility.Collapsed;
                    Button1.Content = "Ок";
                    var uriSource3 = new Uri(@"IMGs/cross.png", UriKind.Relative);
                    MessageIcon.Source = new BitmapImage(uriSource3);

                    MessageText.Text = message;
                    break;
                case MessageType.Warning:
                    Button2.Visibility = Visibility.Collapsed;
                    Button1.Content = "Ок";
                    var uriSource2 = new Uri(@"IMGs/attention.png", UriKind.Relative);
                    MessageIcon.Source = new BitmapImage(uriSource2);

                    MessageText.Text = message;
                    break;
                case MessageType.Confirmation:
                    Button2.Visibility = Visibility.Visible;
                    Button1.Content = "Да";
                    Button2.Content = "Нет";
                    var uriSource1 = new Uri(@"IMGs/question.png", UriKind.Relative);
                    MessageIcon.Source = new BitmapImage(uriSource1);
                    Button1.Tag = "confirmYes";
                    Button2.Tag = "confirmNo";
                    MessageText.Text = message;
                    break;
                case MessageType.Successful:
                    Button2.Visibility = Visibility.Collapsed;
                    Button1.Content = "Ок";
                    var uriSource4 = new Uri(@"IMGs/check.png", UriKind.Relative);
                    MessageIcon.Source = new BitmapImage(uriSource4);

                    MessageText.Text = message;

                    break;

            }
            MessageGrid.Visibility = Visibility.Visible;
            TopGrid.Effect.BeginAnimation(BlurEffect.RadiusProperty, animation);
            MessageGrid.BeginAnimation(OpacityProperty, animation2);
        }

    }
}
