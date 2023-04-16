using System;
using System.Collections.Generic;
using System.Deployment.Internal;
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
        public HomePage(int id,string email, string nickname, string name, string surname, string passw)
        {
            InitializeComponent();
            userId= id;
            Email = email;
            Nickname = nickname;
            Name = name;
            Surname = surname; 
            Password = passw;
            
            nicknameLBL.Content = Nickname;
            ShowFeedBT(null, null);
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
            ProfilePage profilePage = new ProfilePage(userId, Email,Nickname,Name,Surname);
            ContentFrame.Content = profilePage;
        }

        private void NewPostBT(object sender, RoutedEventArgs e)
        {
            PostPage postPage = new PostPage(userId, Password);
            ContentFrame.Content = postPage;
        }

        private void ShowFeedBT(object sender, RoutedEventArgs e)
        {
            PostFeedPage postFeed = new PostFeedPage();
            ContentFrame.Content = postFeed;
        }

        private void OpenOptions(object sender, RoutedEventArgs e)
        {
            var page = new OptionsPage();
            ContentFrame.Content = page;
        }

        private void MessengerOpen(object sender, RoutedEventArgs e)
        {
            var mespage = new MessengerPage(userId,Nickname,Password);
            ContentFrame.Content = mespage;
        }
        private void OpenFriends(object sender, RoutedEventArgs e)
        {
            FriendsPage friends = new FriendsPage(userId);
            ContentFrame.Content = friends;

        }

        private void Logout(object sender, RoutedEventArgs e)
        {
            var MW = Application.Current.MainWindow as MainWindow;
            MW.LogOut();
        }
    }
}
