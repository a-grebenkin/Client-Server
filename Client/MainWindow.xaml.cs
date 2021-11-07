using System;
using System.Collections.Generic;
using System.Linq;
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

namespace Client
{
    public partial class MainWindow : Window
    {
        private const string host = "127.0.0.1";
        private const int port = 8888;

        TcpClient tcpClient;
        NetworkStream stream;

        public MainWindow()
        {
            tcpClient = new TcpClient();
            tcpClient.Connect(host, port);
            stream = tcpClient.GetStream();
            InitializeComponent();
            Task.Factory.StartNew(ReciveMessage);
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Task.Factory.StartNew(SendTextBoxValue);
        }

        private void SendTextBoxValue()
        {
            this.Dispatcher.Invoke(() =>
            {
                SendMessage(textBox1.Text);
            });
        }

        private void SendMessage(string message)
        {
            this.Dispatcher.Invoke(() =>
            {
                message = textBox1.Text;
            });
            byte[] data = Encoding.Unicode.GetBytes(message);
            stream.Write(data, 0, data.Length);
        }

        private void SetTextBoxValue(string message)
        {
            this.Dispatcher.Invoke(() =>
            {
                label1.Content = message;
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

                SetTextBoxValue(builder.ToString());

                Thread.Sleep(100);
            }
        }
    }
}
