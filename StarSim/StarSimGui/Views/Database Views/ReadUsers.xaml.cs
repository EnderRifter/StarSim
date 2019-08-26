using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace StarSimGui.Views.Database_Views
{
    public class ReadUsers : UserControl
    {
        public ReadUsers()
        {
            this.InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}