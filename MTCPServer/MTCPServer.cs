using System;
using System.Net;
using System.Text;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace MTCPServer
{
    public static class Server
    {
        public static string DefaultGreetingMessage            = "Hello!";
        private static string DefaultWaitingMessage         = "Waiting for new client...";
        private static string DefaultClientIsConnecting = "Client is connecting...";
        private static string DefaultClientIsConnectedMessage = "Client is connected!";
        private static string DefaultClosingConnectionMessage = "Closing connection...";
        private static string DefaultSendNextTextMessage      = "Send next data: [enter 'quit' to terminate]";
        
        private static IPAddress   Address  { get; set; }
        private static TcpListener Listener { get; set; }

        public static void Start(int port, string ip = "0.0.0.0")
        {
            if (Listener == null)
            {
                InitListener(port, ip);
            }

            Listen();
        }

        private static void InitListener(int port, string ip)
        {
            Address = IPAddress.Parse(ip);
            Listener = new TcpListener(Address, port);
            Listener.Start();
        }

        private static void Listen()
        {
            while (true)
            {
                ShowWaitingMessageInConsole();
                Task<TcpClient>ClientTask = Listener.AcceptTcpClientAsync();
                if (ClientTask.Result == null) continue;
                ShowClientIsConnectingInConsole();
                
                var clientConnection = new TcpClientConnection(ClientTask);
                ThreadPool.QueueUserWorkItem(Process, clientConnection );
                ShowClientIsConnectedMessageInConsole();
            }
        }

        private static void Process(object obj)
        {
            var clientConnection= (TcpClientConnection) obj;
            clientConnection.SendMessageToClient(DefaultSendNextTextMessage);
            while (!clientConnection.HasClosingConnectionRequest() && clientConnection.IsClientConnected())
            { 
                bool hasNoMessageFromClient= !clientConnection.HasMessageFromClient();
                if (hasNoMessageFromClient) continue;
                clientConnection.ReceiveMessageFromClient();
                clientConnection.ShowMessageFromClientInConsole();
                clientConnection.SendMessageToClient(DefaultSendNextTextMessage);
            }
            ShowClosingConnectionMessageInConsole();
            clientConnection.CloseConnection();
        }

        private static void ShowClosingConnectionMessageInConsole()
        {
            Console.WriteLine(DefaultClosingConnectionMessage);
        }

        private static void ShowWaitingMessageInConsole()
        {
            Console.WriteLine(DefaultWaitingMessage);
        }
        
        private static void ShowClientIsConnectingInConsole()
        {
            Console.WriteLine(DefaultClientIsConnecting);
        }

        private static void ShowClientIsConnectedMessageInConsole()
        {
            Console.WriteLine(DefaultClientIsConnectedMessage);
        }
    }
}