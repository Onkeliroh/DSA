Gliederung
==

Titel: Entwicklung einer Software zur Messdatenerfassung und Signalsteuerung mit Hilfe der Arduino Plattform
--

Einführung
--
Related Work
--
  * -> Ist-Zustand

Theorethische Grundlagen
--
  * Was ist ein Arduino
  * Was ist ein Datenlogger
  * Ist-Zustand (gängiger Überblick bzg. Software-Plattformen zur Kommunikation mit Arduino entspr. Messdatenerfassung, Instrumentino & Co.)
  * Linearisierung von Signalen
  * Analog-Digital Wandler
    * Anwendung auf Arduino (Referenzspanung berücksichtigen)


Vorüberlegungen
--
* Persona (Zielgruppe, Literatur)
* Technische und eigene Einschränkungen
  * Einschränkungen:
    * z.B.: I2C, SPI, Bit-Auflösung, Eingangsspannung
    * Geschwindigkeit und Zuverlässigkeit:
      * Echtzeitnähe der Betriebssysteme
    * CSV-Format
* Software
  * Programmiersprache(n)
    * Begründung (z.B. Verlässlichkeit, Übertragbarkeit auf andere Plattformen, Beschränkungen)
  * unterstützte Systeme
    * einschräkungen bezüglich der getesteten Systeme treffen (MacOS wird nicht getestet, aber in der Theorie unterstützt)
  * Designstudy, Anforderungsanalyse usw.
  * Tabelle mit Features
  * Deployment
    * Packaging
    * Veröffentlichung
    * Support
    * Lizensierung
      * Sichtung der Lizensen der verwendeten Bibliotheken
* Interface
  * Ansprüche:
    * Sprache
    * Terminologien -> z.B. elektrotechnische Begriffe, Micosoft Manual of Style

Implementation
--
* Anforderungen -> Responsive Design, Erweiterbar, Teilung von Front und Backend
* Bibliotheken
  * CmdMessenger
* Erklärung Backend
  * Features -> CommandLine Arguments, Threads und Störanfälligkeit
  * UML
  * FlowChart


* Erklärung Arduino Sketch

* Erklärung Frontend
  * Bibliotheken
    * Gtk
      * Philosophie (Icons, Buttonplatzierungen an Dialogen, Dialoge allgemein)
    * Oxyplot
  * Features
    * HCI usablity Berücksichtigen
    * Designentscheidungen
  * Erklärung Teile:
    1. Anlegen **DPins** und **Sequenzen** inklusive Dialoge und Darstellung auf dem Hauptfenster
    2. Anlegen **APins** und **Kombinationen** inklusive Dialoge und Darstellung auf dem Hauptfenster
    3. Auswahl Board
    4. Einstellungen -> CSV, etc.
    5. Verbindungsaufbau
    6. Darstellung Echtzeitplot
    7. Properties

Test unter Laborbedingungen an praxisnahem Versuchsaufbau
--
* Erläuterung Aufbau
  * Zweck
  * Teile
  * Funktionsweise
* Verwendung und Konfiguration der Software
  * Arduino brennen
  * Software Schritt für Schritt einrichten
* Ergebnis
  * etwaige notwendige Verbesserungen / Bug-Fixes
* Fazit


Zukünftige Arbeit
--
