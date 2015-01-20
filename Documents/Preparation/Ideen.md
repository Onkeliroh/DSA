Pitch
=====

- Arduino genauigkeit? bit auflösung? (differenzieren zwischen chipsätzen)
  -> gegenüberstellung zu anderen produkten
- bautrate? schneller = besser?
- slave für arduino

Zu klären
=========

- sprache?
- libs?
- system kompatibilität? ( alle oder erstmal nur unix/linux )

Programm
========

- Beliebig viele Kanäle
  - -> custom abtastfrequenzen
  - nur bei änderung
- Belibibig viele Diagramme
- konfigurierbare übersetzungsfunktion
- Messungs Logger
    - -> ablauf loggen
- CSV Logger
    - -> UTC | Localtime
- Program Logger ( zwecks debug )

Optionales
----------

- Lokalisierung
- Editierbarkeit des Interfaces 
	- verschieben von elementen/ auskoppeln von elementen in neue fenster

Slave
=====

- slave meldet sich bei software an -> identifizierung des boards -> wissen über
  dinge wie z.b. anzahl pins, takt, etc.
- konfigurationen für chipsätze bzw. modelle
- dynamische pin konfiguration
  -> digital : an | aus
  -> analog : range

- orientierung an controlduino
-  dynamische pin konfiguration
    - -> digital : an | aus
    - -> analog : range
