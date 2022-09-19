using System;
using System.IO;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Text;
using System.Threading.Tasks;
using System.Reflection.Emit;

namespace EbaySellerStuffSearch
{
    public class MainServer
    {
        //private static Socket httpServer;
        private static int serverPort = 80;
        //private static Thread thread;

        //static IPEndPoint endPoint;

        static bool finishServer = false;

        public async static Task RunServer()
        {
            try
            {
                var listener = new TcpListener(IPAddress.Any, serverPort);
                listener.Start();
                Console.WriteLine("Server started");

                while (!finishServer)
                {
                    Console.WriteLine("Waiting for a connection...");
                    TcpClient client = listener.AcceptTcpClient();
                    Console.WriteLine("Client accepted");

                    using (NetworkStream stream = client.GetStream())
                    using (StreamWriter writer = new StreamWriter(stream))
                    using (StreamReader reader = new StreamReader(stream))
                    {
                        writer.AutoFlush = true;
                        foreach (var line in File.ReadAllLines("/Users/mihails/Desktop/EbayProducts.txt"))
                        {
                            writer.WriteLine(line);
                        }

                        writer.WriteLine("No more products");
                        writer.Flush();
                    }

                    Console.WriteLine("All products were send to client");
                    client.Close();
                }

                listener.Stop();

                finishServer = false;

                //httpServer = new Socket(SocketType.Stream, ProtocolType.Tcp);
                //if (serverPort > 65535 || serverPort <= 0)
                //{
                //    throw new Exception("Server port is incorrect");
                //}

                //thread = new Thread(new ThreadStart(connentionThreadMethod));
                //thread.Start();


            }
            catch (Exception)
            {
                Console.WriteLine("Error while starting the server");
            }

            Console.WriteLine("Server is off");
        }

        //static void connentionThreadMethod()
        //{
        //    try
        //    {
        //        endPoint = new IPEndPoint(IPAddress.Any, serverPort);
        //        httpServer.Bind(endPoint);
        //        httpServer.Listen(1);
        //        startListeningForConnection();
        //    }
        //    catch (Exception e)
        //    {
        //        Console.WriteLine("I could not start\n\n" + e);
        //    }
        //}

        public static bool StopServer()
        {
            finishServer = true;

            TcpClient client = new TcpClient("127.0.0.1", serverPort);
            using (NetworkStream stream = client.GetStream()) { }

            return true;
        }

        //static void startListeningForConnection()
        //{
        //    while (true)
        //    {
        //        DateTime time = DateTime.Now;

        //        String data = "";
        //        byte[] bytes = new byte[2048];
        //        Socket client = httpServer.Accept();

        //        while (true)
        //        {
        //            int numBytes = client.Receive(bytes);
        //            data += Encoding.ASCII.GetString(bytes, 0, numBytes);

        //            if (data.IndexOf("\r\n") >= 0)
        //                break;
        //        }

        //        Console.WriteLine("\n\n" + System.Text.Encoding.Default.GetString(bytes));


        //        string text = "";

        //        foreach (string s in File.ReadAllLines("/Users/mihails/Desktop/EbayProducts.txt"))
        //        {
        //            text += s + "<br>";
        //        }

        //        string resHeader = "HTTP/1.1 200 Its ok\nServer: my CSharpServer\nContent-Type: text/html; charset: UTF-8\n\n";

        //        string resBody = File.ReadAllText("/Users/mihails/Desktop/EbayProducts.txt");


        //        string resStr = resHeader + resBody;

        //        byte[] resData = Encoding.ASCII.GetBytes(resStr);

        //        client.SendTo(resData, client.RemoteEndPoint);

        //        Console.WriteLine("\n\n------------End of connection-----------");



        //        client.Close();
        //    }
        //}
    }


}

