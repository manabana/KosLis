using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Deployment.Internal;
using System.Threading;
using System.Security.AccessControl;
using System.Diagnostics;
using System.IO;
using System.Security.Cryptography;
using System.Reflection.Emit;

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
        public static string CheckServer()
        {
            byte[] bytes = new byte[128];
            IPHostEntry ipHost = Dns.GetHostEntry("localhost");
            IPAddress ipAddr = ipHost.AddressList[0];
            IPEndPoint ipEndPoint = new IPEndPoint(ipAddr, 11000);
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
            IPHostEntry ipHost = Dns.GetHostEntry("localhost");
            IPAddress ipAddr = ipHost.AddressList[0];
            IPEndPoint ipEndPoint = new IPEndPoint(ipAddr, 11000);

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
            IPHostEntry ipHost = Dns.GetHostEntry("localhost");
            IPAddress ipAddr = ipHost.AddressList[0];
            IPEndPoint ipEndPoint = new IPEndPoint(ipAddr, 11000);
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
        public static string SendMessage(string SendingMessage,int dialogId, int fromID,int toId)
        {
            byte[] bytes = new byte[128];
            IPHostEntry ipHost = Dns.GetHostEntry("localhost");
            IPAddress ipAddr = ipHost.AddressList[0];
            IPEndPoint ipEndPoint = new IPEndPoint(ipAddr, 11000);
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
            IPHostEntry ipHost = Dns.GetHostEntry("localhost");
            IPAddress ipAddr = ipHost.AddressList[0];
            IPEndPoint ipEndPoint = new IPEndPoint(ipAddr, 11000);
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
        public static byte[] AskPostImage(int postId)
        {
            byte[] bytes = new byte[7340032]; //7 MB
            IPHostEntry ipHost = Dns.GetHostEntry("localhost");
            IPAddress ipAddr = ipHost.AddressList[0];
            IPEndPoint ipEndPoint = new IPEndPoint(ipAddr, 11000);
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
        public static string AskUsers(int userId, AskUsersType askType)
        {
            byte[] bytes = new byte[1024];
            IPHostEntry ipHost = Dns.GetHostEntry("localhost");
            IPAddress ipAddr = ipHost.AddressList[0];
            IPEndPoint ipEndPoint = new IPEndPoint(ipAddr, 11000);
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
            IPHostEntry ipHost = Dns.GetHostEntry("localhost");
            IPAddress ipAddr = ipHost.AddressList[0];
            IPEndPoint ipEndPoint = new IPEndPoint(ipAddr, 11000);

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
            switch(askType)
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
            IPHostEntry ipHost = Dns.GetHostEntry("localhost");
            IPAddress ipAddr = ipHost.AddressList[0];
            IPEndPoint ipEndPoint = new IPEndPoint(ipAddr, 11000);

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


        public static string PostSend(int usrId, string title, string contentTxt, byte[] contentIMG, string passw)
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
            IPHostEntry ipHost = Dns.GetHostEntry("localhost");
            IPAddress ipAddr = ipHost.AddressList[0];
            IPEndPoint ipEndPoint = new IPEndPoint(ipAddr, 11000);

            Socket sender = new Socket(ipAddr.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

            // Соединяем сокет с удаленной точкой
            try
            {
                sender.Connect(ipEndPoint);
                string message = $"newpost;{usrId};{title};{contentTxt};{passw};{IMGAvlbl}";

                
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
            IPHostEntry ipHost = Dns.GetHostEntry("localhost");
            IPAddress ipAddr = ipHost.AddressList[0];
            IPEndPoint ipEndPoint = new IPEndPoint(ipAddr, 11000);
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
            Thread.Sleep(250);
            // Буфер для входящих данных
            byte[] bytes = new byte[1024];

            // Соединяемся с удаленным устройством

            // Устанавливаем удаленную точку для сокета
            IPHostEntry ipHost = Dns.GetHostEntry("localhost");
            IPAddress ipAddr = ipHost.AddressList[0];
            IPEndPoint ipEndPoint = new IPEndPoint(ipAddr, 11001);

            Socket sender = new Socket(ipAddr.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

            // Соединяем сокет с удаленной точкой
            try
            {
                sender.Connect(ipEndPoint);
                int bytesSent = sender.Send(image);
                // Получаем ответ от сервера
                Thread.Sleep(100);
                sender.Shutdown(SocketShutdown.Both);
                sender.Close();
                string checker = CheckPostImage(postId);
                if(checker == "True")
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
            IPHostEntry ipHost = Dns.GetHostEntry("localhost");
            IPAddress ipAddr = ipHost.AddressList[0];
            IPEndPoint ipEndPoint = new IPEndPoint(ipAddr, 11000);
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
            IPHostEntry ipHost = Dns.GetHostEntry("localhost");
            IPAddress ipAddr = ipHost.AddressList[0];
            IPEndPoint ipEndPoint = new IPEndPoint(ipAddr, 11000);
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
            IPHostEntry ipHost = Dns.GetHostEntry("localhost");
            IPAddress ipAddr = ipHost.AddressList[0];
            IPEndPoint ipEndPoint = new IPEndPoint(ipAddr, 11000);

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
            return null;
        }
        public static string RejectRequest(int targetId, int rejectorId, string password)
        {
            return null;
        }
        public static string CancelRequest(int targetId, int cancellerId, string password)
        {
            return null;
        }

    }
}
