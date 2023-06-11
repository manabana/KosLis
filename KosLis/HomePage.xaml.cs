using System;
using System.Collections.Generic;
using System.Deployment.Internal;
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
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace KosLis
{
    /// <summary>
    /// Логика взаимодействия для HomePage.xaml
    /// </summary>
    public partial class HomePage : Page
    {
        private int userId;
        private string Email;
        private string Nickname;
        private string Name;
        private string Surname;
        private string Password;
        private int Photo;
        public HomePage home { get; set; }
        public HomePage(int id,string email, string nickname, string name, string surname, string passw, int photo)
        {
            InitializeComponent();
            userId= id;
            Email = email;
            Nickname = nickname;
            Name = name;
            Surname = surname; 
            Password = passw;
            Photo = photo;
            var uriSource3 = new Uri($@"IMGs/PPs/{photo}.jpg", UriKind.Relative);
            PP.ImageSource = new BitmapImage(uriSource3);
            nicknameLBL.Content = Nickname;
            ShowFeedBT(null, null);
        }
        public void RefreshPhoto(int newPhotoId)
        {
            var uriSource3 = new Uri($@"IMGs/PPs/{newPhotoId}.jpg", UriKind.Relative);
            PP.ImageSource = new BitmapImage(uriSource3);
            Photo = newPhotoId;

        }
        public void OpenSome1Profile(int id)
        {
            var MW = Application.Current.MainWindow as MainWindow;
            MW.ShowSome1Prof(id);

        }
        private void OpenS1P(int id)
        {
            ContentFrame.Content = new Some1ProfilePage(id);
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
            ContentFrame.Content = new ProfilePage(userId, Email, Nickname, Name, Surname, Password, Photo, home);
        }

        private void NewPostBT(object sender, RoutedEventArgs e)
        {
            ContentFrame.Content = new PostPage(userId, Password);
        }

        private void ShowFeedBT(object sender, RoutedEventArgs e)
        {
            ContentFrame.Content = new PostFeedPage();
        }

        private void OpenOptions(object sender, RoutedEventArgs e)
        {
            ContentFrame.Content = new OptionsPage(userId, Email, Password);
        }

        private void MessengerOpen(object sender, RoutedEventArgs e)
        {
            ContentFrame.Content = new MessengerPage(userId, Nickname, Password);
        }
        private void OpenFriends(object sender, RoutedEventArgs e)
        {
            ContentFrame.Content = new FriendsPage(userId, Password);

        }

        private void Logout(object sender, RoutedEventArgs e)
        {
            if (File.Exists("login.bin"))
            {
                File.Delete("login.bin");
            }
            var MW = Application.Current.MainWindow as MainWindow;
            MW.LogOut();
        }
    }
}
