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
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace KosLis
{
    /// <summary>
    /// Логика взаимодействия для PostFeedPage.xaml
    /// </summary>
    public partial class PostFeedPage : Page
    {
        public PostFeedPage()
        {
            InitializeComponent();
            DisplayAsync();
            //DisplayFeed();

        }
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
                    string debyted = Encoding.UTF8.GetString(bytes, 0, 32);

                    BitmapImage bitmap = Dispatcher.Invoke(() => DrawingToBitmap(ByteArrayToImage(bytes)));
                    posts.Add(new Posts(int.Parse(splitedB[0]), splitedB[1], int.Parse(splitedB[2]), splitedB[3], splitedB[4], splitedB[5], bitmap, int.Parse(splitedB[6]), int.Parse(splitedB[7])));
                }
                else
                {
                    posts.Add(new Posts(int.Parse(splitedB[0]), splitedB[1], int.Parse(splitedB[2]), splitedB[3], splitedB[4], splitedB[5], null, int.Parse(splitedB[6]), int.Parse(splitedB[7])));
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
        public BitmapImage DrawingToBitmapBeta(System.Drawing.Image image)
        {
            using (var stream = new MemoryStream())
            {
                // Здесь сохраняем изображение.
                image.Save(stream, ImageFormat.Png); // Попробуй использовать PNG.
                stream.Position = 0; // Сбрасываем позицию потока.

                BitmapImage bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.CacheOption = BitmapCacheOption.OnLoad; // Это поможет с загрузкой.
                bitmap.StreamSource = stream;
                bitmap.EndInit();
                bitmap.Freeze(); // Заморозка для использования в других потоках.

                return bitmap;
            }
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
                    //DisplayAsync();
                    DependencyObject parent = VisualTreeHelper.GetParent(button);
                    if (parent != null && parent is FrameworkElement)
                    {
                        FrameworkElement parentElement = parent as FrameworkElement;
                        Label myLabel = parentElement.FindName("likesOut") as Label;
                        myLabel.Content = int.Parse(myLabel.Content.ToString()) + 1;
                    }
                }
            }
            else
            {
                resp = HomeSender.RateChange(RateType.Down, postid);
                if (resp == "success")
                {

                    //DisplayAsync();
                    DependencyObject parent = VisualTreeHelper.GetParent(button);
                    if (parent != null && parent is FrameworkElement)
                    {
                        FrameworkElement parentElement = parent as FrameworkElement;
                        Label myLabel = parentElement.FindName("likesOut") as Label;
                        myLabel.Content = int.Parse(myLabel.Content.ToString()) - 1;
                    }

                }

            }
        }
        private void OpenSome1Profile(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            int id = int.Parse(button.Tag.ToString());
            var MW = Application.Current.MainWindow as MainWindow;
            MW.ShowSome1Prof(id);

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
        public string profilePhoto { get; set; }
        public Posts(int postId, string userName, int userId, string postTitle, string postText, string postDate, BitmapImage postImage, int likeCount, int PhotoId)
        {
            this.userId = userId;
            this.userName = userName;
            this.postId = postId;
            this.postTitle = postTitle;
            this.postText = postText;
            this.postDate = postDate;
            this.postImage = postImage;
            this.likeCount = likeCount;
            profilePhoto = $@"IMGs/PPs/{PhotoId}.jpg";
            //BitmapImage bitmap = new BitmapImage();
            //bitmap.BeginInit();
            //bitmap.UriSource = new Uri($@"IMGs/PPs/{PhotoId}.jpg", UriKind.Relative);
            //bitmap.EndInit();
            //profilePhoto = bitmap;
        }
    }
}
