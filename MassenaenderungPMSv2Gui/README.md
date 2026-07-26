# Massendatenänderung OSPlus - grafische Oberfläche (GUI)

## 📋 Übersicht

Dieses Tool (MassenaenderungPMSv2Gui.exe) ist eine grafische Oberfläche für das Kommandozeilentool MassenaenderungPMSv2.exe.

Mit dessen Hilfe können Sie:

- Importspezifikationsdateien (XML) erstellen
- Massendatenänderungen auf Basis einer Importspezifikationsdatei (XML) durchführen

## ✅ Voraussetzungen

- Windows 11 / Windows Server 2019/2022/2025
- .NET 8 Runtime installiert
- PMS-Schnittstelle der Finanz Informatik installiert und konfiguriert (PW4S-Paket)
- Beschreibungen der Dynamischen Schnittstelle in Form vom XML-Dateien (FI-Kundenportal)
- Kommendozeilentool MassenaenderungPMSv2.exe installliert und konfiguriert

## ⚙️ Konfiguration (`Config/appsettings.json`)

Die Anwendung nutzt die gleiche Konfiguration wie das Tool MassenaenderungPMSv2.exe.
Eine weitere Konfiguration ist nicht erforderlich.
Das GUI muss sich im gleichen Verzeichnis befinden wie das Kommandozeilentool.

## 📋 Erstellen einer Importspezifikation (xml-Datei) mit Hilfe der Gui

- Erstellung einer Input-Daten mit den Massenänderungsdaten wie in der Readme.md des Kommandozeilentools beschrieben
- MassenaenderungPMSv2Gui.exe starten - Die Anwendung startet im Reiter "Importspezifikation erstellen"
- Alle Eingabefelder im Tab "Prozess auswählen" mit den gewünschten Werten füllen/auswählen
- Anschl. im Tab "Eingabeparameter zuordnen" eine Parameter-Zuordnung zwischen CSV-Datei und DynS-Prozess definierten
- Mit "Importspezifikation erstellen" die XML-Datei für die Massendatenänderung erstellen.
  Anhand dieser Datei kann im Anschluss die Massendatenänderung per Kommandozeilen-Tool oder über die Gui gestartet werden.

## 📋 Massendatenänderung anhand einer vorhandenen Importspezifikation (xml-Datei) durchführen

- MassenaenderungPMSv2Gui.exe starten und Reiter "Massenänderung druchführen" auswählen
- Eine vorhandene Importspezifikation (xml-Datei) auswählen
- Massendatenänderung mit "Massenänderung starten" starten
  Optional kann ein Testlauf mit nur einem Datensatz ausgeführt werden.
- Die Ausgaben des Kommandozeilentools werden im Protokollfenster ausgegeben.
