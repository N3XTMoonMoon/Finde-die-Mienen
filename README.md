# Finde-die-Mienen

Eine GUI Anwendung in C#, welche Minesweeper implementieren soll.
Die Implementierung dient als Abschlussleistung innerhalb des Kurses "DSPC012601 - Programmieren mit C#"


## Requirements bei 4 Gruppenmitgliedern

1. Eingabe der Feldgröße bei Spielstart
2. Verteilung der Minen per Zufallprinzip
3. Felder können ausgewählt werden um auf diese zu gehen oder als Miene zu markieren
4. Nach Betreten des Feldes wird die Anzahl an angrenzenden Mienen angezeigt
5. Spieler verliert eins von drei Leben, wenn es sich um eine Miene handelt
6. Wenn alle Mienen markiert wurden hat der Spieler gewonnen
7. Implementierung von drei Spierigkeiten (Leicht, Mittel, Schwer). Diese setzen die Menge an Mienen und deren Nähe zueinander.
8. Implementierung eines Counters, welcher dem Spieler eine definierte Zeit (Je nach Schwierigkeit) zum lösen des Spiels gibt. Wenn dieser Time auf 0 springt und nicht alle Mienen gefunden wurden, ist das Spiel verloren.
9. Speichern des Spiels soll möglich sein
10. Laden eines gespeicherten Spiels soll möglich sein
11. Mehrspieler variante, bei der der Erste Spieler die Mienen legt und der zweit sie finden muss. Dieses soll über einen PC geregelt sein.

## Voraussetzungen

- [.NET 10.0 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)
- Windows-Betriebssystem (Windows Forms)


## Features

- **Feldgröße wählbar:** Beim Spielstart kann die Größe des Spielfelds angegeben werden.
- **Zufällige Minenverteilung:** Minen werden per Zufall auf dem Spielfeld verteilt.
- **Felder auswählen:** Felder können betreten oder als Minen markiert werden.
- **Anzeige angrenzender Minen:** Nach Betreten eines Feldes wird die Anzahl angrenzender Minen angezeigt.
- **Lebenssystem:** Der Spieler hat 3 Leben und verliert eines beim Betreten einer Mine.
- **Gewinnbedingung:** Das Spiel ist gewonnen, wenn alle Minen korrekt markiert wurden.
- **Schwierigkeitsgrade:** Drei Schwierigkeitsgrade (Leicht, Mittel, Schwer) mit unterschiedlicher Minenanzahl und -verteilung.
- **Timer:** Je nach Schwierigkeitsgrad steht eine begrenzte Zeit zur Verfügung.
- **Speichern & Laden:** Spielstände können gespeichert und wieder geladen werden ([SaveState.cs](klassen/SaveState.cs)).
- **Mehrspielermodus:** Ein Spieler platziert Minen, der andere sucht sie. Danach werden die Rollen getauscht.

## Projektstruktur

- **Programmlogik:**  
  - [`Program.cs`](Program.cs): Einstiegspunkt der Anwendung  
  - [`MainForm.cs`](klassen/MainForm.cs): Hauptfenster und UI-Logik  
  - [`Board.cs`](klassen/Board.cs): Spielfeld-Logik  
  - [`Cell.cs`](klassen/Cell.cs): Einzelne Felder des Spielfelds  
  - [`SaveState.cs`](klassen/SaveState.cs): Speichern und Laden des Spielstands  
- **Tests:**  
  - [`BoardTest.cs`](klassen/BoardTest.cs)  
  - [`MainFormTest.cs`](klassen/MainFormTest.cs)  
- **Projektdateien:**  
  - [`Projekt.csproj`](Projekt.csproj): Projektkonfiguration  
  - [`README.md`](README.md): Projektbeschreibung  
- **Build & Output:**  
  - `bin/`: Kompilierte Binärdateien  
  - `obj/`: Build- und Paketdaten  
  - `TestResults/`: Testergebnisse

## Ausführen

```sh
dotnet build
dotnet run
```

## Testen

Unit-Tests sind mit MSTest implementiert.  
Tests können ausgeführt werden mit:

```sh
dotnet test
```


## Originalanforderungen:

"Anwender*innen sollen die Möglichkeit erhalten, die Größe des Spielfelds vor dem Start anzugeben. Nach dem Start des Spiels wird das Spielfeld vom Computer generiert. Auf dem Spielfeld werden Minen zufällig verteilt. Bei jedem Spielzug wählen Spieler Felder. Es gibt die Möglichkeit Felder zu
wählen, um diese zu betreten oder Felder zu wählen, um diese als Minenfelder zu markieren. Betritt der Spieler ein Feld, an das eine od. mehrere Minen grenzen, wird die Anzahl der Minen ausgegeben. Liegt eine Mine auf dem gewählten Feld, verliert der Spieler eines seiner max. 3 Leben. Spieler können Felder markieren, auf denen sie Minen vermuten. Wurden alle Minen identifiziert hat der Spieler gewonnen."	

Es sind drei verschiedene Schwierigkeitsgrade zu implementieren. Die drei Schwierigkeitsgrade unterscheiden sich durch die Anzahl der Minen (mehr Minen je höher die Schwierigkeit) und ihre Verteilung (je höher die Schwierigkeit, desto dichter liegen die Minen beieinander).	

Es ist ein Counter zu implementieren. Sofern der Nutzer nicht innerhalb einer bestimmten Zeit alle Minen identifiziert, hat er verloren. Je höher die Schwierigkeit, desto weniger Zeit verbleibt dem Nutzer. Zudem ist eine Funktion zu schaffen, um Spiele zu speichern und zu laden.	

Es ist eine Mehrspieler-Version zu entwickeln, bei der ein Spieler*in Minen platziert und ein anderer Spieler*in die Minen suchen muss. Anschließend wechseln die Rollen der beiden Spieler. Die MehrspielerVersion wird auf einem PC abwechselnd gespielt.
