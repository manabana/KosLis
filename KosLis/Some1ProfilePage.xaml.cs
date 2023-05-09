using QuerySender;
using System;
using System.Collections.Generic;
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
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace KosLis
{
    /// <summary>
    /// Логика взаимодействия для Some1ProfilePage.xaml
    /// </summary>
    public partial class Some1ProfilePage : Page
    {
        private int UserId { get; set; }
        private string UserName { get; set; }
        private string Surname { get; set; }
        private string Nickname { get; set; }
        public Some1ProfilePage(int userId)
        {
            InitializeComponent();
            UserId = userId;
            DisplayFeed(UserId);
            GetSome1Info(UserId);

        }
        private void GetSome1Info(int userId)
        {

        }
        private void DisplayFeed(int userId)
        {
            List<Posts> posts = new List<Posts>();

            string resp = HomeSender.AskPosts(userId, AskPostsType.UserPosts);
            string[] splitedA = resp.Split('|');
            for (int i = 0; i < splitedA.Count() - 1; i++)
            {
                string[] splitedB = splitedA[i].Split(';');
                string checker = HomeSender.CheckPostImage(int.Parse(splitedB[0]));
                if (checker == "True")
                {
                    byte[] bytes = HomeSender.AskPostImage(int.Parse(splitedB[0]));
                    string debyted = Encoding.UTF8.GetString(bytes, 0, 256);
                    BitmapImage bitmap = Dispatcher.Invoke(() => DrawingToBitmap(ByteArrayToImage(bytes)));
                    posts.Add(new Posts(int.Parse(splitedB[0]), splitedB[1], int.Parse(splitedB[2]), splitedB[3], splitedB[4], splitedB[5], bitmap, int.Parse(splitedB[6])));
                }
                else
                {
                    posts.Add(new Posts(int.Parse(splitedB[0]), splitedB[1], int.Parse(splitedB[2]), splitedB[3], splitedB[4], splitedB[5], null, int.Parse(splitedB[6])));
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
                    DisplayFeed(UserId);
                }
            }
            else
            {
                resp = HomeSender.RateChange(RateType.Down, postid);
                if (resp == "success")
                {
                    DisplayFeed(UserId);
                }

            }
        }

    }
}
