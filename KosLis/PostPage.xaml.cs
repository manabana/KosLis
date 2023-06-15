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
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Drawing;
using QuerySender;
using Microsoft.Win32;
using System.Windows.Media.Effects;
using System.Drawing.Imaging;
using System.Windows.Media.Animation;
using System.Threading;

namespace KosLis
{
    /// <summary>
    /// Логика взаимодействия для PostPage.xaml
    /// </summary>
    public partial class PostPage : Page
    {
        private int UserID;
        private string Password;
        int ImageSize = 0;
        byte[] ImageBytesG;
        bool ImageSelected= false;
        public PostPage(int id, string pw)
        {
            InitializeComponent();
            UserID = id;
            Password = pw;
        }

        private void TBC(object sender, RoutedEventArgs e)
        {
            TextBox instance = (TextBox)sender;
            instance.Foreground = new SolidColorBrush(Colors.LightGray);
            if (instance.Text == instance.Tag.ToString())
                instance.Text = "";
        }

        private void TBF(object sender, RoutedEventArgs e)
        {
            TextBox instance = (TextBox)sender;
            instance.Foreground = new SolidColorBrush(Colors.DarkGray);
            if (string.IsNullOrWhiteSpace(instance.Text))
                instance.Text = instance.Tag.ToString();
        }

        private void SelectImage(object sender, RoutedEventArgs e)
        {
            try
            {
                BitmapImage bitmap1= new BitmapImage();
                System.Drawing.Image image;
                string filenm;
                OpenFileDialog openFile = new OpenFileDialog();
                openFile.Filter = "Image Files(*.png)|*.png";

                openFile.ShowDialog();
                filenm = openFile.FileName;
                image= System.Drawing.Image.FromFile(filenm);

                byte[] ImageBytes = ImgToByteConverter(image);
                ImageSize = ImageBytes.Length;
                ImageBytesG = ImageBytes;
                bitmap1 = DrawingToBitmap(image);
                PreImg.Source = bitmap1;
                ImageSelected = true;
                PreImg.Visibility = Visibility.Visible;
                ImgSelect.Visibility = Visibility.Collapsed;

            }
            catch (System.ArgumentException)
            {
                MessageShow("Изображение не было выбрано", 0);
            }
            catch { }
        }
        private BitmapImage DrawingToBitmap(System.Drawing.Image image)
        {
            using (var stream = new MemoryStream())
            {
                image.Save(stream, ImageFormat.Bmp);// bitmapGDI - твой System.Drawing.Image
                BitmapImage bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.StreamSource = new MemoryStream(stream.ToArray());
                bitmap.EndInit();

                return bitmap; // bitmap - это WPF'овский BitmapImage
            };

        }
        public static byte[] ImgToByteConverter(System.Drawing.Image inImg)
        {
            ImageConverter imgCon = new ImageConverter();
            return (byte[])imgCon.ConvertTo(inImg, typeof(byte[]));
        }
        public System.Drawing.Image ByteArrayToImage(byte[] byteArrayIn)
        {
            using (MemoryStream mStream = new MemoryStream(byteArrayIn))
            {
                return System.Drawing.Image.FromStream(mStream);
            }
        }

        private void PublishPostBT(object sender, RoutedEventArgs e)
        {
            PostTitleTB.IsEnabled = false;
            selectImgBT.IsEnabled = false;
            ContentTextRTB.IsEnabled = false;
            PublishBT.IsEnabled = false;
            publLBL.Visibility = Visibility.Collapsed;
            loadLBL.Visibility = Visibility.Visible;
            PublishPostAsync();
        }
        private async Task PublishPostAsync()
        {
            int a = UserID;
            string b = PostTitleTB.Text;
            if(b == "Заголовок")
            {
                b = "";
            }
            string c = ContentTextRTB.Text;
            string e = Password;
            await Task.Run(() => PublishPost(a,b,c,ImageBytesG,e,ImageSize));
        }
        private void MessageShow(string message, byte messagetype)
        {
            BlurEffect blur = new BlurEffect {Radius = 16};
            PostStack.Effect = blur;
            switch (messagetype)
            {
                case 0: //UnkErr
                    var uriSource3 = new Uri(@"IMGs/UnkErr.png", UriKind.Relative);
                    MessageIMG.Source = new BitmapImage(uriSource3);
                    MessageText.Text = message;
                    break;
                case 1: //serverErr
                    var uriSourcet = new Uri(@"IMGs/serverErr.png", UriKind.Relative);
                    MessageIMG.Source = new BitmapImage(uriSourcet);
                    MessageText.Text = message;

                    break;
                case 2:
                    break;
                case 3:
                    break;
            }
            MessageStack.Visibility= Visibility.Visible;


        }
        private void ResetUI()
        {
            DeleteIMGBT(null, null);
            FocusIMG(null, null);
            PostTitleTB.Text = "";
            ContentTextRTB.Text = "";
            PostTitleTB.IsEnabled = true;
            selectImgBT.IsEnabled = true;
            ContentTextRTB.IsEnabled = true;
            PublishBT.IsEnabled = true;
            publLBL.Visibility = Visibility.Visible;
            loadLBL.Visibility = Visibility.Collapsed;
        }
        private async Task CheckMark()
        {
            await Task.Run(() => WaitingMark());

        }
        void ShowCheckMark()
        {
            DelBT.IsEnabled = false;
            publLBL.Visibility = Visibility.Visible;
            publLBL.Content = "✔";
            ColorAnimation animation = new ColorAnimation
            {
                From = System.Windows.Media.Color.FromRgb(42, 46, 78),
                To = System.Windows.Media.Color.FromRgb(12, 90, 0),
                Duration = TimeSpan.FromSeconds(0.3)
            };
            PublishBT.Background.BeginAnimation(SolidColorBrush.ColorProperty, animation);

        }
        void WaitingMark()
        {
            Dispatcher.Invoke(() => ShowCheckMark());
            Thread.Sleep(1300);
            Dispatcher.Invoke(() => HideCheckMark());

        }
        void HideCheckMark()
        {
            publLBL.Content = "Опубликовать";
            DelBT.IsEnabled = false;
            ColorAnimation animation = new ColorAnimation
            {
                From = System.Windows.Media.Color.FromRgb(12, 90, 0),
                To = System.Windows.Media.Color.FromRgb(42, 46, 78), 
                Duration = TimeSpan.FromSeconds(0.3)
            };
            PublishBT.Background.BeginAnimation(SolidColorBrush.ColorProperty, animation);


        }
        private void PublishPost(int ui, string tt, string ct, byte[] ci, string pw, int imgSize)
        {
            string resp = HomeSender.PostSend(ui,tt,ct,ci,pw,imgSize);
            Console.WriteLine(resp);
            if(resp == "PostAdded")
            {
                Dispatcher.Invoke(() => CheckMark());
                Dispatcher.Invoke(() => ResetUI());

            }
            else if(resp.IndexOf("PAWaitingImage") >= 0)
            {
                string[] strings = resp.Split(';');
                string respond = HomeSender.SendImage(ImageBytesG, int.Parse(strings[1]));
                if(respond == "IMGAdded")
                {
                    CheckMark();
                    Dispatcher.Invoke(() => ResetUI());
                }
                else if (respond == "IMGNotFound")
                {
                    MessageShow("Неизвестная ошибка", 0);
                }
            }
            else if(resp.IndexOf("Exception") >= 0)
            {
                string[] splited = resp.Split(';');
                if (splited[1] == "NotAdded")
                {
                    Dispatcher.Invoke(() => MessageShow("Пост не опубликован по неизвестной причине!", 0));
                }
                else if(splited[1] == "ICPassword")
                {
                    Dispatcher.Invoke(() => MessageShow("Система безопасности отклонила доступ!",  0));
                }
                else if (splited[1] == "ServerNotResponding")
                {
                    Dispatcher.Invoke(() => MessageShow("Сервер недоступен!", 1));
                }
            }
        }
        private void CloseMessage(object sender, RoutedEventArgs e)
        {
            MessageStack.Visibility = Visibility.Collapsed;
            PostTitleTB.IsEnabled = true;
            selectImgBT.IsEnabled = true;
            ContentTextRTB.IsEnabled = true;
            PublishBT.IsEnabled = true;
            publLBL.Visibility = Visibility.Visible;
            loadLBL.Visibility = Visibility.Collapsed;
            PostStack.Effect= null;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            PostTitleTB.Text = "";
            PostTitleTB.IsEnabled = true;
            ContentTextRTB.Text = "";
            ContentTextRTB.IsEnabled= true;
            publLBL.Visibility= Visibility.Visible;
            loadLBL.Visibility= Visibility.Collapsed;
            PostStack.Visibility = Visibility.Visible;
            PostPreview.Visibility = Visibility.Collapsed;

        }

        private void BlurIMG(object sender, MouseEventArgs e)
        {
            if (ImageSelected)
            {
                DoubleAnimation animation1 = new DoubleAnimation
                {
                    From = 0,
                    To = 100,
                    Duration = TimeSpan.FromSeconds(0.2)
                };

                BlurEffect blurEffect = new BlurEffect();
                PreImg.Effect = blurEffect;
                DoubleAnimation animation = new DoubleAnimation();
                animation.From = 0;
                animation.To = 16;
                animation.Duration = TimeSpan.FromSeconds(0.2);
                PreImg.Effect.BeginAnimation(BlurEffect.RadiusProperty, animation);
                DelGrid.BeginAnimation(Grid.OpacityProperty, animation1);
                DelGrid.Visibility = Visibility.Visible;

            }

        }

        private void FocusIMG(object sender, MouseEventArgs e)
        {
            if (ImageSelected)
            {
                DoubleAnimation animation1 = new DoubleAnimation
                {
                    From = 100,
                    To = 0,
                    Duration = TimeSpan.FromSeconds(0.2)
                };

                DoubleAnimation animation = new DoubleAnimation();
                animation.From = 16;
                animation.To = 0;
                animation.Duration = TimeSpan.FromSeconds(0.2);
                try
                {
                PreImg.Effect.BeginAnimation(BlurEffect.RadiusProperty, animation);

                }
                catch { }
                DelGrid.BeginAnimation(Grid.OpacityProperty, animation1);
                DelGrid.Visibility = Visibility.Collapsed;

            }
        }

        private void DeleteIMGBT(object sender, RoutedEventArgs e)
        {
            FocusIMG(null, null);
            PreImg.Source = null;
            ImageBytesG = null;
            DelGrid.Visibility= Visibility.Collapsed;
            ImgSelect.Visibility = Visibility.Visible;
            ImageSelected = false;
        }
    }
}
