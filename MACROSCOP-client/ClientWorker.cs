using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace MACROSCOP_CLIENT
{
    class ClientWorker
    { 
        // Adress and port to connect - local server.
        const int port = 8888;
        const string address = "127.0.0.1";

        // Standard file directory. Could be changed by user with the option in the console
        public string FilesDirectory { get; set; } = @"..\words";

        // Sends files to server. 1 file = 1 request.
        // For each file creating a new connection 
        public static void ClientRun(FileOperations fileObj)
        {
            List<Task> listOfTasks = new();
            foreach (string word in fileObj.GetFile())
            {
                Task task = new(() => SubClientRun(fileObj, word));
                listOfTasks.Add(task);
                task.Start();
            }
            Task.WaitAll(listOfTasks.ToArray());
        }
        // Client logic starts here
        public static void SubClientRun(FileOperations fileObj, string stringToServer)  
        {
            TcpClient client = null;
            try
            {
                // Connecting to server
                client = new TcpClient(address, port);
                if (client.Connected)
                {
                    using NetworkStream ns = client.GetStream();

                    // Checking if the server is busy
                    string acceptMessage = GetMessage(ns);

                    // Accept message is a response of a server for a request
                    // Soon will rewrite this part so server sends answer byte 0 or 1 instead of a string.
                    if (acceptMessage == "Connected to the server")
                    {
                        Console.WriteLine($"Server: {acceptMessage}");
                        SendData(client, stringToServer);
                        string message = GetMessage(ns);
                        Console.WriteLine($"Server: {message}");
                    }
                    else
                    {
                        // Uncomment to see the server answer if it busy
                        //
                        //Console.WriteLine($"Server: {acceptMessage}");
                        //
                        // Creates a new connection in case of server is busy
                        // !!!Can go into forever loop, temporary solution!!!
                        var t = Task.Run(async () =>
                        {
                            await Task.Delay(1000);
                        });
                        t.Wait();
                        Task.Factory.StartNew(() => SubClientRun(fileObj, stringToServer), TaskCreationOptions.AttachedToParent);
                    }

                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                if (client != null)
                {
                    if (client.Connected)
                        client.GetStream().Close();
                    client.Close();
                }
            }
        }
        
        public static string GetMessage(NetworkStream ns)
        {
            StringBuilder builder = new();
            // Gets server answer
            int bytes = 0;
            byte[] DataReceive = new byte[64];
            do
            {
                bytes = ns.Read(DataReceive, 0, DataReceive.Length);
                builder.Append(Encoding.Unicode.GetString(DataReceive, 0, bytes));
            }
            while (ns.DataAvailable);
            if (builder.Length > 0)
            {
                return builder.ToString();
            }
            return null;
        }
        public static void SendData(TcpClient client, string text)
        {
            if (client.Connected)
            {
                NetworkStream ns = client.GetStream();
                // Data to ByteArray
                byte[] data = Encoding.Unicode.GetBytes(text);
                // Sending a file
                ns.Write(data, 0, data.Length);
            }
        }
    }
}
