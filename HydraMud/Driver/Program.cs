using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace HydraMud.Driver
{
   public class NetworkClient( TcpClient tcpClient )
   {
      {

      }

      private TcpClient TcpClient { get; set; } = tcpClient;

      public async Task GoAsync()
      {
         string clientInfo = TcpClient.Client.RemoteEndPoint.ToString();
         Console.WriteLine("Client connection: " + clientInfo);

         try
         {
            using (var networkStream = TcpClient.GetStream())
            {
               using (var reader = new StreamReader(networkStream))
               {
                  using (var writer = new StreamWriter(networkStream))
                  {
                     writer.AutoFlush = true;

                     while (true)
                     {
                        string dataFromClient = await reader.ReadLineAsync();

                        if (string.IsNullOrEmpty(dataFromClient))
                        {
                           Console.WriteLine("Disconnecting client...");
                           break;
                        }

                        Console.WriteLine("Received data from client: " + dataFromClient);
                        await writer.WriteLineAsync("From server: " + dataFromClient);
                     }
                  }
               }
            }
         }
         catch (Exception ex)
         {
            Console.WriteLine("Exception: " + ex);
         }
         finally
         {
            //tcpClient.Close();
            Console.WriteLine("Client disconnected");
         }
      }
   }

   public static class GlobalConnectionTable
   {

   }

   public class Server
   {
      public async Task Start()
      {
         var tcpListener = new TcpListener(IPAddress.Any, 7777);

         tcpListener.Start();

         while ( true )
         {
            var tcpClient = await tcpListener.AcceptTcpClientAsync();

            var networkClient = new NetworkClient(tcpClient);

            networkClient.GoAsync().Forget();

            //HandleNewClient(tcpClient).Forget();
         }
      }
   }

   internal static class Program
   {
      private static void Main( string[] args )
      {
         var server = new Server();

         server.Start().Forget();

         Console.WriteLine("Awaiting connections...");
         Console.ReadLine();
      }
   }
}
