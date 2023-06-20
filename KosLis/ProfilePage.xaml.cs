using QuerySender;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
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
    /// Логика взаимодействия для ProfilePage.xaml
    /// </summary>
    public partial class ProfilePage : Page
    {
        int ID;
        string Email;
        string Nickname;
        string Name;
        string Surname;
        string Password;
        int Photo;
        HomePage Home;
        public ProfilePage(int id, string em,string nn,string nm,string sn, string pw, int pf, HomePage home)
        {
            InitializeComponent();
            ID = id;
            Email = em;
            Nickname = nn;
            Name = nm;
            Surname = sn;
            Password = pw;
            Photo = pf;
            Home = home;

            var uriSource3 = new Uri($@"IMGs/PPs/{pf}.jpg", UriKind.Relative);
            PP.ImageSource = new BitmapImage(uriSource3);
            EMailTB.Content= Email;
            NicknameTB.Content= Nickname;
            NameTB.Content= Name;
            SurnameTB.Content= Surname;
            DisplayFeed();

        }
        private void DisplayFeed()
        {
            List<Posts> posts = new List<Posts>();

            string resp = HomeSender.AskPosts(ID, AskPostsType.UserPosts);
            string[] splitedA = resp.Split('|');
            for (int i = 0; i < splitedA.Count() - 1; i++)
            {
                string[] splitedB = splitedA[i].Split(';');
                string checker = HomeSender.CheckPostImage(int.Parse(splitedB[0]));
                if (checker == "True")
                {
                    string req = HomeSender.AskPostImageSize(int.Parse(splitedB[0]));
                    int size;
                    if (false)
                    {
                        size = 0;
                    }
                    else
                    {
                        size = int.Parse(req);
                    }

                    byte[] bytes = HomeSender.AskPostImage(int.Parse(splitedB[0]), size);
                    string debyted = Encoding.UTF8.GetString(bytes, 0, 256);
                    BitmapImage bitmap = Dispatcher.Invoke(() => DrawingToBitmap(ByteArrayToImage(bytes)));
                    posts.Add(new Posts(int.Parse(splitedB[0]), splitedB[1], int.Parse(splitedB[2]), splitedB[3], splitedB[4], splitedB[5], bitmap, int.Parse(splitedB[6]), int.Parse(splitedB[7])));
                }
                else
                {
                    posts.Add(new Posts(int.Parse(splitedB[0]), splitedB[1], int.Parse(splitedB[2]), splitedB[3], splitedB[4], splitedB[5], null, int.Parse(splitedB[6]), int.Parse(splitedB[7])));
                }
            }
            posts = posts.OrderByDescending(p => p.postId).ToList();
            FeedList.ItemsSource = null;
            FeedList.ItemsSource = posts;
        }
        public System.Drawing.Image ByteArrayToImage(byte[] byteArrayIn)
        {
            using (MemoryStream mStream = new MemoryStream(byteArrayIn))
            {
                return System.Drawing.Image.FromStream(mStream);
            }
        }
        public BitmapImage DrawingToBitmap(System.Drawing.Image image)
        {
            using (var stream = new MemoryStream())
            {
                image.Save(stream, ImageFormat.Bmp);// bitmapGDI - System.Drawing.Image
                BitmapImage bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.StreamSource = new MemoryStream(stream.ToArray());
                bitmap.EndInit();

                return bitmap; // bitmap - это WPF'овский BitmapImage
            };

        }

        private void Rate(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            int postid = int.Parse(button.Tag.ToString());
            string resp = "Unknown";
            if (button.Content.ToString() == "🡹")
            {
                resp = HomeSender.RateChange(RateType.Up, postid);
                if (resp == "success")
                {
                    DisplayFeed();
                }
            }
            else
            {
                resp = HomeSender.RateChange(RateType.Down, postid);
                if (resp == "success")
                {
                    DisplayFeed();
                }

            }
        }

        private void EditEmail(object sender, RoutedEventArgs e)
        {
            EmailEditor.Text = Email;

            EMailTB.Visibility = Visibility.Collapsed;
            ConfirmEmailEdit.Visibility = Visibility.Visible;
            UndoEmailEdit.Visibility = Visibility.Visible;
            EmailEditor.Visibility = Visibility.Visible;
            EmailEditBT.Visibility = Visibility.Collapsed;
        }

        private void EditNickname(object sender, RoutedEventArgs e)
        {
            NicknameEditor.Text = Nickname;

            NicknameTB.Visibility = Visibility.Collapsed;
            ConfirmNicknameEdit.Visibility = Visibility.Visible;
            UndoNicknameEdit.Visibility = Visibility.Visible;
            NicknameEditor.Visibility = Visibility.Visible;
            NicknameEditBT.Visibility = Visibility.Collapsed;

        }

        private void EditName(object sender, RoutedEventArgs e)
        {
            NameEditor.Text = Name;

            NameTB.Visibility = Visibility.Collapsed;
            ConfirmNameEdit.Visibility = Visibility.Visible;
            UndoNameEdit.Visibility = Visibility.Visible;
            NameEditor.Visibility = Visibility.Visible;
            NameEditBT.Visibility = Visibility.Collapsed;

        }

        private void EditSurname(object sender, RoutedEventArgs e)
        {
            SurnameEditor.Text = Surname;

            SurnameTB.Visibility = Visibility.Collapsed;
            ConfirmSurnameEdit.Visibility = Visibility.Visible;
            UndoSurnameEdit.Visibility = Visibility.Visible;
            SurnameEditor.Visibility = Visibility.Visible;
            SurnameEditBT.Visibility = Visibility.Collapsed;

        }
        private void AnimationIn(object sender, MouseEventArgs e)
        {
            //DoubleAnimation borderup = new DoubleAnimation();
            //borderup.From = Round.StrokeThickness;
            //borderup.To = Round.StrokeThickness + 2;
            //borderup.Duration = TimeSpan.FromSeconds(0.25);
            //Round.BeginAnimation(Ellipse.StrokeThicknessProperty, borderup);

        }

        private void AnimationOut(object sender, MouseEventArgs e)
        {
            //DoubleAnimation borderdown = new DoubleAnimation();
            //borderdown.From = Round.StrokeThickness;
            //borderdown.To = Round.StrokeThickness - 2;
            //borderdown.Duration = TimeSpan.FromSeconds(0.25);
            //Round.BeginAnimation(Ellipse.StrokeThicknessProperty, borderdown);
        }

        private void ToProfileIn(object sender, MouseEventArgs e)
        {
            DoubleAnimation dblAnim = new DoubleAnimation
            {
                From = 0.0,
                To = 1.0,
                Duration = new Duration(TimeSpan.FromMilliseconds(250))
            };
            rndAnim.BeginAnimation(Ellipse.OpacityProperty, dblAnim);
        }

        private void ToProfileOut(object sender, MouseEventArgs e)
        {
            DoubleAnimation dblAnim = new DoubleAnimation
            {
                From = 1.0,
                To = 0.0,
                Duration = new Duration(TimeSpan.FromMilliseconds(250))
            };
            rndAnim.BeginAnimation(Ellipse.OpacityProperty, dblAnim);
        }

        private void ToProfileBT(object sender, RoutedEventArgs e)
        {
            string tag1 = $"Check{Photo}"; // тег, который вы хотите найти
            string tag = $"Image{Photo}";
            System.Windows.Controls.Image image = (System.Windows.Controls.Image)ImageContainer.FindName(tag);
            Label label = (Label)ImageContainer.FindName(tag1);

            BlurEffect blur = new BlurEffect { Radius = 5 };
            if(image != null && label != null)
            {
                image.Effect = blur;
                label.Visibility = Visibility.Visible;
            }

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
            PPSelectorGrid.Visibility = Visibility.Visible;
            TopGrid.IsEnabled = false;
            TopGrid.Effect.BeginAnimation(BlurEffect.RadiusProperty, animation);
            PPSelectorGrid.BeginAnimation(OpacityProperty, animation2);
        }

        private void UndoClicked(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            switch (button.Name) 
            {
                case "UndoEmailEdit":
                    EMailTB.Visibility = Visibility.Visible;
                    ConfirmEmailEdit.Visibility = Visibility.Collapsed;
                    UndoEmailEdit.Visibility = Visibility.Collapsed;
                    EmailEditor.Visibility = Visibility.Collapsed;
                    EmailEditBT.Visibility = Visibility.Visible;
                    break;
                case "UndoNicknameEdit":
                    NicknameTB.Visibility = Visibility.Visible;
                    ConfirmNicknameEdit.Visibility = Visibility.Collapsed;
                    UndoNicknameEdit.Visibility = Visibility.Collapsed;
                    NicknameEditor.Visibility = Visibility.Collapsed;
                    NicknameEditBT.Visibility = Visibility.Visible;
                    break;
                case "UndoNameEdit":
                    NameTB.Visibility = Visibility.Visible;
                    ConfirmNameEdit.Visibility = Visibility.Collapsed;
                    UndoNameEdit.Visibility = Visibility.Collapsed;
                    NameEditor.Visibility = Visibility.Collapsed;
                    NameEditBT.Visibility = Visibility.Visible;
                    break;
                case "UndoSurnameEdit":
                    SurnameTB.Visibility = Visibility.Visible;
                    ConfirmSurnameEdit.Visibility = Visibility.Collapsed;
                    UndoSurnameEdit.Visibility = Visibility.Collapsed;
                    SurnameEditor.Visibility = Visibility.Collapsed;
                    SurnameEditBT.Visibility = Visibility.Visible;
                    break;
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
            DisplayFeed();

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
            TopGrid.IsEnabled = false;
            TopGrid.Effect.BeginAnimation(BlurEffect.RadiusProperty, animation);
            MessageGrid.BeginAnimation(OpacityProperty, animation2);

        }

        private void ApplyEmail(object sender, RoutedEventArgs e)
        {
            string resp = HomeSender.EditUser("email", ID, EmailEditor.Text, Password);
            if (resp == "OK")
            {
                MessageFrame("Электронная почта изменена!", MessageType.Successful);
                EMailTB.Content = EmailEditor.Text;
            }
            else if (resp == "ServerNotResponding")
            {
                MessageFrame("Не удается подключиться к серверу!", MessageType.Error);
            }
            else if (resp == "SqlWillNotStarted")
            {
                MessageFrame("Серверу не удалось подключиться к базе данных!", MessageType.Error);
            }

        }

        private void DeletePost(object sender, RoutedEventArgs e)
        {
            var but = sender as Button;
            string resp = HomeSender.DeletePost(int.Parse(but.Tag.ToString()), Password);
            if(resp == "OK")
            {
                MessageFrame("Пост удален!", MessageType.Successful);
            }
            else if(resp == "ServerNotResponding")
            {
                MessageFrame("Не удается подключиться к серверу!", MessageType.Error);
            }
            else if(resp == "SqlWillNotStarted")
            {
                MessageFrame("Серверу не удалось подключиться к базе данных!", MessageType.Error);
            }
        }

        private void NameConfirm(object sender, RoutedEventArgs e)
        {
            string resp = HomeSender.EditUser("name", ID, NameEditor.Text, Password);
            if (resp == "OK")
            {
                MessageFrame("Имя изменено!", MessageType.Successful);
                NameTB.Content = NameEditor.Text;
            }
            else if (resp == "ServerNotResponding")
            {
                MessageFrame("Не удается подключиться к серверу!", MessageType.Error);
            }
            else if (resp == "SqlWillNotStarted")
            {
                MessageFrame("Серверу не удалось подключиться к базе данных!", MessageType.Error);
            }

        }

        private void SurnameConfirm(object sender, RoutedEventArgs e)
        {
            string resp = HomeSender.EditUser("surname", ID, SurnameEditor.Text, Password);
            if (resp == "OK")
            {
                MessageFrame("Фамилия изменена!", MessageType.Successful);
                SurnameTB.Content = SurnameEditor.Text;
            }
            else if (resp == "ServerNotResponding")
            {
                MessageFrame("Не удается подключиться к серверу!", MessageType.Error);
            }
            else if (resp == "SqlWillNotStarted")
            {
                MessageFrame("Серверу не удалось подключиться к базе данных!", MessageType.Error);
            }

        }

        private void NicknameConfirm(object sender, RoutedEventArgs e)
        {
            string resp = HomeSender.EditUser("nickname", ID, NicknameEditor.Text, Password);
            if (resp == "OK")
            {
                MessageFrame("Имя пользователя изменено!", MessageType.Successful);
                NicknameTB.Content = NicknameEditor.Text;
            }
            else if (resp == "ServerNotResponding")
            {
                MessageFrame("Не удается подключиться к серверу!", MessageType.Error);
            }
            else if (resp == "SqlWillNotStarted")
            {
                MessageFrame("Серверу не удалось подключиться к базе данных!", MessageType.Error);
            }

        }

        private void NavigateBack(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            TopGrid.IsEnabled = true;
            BlurEffect blurEffect = new BlurEffect();
            TopGrid.Effect = blurEffect;
            DoubleAnimation animation = new DoubleAnimation();
            animation.From = 16;
            animation.To = 0;
            animation.Duration = TimeSpan.FromSeconds(0.2);
            PPSelectorGrid.Visibility = Visibility.Collapsed;
            TopGrid.Effect.BeginAnimation(BlurEffect.RadiusProperty, animation);
            TopGrid.Effect = null;
        }
        private void ResetAllPhotos()
        {
            for(int i = 1; i <= 10; i++)
            {
                string tag1 = $"Check{i}"; // тег, который вы хотите найти
                string tag = $"Image{i}";
                System.Windows.Controls.Image image = (System.Windows.Controls.Image)ImageContainer.FindName(tag);
                Label label = (Label)ImageContainer.FindName(tag1);
                image.Effect = null;
                label.Visibility = Visibility.Collapsed;
            }
        }
        private void PhotoSelected(object sender, RoutedEventArgs e)
        {
            ResetAllPhotos();
            var button = sender as Button;
            int pp = int.Parse(button.Tag.ToString());
            string q = HomeSender.ChangeProfilePhoto(ID, pp, Password);
            if (q == "OK")
            {
                string tag1 = $"Check{pp}"; // тег, который вы хотите найти
                string tag = $"Image{pp}";
                System.Windows.Controls.Image image = (System.Windows.Controls.Image)ImageContainer.FindName(tag);
                Label label = (Label)ImageContainer.FindName(tag1);

                BlurEffect blur = new BlurEffect {Radius = 5};
                image.Effect = blur;
                label.Visibility = Visibility.Visible;
                var uriSource3 = new Uri($@"IMGs/PPs/{pp}.jpg", UriKind.Relative);
                PP.ImageSource = new BitmapImage(uriSource3);
                Home.RefreshPhoto(pp);

            }
            else if (q == "ServerNotResponding")
            {
                NavigateBack(null,null);
                MessageFrame("Не удалось подключиться к серверу!", MessageType.Error);
            }
            else if (q == "SqlWillNotStarted")
            {
                NavigateBack(null, null);
                MessageFrame("Серверу не удалось подключиться к базе данных!", MessageType.Error);
            }
            else if(q == "ICPassword")
            {
                NavigateBack(null, null);
                MessageFrame("Система безопасности отклонила доступ!", MessageType.Error);
            }
            else if(q == "UserNotExist")
            {
                NavigateBack(null, null);
                MessageFrame("Пользователь не существует!", MessageType.Error);
            }
        }
    }
}
