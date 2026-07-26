using MassenaenderungPMSv2Gui.ViewModels;
using System.Windows;

namespace MassenaenderungPMSv2Gui
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected async void OnStartup(object sender, StartupEventArgs e)
        {
                 
            //base.OnStartup(e);

            // Splash-Seite zum Start anzeigen
            var splash = new Splash();
            splash.Show();
            System.Threading.Thread.Sleep(2500);

            // 1. Instanz des ViewModels erstellen
            //MainWindowViewModel mainWindowViewModel = new MainWindowViewModel();

            // 2. Instanz der View (Fenster) erstellen
            MainWindow mainWindow = new MainWindow();

            // 3. Hochzeit: ViewModel an den Datenkontext der View übergeben
            //mainWindow.DataContext = mainViewModel;

            // 4. Fenster anzeigen
            mainWindow.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            mainWindow.Show();

            splash.Close();

        }

    }

}
