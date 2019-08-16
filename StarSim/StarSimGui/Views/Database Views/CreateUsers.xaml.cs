using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace StarSimGui.Views.Database_Views
{
    public class CreateUsers : UserControl
    {
        public CreateUsers()
        {
            this.InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}