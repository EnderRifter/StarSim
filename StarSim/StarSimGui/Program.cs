using Avalonia;
using Avalonia.Logging.Serilog;

using StarSimGui.ViewModels;
using StarSimGui.Views;

using StarSimLib.Cryptography;

using System;

namespace StarSimGui
{
    /// <summary>
    /// Main program class.
    /// </summary>
    internal class Program
    {
        // Your application's entry point. Here you can initialize your MVVM framework, DI container, etc.
        private static void AppMain(Application app, string[] args)
        {
            MainWindow window = new MainWindow
            {
                DataContext = new MainWindowViewModel(),
            };

            app.Run(window);
        }

        private static void TestHashing(string password)
        {
            // set a shorthand for nicety reasons
            string BytesToString(byte[] contents) => CryptographyHelper.BytesToString(contents);

            Console.WriteLine($"Password to hash: {password}");

            byte[] passwordBytes = CryptographyHelper.StringToBytes(password);
            Console.WriteLine($"Password bytes:\n{BytesToString(passwordBytes)}");

            // generates a salt of the default length
            byte[] saltBytes = CryptographyHelper.GenerateSalt();
            Console.WriteLine($"Generated salt:\n{BytesToString(saltBytes)}");

            // generates a hash of the default length
            byte[] passwordHash = CryptographyHelper.GenerateHash(passwordBytes, saltBytes);
            Console.WriteLine($"Generated valid hash:\n{BytesToString(passwordHash)}");

            byte[] invalidPasswordBytes = CryptographyHelper.StringToBytes("password ");
            Console.WriteLine($"Invalid password bytes:\n{BytesToString(invalidPasswordBytes)}");

            byte[] invalidPasswordHash = CryptographyHelper.GenerateHash(invalidPasswordBytes, saltBytes);
            Console.WriteLine($"Generated invalid hash:\n{BytesToString(invalidPasswordHash)}");

            Console.WriteLine($"Valid hash == invalid hash: {CryptographyHelper.HashesEqual(passwordHash, invalidPasswordHash)}");
        }

        // Avalonia configuration, don't remove; also used by visual designer.
        public static AppBuilder BuildAvaloniaApp()
            => AppBuilder.Configure<App>()
                .UsePlatformDetect()
                .LogToDebug()
                .UseReactiveUI();

        // Initialization code. Don't use any Avalonia, third-party APIs or any SynchronizationContext-reliant code
        // before AppMain is called: things aren't initialized yet and stuff might break.
        public static void Main(string[] args)
        {
            //TestHashing("Hello World!");
            BuildAvaloniaApp().Start(AppMain, args);
        }
    }
}