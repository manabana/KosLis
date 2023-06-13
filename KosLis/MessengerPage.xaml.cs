using QuerySender;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.Remoting.Channels;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
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

namespace KosLis
{
    /// <summary>
    /// Логика взаимодействия для MessengerPage.xaml
    /// </summary>
    public partial class MessengerPage : Page
    {
        List<Users> users = new List<Users>();
        List<Messages> messages = new List<Messages>();
        MainWindow MW = Application.Current.MainWindow as MainWindow;


        private int BuffDialog;
        int userB;
        private int GlobalId;
        private string Nick;
        private string Password;
        int refreshes = 3000;

        public MessengerPage(int Id, string Nick, string Password)
        {
            InitializeComponent();

            GlobalId = Id;
            this.Nick = Nick;
            this.Password = Password;
            if (File.Exists("fastupdate.bin"))
            {
                refreshes = 750;
            }
            FirstActions();
        }
        private async Task FirstActions()
        {
            await Task.Run(() =>
            {
                string CS = CheckServer();
                if (CS == "OK")
                {

                    DisplayUsers();
                    Refresh();
                }
                else
                {
                    if(CS == "Exception;ServerNotResponding")
                    {
                        Dispatcher.Invoke(() => DisplayMessageBox("Не удалось подключиться к серверу!", MessageType.Error));
                    }
                }

            });
        }
        private async Task DisplayFriendsOnly()
        {
            await Task.Run(() =>
            {
                string CS = CheckServer();
                if (CS == "OK")
                {

                    DisplayFriends(GlobalId);
                    Refresh();
                }
                else
                {
                    if (CS == "Exception;ServerNotResponding")
                    {
                        Dispatcher.Invoke(() => DisplayMessageBox("Не удалось подключиться к серверу!", MessageType.Error));
                    }
                }

            });
        }
        private void DisplayMessageBox(string message, MessageType messageType)
        {
            MessageTextBlock.Text = message;
            MessageTextBlock.Visibility = Visibility.Visible;
            ReloadLBL.Visibility = Visibility.Visible;
            loadLBL.Visibility = Visibility.Collapsed;
            MessageBoxButton.IsEnabled = true;
            switch (messageType)
            {
                case MessageType.Error:
                    var uriSource3 = new Uri(@"IMGs/cross.png", UriKind.Relative);
                    MessageImage.Source = new BitmapImage(uriSource3);
                    break;
                case MessageType.Warning:
                    var uriSource2 = new Uri(@"IMGs/attention.png", UriKind.Relative);
                    MessageImage.Source = new BitmapImage(uriSource2);
                    break;
                case MessageType.Confirmation:
                    var uriSource1 = new Uri(@"IMGs/question.png", UriKind.Relative);
                    MessageImage.Source = new BitmapImage(uriSource1);
                    break;
                case MessageType.Successful:
                    var uriSource4 = new Uri(@"IMGs/check.png", UriKind.Relative);
                    MessageImage.Source = new BitmapImage(uriSource4);
                    break;

            }
            MessageImage.Visibility = Visibility.Visible;
        }
        private string CheckServer()
        {
            return HomeSender.CheckServer();
        }
        private async Task AutoRefreshingAsync()
        {
            await Task.Run(() => Refresh());
        }
        private void Refresh()
        {
            while(true)
            {
                Thread.Sleep(refreshes);
                if(MessengerGrid.Visibility == Visibility.Visible)//if(messages.Count > 0)
                {
                    MessagesAsync(true);
                }
                if (MW.stop)
                {
                    MW.stop = false;
                    break;
                }
            }
        }
        private async Task DisplayUsersAsync()
        {
            users.Clear();
            MessengerGrid.Visibility = Visibility.Visible;
            await Task.Run(() => DisplayUsers());
        }
        private void DisplayUsers()
        {
            string resp = HomeSender.AskUsers(GlobalId, AskUsersType.AskEveryoneExcept);
            string[] splitedA = resp.Split('|');
            users.Clear();
            for (int i = 0; i < splitedA.Count() - 1; i++)
            {
                string[] splitedB = splitedA[i].Split(';');
                users.Add(new Users(int.Parse(splitedB[0]), splitedB[1], int.Parse(splitedB[2])));

            }
            Dispatcher.Invoke(() =>
            {
                MessageGrid.Visibility = Visibility.Collapsed;
                UserListView.ItemsSource = null;
                UserListView.ItemsSource = users;
            });
        }
        private void DisplayFriends(int userid)
        {
            string resp = HomeSender.AskUsers(userid, AskUsersType.AskFriends);
            string[] splitedA = resp.Split('|');
            users.Clear();
            for (int i = 0; i < splitedA.Count() - 1; i++)
            {
                string[] splitedB = splitedA[i].Split(';');
                users.Add(new Users(int.Parse(splitedB[0]), splitedB[1], int.Parse(splitedB[2])));

            }
            Dispatcher.Invoke(() =>
            {
                MessageGrid.Visibility = Visibility.Collapsed;
                UserListView.ItemsSource = null;
                UserListView.ItemsSource = users;
            });
        }

        public class Users
        {
            public int Id { get; set; }
            public string Nickname { get; set; }
            public string ProfilePhoto { get; set; }
            public int PhotoId { get; set; }

            public Users(int Id, string Nickname, int PhotoId)
            {
                this.Id = Id;
                this.Nickname = Nickname;
                ProfilePhoto = $@"IMGs/PPs/{PhotoId}.jpg";
                this.PhotoId = PhotoId;
                //BitmapImage bitmap = new BitmapImage();
                //bitmap.BeginInit();
                //bitmap.UriSource = new Uri($@"IMGs/PPs/{PhotoId}.jpg", UriKind.Relative);
                //bitmap.EndInit();
                //ProfilePhoto = bitmap;

            }
        }
        public class Messages
        {
            public int Id { get; set; }
            public string MessageText { get; set; }
            public int FromId { get; set; }
            public int ToId { get; set; }
            public string MessageDate { get; set; }
            public int DialogId { get; set; }
            public HorizontalAlignment alignment { get; set; }
            public Messages(int GlobalId,int Id, string MessageText, int FromId, int ToId, string MessageDate, int DialogId)
            {
                this.Id = Id;
                this.MessageText = MessageText;
                this.FromId = FromId;
                this.ToId = ToId;
                this.MessageDate = MessageDate;
                this.DialogId = DialogId;
                if(FromId == GlobalId)
                {
                    alignment = HorizontalAlignment.Right;
                }
                else
                {
                    alignment = HorizontalAlignment.Left;
                }
            }
        }
        private async Task MessagesAsync(bool IsRefresh)
        {
            await Task.Run(() => DisplayMessages(BuffDialog, IsRefresh));
        }
        private void FillLV(List<Messages> messages, bool IsRefresh)
        {
            if(int.Parse(MsgLV.Tag.ToString()) != messages.Count)
            {
                IsRefresh = false;
            }
            MsgLV.Tag = messages.Count;
            MsgLV.ItemsSource = null;
            MsgLV.ItemsSource = messages;
            MsgLV.Items.MoveCurrentToLast();
            try
            {
                if (IsRefresh == false)
                {
                    MsgLV.ScrollIntoView(MsgLV.Items.CurrentItem);
                }
            }
            catch
            {
                FillLV(messages, false);
            }

        }
        private void DisplayMessages(int dialogId, bool IsRefresh) 
        {
            messages.Clear();
            string req = HomeSender.AskMessages(dialogId);
            string[] splitedA = req.Split('|');
            Console.WriteLine(req);
            for (int i = 0; i < splitedA.Count() - 1; i++)
            {
                string[] splitedB = splitedA[i].Split(';');
                messages.Add(new Messages(GlobalId ,int.Parse(splitedB[0]), splitedB[1], int.Parse(splitedB[2]), int.Parse(splitedB[3]), splitedB[4], int.Parse(splitedB[5])));

            }
            Dispatcher.Invoke(() => FillLV(messages, IsRefresh));

        }

        private void MesDialogOpen(object sender, RoutedEventArgs e)
        {
            Button but = sender as Button;
            MessengerGrid.Visibility = Visibility.Visible;
            int userA = GlobalId;
            userB = int.Parse(but.Tag.ToString());
            string rep;
            if (userA > userB) 
            {
                rep = HomeSender.OpenOrCreateDialog(userA, userB);

            }
            else
            {
                rep = HomeSender.OpenOrCreateDialog(userB, userA);

            }
            Console.WriteLine(rep);
            var user = users.Single(p=>p.Id == userB);
            usernameOut.Content = user.Nickname;
            var uriSource3 = new Uri($@"IMGs/PPs/{user.PhotoId}.jpg", UriKind.Relative);
            TopPanelPhoto.ImageSource = new BitmapImage(uriSource3);
            ToProfileBT.Tag = user.Id;
            TopPanel.Visibility = Visibility.Visible;
            MessageSender.Visibility = Visibility.Visible;
            if(rep.IndexOf("success") > -1)
            {
                string[] strings = rep.Split(';');
                BuffDialog = int.Parse(strings[1]);
                MessagesAsync(false);
            }
            else
            {

            }
        }

        private void SendMessage(object sender, RoutedEventArgs e)
        {
            string mess = SendingMessage.Text;
            string req = HomeSender.SendMessage(mess, BuffDialog, GlobalId, userB);
            Console.WriteLine(req);
            if(req == "success")
            {
                MessagesAsync(false);
            }
            SendingMessage.Text = "";
        }

        private void MessageBoxButtonClicked(object sender, RoutedEventArgs e)
        {
            MessageBoxButton.IsEnabled = false;
            MessageTextBlock.Visibility = Visibility.Collapsed;
            MessageImage.Visibility = Visibility.Collapsed;
            loadLBL.Visibility = Visibility.Visible;
            ReloadLBL.Visibility = Visibility.Collapsed;

            FirstActions();
        }

        private void SendButtonLocker(object sender, TextChangedEventArgs e)
        {
            if(SendingMessage.Text == "")
            {
                SendMesBT.IsEnabled = false;
            }
            else
            {
                SendMesBT.IsEnabled = true;
            }
        }

        private void OpenSome1Profile(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            int id = int.Parse(button.Tag.ToString());
            var MW = Application.Current.MainWindow as MainWindow;
            MW.ShowSome1Prof(id);

        }

        private void OnlyFriends(object sender, RoutedEventArgs e)
        {
            DisplayFriendsOnly();
        }

        private void AllUsers(object sender, RoutedEventArgs e)
        {
            FirstActions();
        }
    }
}
