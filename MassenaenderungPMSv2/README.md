# Massendatenänderung OSPlus - Command Line Interface

## 📋 Übersicht

Tool zur Massendatenänderung (CLI) für die Gesamtbank-Lösung OSPlus der Finanz Informatik.
Das Tool nutzt die PMS-Schnittstelle zum Aufruf der DynS-Webservices.

Prozessaufrufe und Eingabeparameter werden in `Importspezifikationen` in Form einer XML-Datei definiert
und die Daten für die Massendatenänderung werden in Form von CSV-Dateien bereitgestellt.

Für wiederkehrende Aufgaben kann das Tool über die Windows Aufgabenplanung automatisiert werden.

## ✅ Voraussetzungen

- Windows 11 / Windows Server 2019/2022/2025
- .NET 8 Runtime installiert
- PMS-Schnittstelle der Finanz Informatik installiert und konfiguriert (PW4S-Paket)
- Beschreibungen der Dynamischen Schnittstelle in Form vom XML-Dateien (FI-Kundenportal)

## ⚙️ Konfiguration (`Config/appsettings.json`)

### Abschnitt `fiServices`

**Parameter:**
`XmlPath` – Pfad (mit doppelten `\\`) zu den API-Beschreibungen der Dynamischen Schnittstelle im XML-Format (FI-Kundenportal).
Laden Sie sich diese Dateien herunter und entpacken Sie diese Dateien z.B. ins `Programmverzeichnis`\fiServicesXml des Tools.

### Abschnitt `GlobalSettings`

**Parameter:**

- `Log2Console` – `true`/`false` – Logausgabe auch auf der Konsole aktivieren
- `AppLog.File` – Vollständiger Pfad zur Anwendungslogdatei (mit doppelten `\\`)
- `AppLog.MaxLines` – Maximale Zeilen in der Logdatei (wird bei Bedarf gekürzt)

### Abschnitt `DynSKontext`

Enthält die Verbindungsparameter (DynSKontext) der Prozesse für die Dynamische Schnittstelle.
Diese sind für jedes Institut individuell und müssen hier entsprechend angepasst werden.
Welche Werte für das jeweilige Institut gelten, kann den Handbüchern zur Dyn. Schnittstelle oder
auch dem Testtool für Dyns. Prozesse im OSPlus-Portal entnommen werden.

**Parameter:**

- `PS_USERID` - OSPlus-Benutzerkennung mit dem sich das Tool zum Aufruf der Prozesse am OSPlus anmeldet.
   Der Benutzer muss zum Aufruf der Prozesse mit den jeweiligen Rechten im KURS ausgestattet sein.
   2FA sollte deaktiviert sein und ein Ablauf des Passworts sollte nicht erfolgen.
- `PS_PASSWORT` – Verschlüsseltes Passwort für die PS_USERID
   Das Passwort wird über Auftrufparameter`-enc` ermittelt und im Anschluss hier eingetragen.

## Input-Datei (CSV) und Format

Die Daten für die Massendatenänderung müssen für das Tool in Form einer CSV-Datei bereitgestellt werden. Der Aufbau dieser Daten muss wie folgt erfolgen:

- Encoding: ISO-8859-1
- Erste Zeile wird als Header behandelt (Spaltennamen).
  Der Header muss alle in der Importspezifikation referenzierten Spalten enthalten.
  Als Trennzeichen sollte ";" und als Textqualifizierer sollte '"' verwendet werden.
- Die weiteren Zeilen sind die Datenzeilen und enthalten die Werte für die Massendatenänderung

### CSV-Importdatei

- **Encoding:** ISO-8859-1
- **Trennzeichen und Textqualifizierer:** Aus der Importspezifikation
- **Erste Zeile:** Header mit Spaltennamen
- **CSV-Header:** Muss alle in der Importspezifikation referenzierten Spalten enthalten

## Importspezifikation (XML)

Die Steuerung der Massendatenänderung erfolgt über eine sog. Importspezifikation. Das ist eine XML-Datei mit folgendem Aufbau:

**Wichtige Attribute:**

- `Version` – Versionsnummer der Spezifikation
- `Importdatei` – Vollständiger Pfad zur CSV-Importdatei
- `Trennzeichen` – CSV-Trennzeichen (z.B. `;`)
- `Textqualifizierer` – Textqualifizierer (z.B. `"`)
- `schnittstellen_art` – `PO` (Parameterorientiert) oder `OO` (Objektorientiert)

**Parameter-Definition:**

- `name` – Name des Eingabeparameters im Prozessaufruf
- `lfdn` – Laufende Nummer des Parameters (idR. 1, bei Arrays 1...x)
- `isarray` – `true`, wenn Eingabearray bei PO-Prozessen
- Wert – Name der Spalte in der Eingabedatei

**Einschränkungen:** OO-Prozesse unterstützen derzeit nur Eingabekardinalität 1.

### Aufbau der Importspezifikation (xml-Datei)

```text
<?xml version="1.0" encoding="iso-8859-1" standalone="yes"?>
<Uebersicht>
<Importspezifikation Version="1.0.0" 
                     Beschreibung="Eine kleine Beschreibung zum Auftrag"
                     <!-- Datei mit den Änderungsdatensätzen - vollständiger Pfad -->
                     Importdatei="<Pfad zur Importdatei mit den Daten>.csv"
                     Trennzeichen=";"
                     Textqualifizierer='"'>
  <Prozess name="<Dyn. Schnittstelle Prozessname>" 
           schnittstellen_art="PO" <!-- PO = Parameterorientierter Prozess / OO = Objektorientierter Prozess  -->
           service_operation="Bei OO-Prozess hier das ServiceEingabeObjekt"
           aufrufvariantennummer="1"
           logdatei="<Logdatei>.log">
    <!-- Hier werden die Eingabeparameter definiert -->
    <!-- Bei Eingabekardinlität > 1, müssen auch entsprechende Eingabeparameter als Array (isarray=true) mit entsprechender lfdn definiert werden -->
    <!-- name      : Names ein Eingabeparameters im Prozessaufruf -->
    <!-- eingabe-fo: Names des Fachobjekts als Eingabeparameter bei objektorienterten Prozessen - Dies wird derzeit noch NICHT unterstuetzt! -->
    <!-- lfdn      : Lfdn des Parameters - idR 1, bei Arrayparameter(PO) oder Eingabeparameter als Fachobjekt(OO) hier 1...x -->
    <!-- isarry    : true, wenn es ein Eingabearray bei einem parameterorientierten Prozess ist, entsprechend auch die lfdn pflegen -->
    <!-- Wert      : Die Name der Spalte in der Eingabedatei -->
    <!-- Einschränkungen: Bei objektorientierten Prozessen wird derzeit nur die Eingabekardinalität 1 unterstützt. 
                          Ein Eingabefachobjekt(eingabe-fo) wird derzeit nicht unterstützt.
      -->
    <EingabeParameterCollection eingabekardinalität="1">
      <Eingabeparameter name="PERS_NR" fo="" lfdn="1" isarray="false">PERS_NR</Eingabeparameter>
      <Eingabeparameter name="ANREDE" fo="" lfdn="1" isarray="false">ANREDE</Eingabeparameter>
      <--!  Beispiel der Anrede als Array string[2] -->
      <Eingabeparameter name="ANREDE" fo="" lfdn="1" isarray="true">ANREDE1</Eingabeparameter>
      <Eingabeparameter name="ANREDE" fo="" lfdn="2" isarray="true">ANREDE1</Eingabeparameter>
    </EingabeParameterCollection>
  </Prozess>  
</Importspezifikation>
</Uebersicht>
```

### 💡 Tipp

Die Importspezifikation (XML) kann auch mit der grafischen Oberfläche MassenaenderungPMSv2Gui.exe erstellt werden.

## 🚀 Aufrufparameter

- -i, --impspez — Pfad zur Importspezifikation (erforderlich).
- -t, --test — Testlauf-Flag (optional).
- -d, --delay — Delay in Millisekunden zwischen Aufrufen (optional).
- -enc, --encryption - Verschlüsselungsanfrage für das Userpasswort zur Hinterlegung in der appsettings.json StringToEncrypt (optional).

### 📝 Beispielaufrufe

MassenaenderungPMSv2.exe --impspez "C:\KARTEN_SPERRE_LOESCHEN_02.xml" --test
MassenaenderungPMSv2.exe -enc MeinKlartextPasswort

## 📊 Logs und Ausgaben

- **Statusmeldungen:** Bei aktiviertem `GlobalSettings:Log2Console` auch in der Konsole
- **Applikationslog:** Datei unter `GlobalSettings:AppLog:File`
- **Log-Verwaltung:** Wird bei Programmende ggf. mit `Truncate(maxLines, logFile)` gekürzt

## ⚠️ Fehlerbehandlung & Troubleshooting

- Fehlende/fehlerhafte appsettings.json → Programm beendet sich (Meldung in Konsole).
- Fehlende -i / ungültige Importspezifikation → Abbruch mit Logeintrag.
- CSV-Header stimmt nicht mit EingabeParameterCollection überein → Validierung schlägt fehl.
- Encoding-Probleme → CSV muss ISO-8859-1 sein oder vorab konvertiert werden.
- Weitere Details: Aktivieren Sie GlobalSettings:Log2Console und prüfen Sie die Logdatei für Fehlermeldungen.

**Tipps:**

- Aktivieren Sie `GlobalSettings:Log2Console` für ausführlichere Ausgaben
- Prüfen Sie die Logdatei für detaillierte Fehlermeldungen
- Feldnamen-Vergleiche sind fallunabhängig (Uppercase), aber Tippfehler führen trotzdem zum Fehler

## 💡 Best Practices

- ✅ Alle verpflichtenden Parameter in der Importspezifikation enthalten
- ✅ CSV-Header mit korrekter Kodierung (ISO-8859-1) erstellen
- ✅ Testlauf (`--test`) vor der produktiven Verarbeitung durchführen
- ✅ Logdateien überprüfen
