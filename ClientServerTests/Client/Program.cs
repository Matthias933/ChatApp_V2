using System;
using System.Linq.Expressions;
using System.Net;
using System.Net.Sockets;
using System.Text;


namespace Client
{
    public class Program
    {
        static bool isDisconnected = false;

        static void Main(string[] args)
        {
            Console.WriteLine("Please enter your Name:");
            string clientName = Console.ReadLine();

            Console.WriteLine("Starting Client...");
            StartClient(clientName);
        }

        static void StartClient(string ClientName)
        {
            

            //Set Localhost ip and Server Port
            IPAddress iP = IPAddress.Parse("127.0.0.1");
            int serverPort = 8888;

            //Create a new TCP Client
            TcpClient client = new TcpClient();

            //Connect to server
            client.Connect(iP, serverPort);
            Console.WriteLine("Connected To Server");

            //Create new stream to Read / Write Data
            NetworkStream stream = client.GetStream();
           
            //Send Name to Server
            byte[] nameByte = Encoding.ASCII.GetBytes(ClientName);
            stream.Write(nameByte, 0, nameByte.Length);

            //Handle Received Messaged in a own Thread
            Thread receiveThread = new Thread(() => ReceiveMessages(stream));
            receiveThread.Start();

            while(isDisconnected == false)
            {
                string message = Console.ReadLine();

                if (message == "e")
                {
                    isDisconnected = true;
                    break;
                }

                //Send data to server
                byte[] messageByte = Encoding.ASCII.GetBytes(message);
                stream.Write(messageByte, 0, messageByte.Length);
            }


            //Disconnect Client Again
            client.Close();
            Console.WriteLine("Disconnected from the Server");
        }

        static void ReceiveMessages(NetworkStream stream)
        {
            byte[] response = new byte[1024];
            while (isDisconnected == false)
            {
                try
                {
                    int bytesRead = stream.Read(response, 0, response.Length);
                    string responseMessage = Encoding.ASCII.GetString(response, 0, bytesRead);
                    Console.WriteLine(responseMessage);
                }
                catch(IOException ex)
                {
                    Console.WriteLine("Could not read last response");
                }
            }
        }
    }
}