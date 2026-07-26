using MassenaenderungPMSv2Gui.Helpers;
using MassenaenderungPMSv2Gui.Models;
using MassenaenderungPMSv2Gui.Services;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace MassenaenderungPMSv2Gui.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {

        #region "Services"
        //
        // Services 
        //
        private readonly IFileDialogService _fileDialogService;
        private readonly ICsvService _csvService;
        private readonly IDynsProzessService _prozessService;
        private readonly IExeService _exeService;
        #endregion

        #region "Properties"
        //
        // Properties
        //

        // Zentrales Objekt fuer die Eingaben in diesem ViewModel
        // Hier wird alles gesammelt und damit die XML-Datei mit der Importspezifikation geschrieben
        private ImportspezifikationBuilder ImpSpezBuilder { get; set; } = new ImportspezifikationBuilder();
        public bool EnablebtnImportspezifikationErstellen { get; set; } = true;
        public MassenaenderungPMSv2.ImpSpezXMLClasses.Uebersicht Importspezifikaton { get; set; }

        private MassenaenderungPMSv2.fiServiceXmlClasses.PO.ProzessUebersicht _ProzessUebersichtPO { get; set; } = new MassenaenderungPMSv2.fiServiceXmlClasses.PO.ProzessUebersicht();
        private MassenaenderungPMSv2.fiServiceXmlClasses.OO.ProzessUebersicht _ProzessUebersichtOO { get; set; } = new MassenaenderungPMSv2.fiServiceXmlClasses.OO.ProzessUebersicht();


        // ComboBox Importdatei
        private string _importdatei;
        public string Importdatei
        {
            get => _importdatei;
            set
            {
                // SetProperty prüft auf Änderungen und löst automatisch PropertyChanged aus
                SetProperty(ref _importdatei, value);
                _ = LadeEingabeparameterSpaltenCsvDateiAsync();
            }
        }


        // ComboBox DynsProzess
        public ObservableCollection<DynsProzess> DynsProzesse { get; set; }
        private DynsProzess _selectedProzess;
        public DynsProzess SelectedProzess
        {
            get => _selectedProzess;
            set
            {
                // SetProperty prüft auf Änderungen und löst automatisch PropertyChanged aus
                if (SetProperty(ref _selectedProzess, value))
                {
                    _ = LadeDynsProzessDatenAsync();
                    _ = LadeDynsAufrufvariantenAsync();
                }
            }
        }


        // ComboBox DynsProzess Aufrufvariante
        private DynsAufrufvariante _selectedDynsProzessAufrufvariante;
        public DynsAufrufvariante SelectedDynsProzessAufrufvariante
        {
            get => _selectedDynsProzessAufrufvariante;
            set
            {
                SetProperty(ref _selectedDynsProzessAufrufvariante, value);
                _ = LadeDynsAufrufvariantenParameterAsync();
            }
        }
        private ObservableCollection<DynsAufrufvariante> _dynsProzessAufrufvarianten;
        public ObservableCollection<DynsAufrufvariante> DynsProzessAufrufvarianten
        {
            get => _dynsProzessAufrufvarianten;
            set => SetProperty(ref _dynsProzessAufrufvarianten, value);
        }


        // TextBox Trennzeichen
        private string _trennzeichen;
        public string Trennzeichen
        {
            get => _trennzeichen;
            set => SetProperty(ref _trennzeichen, value);
        }


        // TestBox Textqualifier
        private string _textqualifizierer;
        public string Textqualifizierer
        {
            get => _textqualifizierer;
            set => SetProperty(ref _textqualifizierer, value);
        }


        // ComboBox DynsSchnittstellenarten
        public ObservableCollection<DynsSchnittstellenart> DynsSchnittstellenarten { get; set; }
        private DynsSchnittstellenart _selectedDynsSchnittstellenart;
        public DynsSchnittstellenart SelectedDynsSchnittstellenart
        {
            get => _selectedDynsSchnittstellenart;
            set => SetProperty(ref _selectedDynsSchnittstellenart, value);
        }


        // TextBlock DynsProzessbeschreibung
        private string _dynsProzessFachlicheBeschreibung;
        public string DynsProzessFachlicheBeschreibung
        {
            get => _dynsProzessFachlicheBeschreibung;
            set => SetProperty(ref _dynsProzessFachlicheBeschreibung, value);
        }


        // ComboBox DynsEingabeparameter
        private DynsAufrufvariantenParameter _selectedDynsAufrufvariantenParameter;
        public DynsAufrufvariantenParameter SelectedDynsAufrufvariantenParameter
        {
            get => _selectedDynsAufrufvariantenParameter;
            set
            {
                SetProperty(ref _selectedDynsAufrufvariantenParameter, value);
                SetDynsAufrufvariantenParameterDetails();
            }
        }
        private ObservableCollection<DynsAufrufvariantenParameter> _dynsAufrufvariantenParameter;
        public ObservableCollection<DynsAufrufvariantenParameter> DynsAufrufvariantenParameter
        {
            get => _dynsAufrufvariantenParameter;
            set => SetProperty(ref _dynsAufrufvariantenParameter, value);
        }


        // TextBox DynsEingabeparameterFO
        private string _dynsEingabeparameterFo;
        public string DynsEingabeparameterFo
        {
            get => _dynsEingabeparameterFo;
            set => SetProperty(ref _dynsEingabeparameterFo, value);
        }


        // TextBox DynsEingabeparameterLfdn
        private string _dynsEingabeparameterLfdn;
        public string DynsEingabeparameterLfdn
        {
            get => _dynsEingabeparameterLfdn;
            set => SetProperty(ref _dynsEingabeparameterLfdn, value);
        }


        // CheckBox DynsEingabeparameterArray
        private bool _dynsEingabeparameterArray;
        public bool DynsEingabeparameterArray
        {
            get => _dynsEingabeparameterArray;
            set => SetProperty(ref _dynsEingabeparameterArray, value);
        }


        // ComboBox EingabeparameterSpalten Eingabedatei (csv)
        private CsvFileColumns _selectedEingabeparameterSpalte;
        public CsvFileColumns SelectedEingabeparameterSpalte
        {
            get => _selectedEingabeparameterSpalte;
            set => SetProperty(ref _selectedEingabeparameterSpalte, value);
        }
        private ObservableCollection<CsvFileColumns> _eingabeparameterSpalten;
        public ObservableCollection<CsvFileColumns> EingabeparameterSpalten
        {
            get => _eingabeparameterSpalten;
            set => SetProperty(ref _eingabeparameterSpalten, value);
        }


        // TextBlock DynsEingabeparameterBeschreibung
        private string _dynsEingabeparameterBeschreibung;
        public string DynsEingabeparameterBeschreibung
        {
            get => _dynsEingabeparameterBeschreibung;
            set => SetProperty(ref _dynsEingabeparameterBeschreibung, value);
        }


        // DataGrind DynsEingabeparameterDataGrid
        private Models.Importspezifikation.Eingabeparameter _selectedImpSpezEingabeparameter;
        public Models.Importspezifikation.Eingabeparameter SelectedImpSpezEingabeparameter
        {
            get => _selectedImpSpezEingabeparameter;
            set => SetProperty(ref _selectedImpSpezEingabeparameter, value);
        }

        private ObservableCollection<Models.Importspezifikation.Eingabeparameter> _impSpezEingabeparameter;
        public ObservableCollection<Models.Importspezifikation.Eingabeparameter> ImpSpezEingabeparameterCollection
        {
            get => _impSpezEingabeparameter;
            set => SetProperty(ref _impSpezEingabeparameter, value);
        }


        // ComboBox MADImpSpezXml
        private string _MADImpSpezXml;
        public string MADImpSpezXml
        {
            get => _MADImpSpezXml;
            set
            {
                // SetProperty prüft auf Änderungen und löst automatisch PropertyChanged aus
                SetProperty(ref _MADImpSpezXml, value);
            }
        }


        // Button Massenaenderung starten
        public bool EnablebtnStartMassenaenderung { get; set; } = true;


        // CheckBox Massenaenderung Testlauf
        private bool _MADTestRun = true;
        public bool MADTestRun
        {
            get => _MADTestRun;
            set => SetProperty(ref _MADTestRun, value);
        }


        // Massenaenderung Output-TextBox
        private string _MADOutput;
        public string MADOutput
        {
            get => _MADOutput;
            set { SetProperty(ref _MADOutput, value); }
        }

        // Meldung-/Status-Zeile
        private string _statusMessage;
        public string StatusMessage
        {
            get => _statusMessage;
            set => SetProperty(ref _statusMessage, value);
        }
        #endregion

        #region "Commands"
        //
        // Commands
        //
        public ICommand SelectFileCommand { get; }
        public ICommand AddEingabeparameterCommand { get; }
        public ICommand RemoveEingabeparameterCommand { get; }
        public ICommand CreateImportspezifikationCommand { get; }
        public ICommand SelectMADImpSpezXmlCommand { get; }
        public ICommand StartMassenaenderungCommand { get; }
        #endregion

        #region "Konstruktor"
        //
        // Konstruktor
        // 
        public MainWindowViewModel(

            IFileDialogService fileDialogService,
            ICsvService csvService,
            IDynsProzessService prozessService,
            IExeService exeService)

        {

            _fileDialogService = fileDialogService;
            _csvService = csvService;
            _prozessService = prozessService;
            _exeService = exeService;

            DynsProzesse = new ObservableCollection<DynsProzess>(_prozessService.LoadDynsProzesse());
            DynsSchnittstellenarten = new ObservableCollection<DynsSchnittstellenart>(_prozessService.LoadDynsSchnittstellenarten());
            ImpSpezEingabeparameterCollection = new ObservableCollection<Models.Importspezifikation.Eingabeparameter>(_prozessService.LoadImpSpezEingabeparameterCollection());

            // Aktionen fuer Buttons zuweisen
            SelectFileCommand = new RelayCommand(_ => ExecuteSelectFile(), _ => true);
            AddEingabeparameterCommand = new RelayCommand(_ => ExecuteAddEingabeparameter(), _ => true);
            RemoveEingabeparameterCommand = new RelayCommand(_ => ExecuteRemoveEingabeparameter(SelectedImpSpezEingabeparameter), _ => true);
            CreateImportspezifikationCommand = new RelayCommand(_ => ExecuteCreateImportspezifikation(), _ => true);
            SelectMADImpSpezXmlCommand = new RelayCommand(_ => ExecuteMADImpSpezXmlCommand(), _ => true);
            StartMassenaenderungCommand = new RelayCommand(_ => ExecuteStartMassenaenderungCommand(), _ => true);

            // Default-Werte fuer Importdatei mit den Daten
            Trennzeichen = ";";
            Textqualifizierer = "\"";
            SelectedDynsSchnittstellenart = DynsSchnittstellenarten.FirstOrDefault(p => p.Key == "PO");

        }
        #endregion

        #region "Methoden Commands"
        //
        // Methoden
        //

        /// <summary>
        /// Öffnet ein Dialog-Fenster zum Auswählen einer CSV-Datei und validiert die ausgewählte Datei.
        /// </summary>
        /// <exception cref="ApplicationException"></exception>
        private void ExecuteSelectFile()
        {
            try
            {
                var file = _fileDialogService.OpenCsvFile();
                if (file == null) return;

                Importdatei = file;

                ValidateImportdatei();
                if (HasErrors) { return; }

            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.ToString(), ex);
            }
        }

        /// <summary>
        /// Für den ausgewählten Eingabeparameter wird ein neuer Eintrag in der ImportspezifikationCollection erstellt und validiert.
        /// </summary>
        /// <exception cref="ApplicationException"></exception>
        private void ExecuteAddEingabeparameter()
        {
            try
            {
                ClearErrors(nameof(ImpSpezEingabeparameterCollection));

                ValidateSelectedDynsAufrufvariantenParameter();
                if (HasErrors) { return; }

                ValidateDynsEingabeparameterFo();
                if (HasErrors) { return; }

                ValidateDynsEingabeparameterLfdn();
                if (HasErrors) { return; }

                ValidateDynsEingabeparameterArray();
                if (HasErrors) { return; }

                ValidateSelectedEingabeparameterSpalte();
                if (HasErrors) { return; }

                // Einabeparameter zur ImportspezifikationColletion (Bindung an DataGrid) hinzufuegen 
                var p = new Models.Importspezifikation.Eingabeparameter(SelectedDynsAufrufvariantenParameter.Id, SelectedDynsAufrufvariantenParameter.EingabeFo, DynsEingabeparameterLfdn, (DynsEingabeparameterArray ? "true" : "false"), SelectedEingabeparameterSpalte.ColumnName);

                ValidateAddEingabeparameterCommand(p);
                if (HasErrors) {return; }

                // Paramter hinzufügen
                ImpSpezEingabeparameterCollection.Add(p);

                // Parameter-Details aktualisieren
                SetDynsAufrufvariantenParameterDetails();

            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.ToString(), ex);
            }

        }

        /// <summary>
        /// Entfernt den ausgewählten Eingabeparameter aus der ImportspezifikationCollection.
        /// </summary>
        /// <param name="eingabeparameter"></param>
        /// <exception cref="ApplicationException"></exception>
        private void ExecuteRemoveEingabeparameter(Models.Importspezifikation.Eingabeparameter eingabeparameter)
        {
            try
            {
                if (eingabeparameter != null)
                {
                    ImpSpezEingabeparameterCollection.Remove(eingabeparameter);
                }
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.ToString(), ex);
            }

        }

        /// <summary>
        /// Erstellt die Importspezifikation basierend auf den aktuellen Eingaben und validiert die Eingaben vor der Erstellung.
        /// </summary>
        /// <exception cref="ApplicationException"></exception>
        private void ExecuteCreateImportspezifikation()
        {
            try
            {
                //
                // Diverse Validierungen durchführen, bevor die Importspezifikation erstellt werden kann
                //
                ValidateImportdatei();
                if (HasErrors) { return; }

                ValidateTrennzeichen();
                if (HasErrors) { return; }

                ValidateTextqualifizierer();
                if (HasErrors) { return; }

                ValidateSelectedProzess();
                if (HasErrors) { return; }

                ValidateSelectedAufrufvariante();
                if (HasErrors) { return; }

                ValidateSelectedSchnittstellenart();
                if (HasErrors) { return; }

                ValidateEingabeparameterCollection();
                if (HasErrors) { return; }

                // Importspezifikation erstellen und in Datei schreiben
                BuildImportspezifikation();
                WriteImportspezifikationToFile();

            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.ToString(), ex);
            }
        }

        /// <summary>
        /// Öffnet ein Dialog-Fenster zum Auswählen einer MADImpSpezXml-Datei und validiert die ausgewählte Datei.
        /// </summary>
        /// <exception cref="ApplicationException"></exception>
        private void ExecuteMADImpSpezXmlCommand()
        {
            try
            {
                var file = _fileDialogService.OpenXmlFile();
                if (file == null) return;

                MADImpSpezXml = file;

                ValidateMADImpSpezXml();
                if (HasErrors) { return; }

            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.ToString(), ex);
            }
        }

        /// <summary>
        /// Startet die Massenänderung im OSPlus-System basierend auf der ausgewählten MADImpSpezXml-Datei.
        /// </summary>
        /// <exception cref="ApplicationException"></exception>
        private void ExecuteStartMassenaenderungCommand()
        {
            try
            {

                // Validierung der MADImpSpezXml-Datei durchführen  
                ValidateMADImpSpezXml();
                if (HasErrors) { return; }


                // Aufrufparameter für MassenaenderungPMSv2.exe erstellen
                var args = new CommandArgsBuilder();
                
                if (MADTestRun)
                {
                    args.AddFlag("-", "t");
                }

                args.AddOption("-", "i", MADImpSpezXml);

                // MassenaenderungPMSv2.exe mit den Aufrufparametern starten und Output in MADOutput-Property schreiben
                _exeService.StartProcess(
                    "MassenaenderungPMSv2.exe",
                    args.Build(),
                    onOutput: line => MADOutput += line + Environment.NewLine,
                    onError: line => MADOutput +=  line + Environment.NewLine);

            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.ToString(), ex);
            }
        }

        #endregion

        #region "Methoden"

        /// <summary>
        /// Lädt die Prozessdaten für den ausgewählten Prozess asynchron und aktualisiert die entsprechenden Eigenschaften im ViewModel.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="ApplicationException"></exception>
        private async Task LadeDynsProzessDatenAsync()
        {
            if (SelectedProzess == null)
                return;

            try
            {

                //StatusMessage = "Lade Prozessdaten...";


                // Prozessart bestimmen (Darf hier nicht im nachfolgenden Task.Run() laufen!)
                var schnittstellenart = MassenaenderungPMSv2.Helper.fiServiceXml.PO.GetProzessUebersicht(SelectedProzess.ProcessName);

                if (schnittstellenart.TopEntitaet.Prozess.SchnittstellenArt != null)
                {

                    SelectedDynsSchnittstellenart = DynsSchnittstellenarten.FirstOrDefault(p => p.Key == schnittstellenart.TopEntitaet.Prozess.SchnittstellenArt);

                }


                await Task.Run(() =>
                {

                    switch (SelectedDynsSchnittstellenart?.Key)
                    {
                        case "PO":

                            _ProzessUebersichtPO = _prozessService.GetProzessUebersicht<MassenaenderungPMSv2.fiServiceXmlClasses.PO.ProzessUebersicht>(SelectedDynsSchnittstellenart.Key, SelectedProzess.ProcessName);
                            DynsProzessFachlicheBeschreibung = _ProzessUebersichtPO.TopEntitaet.Prozess.ProzessInfo.FachlicheBeschreibung;
                            break;

                        case "OO":

                            _ProzessUebersichtOO = _prozessService.GetProzessUebersicht<MassenaenderungPMSv2.fiServiceXmlClasses.OO.ProzessUebersicht>(SelectedDynsSchnittstellenart.Key, SelectedProzess.ProcessName);
                            DynsProzessFachlicheBeschreibung = _ProzessUebersichtOO.TopEntitaet.Prozess.ProzessInfo.FachlicheBeschreibung;
                            break;

                        default:
                            break;
                    }

                });

                //StatusMessage = "Prozessdaten geladen.";

            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.ToString(), ex);
            }
        }

        /// <summary>
        /// Lädt die Aufrufvarianten/Service-Operationen für den ausgewählten Prozess und die ausgewählte Schnittstellenart asynchron und aktualisiert die entsprechenden Eigenschaften im ViewModel.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="ApplicationException"></exception>
        private async Task LadeDynsAufrufvariantenAsync()
        {
            if (SelectedProzess == null || SelectedDynsSchnittstellenart == null)
                return;

            try
            {
                //StatusMessage = "Lade Aufrufvarianten/Service-Operationen zum Prozess...";

                var result = await Task.Run(() =>
                {
                    return _prozessService.LoadDynsAufrufvarianten(SelectedDynsSchnittstellenart.Key, SelectedProzess.ProcessName);
                });

                DynsProzessAufrufvarianten = new ObservableCollection<DynsAufrufvariante>(result);

                if (DynsProzessAufrufvarianten.Count > 0)
                {
                    SelectedDynsProzessAufrufvariante = DynsProzessAufrufvarianten[0];
                }

                //StatusMessage = "Aufrufvarianten/Service-Operationen zum Prozess geladen.";
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.ToString(), ex);
            }
        }

        /// <summary>
        /// Ermittelt die Spaltendefinition aus der ausgewählten Importdatei (CSV) asynchron und aktualisiert die entsprechenden Eigenschaften im ViewModel.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="ApplicationException"></exception>
        private async Task LadeEingabeparameterSpaltenCsvDateiAsync()
        {
            if (Importdatei == null || Trennzeichen == null || Textqualifizierer == null || !System.IO.File.Exists(Importdatei))
                return;

            try
            {
                //StatusMessage = "Ermittle Spaltendefinition aus Importdatei...";

                var result = await Task.Run(() =>
                {
                    return _csvService.LoadCsvFileHeader(Importdatei, Trennzeichen, Textqualifizierer);
                });

                EingabeparameterSpalten = new ObservableCollection<CsvFileColumns>(result);

                //StatusMessage = "Spaltendefinition aus Importdatei geladen.";
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.ToString(), ex);
            }
        }

        /// <summary>
        /// Lädt die Eingabeparameter für die ausgewählte Aufrufvariante/Service-Operation und dem ausgewählten Prozess asynchron und aktualisiert die entsprechenden Eigenschaften im ViewModel.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="ApplicationException"></exception>
        private async Task LadeDynsAufrufvariantenParameterAsync()
        {
            if (SelectedDynsSchnittstellenart == null || SelectedDynsProzessAufrufvariante == null || SelectedProzess == null)
                return;

            if (String.IsNullOrEmpty(SelectedDynsSchnittstellenart.Key) || String.IsNullOrEmpty(SelectedDynsProzessAufrufvariante.Key) || String.IsNullOrEmpty(SelectedProzess.ProcessName))
                return;

            try
            {
                //StatusMessage = "Lade Eingabeparameter zur Aufrufvariante/Service-Operation zum Prozess...";

                var result = await Task.Run(() =>
                {
                    return _prozessService.LoadDynsAufrufvariantenParameter(SelectedDynsSchnittstellenart.Key, SelectedDynsProzessAufrufvariante.Key, SelectedProzess.ProcessName);
                });

                DynsAufrufvariantenParameter = new ObservableCollection<DynsAufrufvariantenParameter>(result);

                //StatusMessage = "Eingabeparameter zur Aufrufvariante/Service-Operation zum Prozess geladen.";
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.ToString(), ex);
            }
        }

        /// <summary>
        /// Ermittelt die Details des ausgewählten Eingabeparameters (FO, Lfdn, Array-Flag) und aktualisiert die entsprechenden Eigenschaften im ViewModel.
        /// </summary>
        /// <exception cref="ApplicationException"></exception>
        private void SetDynsAufrufvariantenParameterDetails()
        {
            if (SelectedDynsAufrufvariantenParameter == null)
                return;


            try
            {

                DynsEingabeparameterFo = SelectedDynsAufrufvariantenParameter.EingabeFo;
                DynsEingabeparameterLfdn = NextDynsAufrufvariantenParameterLfdn(SelectedDynsAufrufvariantenParameter).ToString();

                bool isarray = false;
                if (DynsEingabeparameterLfdn.Length > 0 && !DynsEingabeparameterLfdn.Equals("1")) { isarray = true; }
                DynsEingabeparameterArray = isarray;

            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.ToString(), ex);
            }
        }

        /// <summary>
        /// Ermittelt die nächste Lfdn für den ausgewählten Eingabeparameter basierend auf der aktuellen ImportspezifikationCollection und der maximal zulässigen Kardinalität des Parameters.
        /// </summary>
        /// <param name="dynsAufrufvariantenParameter"></param>
        /// <returns></returns>
        /// <exception cref="ApplicationException"></exception>
        private int NextDynsAufrufvariantenParameterLfdn(DynsAufrufvariantenParameter dynsAufrufvariantenParameter)
        {
            try
            {
                // Ungueltige Eingabe → Startwert 1
                if (String.IsNullOrEmpty(dynsAufrufvariantenParameter?.Kardinalitaet))
                    return 1;

                // max. zulässige Eingabekardinalität für diesen Parameter
                if (!int.TryParse(dynsAufrufvariantenParameter.Kardinalitaet, out int maxLfdn))
                    return 1;

                int currentMax = 0;
                foreach (var eingabeparameter in ImpSpezEingabeparameterCollection
                             .Where(p => String.Equals(p.Name, dynsAufrufvariantenParameter.Id, StringComparison.OrdinalIgnoreCase)))
                {
                    if (int.TryParse(eingabeparameter.Lfdn, out int lfdn) && lfdn > currentMax)
                        currentMax = lfdn;
                }

                int next = currentMax + 1;

                if (next > maxLfdn)
                {
                    StatusMessage = $"Maximale Anzahl fuer {dynsAufrufvariantenParameter.Id} erreicht. ({maxLfdn})  ";
                    return maxLfdn;
                }

                return next;
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.ToString(), ex);
            }
        }

        private void BuildImportspezifikation()
        {
            try
            {
                var builder = ImpSpezBuilder
                .WithVersion(DateTime.Now.ToString("yyyy.MM.dd t"))
                .WithBeschreibung("Prozess '" + SelectedProzess.ProcessName + "' Aufrufvariante '" + SelectedDynsProzessAufrufvariante.Key + "'")
                .WithProzessName(SelectedProzess.ProcessName)
                .WithSchnittstellenArt(SelectedDynsSchnittstellenart.Key)
                .WithImportdatei(Importdatei)
                .WithLogdatei(Importdatei + ".log")
                .WithTextqualifizierer(Textqualifizierer)
                .WithTrennzeichen(Trennzeichen)
                ;

                // Bei parameterorientieren Prozessen (PO) wird die Aufrufvarianten-Nummer gesetzt, bei objektorientierten Prozessen (OO) die Service-Operation
                if (SelectedDynsSchnittstellenart.Key == "PO")
                {
                    builder.WithAufrufvariantenNummer(SelectedDynsProzessAufrufvariante.Key);
                    builder.WithServiceOperation(String.Empty);
                }
                else if (SelectedDynsSchnittstellenart.Key == "OO")
                {
                    builder.WithAufrufvariantenNummer(String.Empty);
                    builder.WithServiceOperation(SelectedDynsProzessAufrufvariante.Key);
                }

                int iEingabeKardinalitaet = 1;
                // Eingabeparameter hinzufügen
                foreach (var p in ImpSpezEingabeparameterCollection)
                {
                    // Parameter setzen
                    builder.AddEingabeparameter(p.Name, p.Datenspaltenname, p.EingabeFo, p.Lfdn, p.IsArray);

                    // Max. Eingabekardinalität ermitteln bei PO-Prozessen (parameterorientiert)
                    if (SelectedDynsSchnittstellenart.Key == "PO")
                    {
                        if (int.TryParse(p.Lfdn, out int lfdn))
                        {
                            if (lfdn > iEingabeKardinalitaet)
                            {
                                iEingabeKardinalitaet = lfdn;
                            }
                        }
                    }
                }

                // EingabeKardinalität setzen (nur bei PO-Prozessen relevant)   
                builder.WithEingabeKardinalitaet(iEingabeKardinalitaet.ToString());
         
                Importspezifikaton = builder.Build();

            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.ToString(), ex);
            }
        }

        private void WriteImportspezifikationToFile()
        {
            try
            {
                if (Importspezifikaton == null || Importdatei == null)
                    return;

                if (!MassenaenderungPMSv2.Helper.ImpSpezXml.Global.WriteImportSpezifikationFile(Importspezifikaton, Importdatei + ".xml"))
                {
                    StatusMessage = $"Fehler beim Schreiben der Importspezifikation {Importdatei}.xml";
                } else
                {
                    StatusMessage = $"Importspezifikation {Importdatei}.xml erfolgreich erstellt.";
                }

            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.ToString(), ex);
            }
        }



        #endregion

        #region "Validation"

        //
        // AUTOMATISCHE Validierung durch Base-Klasse
        //
        protected override void ValidateProperty(string propertyName)
        {
            switch (propertyName)
            {
                case nameof(Importdatei):
                    ValidateImportdatei();
                    break;

                case nameof(Trennzeichen):
                    ValidateTrennzeichen();
                    break;

                case nameof(Textqualifizierer):
                    ValidateTextqualifizierer();
                    break;

                case nameof(SelectedProzess):
                    ValidateSelectedProzess();
                    break;

                case nameof(SelectedDynsProzessAufrufvariante):
                    ValidateSelectedAufrufvariante();
                    break;

                case nameof(SelectedDynsSchnittstellenart):
                    ValidateSelectedSchnittstellenart();
                    break;

                case nameof(SelectedDynsAufrufvariantenParameter):
                    ValidateSelectedDynsAufrufvariantenParameter();
                    break;

                case nameof(DynsEingabeparameterFo):
                    ValidateDynsEingabeparameterFo();
                    break;

                case nameof(DynsEingabeparameterLfdn):
                    ValidateDynsEingabeparameterLfdn();
                    break;

                case nameof(DynsEingabeparameterArray):
                    ValidateDynsEingabeparameterArray();
                    break;

                case nameof(SelectedEingabeparameterSpalte):
                    ValidateSelectedEingabeparameterSpalte();
                    break;

                case nameof(MADImpSpezXml):
                    ValidateMADImpSpezXml();
                    break;
            }
        }

        // 
        // Einzelvalidierungen
        // 
        private void ValidateImportdatei()
        {
            ClearErrors(nameof(Importdatei));

            if (string.IsNullOrWhiteSpace(Importdatei))
                AddError(nameof(Importdatei), "Bitte eine Importdatei auswählen.");

            if (!string.IsNullOrWhiteSpace(Importdatei) && !System.IO.File.Exists(Importdatei))
                AddError(nameof(Importdatei), "Die Datei existiert nicht.");

            if (!string.IsNullOrWhiteSpace(Importdatei) && !Importdatei.ToLower().EndsWith(".csv"))
                AddError(nameof(Importdatei), "Die Importdatei muss eine.csv sein.");

            DisplayErrors(nameof(Importdatei));
        }

        private void ValidateTrennzeichen()
        {
            ClearErrors(nameof(Trennzeichen));

            if (string.IsNullOrWhiteSpace(Trennzeichen))
                AddError(nameof(Trennzeichen), "Trennzeichen darf nicht leer sein.");

            if (Trennzeichen?.Length != 1)
                AddError(nameof(Trennzeichen), "Trennzeichen muss genau ein Zeichen sein.");

            DisplayErrors(nameof(Trennzeichen));    
        }

        private void ValidateTextqualifizierer()
        {
            ClearErrors(nameof(Textqualifizierer));

            if (string.IsNullOrWhiteSpace(Textqualifizierer))
                AddError(nameof(Textqualifizierer), "Textqualifizierer darf nicht leer sein.");

            if (Textqualifizierer?.Length != 1)
                AddError(nameof(Textqualifizierer), "Textqualifizierer muss genau ein Zeichen sein.");

            DisplayErrors(nameof(Textqualifizierer));
        }

        private void ValidateSelectedProzess()
        {
            ClearErrors(nameof(SelectedProzess));

            if (SelectedProzess == null)
                AddError(nameof(SelectedProzess), "Bitte einen Prozess auswählen.");

            DisplayErrors(nameof(SelectedProzess));
        }

        private void ValidateSelectedAufrufvariante()
        {
            ClearErrors(nameof(SelectedDynsProzessAufrufvariante));

            if (SelectedDynsProzessAufrufvariante == null)
                AddError(nameof(SelectedDynsProzessAufrufvariante), "Bitte eine Aufrufvariante auswählen.");

            DisplayErrors(nameof(SelectedDynsProzessAufrufvariante));
        }

        private void ValidateSelectedSchnittstellenart()
        {
            ClearErrors(nameof(SelectedDynsSchnittstellenart));

            if (SelectedDynsSchnittstellenart == null)
                AddError(nameof(SelectedDynsSchnittstellenart), "Bitte eine Schnittstellenart auswählen.");

            DisplayErrors(nameof(SelectedDynsSchnittstellenart));
        }

        private void ValidateEingabeparameterCollection()
        {
            ClearErrors(nameof(ImpSpezEingabeparameterCollection));

            if (ImpSpezEingabeparameterCollection == null || ImpSpezEingabeparameterCollection.Count == 0)
                AddError(nameof(ImpSpezEingabeparameterCollection), "Es muss mindestens ein Eingabeparameter definiert sein.");

            // Alle MUSS-Eingabeparameter müssen vorhanden sein
            if (ImpSpezEingabeparameterCollection != null && ImpSpezEingabeparameterCollection.Count > 0)
            {
                foreach (var parameter in DynsAufrufvariantenParameter.Where(p => p.OptionalKz.Equals("M", StringComparison.OrdinalIgnoreCase)))
                {
                    if (!ImpSpezEingabeparameterCollection.Any(p => p.Name == parameter.Id))
                    {
                        AddError(nameof(ImpSpezEingabeparameterCollection), $"Der Eingabeparameter {parameter.Id} ist ein MUSS-Parameter und muss definiert sein.");
                    }
                }
            }

            // Bei einem Array-Parameter muss das Array-Flag gesetzt sein, auch bei der Lfdn =1 (erstes Element des Arrays)
            if (ImpSpezEingabeparameterCollection != null && ImpSpezEingabeparameterCollection.Count > 0)
            {
                foreach (var parameter in ImpSpezEingabeparameterCollection)
                {
                    if (parameter != null && !parameter.Lfdn.Equals("1") && parameter.IsArray.Equals("false"))
                    {
                        AddError(nameof(ImpSpezEingabeparameterCollection), $"Eingabeparameter {parameter.Name} mit Lfdn {parameter.Lfdn} muss das Array-Flag gesetzt haben.");
                    }
                }

                foreach (var parameter in ImpSpezEingabeparameterCollection)
                { 
                    if (parameter != null && !parameter.Lfdn.Equals("1"))
                    {
                        // Pruefe, ob es ein Parameter mit Lfdn=1 gibt, der das Array-Flag gesetzt hat
                        if (!ImpSpezEingabeparameterCollection.Any(p => p.Name == parameter.Name && p.Lfdn.Equals("1") && p.IsArray.Equals("true")))
                        {
                            AddError(nameof(ImpSpezEingabeparameterCollection), $"Eingabeparameter {parameter.Name} hat kein Element mit Lfdn=1 und Array-Flag.");
                        }
                    }
                }
            }

            DisplayErrors(nameof(ImpSpezEingabeparameterCollection));
        }

        private void ValidateSelectedDynsAufrufvariantenParameter()
        {
            ClearErrors(nameof(SelectedDynsAufrufvariantenParameter));
            
            if (SelectedDynsAufrufvariantenParameter == null)
                AddError(nameof(SelectedDynsAufrufvariantenParameter), "Bitte einen Eingabeparameter auswählen.");

            DisplayErrors(nameof(SelectedDynsAufrufvariantenParameter));
        }

        private void ValidateDynsEingabeparameterFo()
        {
            ClearErrors(nameof(DynsEingabeparameterFo));
            // Hier können Sie weitere Validierungen für DynsEingabeparameterArray hinzufügen, falls erforderlich

            DisplayErrors(nameof(DynsEingabeparameterFo));
        }

        private void ValidateDynsEingabeparameterLfdn()
        {
            ClearErrors(nameof(DynsEingabeparameterLfdn));
            if (DynsEingabeparameterLfdn == null || String.IsNullOrEmpty(DynsEingabeparameterLfdn))
                AddError(nameof(DynsEingabeparameterLfdn), "Bitte eine Lfdn für den Eingabeparameter angeben.");

            DisplayErrors(nameof(DynsEingabeparameterLfdn));
        }

        private void ValidateDynsEingabeparameterArray()
        {
            ClearErrors(nameof(DynsEingabeparameterArray));
            if (DynsEingabeparameterLfdn != null && !String.IsNullOrEmpty(DynsEingabeparameterLfdn) && !DynsEingabeparameterLfdn.Equals("1") && !DynsEingabeparameterArray)
            {
                AddError(nameof(DynsEingabeparameterArray), "Der Eingabeparameter scheint ein Array zu sein, daher muss das Array-Flag gesetzt sein.");
            }

            DisplayErrors(nameof(DynsEingabeparameterArray));
        }

        private void ValidateSelectedEingabeparameterSpalte()
        {
            ClearErrors(nameof(SelectedEingabeparameterSpalte));
            
            if (SelectedEingabeparameterSpalte == null)
                AddError(nameof(SelectedEingabeparameterSpalte), "Bitte eine Datenspalte auswählen.");

            DisplayErrors(nameof(SelectedEingabeparameterSpalte));
        }

        private void ValidateAddEingabeparameterCommand(Models.Importspezifikation.Eingabeparameter eingabeparameter)
        {
            ClearErrors(nameof(AddEingabeparameterCommand));

            // Noch kein Eingabeparameter definiert
            if (ImpSpezEingabeparameterCollection.Count == 0) return;

            if (ImpSpezEingabeparameterCollection.Where(p => p.Name.Equals(eingabeparameter.Name, StringComparison.OrdinalIgnoreCase)).Where(p => p.Lfdn.Equals(eingabeparameter.Lfdn, StringComparison.OrdinalIgnoreCase)).Any())
            {
                AddError(nameof(AddEingabeparameterCommand), $"Eingabeparameter {eingabeparameter.Name}({eingabeparameter.Lfdn}) bereits definiert!");
            }

            DisplayErrors(nameof(AddEingabeparameterCommand));

        }

        private async void ValidateMADImpSpezXml()
        {
            ClearErrors(nameof(MADImpSpezXml));

            if (string.IsNullOrWhiteSpace(MADImpSpezXml))
                AddError(nameof(MADImpSpezXml), "Bitte eine Importspezifikation auswählen.");

            if (!string.IsNullOrWhiteSpace(MADImpSpezXml) && !System.IO.File.Exists(MADImpSpezXml))
                AddError(nameof(MADImpSpezXml), "Die Datei existiert nicht.");

            if (!string.IsNullOrWhiteSpace(MADImpSpezXml) && !MADImpSpezXml.ToLower().EndsWith(".xml"))
                AddError(nameof(MADImpSpezXml), "Die Importspezifikation muss eine.xml sein.");

            var checkXml = await MassenaenderungPMSv2.Helper.ImpSpezXml.Global.GetImpSpezUebersichtAsync(MADImpSpezXml).ConfigureAwait(false);
            if (checkXml == null || checkXml.Importspezifikation == null || checkXml.Importspezifikation?.Prozess == null)
            {
                AddError(nameof(MADImpSpezXml), "Die Importspezifikation ist ungültig oder konnte nicht gelesen werden.");
            }

            DisplayErrors(nameof(MADImpSpezXml));
        }

        // -----------------------------------------
        // Gesamtvalidierung
        // -----------------------------------------
        public override bool ValidateAll()
        {
            ValidateImportdatei();
            ValidateTrennzeichen();
            ValidateTextqualifizierer();
            ValidateSelectedProzess();
            ValidateSelectedAufrufvariante();
            ValidateSelectedSchnittstellenart();
            ValidateEingabeparameterCollection();

            ValidateSelectedDynsAufrufvariantenParameter();
            ValidateDynsEingabeparameterLfdn();
            ValidateDynsEingabeparameterArray();
            ValidateSelectedEingabeparameterSpalte();

            return !HasErrors;
        }
        
        // -----------------------------------------
        // Fehler dem Benutzer anzeigen
        // -----------------------------------------
        private void DisplayErrors(string propertyName)
        {
            StatusMessage = String.Empty;   

            var errors = GetErrors(propertyName);
            if (errors != null)
            {
                foreach (var error in errors)
                {
                    StatusMessage += error.ToString() + Environment.NewLine;
                }
            }
        }

        #endregion

    }
}

