# Megafilter - Paint.NET 5 Plugin

Ein flexibles Filter-Plugin für Paint.NET 5, das Filtereffekte aus einer JSON-Datei lädt und eine Benutzeroberfläche mit Vorschau bereitstellt.

## Features

- **JSON-basierte Filterdefinitionen**: Alle Filtereffekte werden aus `FilterDefinitions.json` geladen
- **Vorschau in Echtzeit**: Paint.NET zeigt automatisch eine Live-Vorschau der Effekte
- **Mehrere Filter enthalten**:

### 🎨 **Basis-Anpassungen** (8 Filter)

- **Helligkeit** ✅ - Helligkeit anpassen (-100 bis +100)
- **Kontrast** ✅ - Kontrast verstärken/reduzieren (-100 bis +100)
- **Sättigung** ✅ - Farbsättigung anpassen (-100 bis +100)
- **Weichzeichner** ✅ - Gaußscher Weichzeichner (Radius 0-100)
- **Schärfen** ✅ - Schärfen-Filter (0-100)
- **Farbton verschieben** ✅ - Hue-Rotation (-180° bis +180°)
- **Vignette** ✅ - Randabdunklung (Strength + Radius 0.0-1.0)
- **Sepia** ✅ - Sepia-Ton-Effekt (Intensity 0-100)

### 🖌️ **Farbfilter** (4 Filter)

- **Graustufen** ✅ - Schwarzweiß (3 Methoden: Average, Luminosity, Desaturation)
- **Invertieren** ✅ - Farben umkehren (Intensity 0-100%)
- **Farbtemperatur** ✅ - Warm/Kalt + Tint-Anpassung (-100 bis +100)
- **Kanalmixer** ✅ - RGB-Kanäle einzeln anpassen (-100 bis +100)

### 🔍 **Schärfe & Weichzeichner** (3 Filter)

- **Bewegungsunschärfe** ✅ - Motion Blur (Angle 0-360°, Distance 0-100px)
- **Kantenerkennung** ✅ - Edge Detection mit Sobel-Operator (Threshold 0-255)
- **Unscharf maskieren** ✅ - Professionelles Schärfen (Amount 0-500%, Radius 1-10, Threshold 0-255)

### 🎭 **Stilisierung** (4 Filter)

- **Posterisieren** ✅ - Farbreduktion (2-256 Stufen)
- **Verpixeln** ✅ - Pixelate-Effekt (Block Size 2-50px)
- **Prägen** ✅ - 3D-Relief-Effekt (Angle 0-360°, Depth 0-100)
- **Ölgemälde** ✅ - Künstlerischer Look (Brush Size 1-20, Intensity 0-100)

### ⚡ **Tonwert-Korrekturen** (4 Filter)

- **Belichtung** ✅ - EV-Stops (-3 bis +3)
- **Schatten/Lichter** ✅ - Dynamikbereich optimieren (je -100 bis +100)
- **Gamma** ✅ - Gamma-Korrektur (0.1-3.0)
- **Tonwertkorrektur** ✅ - Levels mit Black/White/Mid-Point

### 🌟 **Spezialeffekte** (4 Filter)

- **Glühen** ✅ - Bloom/Glow-Effekt (Intensity 0-100, Radius 0-50)
- **Rauschen** ✅ - 3 Rausch-Typen (Gaussian, Uniform, Salt&Pepper)
- **Chromatische Aberration** ✅ - Farbversatz-Linseneffekt (Strength 0-10)
- **Linsenverzerrung** ✅ - Barrel/Pincushion Distortion (-100 bis +100)

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
