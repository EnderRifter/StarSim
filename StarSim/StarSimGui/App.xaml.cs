using Avalonia;
using Avalonia.Markup.Xaml;

namespace StarSimGui
{
    public class App : Application
    {
        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
