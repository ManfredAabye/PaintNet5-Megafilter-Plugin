# Megafilter - Paint.NET 5 Plugin

Ein flexibles Filter-Plugin für Paint.NET 5, das Filtereffekte aus einer JSON-Datei lädt und eine Benutzeroberfläche mit Vorschau bereitstellt.

## Features

- **JSON-basierte Filterdefinitionen**: Alle Filtereffekte werden aus `FilterDefinitions.json` geladen
- **Vorschau in Echtzeit**: Paint.NET zeigt automatisch eine Live-Vorschau der Effekte
- **Mehrere Filter enthalten**:
  - Helligkeit
  - Kontrast
  - Sättigung
  - Weichzeichner
  - Schärfen
  - Farbton verschieben
  - Vignette
  - Sepia

## Installation

1. **Projekt kompilieren**:

   ```powershell
   dotnet build -c Release
   ```

2. **Plugin installieren**:
   - Kopiere die `Megafilter.dll` und `FilterDefinitions.json` aus dem `bin\Release\net9.0-windows\` Ordner
   - Füge beide Dateien in den Paint.NET Effects-Ordner ein:
     - Für Paint.NET 5: `C:\Program Files\paint.net\Effects\`
     - Oder im User-Ordner: `%USERPROFILE%\Documents\paint.net App Files\Effects\`

3. **Paint.NET neu starten**

## Verwendung

1. Öffne ein Bild in Paint.NET
2. Gehe zu `Effekte > Filter > Megafilter`
3. Wähle einen Filter aus der Dropdown-Liste
4. Passe die Parameter an - die Vorschau wird automatisch aktualisiert
5. Klicke auf OK, um den Effekt anzuwenden

## Eigene Filter hinzufügen

Du kannst eigene Filter hinzufügen, indem du die `FilterDefinitions.json` bearbeitest:

```json
{
  "filters": [
    {
      "id": "mein_filter",
      "name": "Mein Filter",
      "description": "Beschreibung des Filters",
      "parameters": [
        {
          "name": "strength",
          "displayName": "Stärke",
          "type": "int",
          "min": 0,
          "max": 100,
          "default": 50
        }
      ]
    }
  ]
}
```

Anschließend musst du die entsprechende Filterlogik in `MegafilterEffect.cs` in der Methode `ApplyFilter` implementieren.

## Anforderungen

- .NET 9.0 oder höher
- Paint.NET 5.x
- Windows 10/11

## Projektstruktur

```bash
Megafilter/
├── Megafilter.csproj          # Projektdatei
├── MegafilterEffect.cs        # Haupteffekt-Klasse
├── FilterDefinitions.cs       # JSON-Datenmodelle
├── PluginSupportInfo.cs       # Plugin-Informationen
├── FilterDefinitions.json     # Filterdefinitionen
└── README.md                  # Diese Datei
```

## Hinweise

- **Paint.NET Pfad**: Passe den Pfad zu den Paint.NET DLLs in der `.csproj` Datei an deine Installation an
- **Vorschau**: Die Vorschau wird automatisch von Paint.NET bereitgestellt
- **Performance**: Komplexe Filter können bei großen Bildern langsam sein

## Lizenz

Dieses Projekt ist frei verfügbar. Passe es nach Belieben an!
