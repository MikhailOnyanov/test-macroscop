using System;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using System.IO;

namespace MACROSCOP_backend
{
    public class Program
    {
        // Count of requests that server can handle in one time
        public static int maxRequestCount { get; private set; }
        // Current requests count. Increments or decrements in each client with mutexObject
        public static int currentRequestCount { get; set; } = 0;
        public static Mutex mutexObject = new();
        const int port = 8888;

        static TcpListener listener;
        public static async Task Main(string[] args)
        {
            Console.WriteLine("Введите максимальное кол-во одновременно обрабатываемых запросов сервером: ");
            maxRequestCount = InputDigit();
            // Runs the server
            await ServerRun();
        }
        private static async Task ServerRun()
        {
            try
            {
                listener = new TcpListener(IPAddress.Any, port);
                listener.Start();

                // For a future use
                CancellationToken cancellationTokenSource = new();

                while (!cancellationTokenSource.IsCancellationRequested)
                {
                    Console.WriteLine("Ожидание подключений...");
                    // Accepting new client
                    var client = await listener.AcceptTcpClientAsync();
                    _ = Task.Run(async () =>
                    {
                        await HandleTcpClientAsync(client);
                    });
                    // Run new thread to work with client
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                if (listener != null)
                    listener.Stop();
            }
        }
        // Client logic starts here
        private static async Task HandleTcpClientAsync(TcpClient client)
        {
            using NetworkStream ns = client.GetStream();
            Console.WriteLine($"Подключил клиента: {client.Client.RemoteEndPoint}");

            // Get current requests count
            mutexObject.WaitOne();
            int current = currentRequestCount++;
            mutexObject.ReleaseMutex();

            // Checking if server can work with one more client
            // If is true disconnects the client
            if (current >= maxRequestCount)
            {
                string stringToClient = "Server is busy!";
                byte[] data = Encoding.Unicode.GetBytes(stringToClient);
                
                // Write the data so the client gets the message
                ns.WriteAsync(data, 0, data.Length);

                mutexObject.WaitOne();
                currentRequestCount--;
                mutexObject.ReleaseMutex();

                Console.WriteLine($"Disconnected the {client.Client.RemoteEndPoint} - server busy.");
                client.GetStream().Close();
                client.Close();
            }
            else
            {
                ClientObject obj = new(client);
                // Send that server is not busy now
                ClientObject.SendMessage(ns, "Connected to the server");
                
                // Start synchronous client Process
                obj.Process();

                mutexObject.WaitOne();
                currentRequestCount--;
                mutexObject.ReleaseMutex();
            }
        }
        private static int InputDigit()
        {
            bool success = false;
            int number;
            do
            {
                string text = Console.ReadLine();
                success = int.TryParse(text, out number);
                if (!success) Console.WriteLine($"Can't parse '{text}' into the 'int' format. Repeat the input: ");
            } while (!success);
            return number;
        }
    }
}
