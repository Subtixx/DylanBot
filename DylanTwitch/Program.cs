using System;
using System.IO;
using System.Runtime.InteropServices;

//27yqxc53mrldhm1mwtobwuqbr7x85f6
//

namespace DylanTwitch
{
    internal class Program
    {
        private const int MF_BYCOMMAND = 0x00000000;
        public const int SC_CLOSE = 0xF060;

        [DllImport("user32.dll")]
        public static extern int DeleteMenu(IntPtr hMenu, int nPosition, int wFlags);

        [DllImport("user32.dll")]
        private static extern IntPtr GetSystemMenu(IntPtr hWnd, bool bRevert);

        [DllImport("kernel32.dll", ExactSpelling = true)]
        private static extern IntPtr GetConsoleWindow();

        [STAThread]
        private static void Main(string[] args)
        {
            // Disable X button on the console window.
            DeleteMenu(GetSystemMenu(GetConsoleWindow(), false), SC_CLOSE, MF_BYCOMMAND);

            // Pre-Startup Checks (Ensures our directories are in place.)
            if (!Directory.Exists("Plugins"))
                Directory.CreateDirectory("Plugins");
            if (!Directory.Exists("Settings"))
                Directory.CreateDirectory("Settings");

            var bot = new ChatBot();
            PluginSystem.Initialize();
            do
            {
                while (!Console.KeyAvailable)
                {
                    // Do something
                }
            } while (Console.ReadKey(true).Key != ConsoleKey.Escape);

            PluginSystem.Shutdown();
            bot.Shutdown();
        }
    }
}