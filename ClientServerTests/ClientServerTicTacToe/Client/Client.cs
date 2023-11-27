using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Client
{
    public class Client
    {
        private static bool myTurn = true;
        private static string placeSign = "x";
        private static string[] grid = {"1", "2", "3", "4", "5", "6", "7", "8", "9", }; 

        static void Main(string[] args)
        {
            Console.WriteLine("+++++++++++++++++++++++++++++++++++++++++++++++");
            Console.WriteLine("Welcome to a simple Client / Server TIC TAC TOE\n");
            Console.WriteLine("To Continue please enter your name...\n");
            string name = Console.ReadLine();
            Console.WriteLine("Connecting to server...");
            StartClient(name);

        }

        public static void StartClient(string name)
        {
            IPAddress iP  = IPAddress.Parse("127.0.0.1");
            int serverPort = 8888;

            TcpClient tcpClient = new TcpClient();

            tcpClient.Connect(iP, serverPort);
            Console.WriteLine("Connected to Server");

            NetworkStream stream = tcpClient.GetStream();

            //send name to server
            byte[] nameByte = Encoding.ASCII.GetBytes(name);
            stream.Write(nameByte, 0, nameByte.Length);

            //Handle received Messages in a own Task
            Thread receiveThread = new Thread(() => ReceiveMessages(stream));
            receiveThread.Start();

            //Handle sending Messages in a own Task
            Thread sendThread = new Thread(() => SendMessages(stream));
            sendThread.Start();
        }

        private static void SendMessages(NetworkStream stream)
        {
            while(true)
            {
                if (myTurn)
                {
                    Console.WriteLine("Please enter a valid move...");
                    string turn = Console.ReadLine();
                    UpdateGrid(turn);
                    byte[] turnByte = Encoding.ASCII.GetBytes(turn);
                    stream.Write(turnByte, 0, turnByte.Length);
                }
            }
        }

        private static void ReceiveMessages(NetworkStream stream)
        {
            byte[] msg = new byte[1024];
            while (true)
            {
                try
                {
                    int bytesRead = stream.Read(msg, 0, msg.Length);
                    if (bytesRead > 0)
                    {
                        string response = Encoding.ASCII.GetString(msg, 0, bytesRead);
                        if(Int32.TryParse(response, out bytesRead))
                        {
                            UpdateGrid(response);
                        }
                        else
                        {
                            Console.WriteLine("Could not parse data");
                        }
                    }
                }
                catch(IOException ex)
                {
                    Console.ForegroundColor  = ConsoleColor.Red;    
                    Console.WriteLine("Could not read response data from Server");
                    Console.ForegroundColor = ConsoleColor.White;
                }
               
            }

        }

        private static void UpdateGrid(string turn)
        {
            Console.Clear();
            grid[Int32.Parse(turn)] = placeSign;
            Console.WriteLine("{0} | {1} | {2}", grid[0], grid[1], grid[2]);
            Console.WriteLine("---------------");
            Console.WriteLine("{0} | {1} | {2}", grid[3], grid[4], grid[5]);
            Console.WriteLine("---------------");
            Console.WriteLine("{0} | {1} | {2}", grid[6], grid[7], grid[8]);
        }
    }
}