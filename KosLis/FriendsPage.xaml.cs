using QuerySender;
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
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using static KosLis.MessengerPage;

namespace KosLis
{
    /// <summary>
    /// Логика взаимодействия для FriendsPage.xaml
    /// </summary>
    public partial class FriendsPage : Page
    {
        internal int UserID { get; set; }
        private string Password;
        public FriendsPage(int userid, string password)
        {
            InitializeComponent();
            UserID = userid;
            Password = password;
            UserListView.ItemsSource = DisplayFriends();
            UserSentListView.ItemsSource = DisplaySends();
            UserRecieveListView.ItemsSource = DisplayReceives();
        }

        private void AddFriend(object sender, RoutedEventArgs e)
        {
            MessageFrame($"Вы действительно хотите добавить пользователя {FriendTB.Text} в друзья?", MessageType.Confirmation);
        }


        private void MessageFrame(string message, MessageType type)
        {
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
        private void ButClicked(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            TopGrid.IsEnabled = true;
            if(button.Tag.ToString().IndexOf("confirm") > -1)
            {
                if(button.Tag.ToString() == "confirmYes")
                {
                    string req = HomeSender.AddFriend(UserID, FriendTB.Text, Password);
                    if(req == "OK")
                    {
                        Button1.Tag = "";
                        Button2.Tag = "";
                        Button2.Visibility = Visibility.Visible;
                        MessageFrame($"Заявка в друзья отправлена пользователю {FriendTB.Text}", MessageType.Successful);
                        UserListView.ItemsSource = DisplayFriends();
                        UserSentListView.ItemsSource = DisplaySends();
                        UserRecieveListView.ItemsSource = DisplayReceives();
                        return;
                    }
                    if(req == "alreadyRecieved")
                    {
                        Button1.Tag = "";
                        Button2.Tag = "";
                        Button2.Visibility = Visibility.Visible;
                        MessageFrame($"Пользователь {FriendTB.Text} уже отправил вам заявку", MessageType.Error);
                        return;

                    }
                    if (req == "alreadySended")
                    {
                        Button1.Tag = "";
                        Button2.Tag = "";
                        Button2.Visibility = Visibility.Visible;
                        MessageFrame($"Заявка пользователю {FriendTB.Text} была отправлена ранее", MessageType.Error);
                        return;

                    }
                    if (req == "alreadyFriends")
                    {
                        Button1.Tag = "";
                        Button2.Tag = "";
                        Button2.Visibility = Visibility.Visible;
                        MessageFrame($"Пользователь {FriendTB.Text} уже ваш друг", MessageType.Error);
                        return;

                    }

                    if (req == "senderIsReceiver")
                    {
                        Button1.Tag = "";
                        Button2.Tag = "";
                        Button2.Visibility = Visibility.Visible;
                        MessageFrame("Невозможно стать другом самому себе", MessageType.Error);
                        return;
                    }
                    else if(req == "friendNotFound")
                    {
                        Button1.Tag = "";
                        Button2.Tag = "";
                        Button2.Visibility = Visibility.Visible;
                        MessageFrame("Такого пользователя не существует", MessageType.Error);
                        return;
                    }
                    else if (req == "error")
                    {
                        Button1.Tag = "";
                        Button2.Tag = "";
                        Button2.Visibility = Visibility.Visible;
                        MessageFrame("Неизвестная ошибка", MessageType.Error);
                        return;
                    }
                    else if (req == "Exception;ServerNotResponding")
                    {
                        Button1.Tag = "";
                        Button2.Tag = "";
                        Button2.Visibility = Visibility.Visible;
                        MessageFrame("Не удалось подключиться к серверу", MessageType.Error);
                        return;

                    }
                }
                Button1.Tag = null;
                Button2.Tag = null;
                Button2.Visibility = Visibility.Visible;
                
            }
            BlurEffect blurEffect = new BlurEffect();
            TopGrid.Effect = blurEffect;
            DoubleAnimation animation = new DoubleAnimation();
            animation.From = 16;
            animation.To = 0;
            animation.Duration = TimeSpan.FromSeconds(0.2);
            MessageGrid.Visibility = Visibility.Collapsed;
            TopGrid.Effect.BeginAnimation(BlurEffect.RadiusProperty, animation);

        }
        private List<Users> DisplayFriends()
        {
            List<Users> users = new List<Users>();
            users.Clear();
            string resp = HomeSender.AskUsers(UserID, AskUsersType.AskFriends);
            Console.WriteLine(resp);
            string[] splitedA = resp.Split('|');
            for (int i = 0; i < splitedA.Count() - 1; i++)
            {
                string[] splitedB = splitedA[i].Split(';');
                users.Add(new Users(int.Parse(splitedB[0]), splitedB[1]));

            }
            return users;
        }
        private List<Users> DisplayReceives()
        {
            List<Users> users = new List<Users>();
            users.Clear();
            string resp = HomeSender.AskUsers(UserID, AskUsersType.AskReceiveRequests);
            Console.WriteLine(resp);
            string[] splitedA = resp.Split('|');
            for (int i = 0; i < splitedA.Count() - 1; i++)
            {
                string[] splitedB = splitedA[i].Split(';');
                users.Add(new Users(int.Parse(splitedB[0]), splitedB[1]));

            }
            return users;

        }
        private List<Users> DisplaySends()
        {
            List<Users> users = new List<Users>();
            users.Clear();
            string resp = HomeSender.AskUsers(UserID, AskUsersType.AskSendRequests);
            Console.WriteLine(resp);
            string[] splitedA = resp.Split('|');
            for (int i = 0; i < splitedA.Count() - 1; i++)
            {
                string[] splitedB = splitedA[i].Split(';');
                users.Add(new Users(int.Parse(splitedB[0]), splitedB[1]));

            }
            return users;

        }
        private void SwitchToFriends(object sender, RoutedEventArgs e)
        {
            FriendsBT.Foreground = Brushes.White;
            FriendsBT.Background = new SolidColorBrush(Color.FromRgb(81, 45, 168));
            FriendsBT.IsEnabled = false;
            SentFriendsBT.IsEnabled = true;
            ReceivedFriendsBT.IsEnabled = true;
            FriendsBT.Opacity = 1;
            SentFriendsBT.Foreground = new SolidColorBrush(Color.FromRgb(81, 45, 168));
            SentFriendsBT.Background = null;
            ReceivedFriendsBT.Background = null;
            ReceivedFriendsBT.Foreground = new SolidColorBrush(Color.FromRgb(81, 45, 168));
            RecieveFriends.Visibility = Visibility.Collapsed;
            SentFriends.Visibility = Visibility.Collapsed;
            CurrentFriends.Visibility = Visibility.Visible;
        }

        private void SwitchToSentFriends(object sender, RoutedEventArgs e)
        {
            SentFriendsBT.Foreground = Brushes.White;
            SentFriendsBT.Background = new SolidColorBrush(Color.FromRgb(81, 45, 168));
            SentFriendsBT.IsEnabled = false;
            FriendsBT.IsEnabled = true;
            ReceivedFriendsBT.IsEnabled= true;
            SentFriendsBT.Opacity = 1;
            FriendsBT.Foreground = new SolidColorBrush(Color.FromRgb(81, 45, 168));
            FriendsBT.Background = null;
            ReceivedFriendsBT.Background = null;
            ReceivedFriendsBT.Foreground = new SolidColorBrush(Color.FromRgb(81, 45, 168));
            RecieveFriends.Visibility = Visibility.Collapsed;
            CurrentFriends.Visibility = Visibility.Collapsed;
            SentFriends.Visibility = Visibility.Visible;

        }

        private void SwitchToRecievedToFriends(object sender, RoutedEventArgs e)
        {
            ReceivedFriendsBT.Foreground = Brushes.White;
            ReceivedFriendsBT.Background = new SolidColorBrush(Color.FromRgb(81, 45, 168));
            ReceivedFriendsBT.IsEnabled = false;
            SentFriendsBT.IsEnabled = true;
            FriendsBT.IsEnabled = true;
            ReceivedFriendsBT.Opacity = 1;
            SentFriendsBT.Foreground = new SolidColorBrush(Color.FromRgb(81, 45, 168));
            SentFriendsBT.Background = null;
            FriendsBT.Background = null;
            FriendsBT.Foreground = new SolidColorBrush(Color.FromRgb(81, 45, 168));
            CurrentFriends.Visibility = Visibility.Collapsed;
            SentFriends.Visibility = Visibility.Collapsed;
            RecieveFriends.Visibility = Visibility.Visible;
        }
        private void DeleteFriend(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            int targetId = int.Parse(button.Tag.ToString());
            int removerid = UserID;
            string password = Password;
            string resp = HomeSender.RemoveFriend(targetId, removerid, password);
            if (resp == "OK")
            {
                UserListView.ItemsSource = DisplayFriends();
                UserSentListView.ItemsSource = DisplaySends();
                UserRecieveListView.ItemsSource = DisplayReceives();
                Button1.Tag = "";
                Button2.Tag = "";
                Button2.Visibility = Visibility.Visible;
                MessageFrame("Пользователь удален из друзей!", MessageType.Successful);
            }
            else if (resp == "already")
            {
                UserListView.ItemsSource = DisplayFriends();
                UserSentListView.ItemsSource = DisplaySends();
                UserRecieveListView.ItemsSource = DisplayReceives();
                Button1.Tag = "";
                Button2.Tag = "";
                Button2.Visibility = Visibility.Visible;
                MessageFrame("Вы не друзья!", MessageType.Error);
            }
            else if (resp == "ICPassword")
            {
                Button1.Tag = "";
                Button2.Tag = "";
                Button2.Visibility = Visibility.Visible;
                MessageFrame("Система безопасности отлонила доступ!", MessageType.Error);
            }
            else if (resp == "Exception;SQLWillNotStarted")
            {
                Button1.Tag = "";
                Button2.Tag = "";
                Button2.Visibility = Visibility.Visible;
                MessageFrame("Серверу не удалось подключиться к базе данных!", MessageType.Error);
            }
            else if (resp == "Exception;ServerNotResponding")
            {
                Button1.Tag = "";
                Button2.Tag = "";
                Button2.Visibility = Visibility.Visible;
                MessageFrame("Сервер недоступен!", MessageType.Error);
            }

        }

        private void AcceptRequestBT(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            int acceptor = UserID;
            int senderId = int.Parse(button.Tag.ToString());
            string password = Password;
            string resp = HomeSender.AcceptFriendRequest(senderId, acceptor, password);
            if (resp == "OK")
            {
                UserListView.ItemsSource = DisplayFriends();
                UserSentListView.ItemsSource = DisplaySends();
                UserRecieveListView.ItemsSource = DisplayReceives();
                Button1.Tag = "";
                Button2.Tag = "";
                Button2.Visibility = Visibility.Visible;
                MessageFrame("Теперь вы друзья!", MessageType.Successful);
            }
            else if (resp == "already")
            {
                UserListView.ItemsSource = DisplayFriends();
                UserSentListView.ItemsSource = DisplaySends();
                UserRecieveListView.ItemsSource = DisplayReceives();
                Button1.Tag = "";
                Button2.Tag = "";
                Button2.Visibility = Visibility.Visible;
                MessageFrame("Вы уже друзья!", MessageType.Error);
            }
            else if (resp == "ICPassword")
            {
                Button1.Tag = "";
                Button2.Tag = "";
                Button2.Visibility = Visibility.Visible;
                MessageFrame("Система безопасности отлонила доступ!", MessageType.Error);
            }
            else if (resp == "Exception;SQLWillNotStarted")
            {
                Button1.Tag = "";
                Button2.Tag = "";
                Button2.Visibility = Visibility.Visible;
                MessageFrame("Серверу не удалось подключиться к базе данных!", MessageType.Error);
            }
            else if (resp == "Exception;ServerNotResponding")
            {
                Button1.Tag = "";
                Button2.Tag = "";
                Button2.Visibility = Visibility.Visible;
                MessageFrame("Сервер недоступен!", MessageType.Error);
            }
        }


        private void CancelRequest(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            int targetId = int.Parse(button.Tag.ToString());
            int cancellerId = UserID;
            string password = Password;
            string resp = HomeSender.CancelRequest(targetId, cancellerId, password);
            if (resp == "OK")
            {
                UserListView.ItemsSource = DisplayFriends();
                UserSentListView.ItemsSource = DisplaySends();
                UserRecieveListView.ItemsSource = DisplayReceives();
                Button1.Tag = "";
                Button2.Tag = "";
                Button2.Visibility = Visibility.Visible;
                MessageFrame("Заявка отменена!", MessageType.Successful);
            }
            else if (resp == "already")
            {
                UserListView.ItemsSource = DisplayFriends();
                UserSentListView.ItemsSource = DisplaySends();
                UserRecieveListView.ItemsSource = DisplayReceives();
                Button1.Tag = "";
                Button2.Tag = "";
                Button2.Visibility = Visibility.Visible;
                MessageFrame("Заявки и так не было!", MessageType.Error);
            }
            else if (resp == "ICPassword")
            {
                Button1.Tag = "";
                Button2.Tag = "";
                Button2.Visibility = Visibility.Visible;
                MessageFrame("Система безопасности отлонила доступ!", MessageType.Error);
            }
            else if (resp == "Exception;SQLWillNotStarted")
            {
                Button1.Tag = "";
                Button2.Tag = "";
                Button2.Visibility = Visibility.Visible;
                MessageFrame("Серверу не удалось подключиться к базе данных!", MessageType.Error);
            }
            else if (resp == "Exception;ServerNotResponding")
            {
                Button1.Tag = "";
                Button2.Tag = "";
                Button2.Visibility = Visibility.Visible;
                MessageFrame("Сервер недоступен!", MessageType.Error);
            }



        }

        private void RejectRequest(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            int targetId = int.Parse(button.Tag.ToString());
            int rejectorId = UserID;
            string password = Password;
            string resp = HomeSender.RejectRequest(targetId, rejectorId, password);
            if (resp == "OK")
            {
                UserListView.ItemsSource = DisplayFriends();
                UserSentListView.ItemsSource = DisplaySends();
                UserRecieveListView.ItemsSource = DisplayReceives();
                Button1.Tag = "";
                Button2.Tag = "";
                Button2.Visibility = Visibility.Visible;
                MessageFrame("Заявка отклонена!", MessageType.Successful);
            }
            else if (resp == "already")
            {
                UserListView.ItemsSource = DisplayFriends();
                UserSentListView.ItemsSource = DisplaySends();
                UserRecieveListView.ItemsSource = DisplayReceives();
                Button1.Tag = "";
                Button2.Tag = "";
                Button2.Visibility = Visibility.Visible;
                MessageFrame("Заявки и так не было!", MessageType.Error);
            }
            else if (resp == "ICPassword")
            {
                Button1.Tag = "";
                Button2.Tag = "";
                Button2.Visibility = Visibility.Visible;
                MessageFrame("Система безопасности отлонила доступ!", MessageType.Error);
            }
            else if (resp == "Exception;SQLWillNotStarted")
            {
                Button1.Tag = "";
                Button2.Tag = "";
                Button2.Visibility = Visibility.Visible;
                MessageFrame("Серверу не удалось подключиться к базе данных!", MessageType.Error);
            }
            else if (resp == "Exception;ServerNotResponding")
            {
                Button1.Tag = "";
                Button2.Tag = "";
                Button2.Visibility = Visibility.Visible;
                MessageFrame("Сервер недоступен!", MessageType.Error);
            }

        }
    }
    public enum MessageType
    {
        Warning,
        Error,
        Confirmation,
        Successful
    }
}
