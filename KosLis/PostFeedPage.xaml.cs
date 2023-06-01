using QuerySender;
using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
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
    /// Логика взаимодействия для PostFeedPage.xaml
    /// </summary>
    public partial class PostFeedPage : Page
    {
        private void DisplayFeed()
        {
            List<Posts> posts = new List<Posts>();

            string resp = HomeSender.AskPosts();
            string[] splitedA = resp.Split('|');
            for (int i = 0; i < splitedA.Count() - 1; i++)
            {
                string[] splitedB = splitedA[i].Split(';');
                string checker = HomeSender.CheckPostImage(int.Parse(splitedB[0]));
                if (checker == "True")
                {
                    byte[] bytes = HomeSender.AskPostImage(int.Parse(splitedB[0]));
                    string debyted = Encoding.UTF8.GetString(bytes, 0, 7000000);
                    BitmapImage bitmap = Dispatcher.Invoke(() => DrawingToBitmap(ByteArrayToImage(bytes)));
                    posts.Add(new Posts(int.Parse(splitedB[0]), splitedB[1], int.Parse(splitedB[2]), splitedB[3], splitedB[4], splitedB[5], bitmap, int.Parse(splitedB[6])));
                }
                else
                {
                    posts.Add(new Posts(int.Parse(splitedB[0]), splitedB[1], int.Parse(splitedB[2]), splitedB[3], splitedB[4], splitedB[5], null, int.Parse(splitedB[6])));
                }
            }
            posts = posts.OrderByDescending(p => p.postId).ToList();
            Dispatcher.Invoke(() => {
                FeedList.ItemsSource = null;
                FeedList.ItemsSource = posts;
                LoadingStack.Visibility = Visibility.Collapsed;
            });
        }
        private async Task DisplayAsync()
        {
            LoadingStack.Visibility = Visibility.Visible;
            await Task.Run(() => DisplayFeed());
        }
        public PostFeedPage()
        {
            InitializeComponent();
            DisplayAsync();
            //DisplayFeed();

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
                    DisplayAsync();

                }
            }
            else
            {
                resp = HomeSender.RateChange(RateType.Down, postid);
                if (resp == "success")
                {
                    DisplayAsync();

                }

            }
        }
    }

    public class Posts
    {
        public int postId { get; set; }
        public string postTitle { get; set; }
        public string postText { get; set; }
        public string userName { get; set; }
        public int userId { get; set; }
        public string postDate { get; set; }
        public BitmapImage postImage { get; set; }
        public int likeCount { get; set; }
        public Posts(int userId, string userName, int postId, string postTitle, string postText, string postDate, BitmapImage postImage, int likeCount)
        {
            this.userId = userId;
            this.userName = userName;
            this.postId = postId;
            this.postTitle = postTitle;
            this.postText = postText;
            this.postDate = postDate;
            this.postImage = postImage;
            this.likeCount = likeCount;
        }
    }
}
