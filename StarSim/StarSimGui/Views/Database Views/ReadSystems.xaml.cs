using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace StarSimGui.Views.Database_Views
{
    public class ReadSystems : UserControl
    {
        public ReadSystems()
        {
            this.InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}