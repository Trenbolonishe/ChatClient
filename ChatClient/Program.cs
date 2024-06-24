using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace ChatClient
{
    class Program
    {
        static Socket clientSocket;

        static void Main(string[] args)
        {
            IPAddress ip = IPAddress.Parse("127.0.0.1");
            IPEndPoint endpoint = new IPEndPoint(ip, 12345);
            clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            try
            {
                clientSocket.Connect(endpoint);
                Console.WriteLine("Подключен к серверу.");

                Thread receiveThread = new Thread(ReceiveMessages);
                receiveThread.Start();

                while (true)
                {
                    string message = Console.ReadLine();
                    SendMessage(message);
                }
            }
            catch (SocketException)
            {
                Console.WriteLine("Не удалось подключиться к серверу.");
            }
        }

        static void ReceiveMessages()
        {
            while (true)
            {
                try
                {
                    byte[] buffer = new byte[1024];
                    int receivedBytes = clientSocket.Receive(buffer);
                    if (receivedBytes == 0) break;
                    string message = Encoding.UTF8.GetString(buffer, 0, receivedBytes);
                    Console.WriteLine("Получено сообщение: {0}", message);
                }
                catch (SocketException)
                {
                    Console.WriteLine("Соединение с сервером потеряно.");
                    clientSocket.Close();
                    break;
                }
            }
        }

        static void SendMessage(string message)
        {
            byte[] buffer = Encoding.UTF8.GetBytes(message);
            clientSocket.Send(buffer);
        }
    }
}