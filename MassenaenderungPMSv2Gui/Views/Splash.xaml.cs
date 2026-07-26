using System.Windows;

namespace MassenaenderungPMSv2Gui
{
    /// <summary>
    /// Interaktionslogik für Splash.xaml
    /// </summary>
    public partial class Splash : Window
    {
        public Splash()
        {
            InitializeComponent();
            
            var version = System.Reflection.Assembly.GetExecutingAssembly().GetName()?.Version?.ToString();
            VersionLabel.Text = "Version: " + version;  


        }
    }
}
