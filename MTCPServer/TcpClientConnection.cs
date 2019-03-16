using System;
using System.Diagnostics;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace MTCPServer
{
    public class TcpClientConnection
    {
        
        public string MessageFromClient { get; set; }
        public TcpClientConnection(Task<TcpClient> clientTask)
        {
            Client = clientTask.Result;
             _buffer = new byte[DefaultBufferSize];
        }

        public void CloseConnection()
        {
            Client.GetStream().Dispose();
        }

        public void SendMessageToClient(string messageToClient)
        {
            byte[] data = Encoding.UTF8.GetBytes(messageToClient);
            Client.GetStream().Write(data, 0, data.Length);
        }

        public void ReceiveMessageFromClient()
        {
            Client.GetStream().Read(_buffer, 0, _buffer.Length);
            MessageFromClient = Encoding.UTF8.GetString(_buffer);
            ResetBuffer();
        }

        public void ShowMessageFromClientInConsole()
        {
            Console.WriteLine(MessageFromClient);
        }

        public bool HasMessageFromClient()
        {
            return Client.Available > 0;
        }

        public bool HasClosingConnectionRequest()
        {
            var result = false;
            if (MessageFromClient != null)
            result = (MessageFromClient.IndexOf(DefaultCloseConnectionPattern, StringComparison.Ordinal) > -1);
            return result;
        }

        public bool IsClientConnected()
        {
            return Client.Connected;
        }

        private void ResetBuffer()
        {
            _buffer = new byte[DefaultBufferSize];
        }

        public static string DefaultCloseConnectionPattern { get; set; } = "quit";
        public uint DefaultBufferSize { get; set; } = 1024;

        private TcpClient Client { get; set; }
        private static byte[] _buffer;
    }
}