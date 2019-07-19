using System;
using Avalonia;
using Avalonia.Logging.Serilog;
using StarSimGui.ViewModels;
using StarSimGui.Views;

namespace StarSimGui
{
    internal class Program
    {
        // Your application's entry point. Here you can initialize your MVVM framework, DI
        // container, etc.
        private static void AppMain(Application app, string[] args)
        {
            MainWindow window = new MainWindow
            {
                DataContext = new MainWindowViewModel(),
            };

            app.Run(window);
        }

        // Avalonia configuration, don't remove; also used by visual designer.
        public static AppBuilder BuildAvaloniaApp()
            => AppBuilder.Configure<App>()
                .UsePlatformDetect()
                .LogToDebug()
                .UseReactiveUI();

        // Initialization code. Don't use any Avalonia, third-party APIs or any
        // SynchronizationContext-reliant code before AppMain is called: things aren't initialized
        // yet and stuff might break.
        public static void Main(string[] args) => BuildAvaloniaApp().Start(AppMain, args);
    }
}