using QuerySender;
using System;
using System.CodeDom.Compiler;
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
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Text.RegularExpressions;
using System.IO;
using System.Runtime.CompilerServices;

namespace KosLis
{
    /// <summary>
    /// Логика взаимодействия для LoginPage.xaml
    /// </summary>
    public partial class LoginPage : Page
    {
        bool swtVis = false;
        bool swtVis1 = false;
        bool swtVis3 = false;
        public LoginPage()
        {
            InitializeComponent();
            if (File.Exists("login.bin"))
            {
                using (BinaryReader reader = new BinaryReader(File.Open("login.bin", FileMode.Open)))
                {
                    // Чтение логина и пароля из файла
                    string login = reader.ReadString();
                    string password = reader.ReadString();
                    LoginStack.Visibility = Visibility.Collapsed;
                    LoadingStack.Visibility = Visibility.Visible;
                    LoginSendAsync(login, password);
                }

            }

        }

        private void TBCleaning(object sender, RoutedEventArgs e)
        {
            TextBox instance = (TextBox)sender;
            instance.Foreground= new SolidColorBrush(Colors.LightGray);
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

        private void MoveToReg(object sender, RoutedEventArgs e)
        {
            LoginStack.Visibility = Visibility.Collapsed;
            RegStack.Visibility = Visibility.Visible;
            DoubleAnimation upScaling = new DoubleAnimation();
            upScaling.From = SignGrid.ActualHeight;
            upScaling.To = 400;
            upScaling.Duration = TimeSpan.FromSeconds(0.25);
            SignGrid.BeginAnimation(Grid.HeightProperty, upScaling);

        }

        private void MoveToLog(object sender, RoutedEventArgs e)
        {
            RegStack.Visibility = Visibility.Collapsed;
            LoginStack.Visibility = Visibility.Visible;
            DoubleAnimation downScaling = new DoubleAnimation();
            downScaling.From = SignGrid.ActualHeight;
            downScaling.To = 370;
            downScaling.Duration = TimeSpan.FromSeconds(0.25);
            SignGrid.BeginAnimation(Grid.HeightProperty, downScaling);


        }
        private void CreateAcc(string em, string nn, string nm, string sn, string pw)
        {
            string response = LoginSender.Registering(em, nn, nm, sn, pw);
            if (response == "EMUsed")
            {
                Dispatcher.Invoke(() => ErrOut(7));
                
            }
            else if (response == "NNUsed")
            {
                Dispatcher.Invoke(() => ErrOut(8));
            }
            else if (response.IndexOf("AccAdded") >= 0)
            {
                string[] strings = response.Split(';');
                Dispatcher.Invoke(() => HomePageOpen(int.Parse(strings[1]),em, nn, nm, sn, pw, 0));
            }
            else if (response == "Exception;ServerNotResponding")
            {
                Dispatcher.Invoke(() => ErrOut(1));
            }
            else
            {
                Dispatcher.Invoke(() => ErrUnknown(response));
            }

        }
        private void CreateAccSend(object sender, RoutedEventArgs e)
        {
            DoubleAnimation downScaling = new DoubleAnimation();
            downScaling.From = SignGrid.ActualHeight;
            downScaling.To = 370;
            downScaling.Duration = TimeSpan.FromSeconds(0.25);
            SignGrid.BeginAnimation(Grid.HeightProperty, downScaling);
            RegStack.Visibility = Visibility.Collapsed;
            LoadingStack.Visibility = Visibility.Visible;
            CreateAccSendAsync();
        }
        private async Task CreateAccSendAsync()
        {
            string s = EmailTB.Text;
            string p = PasswordPB.Password;
            Regex regex = new Regex(@"(\w+)[@](\w+)[.](\w+)");
            MatchCollection matches = regex.Matches(s);
            if (p.Length >= 8) 
            {
                if (s == "Электронная почта")
                {
                    ErrOut(5);
                }
                else
                {
                    if (matches.Count > 0)
                    {
                        string em = EmailTB.Text;
                        string nn = NicknameTB.Text;
                        string nm = NameTB.Text;
                        string sn = SurnameTB.Text;
                        string pw = PasswordTB.Text;
                        await Task.Run(() => CreateAcc(em, nn, nm, sn, pw));

                    }
                    else
                    {
                        ErrOut(4);
                    }
                }

            }
            else { ErrOut(6); }
        }
        private void LoginSend(string em, string pw)
        {
            string response = LoginSender.Login(em,pw);
            string[] splitedResponse = response.Split(';');
            if (splitedResponse[0] == "userdata")
            {
                Dispatcher.Invoke(() => HomePageOpen(int.Parse(splitedResponse[1]), splitedResponse[2], splitedResponse[3], splitedResponse[4], splitedResponse[5], splitedResponse[6], int.Parse(splitedResponse[7])));
            }
            else if (splitedResponse[0] == "Exception")
            {
                if (splitedResponse[1] == "ServerNotResponding")
                {
                    Dispatcher.Invoke(() => ErrOut(0));
                }
                else if (splitedResponse[1] == "ICData")
                {
                    Dispatcher.Invoke(() => ErrOut(2));
                }
                else if (splitedResponse[1] == "SQLWillNotStarted")
                {
                    Dispatcher.Invoke(() => ErrOut(3));
                }

            }
            else
            {
                Dispatcher.Invoke(() => ErrUnknown(response));
            }

        }
        private async Task LoginSendAsync(string s, string p)
        {
            Regex regex = new Regex(@"\w*@\w*.\w*");
            MatchCollection matches = regex.Matches(s);
            if(p.Length >= 8 || p.Length <= 25)
            {
                if (s == "Электронная почта")
                {
                    ErrOut(5);
                }
                else
                {
                    if (matches.Count > 0)
                    {
                        string em = s;
                        string pw = p;
                        await Task.Run(() => LoginSend(em, pw));
                    }
                    else
                    {
                        ErrOut(4);
                    }

                }

            }
            else
            {
                ErrOut(6);
            }
        }
        private void LoginSendBT(object sender, RoutedEventArgs e)
        {
            LoginStack.Visibility = Visibility.Collapsed;
            LoadingStack.Visibility = Visibility.Visible;
            LoginSendAsync(EmailTBL.Text, pbPassword.Password);
        }

        private void ErrOk(object sender, RoutedEventArgs e)
        {
            MessageStack.Visibility = Visibility.Collapsed;
            LoginStack.Visibility = Visibility.Visible;
        }
        internal void ErrOut(byte type)
        {
            switch (type)
            {
                case 0: //ServerNotResponding login
                    LoginStack.Visibility = Visibility.Collapsed;
                    MessageStack.Visibility = Visibility.Visible;
                    LoadingStack.Visibility= Visibility.Collapsed;
                    ResetStack.Visibility= Visibility.Collapsed;
                    var uriSource3 = new Uri(@"IMGs/serverErr.png", UriKind.Relative);
                    ErrIMG.Source = new BitmapImage(uriSource3);
                    ErrText.Text = "Сервер недоступен!";
                    break;
                case 1: //ServerNotResponding reg
                    RegStack.Visibility = Visibility.Collapsed;
                    MessageStack.Visibility = Visibility.Visible;
                    LoadingStack.Visibility = Visibility.Collapsed;
                    var uriSource2 = new Uri(@"IMGs/serverErr.png", UriKind.Relative);
                    ErrIMG.Source = new BitmapImage(uriSource2);
                    ErrText.Text = "Сервер недоступен!";
                    break;
                case 2: //ICData login
                    LoginStack.Visibility = Visibility.Collapsed;
                    MessageStack.Visibility = Visibility.Visible;
                    LoadingStack.Visibility = Visibility.Collapsed;
                    var uriSource1 = new Uri(@"IMGs/UnkErr.png", UriKind.Relative);
                    ErrIMG.Source = new BitmapImage(uriSource1);
                    ErrText.Text = "Неправильный логин/пароль!";
                    break;
                case 3: //SQLWillNotStarted login
                    LoginStack.Visibility = Visibility.Collapsed;
                    MessageStack.Visibility = Visibility.Visible;
                    LoadingStack.Visibility = Visibility.Collapsed;
                    var uriSource4 = new Uri(@"IMGs/serverErr.png", UriKind.Relative);
                    ErrIMG.Source = new BitmapImage(uriSource4);
                    ErrText.Text = "Серверу не удалось подключиться к базе данных!";
                    break;
                case 4:
                    LoginStack.Visibility = Visibility.Collapsed;
                    MessageStack.Visibility = Visibility.Visible;
                    LoadingStack.Visibility = Visibility.Collapsed;
                    ResetStack.Visibility = Visibility.Collapsed;
                    var uriSource5 = new Uri(@"IMGs/UnkErr.png", UriKind.Relative);
                    ErrIMG.Source = new BitmapImage(uriSource5);
                    ErrText.Text = "Электронная почта введена неверно!";
                    break;
                case 5:
                    LoginStack.Visibility = Visibility.Collapsed;
                    MessageStack.Visibility = Visibility.Visible;
                    LoadingStack.Visibility = Visibility.Collapsed;
                    var uriSource6 = new Uri(@"IMGs/UnkErr.png", UriKind.Relative);
                    ErrIMG.Source = new BitmapImage(uriSource6);
                    ErrText.Text = "Вы не ввели электронную почту!";
                    break;
                case 6:
                    LoginStack.Visibility = Visibility.Collapsed;
                    MessageStack.Visibility = Visibility.Visible;
                    LoadingStack.Visibility = Visibility.Collapsed;
                    ResetStack.Visibility= Visibility.Collapsed;
                    var uriSource7 = new Uri(@"IMGs/UnkErr.png", UriKind.Relative);
                    ErrIMG.Source = new BitmapImage(uriSource7);
                    ErrText.Text = "Допустимая длина пароля - от 8 до 25 символов!";
                    break;
                case 7:
                    LoginStack.Visibility = Visibility.Collapsed;
                    MessageStack.Visibility = Visibility.Visible;
                    LoadingStack.Visibility = Visibility.Collapsed;
                    var uriSource8 = new Uri(@"IMGs/UnkErr.png", UriKind.Relative);
                    ErrIMG.Source = new BitmapImage(uriSource8);
                    ErrText.Text = "E-mail занят!";
                    break;
                case 8:
                    LoginStack.Visibility = Visibility.Collapsed;
                    MessageStack.Visibility = Visibility.Visible;
                    LoadingStack.Visibility = Visibility.Collapsed;
                    var uriSource9 = new Uri(@"IMGs/UnkErr.png", UriKind.Relative);
                    ErrIMG.Source = new BitmapImage(uriSource9);
                    ErrText.Text = "Ник занят!";
                    break;
                case 9:
                    LoginStack.Visibility = Visibility.Collapsed;
                    MessageStack.Visibility = Visibility.Visible;
                    LoadingStack.Visibility = Visibility.Collapsed;
                    ResetStack.Visibility = Visibility.Collapsed;
                    var uriSource10 = new Uri(@"IMGs/UnkErr.png", UriKind.Relative);
                    ErrIMG.Source = new BitmapImage(uriSource10);
                    ErrText.Text = "Указан неверный код!";
                    break;
                case 10:
                    LoginStack.Visibility = Visibility.Collapsed;
                    MessageStack.Visibility = Visibility.Visible;
                    LoadingStack.Visibility = Visibility.Collapsed;
                    ResetStack.Visibility = Visibility.Collapsed;
                    var uriSource11 = new Uri(@"IMGs/UnkErr.png", UriKind.Relative);
                    ErrIMG.Source = new BitmapImage(uriSource11);
                    ErrText.Text = "Код не был отправлен!";
                    break;
            }
        }
        internal void ErrUnknown(string resp)
        {
            if(RegStack.Visibility== Visibility.Visible) { RegStack.Visibility= Visibility.Collapsed; }
            else { LoginStack.Visibility = Visibility.Collapsed; }
            LoadingStack.Visibility =Visibility.Collapsed;
            MessageStack.Visibility = Visibility.Visible;
            var uriSource = new Uri(@"IMGs/UnkErr.png", UriKind.Relative);
            ErrIMG.Source = new BitmapImage(uriSource);
            ErrText.Text = "Неизвестная команда сервера: " + resp;
        }
        private void HomePageOpen(int id, string em,string nn,string nm,string sn, string pw, int pp)
        {
            HomePage homePage = new HomePage(id,em,nn,nm,sn,pw,pp);
            homePage.home = homePage;
            var MW = Application.Current.MainWindow as MainWindow;
            MW.MainFrame.Content = homePage;         
        }

        private void PWBCleaning(object sender, RoutedEventArgs e)
        {
            //tblPasswordHint.Visibility = pbPassword.Password.Length == 0 ? Visibility.Visible : Visibility.Hidden;
            if(pbPassword.Password.Length == 0)
            {
                tblPasswordHint.Visibility = Visibility.Collapsed;
            }
            else { }

        }

        private void PWBFilling(object sender, RoutedEventArgs e)
        {
            if(pbPassword.Password.Length == 0)
            {
                tblPasswordHint.Visibility = Visibility.Visible;
            }
            else { }
        }

        private void PassWordChanged(object sender, RoutedEventArgs e)
        {
            //tbPassword.Text = pbPassword.Password;
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

        private void TextBoxChanged(object sender, TextChangedEventArgs e)
        {
            //pbPassword.Password = "";
            //pbPassword.Password += tbPassword.Text;
        }

        private void PWVisibilitySwitch(object sender, RoutedEventArgs e)
        {
            if(swtVis == false)
            {
                var uriSource4 = new Uri(@"IMGs/hide.png", UriKind.Relative);
                PWVis.Source = new BitmapImage(uriSource4);
                tbPassword.Visibility = Visibility.Visible;
                pbPassword.Visibility = Visibility.Collapsed;
                swtVis = !swtVis;
            }
            else
            {
                var uriSource4 = new Uri(@"IMGs/show.png", UriKind.Relative);
                PWVis.Source = new BitmapImage(uriSource4);
                tbPassword.Visibility = Visibility.Collapsed;
                pbPassword.Visibility = Visibility.Visible;
                swtVis= !swtVis;
            }
        }

        private void PWVisibilitySwitch1(object sender, RoutedEventArgs e)
        {
            if (swtVis1 == false)

            {
                var uriSource4 = new Uri(@"IMGs/hide.png", UriKind.Relative);
                PWVis1.Source = new BitmapImage(uriSource4);
                PasswordTB.Visibility = Visibility.Visible;
                PasswordPB.Visibility = Visibility.Collapsed;
                swtVis1 = !swtVis1;
            }
            else
            {
                var uriSource4 = new Uri(@"IMGs/show.png", UriKind.Relative);
                PWVis1.Source = new BitmapImage(uriSource4);
                PasswordTB.Visibility = Visibility.Collapsed;
                PasswordPB.Visibility = Visibility.Visible;
                swtVis1 = !swtVis1;
            }

        }

        private void PasswordDown1(object sender, KeyEventArgs e)
        {
            PasswordTB.Text = PasswordPB.Password;

        }

        private void TextBoxDown1(object sender, KeyEventArgs e)
        {
            PasswordPB.Password = "";
            PasswordPB.Password += PasswordTB.Text;
        }

        private void PWBFilling1(object sender, RoutedEventArgs e)
        {
            if (PasswordPB.Password.Length == 0)
            {
                tbl1PasswordHint.Visibility = Visibility.Visible;
            }
            else { }

        }

        private void PWBCleaning1(object sender, RoutedEventArgs e)
        {
            if (PasswordPB.Password.Length == 0)
            {
                tbl1PasswordHint.Visibility = Visibility.Collapsed;
            }
            else { }

        }
        private void PWVisibilitySwitch3(object sender, RoutedEventArgs e)
        {
            if (swtVis3 == false)

            {
                var uriSource8 = new Uri(@"IMGs/hide.png", UriKind.Relative);
                PWVis3.Source = new BitmapImage(uriSource8);
                PasswordTB3.Visibility = Visibility.Visible;
                PasswordPB3.Visibility = Visibility.Collapsed;
                swtVis3 = !swtVis3;
            }
            else
            {
                var uriSource4 = new Uri(@"IMGs/show.png", UriKind.Relative);
                PWVis3.Source = new BitmapImage(uriSource4);
                PasswordTB3.Visibility = Visibility.Collapsed;
                PasswordPB3.Visibility = Visibility.Visible;
                swtVis3 = !swtVis3;
            }

        }

        private void PasswordDown3(object sender, KeyEventArgs e)
        {
            PasswordTB3.Text = PasswordPB3.Password;

        }

        private void TextBoxDown3(object sender, KeyEventArgs e)
        {
            PasswordPB3.Password = "";
            PasswordPB3.Password += PasswordTB3.Text;
        }

        private void PWBFilling3(object sender, RoutedEventArgs e)
        {
            if (PasswordPB3.Password.Length == 0)
            {
                tblPasswordHint3.Visibility = Visibility.Visible;
            }
            else { }

        }

        private void PWBCleaning3(object sender, RoutedEventArgs e)
        {
            if (PasswordPB3.Password.Length == 0)
            {
                tblPasswordHint3.Visibility = Visibility.Collapsed;
            }
            else { }

        }
        private void SendResetQuery(object sender, RoutedEventArgs e)
        {
            publLBL.Visibility = Visibility.Collapsed;
            loadLBL.Visibility = Visibility.Visible;
            ResetStack.IsEnabled = false;
            SendResetQueryAsync();
        }
        private async Task SendResetQueryAsync()
        {
            string a = ResEmailTB.Text;
            Regex regex = new Regex(@"\w*@\w*.\w*");
            MatchCollection matches = regex.Matches(a);
            if (matches.Count > 0)
            {
                await Task.Run(() => SRQ(a));
            }
            else
            {
                ErrOut(4);
                publLBL.Visibility = Visibility.Visible;
                loadLBL.Visibility = Visibility.Collapsed;
                ResetStack.IsEnabled = true;

            }
        }
        private void SRQ(string email)
        {
            string resp = HomeSender.SendEmailResetQuery(email);
            Dispatcher.Invoke(() =>
            {
                if (resp == "OK")
                {
                    FirstResetStage.Visibility = Visibility.Collapsed;
                    SecondResetStage.Visibility = Visibility.Visible;
                    publLBL.Visibility = Visibility.Visible;
                    loadLBL.Visibility = Visibility.Collapsed;
                    ResetStack.IsEnabled = true;
                }
                else if(resp == "NotSended")
                {
                    ErrOut(10);
                    publLBL.Visibility = Visibility.Visible;
                    loadLBL.Visibility = Visibility.Collapsed;
                    ResetStack.IsEnabled = true;
                }
                else if (resp == "ServerNotResponding")
                {
                    ErrOut(0);
                    publLBL.Visibility = Visibility.Visible;
                    loadLBL.Visibility = Visibility.Collapsed;
                    ResetStack.IsEnabled = true;
                }

            });
        }
        private void SendCode(object sender, RoutedEventArgs e)
        {
            publLBL2.Visibility = Visibility.Collapsed;
            loadLBL2.Visibility = Visibility.Visible;
            ResetStack.IsEnabled = false;
            SendCodeAsync();
        }
        private async Task SendCodeAsync()
        {
            string a = ResEmailTB.Text;
            string b = CodeTBL.Text;
            string c = PasswordPB3.Password;
            if(c.Length >= 8 && c.Length <= 25)
            {
                await Task.Run(() => CodeSender(a,b,c));
            }
            else
            {
                ErrOut(6);
                publLBL.Visibility = Visibility.Visible;
                loadLBL.Visibility = Visibility.Collapsed;
                ResetStack.IsEnabled = true;

            }
        }
        private void CodeSender(string email, string code, string password)
        {
            string resp = HomeSender.SendResCode(email, code, password);
            Console.WriteLine(resp);
            Dispatcher.Invoke(() =>
            {
                if (resp == "Success")
                {
                    ResetStack.Visibility = Visibility.Collapsed;
                    LoginStack.Visibility = Visibility.Visible;
                    publLBL2.Visibility = Visibility.Visible;
                    loadLBL2.Visibility = Visibility.Collapsed;
                    ResetStack.IsEnabled = true;
                }
                else if (resp == "NotVerified")
                {
                    ErrOut(9);
                    publLBL2.Visibility = Visibility.Visible;
                    loadLBL2.Visibility = Visibility.Collapsed;
                    ResetStack.IsEnabled = true;
                }
                else if (resp == "ServerNotResponding")
                {
                    ErrOut(0);
                    publLBL2.Visibility = Visibility.Visible;
                    loadLBL2.Visibility = Visibility.Collapsed;
                    ResetStack.IsEnabled = true;
                }
            });

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            ResetStack.Visibility = Visibility.Visible;
            LoginStack.Visibility = Visibility.Collapsed;
        }

        private void UndoReset(object sender, RoutedEventArgs e)
        {
            ResetStack.Visibility = Visibility.Collapsed;
            LoginStack.Visibility = Visibility.Visible;
        }
    }
}
