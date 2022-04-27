using System;
using System.Collections.Generic;

namespace MACROSCOP_CLIENT
{
    public class ConsoleMenu
    {
        // Delegate
        public delegate void MenuMethod();
        // Delegate list
        public List<MenuMethod> Methods;
        public List<string> MethodsNames;

        public ConsoleMenu(string[] methodsNames, params MenuMethod[] methods)
        {
            Methods = new List<MenuMethod>();
            Methods.AddRange(methods);
            MethodsNames = new List<string>();
            MethodsNames.AddRange(methodsNames);
            ItemColor = ConsoleColor.Yellow;
            SelectionColor = ConsoleColor.Blue;
        }

        public ConsoleColor ItemColor;
        public ConsoleColor SelectionColor;
        public int SelectedItem { get; private set; }
        // First line position
        private int top;

        // Displays menu, changes option colors, invokes arrows handler
        public void Show(bool addLineBefore = true, string line = "")
        {
            top = Console.CursorTop;

            if (addLineBefore)
            {
                // Header
                Console.WriteLine(line); 
                top++;
            }
            Console.ForegroundColor = ItemColor;

            for (int i = 0; i < Methods.Count; i++)
            {
                if (i == SelectedItem)
                {
                    Console.BackgroundColor = SelectionColor;
                }
                else
                {
                    Console.ResetColor();
                    Console.ForegroundColor = ItemColor;
                }
                Console.WriteLine(MethodsNames[i]);
            }
            Console.ResetColor();
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.WriteLine("Нажмите ESC для выхода...");
            Console.ResetColor();
            WaitForInput();
        }

        // Handle arrow click
        private void WaitForInput()
        {
            ConsoleKeyInfo cki = Console.ReadKey();
            switch (cki.Key)
            {
                case ConsoleKey.DownArrow:
                    MoveDown();
                    break;
                case ConsoleKey.UpArrow:
                    MoveUp();
                    break;
                case ConsoleKey.Enter:
                    Methods[SelectedItem]();
                    Show();
                    break;
                case ConsoleKey.Escape:
                    return;
                default:
                    Console.Clear();
                    Show();
                    break;
            }
        }

        // Brings cursor down or to the beginning
        private void MoveDown()
        {
            SelectedItem = SelectedItem == Methods.Count - 1 ? 0 : SelectedItem + 1;
            Console.SetCursorPosition(0, top);
            Show(false);
        }

        // Brings cursor up or to the end
        private void MoveUp()
        {
            SelectedItem = SelectedItem == 0 ? Methods.Count - 1 : SelectedItem - 1;
            Console.SetCursorPosition(0, top);
            Show(false);
        }
    }
}
