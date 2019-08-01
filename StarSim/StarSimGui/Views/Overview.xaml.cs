using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace StarSimGui.Views
{
    public class Overview : UserControl
    {
        public Overview()
        {
            this.InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
