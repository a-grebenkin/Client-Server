using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
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

namespace Server
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        IPAddress localAddr = IPAddress.Parse("127.0.0.1");
        int port = 8888;
        TcpListener tcpListener;
        TcpClient tcpClient;
        NetworkStream stream;

        public MainWindow()
        {
            InitializeComponent();
            tcpListener = new TcpListener(localAddr, port);
            tcpListener.Start();
            tcpClient = tcpListener.AcceptTcpClient();
            stream = tcpClient.GetStream();
            Task.Factory.StartNew(ReciveMessage);
        }

        private void AddTextBlockLine(string message)
        {
            this.Dispatcher.Invoke(() =>
            {
                textBlock.Text += message + '\n';
            });
        }

        private void ReciveMessage()
        {
            while (true)
            {
                byte[] data = new byte[64];
                StringBuilder builder = new StringBuilder();
                do
                {
                    int bytes = stream.Read(data, 0, data.Length);
                    builder.Append(Encoding.Unicode.GetString(data, 0, bytes));
                }
                while (stream.DataAvailable);

                string message = builder.ToString();
                AddTextBlockLine(message);
                SendMessage(message);
                Thread.Sleep(100);
            }
        }

        private void SendMessage(string message)
        {
            string response = "Server received: " + message;

            byte[] data = Encoding.Unicode.GetBytes(response);
            stream.Write(data, 0, data.Length);
        }
    }
}
