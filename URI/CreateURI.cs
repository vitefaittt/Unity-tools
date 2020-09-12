using Microsoft.Win32;
using System;
using System.IO;

namespace Create_URI
{
    class CreateURI
    {
        static void Main(string[] args)
        {
            // Create a key for ApplicationA.exe (which is in our parent directory).
            RegistryKey key = Registry.ClassesRoot.OpenSubKey(@"ApplicationA\shell\open\command", true);
            key.SetValue("", "\"" + Directory.GetParent(Directory.GetCurrentDirectory()) + "\\ApplicationA.exe\" \"%1\"");
            key.Close();

            // Show success message.
            Console.WriteLine("Installation terminée.");
            Console.WriteLine("Vous pouvez fermer cette fenêtre et ouvrir \"Website with URI.html\".");
            Console.ReadKey();
        }
    }
}
