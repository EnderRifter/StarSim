using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace StarSimGui.Views
{
    public class Simulation : UserControl
    {
        public Simulation()
        {
            this.InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}