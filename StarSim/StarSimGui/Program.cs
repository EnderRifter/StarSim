using Avalonia;
using Avalonia.Logging.Serilog;

using Newtonsoft.Json;

using StarSimGui.ViewModels;
using StarSimGui.Views;

using StarSimLib.Configuration;

using System;
using System.IO;

namespace StarSimGui
{
    /// <summary>
    /// Main program class.
    /// </summary>
    internal class Program
    {
        /// <summary>
        /// The current configuration of the application.
        /// </summary>
        public static Config configuration;

        // Your application's entry point. Here you can initialize your MVVM framework, DI container, etc.
        private static void AppMain(Application app, string[] args)
        {
            MainWindow window = new MainWindow
            {
                DataContext = new MainWindowViewModel(configuration)
            };

            app.Run(window);
        }

        /// <summary>
        /// Reads in and deserialises the configuration file at the given path.
        /// </summary>
        /// <param name="path">The path at which the configuration is located.</param>
        private static void ReadConfigFile(string path = @"./config.txt")
        {
            try
            {
                string fullPath = Path.GetFullPath(path);

                if (!File.Exists(fullPath))
                {
                    File.WriteAllText(fullPath, JsonConvert.SerializeObject(new Config(), Formatting.Indented));
                }

                using (FileStream fs = File.OpenRead(fullPath))
                {
                    using (StreamReader fileReader = new StreamReader(fs))
                    {
                        configuration = Config.Load(fileReader.ReadToEnd());
                    }
                }
            }
            catch (Exception)
            {
                configuration = new Config();
            }
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
            ReadConfigFile();
            BuildAvaloniaApp().Start(AppMain, args);
        }
    }
}