using System;
using System.Drawing;
using System.Windows.Forms;
using System.Net.Sockets;
using System.Net;
using System.Threading;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace Server
{
    class Program
    {
        static byte[] Serializer(object _object)
        {
            byte[] bytes;
            using (var _MemoryStream = new MemoryStream())
            {
                IFormatter _BinaryFormatter = new BinaryFormatter();
                _BinaryFormatter.Serialize(_MemoryStream, _object);
                bytes = _MemoryStream.ToArray();
            }
            return bytes;
        }

        static void SendPrintScreen(object obj)
        {
            Bitmap printscreen = new Bitmap(Screen.PrimaryScreen.Bounds.Width, Screen.PrimaryScreen.Bounds.Height);
            Graphics graphics = Graphics.FromImage(printscreen as Image);
            graphics.CopyFromScreen(0, 0, 0, 0, printscreen.Size);
           
            byte[] printscreenArr = Serializer(printscreen);
            Console.WriteLine(printscreenArr.Length);
            (obj as TcpClient).GetStream().Write(printscreenArr, 0, printscreenArr.Length);
            (obj as TcpClient).Close();
        }
        static void Main(string[] args)
        {
            TcpListener server = new TcpListener(IPAddress.Parse("127.0.0.1"), 7777);
            server.Start();
            while (true)
            {
                TcpClient client = server.AcceptTcpClient();
                try
                {
                    Console.WriteLine(client.Client.RemoteEndPoint);
                    ThreadPool.QueueUserWorkItem(SendPrintScreen, client);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    Console.WriteLine(ex.StackTrace);
                }
            }
        }
    }
}
