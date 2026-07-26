using MassenaenderungPMSv2Gui.Models;
using System.Collections.ObjectModel;

namespace MassenaenderungPMSv2Gui.Services
{
    public interface IDynsProzessService
    {
        IEnumerable<DynsProzess> LoadDynsProzesse();

        IEnumerable<DynsSchnittstellenart> LoadDynsSchnittstellenarten();
        
        IEnumerable<DynsAufrufvariante> LoadDynsAufrufvarianten(string schnittstellenArt, string prozessName);

        IEnumerable<DynsAufrufvariantenParameter> LoadDynsAufrufvariantenParameter(string schnittstellenArt, string aufrufvariante, string prozessName);

        IEnumerable<Models.Importspezifikation.Eingabeparameter> LoadImpSpezEingabeparameterCollection();

        T GetProzessUebersicht<T>(string schnittstellenArt, string prozessName);

    }

    public class DynsProzessService : IDynsProzessService
    {
        private const int MaxBeschreibungLength = 90;
        private List<DynsProzess> _DynsProzess { get; set; } = new List<DynsProzess>();

        public IEnumerable<DynsProzess> LoadDynsProzesse()
        {

            try
            {
                _DynsProzess.Clear();

                var config = MassenaenderungPMSv2.MyConfiguration.GetConfiguration();

                if (config == null)
                    return _DynsProzess;    

                if (String.IsNullOrEmpty(config.fiServicesXMLPath))
                    return _DynsProzess;

                if (!System.IO.Directory.Exists(config.fiServicesXMLPath))
                    return _DynsProzess;

                var files = System.IO.Directory.GetFiles(config.fiServicesXMLPath);

                foreach (var file in files)
                {
                    var processName = System.IO.Path.GetFileNameWithoutExtension(file);

                    if (!processName.Contains("_LESEN") && !processName.Contains("_LES"))
                    {
                        _DynsProzess.Add(new DynsProzess(System.IO.Path.GetFileNameWithoutExtension(file), file));
                    }
                }

                return _DynsProzess;
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.ToString(), ex);
            }
        }

        public void AddDynsProzess(DynsProzess prozess)
        {
            _DynsProzess.Add(prozess);
        }
               
        public T GetProzessUebersicht<T>(string schnittstellenArt, string prozessName)
        {
            if (schnittstellenArt == "PO")
            {
                return (T)(object)MassenaenderungPMSv2.Helper.fiServiceXml.PO.GetProzessUebersicht(prozessName);
            }

            if (schnittstellenArt == "OO")
            {
                return (T)(object)MassenaenderungPMSv2.Helper.fiServiceXml.OO.GetProzessUebersicht(prozessName);
            }

            throw new ArgumentException($"Unbekannte Schnittstellenart: {schnittstellenArt}");
        }

        private List<DynsSchnittstellenart> _DynsSchnittstellenart { get; set; } = new List<DynsSchnittstellenart> { };

        public IEnumerable<DynsSchnittstellenart> LoadDynsSchnittstellenarten()
        {
            _DynsSchnittstellenart.Clear();
            _DynsSchnittstellenart.Add(new DynsSchnittstellenart("PO", "Parameterorientiert"));
            _DynsSchnittstellenart.Add(new DynsSchnittstellenart("OO", "Objektorientiert"));
            return _DynsSchnittstellenart;
        }

        public void AddDynsSchnittstellenart(DynsSchnittstellenart schnittstellenart)
        {
            _DynsSchnittstellenart.Add(schnittstellenart);
        }

        private List<DynsAufrufvariante> _DynsAufrufvarianten { get; set; } = new List<DynsAufrufvariante> { };

        public IEnumerable<DynsAufrufvariante> LoadDynsAufrufvarianten(string schnittstellenArt, string prozessName)
        {

            _DynsAufrufvarianten.Clear();

            if (String.IsNullOrEmpty(schnittstellenArt) || String.IsNullOrEmpty(prozessName)) { return _DynsAufrufvarianten; }

            if (schnittstellenArt == "PO")
            {

                var prozessUebersicht = GetProzessUebersicht<MassenaenderungPMSv2.fiServiceXmlClasses.PO.ProzessUebersicht>(schnittstellenArt, prozessName);

                foreach (var av in prozessUebersicht.TopEntitaet.Prozess.AufrufVarianten.Varianten )
                {
                    var _avNr = av.Nummer;
                    var _avBeschreibung = "ARV" + av.Nummer.ToString() + ": | ";

                    foreach (var parameter in av.Parameter.Where(p => p.OptionalKz == "M" ))
                    {
                        _avBeschreibung += parameter.Name + "[" + parameter.Kardinalitaet + "] "; 
                    }
                    _DynsAufrufvarianten.Add(new DynsAufrufvariante(_avNr.ToString(), Helpers.StringHelper.LimitLength(_avBeschreibung, MaxBeschreibungLength)));
                }
                
            }

            if (schnittstellenArt == "OO")
            {
                var prozessUebersicht = GetProzessUebersicht<MassenaenderungPMSv2.fiServiceXmlClasses.OO.ProzessUebersicht>(schnittstellenArt, prozessName);

                foreach (var op in prozessUebersicht.TopEntitaet.Prozess.Operationen)
                {
                    _DynsAufrufvarianten.Add(new DynsAufrufvariante(op.Name, Helpers.StringHelper.LimitLength((op.Name + " - " + op.Beschreibung), MaxBeschreibungLength)));
                }
            }

            return _DynsAufrufvarianten;

        }

        private List<DynsAufrufvariantenParameter> _DynsAufrufvariantenParameter { get; set; } = new List<DynsAufrufvariantenParameter> { };

        public IEnumerable<DynsAufrufvariantenParameter> LoadDynsAufrufvariantenParameter(string schnittstellenArt, string aufrufVariante, string prozessName)
        {

            _DynsAufrufvariantenParameter.Clear();

            if (String.IsNullOrEmpty(schnittstellenArt) || String.IsNullOrEmpty(prozessName)) { return _DynsAufrufvariantenParameter; }

            if (schnittstellenArt == "PO")
            {

                var prozessUebersicht = GetProzessUebersicht<MassenaenderungPMSv2.fiServiceXmlClasses.PO.ProzessUebersicht>(schnittstellenArt, prozessName);

                foreach (var av in prozessUebersicht.TopEntitaet.Prozess.AufrufVarianten.Varianten)
                {

                    if (av.Nummer.ToString() == aufrufVariante)
                    {
                        foreach (var parameter in av.Parameter.OrderByDescending(p => p.OptionalKz).ThenBy(p => p.Name))
                        {

                            _DynsAufrufvariantenParameter.Add(
                                new DynsAufrufvariantenParameter()
                                {
                                    Id = parameter.Name,
                                    Name = parameter.Name + "[" + parameter.Kardinalitaet + "] [" + parameter.OptionalKz + "]",
                                    OptionalKz = parameter.OptionalKz,
                                    Kardinalitaet = parameter.Kardinalitaet,
                                }
                            );
                        }
                    } 
                }
            }

            if (schnittstellenArt == "OO")
            {

                var eingabeFO = MassenaenderungPMSv2.Helper.fiServiceXml.OO.GetFachObjektEingabeByServiceOperator(prozessName, aufrufVariante);

                if (eingabeFO != null) {

                    var parameterDict = MassenaenderungPMSv2.Helper.fiServiceXml.OO.GetFachobjektParameter(eingabeFO, prozessName);

                    foreach (var (key, (Id, ParameterName, isMandatory)) in parameterDict.OrderByDescending(x => x.Value.IsMandatory).ThenBy(x => x.Value.ParameterName))
                    {
                        _DynsAufrufvariantenParameter.Add(
                                new DynsAufrufvariantenParameter()
                                {
                                    Id = parameterDict[key].ParameterName,
                                    Name = parameterDict[key].ParameterName + "[" + DynsAufrufvariantenParameter.OptionalKzFromBool(parameterDict[key].IsMandatory) + "]",
                                    OptionalKz = DynsAufrufvariantenParameter.OptionalKzFromBool(parameterDict[key].IsMandatory),
                                }
                        );
                    }

                }
                
            }

            return _DynsAufrufvariantenParameter;

        }

        private List<Models.Importspezifikation.Eingabeparameter> _impSpezEingabeparameterCollection { get; set; } = new List<Models.Importspezifikation.Eingabeparameter> { };
        
        public IEnumerable<Models.Importspezifikation.Eingabeparameter> LoadImpSpezEingabeparameterCollection()
        {

            // Hier ist nicht zu tun, da die Collection in der Anwendung durch Benutzereingaben gefüllt wird.
            
            try
            {
                _impSpezEingabeparameterCollection.Clear();
                return _impSpezEingabeparameterCollection;
            }
            catch (Exception ex)
            {

                throw new ApplicationException(ex.ToString(), ex);
            }

        }


       

    }

}
