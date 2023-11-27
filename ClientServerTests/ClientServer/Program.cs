using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Text;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ClientServer
{
    public class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Starting Server...");
            StartServer();
        }

        static void StartServer()
        {
            //Set ip Adress to localhost
            IPAddress iP = IPAddress.Parse("127.0.0.1");
            int port = 8888;

            //Creating the TCP Listener
            TcpListener listener = new TcpListener(iP, port);

            listener.Start();
            Console.WriteLine("Server is listening for incoming connections...");

            List<TcpClient> connectedClients = new List<TcpClient>();
            Dictionary<TcpClient, string> clientNames = new Dictionary<TcpClient, string>();


            while (true)
            {
                //Accept all Clients
                TcpClient client = listener.AcceptTcpClient();
                Console.WriteLine("new Client connected!");

                NetworkStream stream = client.GetStream();

                //Get Client Name
                byte[] clientNameBytes = new byte[1024];
                int bytesRead = stream.Read(clientNameBytes, 0, clientNameBytes.Length);
                string clientName = Encoding.ASCII.GetString(clientNameBytes, 0, bytesRead);
                Console.WriteLine($"Received Client name: {clientName}");

                //add new client to the list
                connectedClients.Add(client);
                clientNames.Add(client, clientName);

                //Handle Clients in a own Thread
                Thread clientThread = new Thread(() => HandleClient(client, connectedClients, clientNames));
                clientThread.Start();       
            }
        }

        static void HandleClient(TcpClient client, List<TcpClient> connectedClients, Dictionary<TcpClient, string> clientNames)
        {
            //Storing the count of data read
            int bytesRead;

            string clientName = string.Empty;

            //Create new stream to Read / Write Data
            NetworkStream stream = client.GetStream();

           

            while (true)
            {
                //get data from client
                byte[] databyte = new byte[1024];
                bytesRead = stream.Read(databyte, 0, databyte.Length);
                string data = Encoding.ASCII.GetString(databyte, 0, bytesRead);


                //Disconnect CLient
                if (data == "e")
                {
                    DisconnectClient(client, connectedClients, clientNames);
                    break;
                }


                clientName = clientNames[client];
                //Send message to all Clients
                BroadcastMessage(string.Format($"{clientName}: {data}"), client, connectedClients);


            }

            client.Close();
            Console.WriteLine($"Disconnected Client {clientName}");
        }

        static void DisconnectClient(TcpClient client, List<TcpClient> connectedClients, Dictionary<TcpClient, string> clientNames)
        {
            string name = clientNames[client];
            Console.WriteLine($"{name} has disconnected");
            connectedClients.Remove(client);
            clientNames.Remove(client);
            client.Close();

            BroadcastMessage(string.Format($"{name} has left the chat"), null, connectedClients);
        }

        static void BroadcastMessage(string message, TcpClient sender, List<TcpClient> connectedClients)
        {
            foreach (TcpClient connectedClient in connectedClients)
            {
                //check if current receiver client not sender client
                if (connectedClient != sender)
                {
                    NetworkStream clientStrem = connectedClient.GetStream();
                    byte[] messageBytes = Encoding.ASCII.GetBytes($"{message}");
                    clientStrem.Write(messageBytes, 0, message.Length);
                }
            }
        }
    }
}