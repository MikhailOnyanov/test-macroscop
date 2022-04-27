using System;
using System.IO;
using System.Net.Sockets;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MACROSCOP_CLIENT
{
    class Program
    {
        const int port = 8888;
        const string address = "127.0.0.1";
        public static string FilesDirectory { get; private set; } = @"C:\Users\misa5\source\repos\MACROSCOP-client\words";
        private static FileOperations fileObject;
        static void Main(string[] args)
        {
            
            string[] options = new string[] {"Задать путь к директории", "Отправить данные"};
            ConsoleMenu menu = new(options, UISetPath, UISendData);
            menu.Show(true, "Welcome to MACROSCOP-client app!");
        }
        
        #region UI methods
        static void UISetPath()
        {
            FilesDirectory = SetPath();
            UIContinue();
            //fileObject = new(FilesDirectory);
        }
        static void UISendData()
        {
            fileObject = new(FilesDirectory);
            ClientWorker.ClientRun(fileObject);
            UIContinue();
        }
        static void UIContinue()
        {
            Console.WriteLine("Нажмите любую клавишу, чтобы продолжить...");
            Console.ReadKey();
            Console.Clear();
        }
        #endregion
        public static string SetPath()
        {
            Console.WriteLine("Вставьте адрес папки с файлами, которые нужно обработать: ");
            string directory = Console.ReadLine();
            while (!Directory.Exists(directory))
            {
                Console.WriteLine("Такой директории не существует! Повторите ввод: ");
                directory = Console.ReadLine();
            }
            Console.WriteLine($"Текущая директория: {directory}");
            return directory;
        }
    }
}
