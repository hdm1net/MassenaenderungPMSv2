using MassenaenderungPMSv2Gui.Services;
using MassenaenderungPMSv2Gui.ViewModels;
using System.Windows;

namespace MassenaenderungPMSv2Gui
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
            
   
        public MainWindow()
        {
            //
            // Initialisieren des Fensters
            //
            InitializeComponent();

            //
            // Setzen des DataContext auf die MainWindowViewModel-Instanz
            //
            DataContext = new MainWindowViewModel(

                new FileDialogService(),
                new CsvService(),
                new DynsProzessService(),
                new ExeService()
                );

            //
            // !! Alles weitere ist über ViewModels und Services zu realisieren !!
            //


        }
    }
}