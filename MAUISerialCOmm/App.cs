using Microsoft.Maui.Controls;

namespace MAUISerialCOmm
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
            MainPage = new AppShell();
        }
    }
}
