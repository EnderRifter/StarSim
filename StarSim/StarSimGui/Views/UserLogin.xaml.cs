using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace StarSimGui.Views
{
    public class UserLogin : UserControl
    {
        public UserLogin()
        {
            this.InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
