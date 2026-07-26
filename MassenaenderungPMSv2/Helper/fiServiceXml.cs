using System.Text;
using System.Xml.Serialization;

namespace MassenaenderungPMSv2.Helper.fiServiceXml
{
    /// <summary>
    /// Klasse fuer parameterorientierte Prozesse
    /// </summary>
    public class PO
    {
        public PO() { }

        /// <summary>
        /// Deserialisiert die XML-Datei zum Prozess und gibt diese als Prozessuebersicht zurueck
        /// </summary>
        /// <param name="fiServicename"></param>
        /// <returns>fiServiceXmlClasses.PO.ProzessUebersicht</returns>
        /// <exception cref="Exception"></exception>
        public static fiServiceXmlClasses.PO.ProzessUebersicht GetProzessUebersicht(string fiServicename)
        {

            try
            {
                // https://www.codeproject.com/articles/XML-Serialization-and-Deserialization-Part-1#comments-section

                // Return-Object
                fiServiceXmlClasses.PO.ProzessUebersicht rcProzessUebersicht = new fiServiceXmlClasses.PO.ProzessUebersicht();

                var config = MyConfiguration.GetConfiguration();
                var xmlFiServiceFilePath = String.Concat(config.fiServicesXMLPath, "\\", fiServicename + ".xml");

                if (!File.Exists(xmlFiServiceFilePath))
                {
                    return rcProzessUebersicht;
                }

                XmlSerializer xmlSerializer = new XmlSerializer(typeof(fiServiceXmlClasses.PO.ProzessUebersicht));

                using (TextReader textReader = new StreamReader(xmlFiServiceFilePath, Encoding.GetEncoding("iso-8859-1")))
                {

                    var deserialized = xmlSerializer.Deserialize(textReader) as fiServiceXmlClasses.PO.ProzessUebersicht;

                    if (deserialized != null)
                    {
                        rcProzessUebersicht = deserialized;
                    }

                }

                return rcProzessUebersicht;

            }
            catch (Exception e)
            {
                throw new InvalidOperationException($"Fehler beim Deserialisieren der Datei '{fiServicename}'", e);
            }

        }

        /// <summary>
        /// Ruft die Aufrufvariante incl. der Aufrufparameter zu einem DynsWs ab
        /// </summary>
        /// <param name="fiServicename"></param>
        /// <param name="aufrufvariantennummer"></param>
        /// <returns>fiServiceXmlClasses.PO.Aufrufvariante</returns>
        /// <exception cref="Exception"></exception>
        public static fiServiceXmlClasses.PO.AufrufVariante GetAufrufvariante(string fiServicename, string aufrufvariantennummer)
        {
            return GetAufrufvarianteAsync(fiServicename, aufrufvariantennummer).Result;
        }

        /// <summary>
        /// Ruft die Aufrufvariante incl. der Aufrufparameter zu einem DynsWs ab
        /// </summary>
        /// <param name="fiServicename"></param>
        /// <param name="aufrufvariantennummer"></param>
        /// <returns>fiServiceXmlClasses.PO.Aufrufvariante</returns>
        /// <exception cref="Exception"></exception>
        public static async Task<fiServiceXmlClasses.PO.AufrufVariante> GetAufrufvarianteAsync(string fiServicename, string aufrufvariantennummer)
        {

            try
            {

                fiServiceXmlClasses.PO.AufrufVariante rcAV = new fiServiceXmlClasses.PO.AufrufVariante();
                fiServiceXmlClasses.PO.ProzessUebersicht prozessUebersicht = Helper.fiServiceXml.PO.GetProzessUebersicht(fiServicename);

                await Task.Run(action: () => {

                    foreach (fiServiceXmlClasses.PO.AufrufVariante av in prozessUebersicht.TopEntitaet.Prozess.AufrufVarianten.Varianten)
                    {
                        if (av.Nummer.ToString() == aufrufvariantennummer)
                        {
                            rcAV = av;
                            break;
                        }

                    }
                });

                return rcAV;

            }
            catch (Exception e)
            {
                throw new InvalidOperationException("Error GetAufrufvarianteAsync()", e);
            }

        }

        /// <summary>
        /// Ruft die Detail zu den notwendigen Aufrufparametern anhand der ImportSpezifikation ab
        /// </summary>
        /// <param name="fiServicename"></param>
        /// <param name="impSpezUebersicht"></param>
        /// <returns>List<fiServiceXmlClasses.PO.Parameter></returns>
        /// <exception cref="Exception"></exception>
        public static List<fiServiceXmlClasses.PO.Parameter> GetAufrufParameter(ImpSpezXMLClasses.Uebersicht impSpezUebersicht)
        {
            return GetAufrufParameterAsync(impSpezUebersicht).Result;
        }

        /// <summary>
        /// Ruft die Detail zu den notwendigen Aufrufparametern anhand der ImportSpezifikation ab
        /// </summary>
        /// <param name="fiServicename"></param>
        /// <param name="impSpezUebersicht"></param>
        /// <returns>List<fiServiceXmlClasses.PO.Parameter></returns>
        /// <exception cref="Exception"></exception>
        public static async Task<List<fiServiceXmlClasses.PO.Parameter>> GetAufrufParameterAsync(ImpSpezXMLClasses.Uebersicht impSpezUebersicht)
        {
            try
            {

                List<fiServiceXmlClasses.PO.Parameter> rcParameters = new List<fiServiceXmlClasses.PO.Parameter>();
                
                await Task.Run(action: () => {
                
                    fiServiceXmlClasses.PO.ProzessUebersicht prozessUebersicht = Helper.fiServiceXml.PO.GetProzessUebersicht(String.Concat(impSpezUebersicht.Importspezifikation?.Prozess?.Name, ""));

                    if (prozessUebersicht.TopEntitaet.Prozess.ParameterUebersicht.Parameter.Count > 0)
                    {
                       
                        foreach (ImpSpezXMLClasses.Eingabeparameter impSpezEingabeparameter in (impSpezUebersicht.Importspezifikation?.Prozess?.EingabeParameterCollection?.Eingabeparameter ?? Enumerable.Empty<ImpSpezXMLClasses.Eingabeparameter>()))
                        {

                            foreach (fiServiceXmlClasses.PO.Parameter prozessParameter in prozessUebersicht.TopEntitaet.Prozess.ParameterUebersicht.Parameter)
                            {
                                
                                if (string.Equals(prozessParameter.Name, impSpezEingabeparameter.Name, StringComparison.OrdinalIgnoreCase))
                                {

                                    // Der StringTyp 13 wird in BWSPS als PsString behandelt, da 13 nicht in der EnumParameterTyp definiert ist
                                    if (prozessParameter.Typ == "13") 
                                    {
                                        prozessParameter.Typ = BWSPS.EnumParameterTyp.PsString.ToString();
                                    }

                                    rcParameters.Add(prozessParameter);
                                    break;
                                }

                            }

                        }

                    }
                   
                });

                return rcParameters;
            }
            catch (Exception e)
            {
                throw new InvalidOperationException("Error GetAufrufParameterAsync()", e);
            }
        }
    }

    /// <summary>
    /// Klasse fuer objektorientierte Prozesse
    /// </summary>
    public class OO
    {
        public OO() { }

        /// <summary>
        /// Deserialisiert die XML-Datei zum DysWs-Prozess und gibt diese als Prozessuebersichtklasse zurueck
        /// </summary>
        /// <param name="fiServicename"></param>
        /// <returns>fiServiceXmlClasses.OO.PROZESS_UEBERSICHT</returns>
        /// <exception cref="Exception"></exception>
        public static fiServiceXmlClasses.OO.ProzessUebersicht GetProzessUebersicht(string fiServicename)
        {

            try
            {
                // https://www.codeproject.com/articles/XML-Serialization-and-Deserialization-Part-1#comments-section

                // Return-Object
                fiServiceXmlClasses.OO.ProzessUebersicht rcProzessUebersicht = new fiServiceXmlClasses.OO.ProzessUebersicht();

                var config = MyConfiguration.GetConfiguration();
                var xmlFiServiceFilePath = String.Concat(config.fiServicesXMLPath,"\\", fiServicename + ".xml");

                if (!File.Exists(xmlFiServiceFilePath))
                {
                    return rcProzessUebersicht;
                }

                XmlSerializer xmlSerializer = new XmlSerializer(typeof(fiServiceXmlClasses.OO.ProzessUebersicht));

                using (TextReader textReader = new StreamReader(xmlFiServiceFilePath, Encoding.GetEncoding("iso-8859-1")))
                {

                    var deserialized = xmlSerializer.Deserialize(textReader) as fiServiceXmlClasses.OO.ProzessUebersicht;

                    if (deserialized != null)
                    {
                        rcProzessUebersicht = deserialized;
                    }
                }

                return rcProzessUebersicht;

            }
            catch (Exception e)
            {
                throw new InvalidOperationException($"Fehler beim Deserialisieren der Datei '{fiServicename}'", e);
            }

        }


        /// <summary>
        /// Ruft das Fachobjekt (Eingabeobjekt) incl. der Aufrufparameter zu einem DynsWs ab
        /// </summary>
        /// <param name="fiServicename"></param>
        /// <param name="fachobjektName"></param>
        /// <returns>fiServiceXmlClasses.OO.Fachobjekt</returns>
        /// <exception cref="Exception"></exception>
        public static fiServiceXmlClasses.OO.FachObjekt GetFachobjekt(string fiServicename, string fachobjektName)
        { 
            return GetFachobjektAsync(fiServicename, fachobjektName).Result;
        }


        /// <summary>
        /// Ruft das Fachobjekt (Eingabeobjekt) incl. der Aufrufparameter zu einem DynsWs ab
        /// </summary>
        /// <param name="fiServicename"></param>
        /// <param name="fachobjektName"></param>
        /// <returns>fiServiceXmlClasses.OO.Fachobjekt</returns>
        /// <exception cref="Exception"></exception>
        public static async Task<fiServiceXmlClasses.OO.FachObjekt> GetFachobjektAsync(string fiServicename, string fachobjektName) {

            try
            {

                fiServiceXmlClasses.OO.FachObjekt rcFO = new fiServiceXmlClasses.OO.FachObjekt();
                fiServiceXmlClasses.OO.ProzessUebersicht prozessUebersicht = Helper.fiServiceXml.OO.GetProzessUebersicht(fiServicename);

                await Task.Run(action: () => {

                    foreach (fiServiceXmlClasses.OO.FachObjekt fo in prozessUebersicht.TopEntitaet.Prozess.FachObjekte)
                    {
                        if (string.Equals(fo.Name, fachobjektName, StringComparison.OrdinalIgnoreCase))
                        {
                            rcFO = fo;
                            break;
                        }

                    }
                });

                return rcFO;

            }
            catch (Exception e)
            {
                throw new InvalidOperationException("Error GetServiceFachobjektAsync()", e);
            }

        }


        /// <summary>
        /// Ruft die Serviceoperation zu einem DynsWs Prozessaufruf ab
        /// </summary>
        /// <param name="fiServicename"></param>
        /// <param name="serviceOperationName"></param>
        /// <returns>fiServiceXmlClasses.OO.Operation</returns>
        /// <exception cref="Exception"></exception>
        public static fiServiceXmlClasses.OO.Operation GetServiceOperation(string fiServicename, string serviceOperationName)
        {
            return GetServiceOperationAsync(fiServicename, serviceOperationName).Result;
        }

        /// <summary>
        /// Ruft die Serviceoperation zu einem DynsWs Prozessaufruf ab
        /// </summary>
        /// <param name="fiServicename"></param>
        /// <param name="serviceOperationName"></param>
        /// <returns>fiServiceXmlClasses.OO.Operation</returns>
        /// <exception cref="Exception"></exception>
        public static async Task<fiServiceXmlClasses.OO.Operation> GetServiceOperationAsync(string fiServicename, string serviceOperationName)
        {

            try
            {

                fiServiceXmlClasses.OO.Operation rcOP = new fiServiceXmlClasses.OO.Operation();
                fiServiceXmlClasses.OO.ProzessUebersicht prozessUebersicht = Helper.fiServiceXml.OO.GetProzessUebersicht(fiServicename);

                await Task.Run(action: () => {

                    foreach (fiServiceXmlClasses.OO.Operation op in prozessUebersicht.TopEntitaet.Prozess.Operationen)
                    {
                        if (string.Equals(op.Name, serviceOperationName, StringComparison.OrdinalIgnoreCase))
                        {
                            rcOP = op;
                            break;
                        }

                    }
                });

                return rcOP;

            }
            catch (Exception e)
            {
                throw new InvalidOperationException("Error GetServiceOperationAsync()", e);
            }

        }


        public static fiServiceXmlClasses.OO.Operation GetServiceOperationbyFachobjekt(string fiServicename, string fachobjektName)
        {
            return GetServiceOperationByFachobjektAsync(fiServicename, fachobjektName).Result;
        }

        public static async Task<fiServiceXmlClasses.OO.Operation> GetServiceOperationByFachobjektAsync(string fiServicename, string fachobjektName)
        {
            try
            {
                
                fiServiceXmlClasses.OO.Operation rcOP = new fiServiceXmlClasses.OO.Operation();

                fiServiceXmlClasses.OO.ProzessUebersicht prozessUebersicht = Helper.fiServiceXml.OO.GetProzessUebersicht(fiServicename);

                await Task.Run(action: () => {

                    foreach (fiServiceXmlClasses.OO.FachObjekt fo in prozessUebersicht.TopEntitaet.Prozess.FachObjekte)
                    {
                        if (string.Equals(fo.Name, fachobjektName, StringComparison.OrdinalIgnoreCase))
                        {
                            
                            prozessUebersicht.TopEntitaet.Prozess.Operationen.Where(op => op.EingabeFoRefId == fo.Id).ToList().ForEach(op =>
                            {
                                rcOP = op;
                            });

                        }

                    }
                });

                return rcOP;

            }
            catch (Exception e)
            {
                throw new InvalidOperationException("Error GetServiceOperationByFachobjektAsync()", e);
            }
        }

        public static fiServiceXmlClasses.OO.FachObjekt GetFachObjektEingabeByServiceOperator(string fiServicename, string serviceOperatorName)
        {
            return GetFachObjektEingabeByServiceOperatorAsync(fiServicename, serviceOperatorName).Result;
        }


        public static async Task<fiServiceXmlClasses.OO.FachObjekt> GetFachObjektEingabeByServiceOperatorAsync(string fiServicename, string serviceOperatorName)
        {
            try
            {
                fiServiceXmlClasses.OO.FachObjekt rcFO = new fiServiceXmlClasses.OO.FachObjekt();
                
                fiServiceXmlClasses.OO.ProzessUebersicht prozessUebersicht = Helper.fiServiceXml.OO.GetProzessUebersicht(fiServicename);

                await Task.Run(action: () => {

                    foreach (fiServiceXmlClasses.OO.Operation op in prozessUebersicht.TopEntitaet.Prozess.Operationen)
                    {
                        if (string.Equals(op.Name, serviceOperatorName, StringComparison.OrdinalIgnoreCase))
                        {
                            foreach (fiServiceXmlClasses.OO.FachObjekt fo in prozessUebersicht.TopEntitaet.Prozess.FachObjekte)
                            {
                                if (fo.Id == op.EingabeFoRefId)
                                {
                                    rcFO = fo;
                                    break;
                                }
                            }
                        }
                    }

                });

                return rcFO;

            }
            catch (Exception e)
            {
                throw new InvalidOperationException("Error GetFachObjektEingabeByServiceOperatornAsync()", e);
            }
        }


        public static Dictionary<string, (string ParameterId, string ParameterName, bool IsMandatory)> GetFachobjektParameter(fiServiceXmlClasses.OO.FachObjekt fo, string fiServicename)
        {
            return GetFachobjektParameterAsync(fo, fiServicename).Result;
        }

        public static async Task<Dictionary<string, (string ParameterId, string ParameterName, bool IsMandatory)>> GetFachobjektParameterAsync(fiServiceXmlClasses.OO.FachObjekt fo, string fiServicename)
        {
            try
            {
  
                // Result dictionary: PARAMETER_NAME → (PARAMETER_ID, PARAMETER_NAME, IS_MANDATORY)
                var result = new Dictionary<string, (string ParameterId, string ParameterName, bool IsMandatory)>();
                  
                await Task.Run(action: () => {

                    // Build dictionary for quick lookup
                    var parameterDict = Helper.fiServiceXml.OO
                        .GetProzessUebersicht(fiServicename)
                        .TopEntitaet.Prozess.ParameterUebersicht
                        .ToDictionary(p => p.Id, p => p);

                    if (fo?.ParameterRefs != null)
                    {

                        foreach (var parameterRef in fo.ParameterRefs)
                        {
                            
                            if (parameterDict.TryGetValue(parameterRef.ParameterRefId, out var parameter))
                            {
                                result[parameter.Name] = (
                                    parameter.Id.ToString(),
                                    parameter.Name,
                                    parameterRef.IsMandatory
                                );
                            }

                        }

                    }

                });

                return result;

            }
            catch (Exception e)
            {
                throw new InvalidOperationException("Error GetFachobjektParameterAsync()", e);
            }
        }

        public static fiServiceXmlClasses.OO.Parameter GetFachobjektParameterDetails(string parameterId, string fiServicename)
        {
            return GetFachobjektParameterDetailsAsync(parameterId, fiServicename).Result;
        }

        public static async Task<fiServiceXmlClasses.OO.Parameter> GetFachobjektParameterDetailsAsync(string parameterId, string fiServicename)
        {
            try
            {
                
                fiServiceXmlClasses.OO.Parameter rcParameter = new fiServiceXmlClasses.OO.Parameter();

                await Task.Run(action: () => {

                    var prozessUebersicht = Helper.fiServiceXml.OO.GetProzessUebersicht(fiServicename);
                    
                    foreach (var parameter in prozessUebersicht.TopEntitaet.Prozess.ParameterUebersicht)
                    {
                    
                        if (string.Equals(parameter.Id, parameterId, StringComparison.OrdinalIgnoreCase))
                        {
                            rcParameter = parameter;
                            break;
                        }

                    }
                
                });

                return rcParameter;

            }
            catch (Exception e)
            {
                throw new InvalidOperationException("Error GetFachobjektParameterDetailsAsync()", e);
            }
        }


    }
}
