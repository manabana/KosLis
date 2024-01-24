using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace QuerySender
{
    public enum AskPostsType
    {
        UserFriendsPosts,
        UserPosts
    }
    public enum RateType
    {
        Up, Down
    }
    public enum AskUsersType
    {
        AskEveryoneExcept, AskEveryone, AskSingle, AskFriends, AskReceiveRequests, AskSendRequests
    }
    public class HomeSender
    {
        static IPAddress ipAddr;
        static int MainPort;
        static int ImagePort;
        static IPEndPoint ipEndPoint;
        static IPEndPoint ipEndPointI;

        public static void InitializeTCP()
        {
            try
            {
                using (BinaryReader reader = new BinaryReader(File.Open("IP.dat", FileMode.Open)))
                {
                    string IP = reader.ReadString();
                    ipAddr = IPAddress.Parse(IP);
                }
                using (BinaryReader reader = new BinaryReader(File.Open("MainPort.dat", FileMode.Open)))
                {
                    MainPort = reader.ReadUInt16();
                }
                using (BinaryReader reader = new BinaryReader(File.Open("ImagePort.dat", FileMode.Open)))
                {
                    ImagePort = reader.ReadUInt16();
                }
                ipEndPoint = new IPEndPoint(ipAddr, MainPort);
                ipEndPointI = new IPEndPoint(ipAddr, ImagePort);

            }
            catch (System.IO.FileNotFoundException)
            {
                using (BinaryWriter writer = new BinaryWriter(File.Open("IP.dat", FileMode.OpenOrCreate)))
                {
                    writer.Write("0.0.0.0");
                }
                using (BinaryWriter writer = new BinaryWriter(File.Open("MainPort.dat", FileMode.OpenOrCreate)))
                {
                    writer.Write(11000);
                }
                using (BinaryWriter writer = new BinaryWriter(File.Open("ImagePort.dat", FileMode.OpenOrCreate)))
                {
                    writer.Write(11001);
                }
                InitializeTCP();
            }
        }
        public static string CheckServer()
        {
            byte[] bytes = new byte[128];


            using (var sender = new Socket(ipAddr.AddressFamily, SocketType.Stream, ProtocolType.Tcp))
            {
                try
                {
                    sender.Connect(ipEndPoint);

                }
                catch (System.Net.Sockets.SocketException)
                {
                    return "Exception;ServerNotResponding";
                }
                string message = $"checkserver";
                byte[] msg = Encoding.UTF8.GetBytes(message);
                int bytesSent = sender.Send(msg);
                int bytesRec = sender.Receive(bytes);
                sender.Shutdown(SocketShutdown.Both);
                sender.Close();
                return Encoding.UTF8.GetString(bytes, 0, bytesRec);
            }
        }
        public static string RateChange(RateType rateType, int postId)
        {
            byte[] bytes = new byte[128];


            Socket sender = new Socket(ipAddr.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            try
            {
                sender.Connect(ipEndPoint);

            }
            catch (System.Net.Sockets.SocketException)
            {
                return "Exception;ServerNotResponding";
            }
            string message = $"postsList;all";
            if (rateType == RateType.Up)
            {
                message = $"ratechange;up;{postId}";
            }
            else
            {
                message = $"ratechange;down;{postId}";
            }
            byte[] msg = Encoding.UTF8.GetBytes(message);
            int bytesSent = sender.Send(msg);
            int bytesRec = sender.Receive(bytes);
            sender.Shutdown(SocketShutdown.Both);
            sender.Close();
            return Encoding.UTF8.GetString(bytes, 0, bytesRec);

        }
        public static string AskMessages(int dialogId)
        {
            byte[] bytes = new byte[4096];
            Socket sender = new Socket(ipAddr.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            try
            {
                sender.Connect(ipEndPoint);

            }
            catch (System.Net.Sockets.SocketException)
            {
                return "Exception;ServerNotResponding";
            }
            string message = $"reqDialog;{dialogId}";

            byte[] msg = Encoding.UTF8.GetBytes(message);

            // Отправляем данные через сокет
            int bytesSent = sender.Send(msg);

            // Получаем ответ от сервера
            int bytesRec = sender.Receive(bytes);

            // Освобождаем сокет
            sender.Shutdown(SocketShutdown.Both);
            sender.Close();
            return Encoding.UTF8.GetString(bytes, 0, bytesRec);


        }
        public static string SendMessage(string SendingMessage, int dialogId, int fromID, int toId)
        {
            byte[] bytes = new byte[128];
            Socket sender = new Socket(ipAddr.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            try
            {
                sender.Connect(ipEndPoint);

            }
            catch (System.Net.Sockets.SocketException)
            {
                return "Exception;ServerNotResponding";
            }
            string message = $"sendMes;{SendingMessage};{dialogId};{fromID};{toId}";
            byte[] msg = Encoding.UTF8.GetBytes(message);
            int bytesSent = sender.Send(msg);
            int bytesRec = sender.Receive(bytes);
            sender.Shutdown(SocketShutdown.Both);
            sender.Close();
            return Encoding.UTF8.GetString(bytes, 0, bytesRec);

        }
        public static string OpenOrCreateDialog(int IdA, int IdB)
        {
            byte[] bytes = new byte[1024];
            Socket sender = new Socket(ipAddr.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            try
            {
                sender.Connect(ipEndPoint);

            }
            catch (System.Net.Sockets.SocketException)
            {
                return "Exception;ServerNotResponding";
            }
            string message = $"dialog;{IdA};{IdB}";
            byte[] msg = Encoding.UTF8.GetBytes(message);
            int bytesSent = sender.Send(msg);
            int bytesRec = sender.Receive(bytes);
            sender.Shutdown(SocketShutdown.Both);
            sender.Close();
            return Encoding.UTF8.GetString(bytes, 0, bytesRec);

        }
        public static byte[] AskPostImage(int postId, int size)
        {
            byte[] bytes = new byte[size]; //7 MB
            Socket sender = new Socket(ipAddr.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            try
            {
                sender.Connect(ipEndPoint);

            }
            catch (System.Net.Sockets.SocketException)
            {
                return Encoding.UTF8.GetBytes("Exception;ServerNotResponding");
            }
            string message;
            message = $"postimage;{postId}";
            byte[] msg = Encoding.UTF8.GetBytes(message);
            int bytesSent = sender.Send(msg);
            int bytesRec = sender.Receive(bytes);
            sender.Shutdown(SocketShutdown.Both);
            sender.Close();
            return bytes;


        }
        public static string AskPostImageSize(int postId)
        {
            byte[] bytes = new byte[32];
            Socket sender = new Socket(ipAddr.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            try
            {
                sender.Connect(ipEndPoint);

            }
            catch (System.Net.Sockets.SocketException)
            {
                return "Exception;ServerNotResponding";
            }
            string message;
            message = $"postImageSize;{postId}";
            byte[] msg = Encoding.UTF8.GetBytes(message);
            int bytesSent = sender.Send(msg);
            int bytesRec = sender.Receive(bytes);
            sender.Shutdown(SocketShutdown.Both);
            sender.Close();
            return Encoding.UTF8.GetString(bytes, 0, bytesRec);


        }
        public static string AskUsers(int userId, AskUsersType askType)
        {
            byte[] bytes = new byte[1024];
            Socket sender = new Socket(ipAddr.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            try
            {
                sender.Connect(ipEndPoint);

            }
            catch (System.Net.Sockets.SocketException)
            {
                return "Exception;ServerNotResponding";
            }
            string message;
            switch (askType)
            {
                case AskUsersType.AskEveryoneExcept:
                    message = $"usersList;except;{userId}";
                    break;
                case AskUsersType.AskEveryone:
                    message = $"usersList;all";
                    break;
                case AskUsersType.AskFriends:
                    message = $"usersList;friends;{userId}";
                    break;
                case AskUsersType.AskSendRequests:
                    message = $"usersList;sends;{userId}";
                    break;
                case AskUsersType.AskReceiveRequests:
                    message = $"usersList;receives;{userId}";
                    break;
                default:
                    message = "";
                    break;
            }

            byte[] msg = Encoding.UTF8.GetBytes(message);
            int bytesSent = sender.Send(msg);
            int bytesRec = sender.Receive(bytes);
            sender.Shutdown(SocketShutdown.Both);
            sender.Close();
            return Encoding.UTF8.GetString(bytes, 0, bytesRec);
        }
        public static string AskPosts(int userId, AskPostsType askType)
        {
            byte[] bytes = new byte[1024];

            Socket sender = new Socket(ipAddr.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

            try
            {
                sender.Connect(ipEndPoint);

            }
            catch (System.Net.Sockets.SocketException)
            {
                return "Exception;ServerNotResponding";
            }
            string message;
            switch (askType)
            {
                case AskPostsType.UserFriendsPosts:
                    message = $"postsList;friends;{userId}";
                    break;
                case AskPostsType.UserPosts:
                    message = $"postsList;user;{userId}";
                    break;
                default:
                    message = "";
                    break;
            }
            byte[] msg = Encoding.UTF8.GetBytes(message);
            int bytesSent = sender.Send(msg);
            int bytesRec = sender.Receive(bytes);
            sender.Shutdown(SocketShutdown.Both);
            sender.Close();
            return Encoding.UTF8.GetString(bytes, 0, bytesRec);

        }

        public static string AskPosts()
        {
            byte[] bytes = new byte[2097152];

            // Соединяемся с удаленным устройством

            // Устанавливаем удаленную точку для сокета

            Socket sender = new Socket(ipAddr.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

            // Соединяем сокет с удаленной точкой
            try
            {
                sender.Connect(ipEndPoint);
                string message = $"postsList;all";
                byte[] msg = Encoding.UTF8.GetBytes(message);
                int bytesSent = sender.Send(msg);
                int bytesRec = sender.Receive(bytes);
                sender.Shutdown(SocketShutdown.Both);
                sender.Close();
                return Encoding.UTF8.GetString(bytes, 0, bytesRec);
            }
            catch (System.Net.Sockets.SocketException)
            {
                return "Exception;ServerNotResponding";
            }


        }


        public static string PostSend(int usrId, string title, string contentTxt, byte[] contentIMG, string passw, int imageSize)
        {
            bool IMGAvlbl;
            if (contentIMG != null)
            {
                IMGAvlbl = true;
            }
            else
            {
                IMGAvlbl = false;
            }
            // Буфер для входящих данных
            byte[] bytes = new byte[1024];
            // Соединяемся с удаленным устройством
            // Устанавливаем удаленную точку для сокета

            Socket sender = new Socket(ipAddr.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

            // Соединяем сокет с удаленной точкой
            try
            {
                sender.Connect(ipEndPoint);
                string message = $"newpost;{usrId};{title};{contentTxt};{passw};{IMGAvlbl};{imageSize}";


                byte[] msg = Encoding.UTF8.GetBytes(message);

                // Отправляем данные через сокет
                int bytesSent = sender.Send(msg);

                // Получаем ответ от сервера
                int bytesRec = sender.Receive(bytes);

                // Освобождаем сокет
                sender.Shutdown(SocketShutdown.Both);
                sender.Close();
                return Encoding.UTF8.GetString(bytes, 0, bytesRec);
            }
            catch (System.Net.Sockets.SocketException)
            {
                return "Exception;ServerNotResponding";
            }

        }
        public static string CheckPostImage(int postId)
        {
            byte[] bytes = new byte[128];
            Socket sender = new Socket(ipAddr.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            try
            {
                sender.Connect(ipEndPoint);
                string message = $"checkImage;{postId}";
                byte[] msg = Encoding.UTF8.GetBytes(message);
                int bytesSent = sender.Send(msg);
                int bytesRec = sender.Receive(bytes);
                sender.Shutdown(SocketShutdown.Both);
                sender.Close();
                return Encoding.UTF8.GetString(bytes, 0, bytesRec);
            }
            catch (System.Net.Sockets.SocketException)
            {
                return "Exception;ServerNotResponding";
            }



        }
        public static string SendImage(byte[] image, int postId)
        {
            //Thread.Sleep(250);
            // Буфер для входящих данных
            byte[] bytes = new byte[256];

            // Соединяемся с удаленным устройством

            // Устанавливаем удаленную точку для сокета

            Socket sender = new Socket(ipAddr.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

            // Соединяем сокет с удаленной точкой
            try
            {
                sender.Connect(ipEndPointI);
                int bytesSent = sender.Send(image);
                // Получаем ответ от сервера
                //Thread.Sleep(100);
                sender.Shutdown(SocketShutdown.Both);
                sender.Close();
                string checker = CheckPostImage(postId);
                if (checker == "True")
                {
                    return "IMGAdded";//Encoding.UTF8.GetString(bytes, 0, bytesRec);
                }
                else
                {
                    return "IMGNotFound";
                }
            }
            catch (System.Net.Sockets.SocketException)
            {
                return "Exception;ServerNotResponding";
            }
        }

        public static string AskPhoto(int id)
        {
            byte[] bytes = new byte[1024];
            Socket sender = new Socket(ipAddr.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            try
            {
                sender.Connect(ipEndPoint);
                string message = $"profphoto;{id}";
                byte[] msg = Encoding.UTF8.GetBytes(message);
                int bytesSent = sender.Send(msg);
                int bytesRec = sender.Receive(bytes);
                sender.Shutdown(SocketShutdown.Both);
                sender.Close();
                return Encoding.UTF8.GetString(bytes, 0, bytesRec);
            }
            catch (System.Net.Sockets.SocketException)
            {
                return "Exception;ServerNotResponding";
            }
        }
        public static string AddFriend(int userId, string friendNN, string password)
        {
            byte[] bytes = new byte[32];
            Socket sender = new Socket(ipAddr.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            try
            {
                sender.Connect(ipEndPoint);
                string message = $"sendFriendRequest;{userId};{friendNN};{password}";
                byte[] msg = Encoding.UTF8.GetBytes(message);
                int bytesSent = sender.Send(msg);
                int bytesRec = sender.Receive(bytes);
                sender.Shutdown(SocketShutdown.Both);
                sender.Close();
                return Encoding.UTF8.GetString(bytes, 0, bytesRec);
            }
            catch (System.Net.Sockets.SocketException)
            {
                return "Exception;ServerNotResponding";
            }

        }
        public static string AcceptFriendRequest(int senderId, int acceptorId, string password)
        {
            byte[] bytes = new byte[32];

            // Соединяемся с удаленным устройством

            // Устанавливаем удаленную точку для сокета
            Socket sender = new Socket(ipAddr.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

            // Соединяем сокет с удаленной точкой
            try
            {
                sender.Connect(ipEndPoint);

            }
            catch (System.Net.Sockets.SocketException)
            {
                return "Exception;ServerNotResponding";
            }
            string message = $"acceptRequest;{senderId};{acceptorId};{password}";
            byte[] msg = Encoding.UTF8.GetBytes(message);

            // Отправляем данные через сокет
            int bytesSent = sender.Send(msg);

            // Получаем ответ от сервера
            int bytesRec = sender.Receive(bytes);

            // Освобождаем сокет
            sender.Shutdown(SocketShutdown.Both);
            sender.Close();
            return Encoding.UTF8.GetString(bytes, 0, bytesRec);
        }
        public static string RemoveFriend(int targetId, int removerId, string password)
        {
            byte[] bytes = new byte[32];
            Socket sender = new Socket(ipAddr.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            try
            {
                sender.Connect(ipEndPoint);
                string message = $"removeFriend;{targetId};{removerId};{password}";
                byte[] msg = Encoding.UTF8.GetBytes(message);
                int bytesSent = sender.Send(msg);
                int bytesRec = sender.Receive(bytes);
                sender.Shutdown(SocketShutdown.Both);
                sender.Close();
                return Encoding.UTF8.GetString(bytes, 0, bytesRec);
            }
            catch (System.Net.Sockets.SocketException)
            {
                return "Exception;ServerNotResponding";
            }

        }
        public static string RejectRequest(int targetId, int rejectorId, string password)
        {
            byte[] bytes = new byte[32];
            Socket sender = new Socket(ipAddr.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            try
            {
                sender.Connect(ipEndPoint);
                string message = $"rejectRequest;{targetId};{rejectorId};{password}";
                byte[] msg = Encoding.UTF8.GetBytes(message);
                int bytesSent = sender.Send(msg);
                int bytesRec = sender.Receive(bytes);
                sender.Shutdown(SocketShutdown.Both);
                sender.Close();
                return Encoding.UTF8.GetString(bytes, 0, bytesRec);
            }
            catch (System.Net.Sockets.SocketException)
            {
                return "Exception;ServerNotResponding";
            }

        }
        public static string CancelRequest(int targetId, int cancellerId, string password)
        {
            byte[] bytes = new byte[32];
            Socket sender = new Socket(ipAddr.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            try
            {
                sender.Connect(ipEndPoint);
                string message = $"cancelRequest;{targetId};{cancellerId};{password}";
                byte[] msg = Encoding.UTF8.GetBytes(message);
                int bytesSent = sender.Send(msg);
                int bytesRec = sender.Receive(bytes);
                sender.Shutdown(SocketShutdown.Both);
                sender.Close();
                return Encoding.UTF8.GetString(bytes, 0, bytesRec);
            }
            catch (System.Net.Sockets.SocketException)
            {
                return "Exception;ServerNotResponding";
            }

        }
        public static string GetSome1Info(int userId)
        {
            byte[] bytes = new byte[512];
            Socket sender = new Socket(ipAddr.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            try
            {
                sender.Connect(ipEndPoint);
                string message = $"userLimitedInfo;{userId}";
                byte[] msg = Encoding.UTF8.GetBytes(message);
                int bytesSent = sender.Send(msg);
                int bytesRec = sender.Receive(bytes);
                sender.Shutdown(SocketShutdown.Both);
                sender.Close();
                return Encoding.UTF8.GetString(bytes, 0, bytesRec);
            }
            catch (System.Net.Sockets.SocketException)
            {
                return "ServerNotResponding";
            }

        }
        public static string DeletePost(int postid, string userPassword)
        {
            byte[] bytes = new byte[32];
            Socket sender = new Socket(ipAddr.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            try
            {
                sender.Connect(ipEndPoint);
                string message = $"deletePost;{postid};{userPassword}";
                byte[] msg = Encoding.UTF8.GetBytes(message);
                int bytesSent = sender.Send(msg);
                int bytesRec = sender.Receive(bytes);
                sender.Shutdown(SocketShutdown.Both);
                sender.Close();
                return Encoding.UTF8.GetString(bytes, 0, bytesRec);
            }
            catch (System.Net.Sockets.SocketException)
            {
                return "ServerNotResponding";
            }

        }
        public static string EditUser(string type, int userid, string text, string password)
        {
            byte[] bytes = new byte[32];
            Socket sender = new Socket(ipAddr.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            try
            {
                sender.Connect(ipEndPoint);
                string message = $"editUser;{type};{userid};{text};{password}";
                byte[] msg = Encoding.UTF8.GetBytes(message);
                int bytesSent = sender.Send(msg);
                int bytesRec = sender.Receive(bytes);
                sender.Shutdown(SocketShutdown.Both);
                sender.Close();
                return Encoding.UTF8.GetString(bytes, 0, bytesRec);
            }
            catch (System.Net.Sockets.SocketException)
            {
                return "ServerNotResponding";
            }

        }
        public static string ChangePassword(int userid, string pastPassword, string nextPassvord)
        {
            byte[] bytes = new byte[32];
            Socket sender = new Socket(ipAddr.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            try
            {
                sender.Connect(ipEndPoint);
                string message = $"changePassword;{userid};{pastPassword};{nextPassvord}";
                byte[] msg = Encoding.UTF8.GetBytes(message);
                int bytesSent = sender.Send(msg);
                int bytesRec = sender.Receive(bytes);
                sender.Shutdown(SocketShutdown.Both);
                sender.Close();
                return Encoding.UTF8.GetString(bytes, 0, bytesRec);
            }
            catch (System.Net.Sockets.SocketException)
            {
                return "ServerNotResponding";
            }
        }
        public static string ChangeProfilePhoto(int userid, int photo, string password)
        {
            byte[] bytes = new byte[32];
            Socket sender = new Socket(ipAddr.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            try
            {
                sender.Connect(ipEndPoint);
                string message = $"changeProfilePhoto;{userid};{photo};{password}";
                byte[] msg = Encoding.UTF8.GetBytes(message);
                int bytesSent = sender.Send(msg);
                int bytesRec = sender.Receive(bytes);
                sender.Shutdown(SocketShutdown.Both);
                sender.Close();
                return Encoding.UTF8.GetString(bytes, 0, bytesRec);
            }
            catch (System.Net.Sockets.SocketException)
            {
                return "ServerNotResponding";
            }
        }
        public static string SendEmailResetQuery(string email)
        {
            byte[] bytes = new byte[32];
            Socket sender = new Socket(ipAddr.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            try
            {
                sender.Connect(ipEndPoint);
                string message = $"REmail;{email}";
                byte[] msg = Encoding.UTF8.GetBytes(message);
                int bytesSent = sender.Send(msg);
                int bytesRec = sender.Receive(bytes);
                sender.Shutdown(SocketShutdown.Both);
                sender.Close();
                return Encoding.UTF8.GetString(bytes, 0, bytesRec);
            }
            catch (System.Net.Sockets.SocketException)
            {
                return "ServerNotResponding";
            }

        }
        public static string SendResCode(string email, string code, string password)
        {
            byte[] bytes = new byte[32];
            Socket sender = new Socket(ipAddr.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            try
            {
                sender.Connect(ipEndPoint);
                string message = $"resetCode;{email};{code};{password}";
                byte[] msg = Encoding.UTF8.GetBytes(message);
                int bytesSent = sender.Send(msg);
                int bytesRec = sender.Receive(bytes);
                sender.Shutdown(SocketShutdown.Both);
                sender.Close();
                return Encoding.UTF8.GetString(bytes, 0, bytesRec);
            }
            catch (System.Net.Sockets.SocketException)
            {
                return "ServerNotResponding";
            }

        }
        public static string Login(string lg, string pw)
        {
            // Буфер для входящих данных
            byte[] bytes = new byte[1024];

            Socket sender = new Socket(ipAddr.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

            // Соединяем сокет с удаленной точкой
            try
            {
                sender.Connect(ipEndPoint);
                string message = $"login;{lg};{pw}";

                Console.WriteLine("Сокет соединяется с {0} ", sender.RemoteEndPoint.ToString());
                byte[] msg = Encoding.UTF8.GetBytes(message);

                // Отправляем данные через сокет
                int bytesSent = sender.Send(msg);

                // Получаем ответ от сервера
                int bytesRec = sender.Receive(bytes);

                // Освобождаем сокет
                sender.Shutdown(SocketShutdown.Both);
                sender.Close();
                return Encoding.UTF8.GetString(bytes, 0, bytesRec);

            }
            catch (System.Net.Sockets.SocketException)
            {
                return "Exception;ServerNotResponding";
            }

        }
        public static string Registering(string email, string nickname, string name, string surname, string password)
        {
            // Буфер для входящих данных
            byte[] bytes = new byte[1024];

            // Соединяемся с удаленным устройством

            Socket sender = new Socket(ipAddr.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

            // Соединяем сокет с удаленной точкой
            try
            {
                sender.Connect(ipEndPoint);

            }
            catch (System.Net.Sockets.SocketException)
            {
                return "Exception;ServerNotResponding";
            }
            string message = $"signup;{email};{nickname};{name};{surname};{password}";

            Console.WriteLine("Сокет соединяется с {0} ", sender.RemoteEndPoint.ToString());
            byte[] msg = Encoding.UTF8.GetBytes(message);

            // Отправляем данные через сокет
            int bytesSent = sender.Send(msg);

            // Получаем ответ от сервера
            int bytesRec = sender.Receive(bytes);

            // Освобождаем сокет
            sender.Shutdown(SocketShutdown.Both);
            sender.Close();
            return Encoding.UTF8.GetString(bytes, 0, bytesRec);

        }

    }
}
