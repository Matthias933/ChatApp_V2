using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Collections.Generic;

namespace Server
{
    public class Server
    {
        private static TcpListener listener;
        private static List<NetworkStream> clientStreams = new List<NetworkStream>();

        static void Main(string[] args)
        {
            Console.WriteLine("+++++++++++++++++++++++++++++++++++++++++++++++");
            Console.WriteLine("Welcome to a simple Client / Server TIC TAC TOE Server\n");
            Console.WriteLine("Listening for client connections...");

            // Start the server and listen for incoming client connections.
            StartServer();
        }

        public static void StartServer()
        {
            // Set up the server to listen on a specific IP address and port.
            IPAddress iP = IPAddress.Parse("127.0.0.1");
            int serverPort = 8888;
            
            listener = new TcpListener(iP, serverPort);
            listener.Start();

            // Accept client connections and start a new thread for each client.
            while (true)
            {
                if(clientStreams.Count < 2) 
                {
                    TcpClient client = listener.AcceptTcpClient();
                    NetworkStream clientStream = client.GetStream();
                    clientStreams.Add(clientStream);

                    // Handle the client in a new thread.
                    Thread clientThread = new Thread(() => HandleClient(clientStream));
                    clientThread.Start();
                }
            }
        }

        private static void HandleClient(NetworkStream clientStream)
        {
            byte[] nameBuffer = new byte[1024];
            int bytesRead = clientStream.Read(nameBuffer, 0, nameBuffer.Length);

            if (bytesRead > 0)
            {
                string playerName = Encoding.ASCII.GetString(nameBuffer, 0, bytesRead);
                Console.WriteLine($"Player '{playerName}' connected.");

                // Inform other players about the new player.
                BroadcastMessage(playerName + " joined the game.");

                while (true)
                {
                    byte[] turnBuffer = new byte[1024];
                    bytesRead = clientStream.Read(turnBuffer, 0, turnBuffer.Length);

                    if (bytesRead > 0)
                    {
                        string turn = Encoding.ASCII.GetString(turnBuffer, 0, bytesRead);
                        Console.WriteLine($"Received move from {playerName}: {turn}");

                        // Broadcast the move to other players.
                        BroadcastMessage($"{turn}");
                    }
                }
            }
        }

        private static void BroadcastMessage(string message)
        {
            foreach (var clientStream in clientStreams)
            {
                byte[] messageBytes = Encoding.ASCII.GetBytes(message);
                clientStream.Write(messageBytes, 0, messageBytes.Length);
            }
        }
    }
}