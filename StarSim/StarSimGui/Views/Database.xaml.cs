using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace StarSimGui.Views
{
    public class Database : UserControl
    {
        public Database()
        {
            this.InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
