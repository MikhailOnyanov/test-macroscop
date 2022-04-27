using System;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MACROSCOP_backend
{
    public class ClientObject
    {
        private TcpClient client;
        private NetworkStream ns;
        public ClientObject(TcpClient tcpClient)
        {
            client = tcpClient;
            ns = client.GetStream();
        }
        public void Process()
        {
            try
            {
                // Buff for receiving data
                byte[] data = new byte[64];

                StringBuilder builder = new();
                int bytes = 0;

                // Receiving the message
                do
                {
                    bytes = ns.Read(data, 0, data.Length);
                    builder.Append(Encoding.Unicode.GetString(data, 0, bytes));
                }
                while (ns.DataAvailable && ns.CanRead);

                string message = builder.ToString();
                Console.WriteLine($"Client: {client.Client.RemoteEndPoint}: {message}");

                bool isPalindromResult = IsPalindrom(message.Trim().ToCharArray());
                string stringToClient = message.Trim() + " : " + isPalindromResult.ToString().Trim().ToUpper();

                // Imitate long work
                Thread.Sleep(2000);
                // Send result
                SendMessage(ns, stringToClient);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                Console.WriteLine($"Завершил работу с клиентом {client.Client.RemoteEndPoint}\n");
                if (client.Connected)
                    Close(client.GetStream());
            }
        }
        private void Close(NetworkStream ns)
        {
            if (ns != null)
                ns.Close();
            if (client != null)
                client.Close();
        }
        // Sends text in current NetworkStream
        public static void SendMessage(NetworkStream ns, string text)
        {
            byte[] data = Encoding.Unicode.GetBytes(text);
            ns.WriteAsync(data, 0, data.Length);
        }
        private static bool IsPalindrom(char[] text)
        {
            int lenText = text.Length;
            for (int i = 0; i < text.Length / 2; i++)
            {
                if (text[i] != text[lenText - i - 1]) 
                    return false;
            }
            return true;
        }
    }
}
