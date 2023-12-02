using System;
using System.Net;
using System.Net.Sockets;
using System.Reflection.Metadata;

namespace ChatApp
{
    class Server
    {
        TcpListener server;
        TcpClient client;
        IPAddress ip;
        int port;

        public Server(IPAddress serverIP, int serverPort)
        {
            this.ip = serverIP;
            this.port = serverPort;
        }

        public void run()
        {
            server = new TcpListener(ip, port);
            server.Start();
            Console.WriteLine($"Server gestartet auf {ip}:{port}.");
            while (true)
            {
                client = server.AcceptTcpClient();
                Console.WriteLine("Client hat sich verbunden.");
                ClientHandler clientHandler = new ClientHandler(client);
                Thread clientThread = new Thread(clientHandler.communicate);
                clientThread.Start();
            }
        }
    }

    class ClientHandler
    {
        TcpClient client;
        NetworkStream stream;
        Byte[] lesePuffer;
        Byte[] schreibePuffer;
        string zeichenkette;
        int zahlenkette;
        public ClientHandler(TcpClient c)
        {
            this.client = c;
        }
        public void communicate()
        {
            while (true)
            {
                stream = client.GetStream();
                lesePuffer = new byte[256];
                zahlenkette = stream.Read(lesePuffer, 0, lesePuffer.Length);
                if (zahlenkette != 0)
                {
                    zeichenkette = System.Text.Encoding.ASCII.GetString(lesePuffer, 0, zahlenkette);
                    Console.WriteLine("Empfangen: " + zeichenkette);
                }
            }
        }
    }

    class Client
    {
        TcpClient client;
        NetworkStream stream;
        Byte[] lesePuffer;
        Byte[] schreibePuffer;
        string zeichenkette;
        int zahlenkette;

        public Client(string serverIP, int serverPort)
        {
            client = new TcpClient(serverIP, serverPort);
            stream = client.GetStream();
        }

        public void receive()
        {
            while (true)
            {
                lesePuffer = new Byte[256];
                zahlenkette = stream.Read(lesePuffer, 0, lesePuffer.Length);
                zeichenkette = System.Text.Encoding.ASCII.GetString(lesePuffer, 0, zahlenkette);
                Console.WriteLine("Client bekommt: " + zeichenkette);
            }
        }

        public void send()
        {
            while (true)
            {
                zeichenkette = String.Empty;
                Console.WriteLine("Geben Sie eine Nachricht ein: ");
                zeichenkette = Console.ReadLine();
                schreibePuffer = System.Text.Encoding.ASCII.GetBytes(zeichenkette);
                stream.Write(schreibePuffer, 0, schreibePuffer.Length);
            }
        }
    }

    class Program
    {
        public static void Main(string[] args)
        {
            string ipString = "127.0.0.1";
            IPAddress ip = IPAddress.Parse(ipString);
            int port = 8080;
            bool isServer = false;

            Console.WriteLine("Möchten Sie als Server (S) oder Client (C) starten?");
            string mode = Console.ReadLine().ToUpper();

            if (mode == "S")
            {
                isServer = true;
            }
            else if (mode != "C")
            {
                Console.WriteLine("Ungültiger Modus. Programm wird beendet.");
                return;
            }
            if (isServer)
            {
                Server server = new Server(ip, port);
                server.run();
            }
            else
            {
                Client client = new Client(ipString, port);
                client.send();
            }
        }
    }
}