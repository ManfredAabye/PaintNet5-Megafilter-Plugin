# Megafilter Plugin - MEGA-UPDATE 3.0 - PIPELINE EDITION 🚀✨

## Status: 27 FILTER - ALLE GLEICHZEITIG ANWENDBAR! 🎨⚡

Das Plugin ist jetzt eine vollwertige **Filter-Pipeline** - alle 27 professionellen Effekte können **gleichzeitig kombiniert** werden!

## ✅ ALLE 27 FILTER IMPLEMENTIERT - PIPELINE-MODUS

**🎯 NEUE FUNKTION: Alle Filter gleichzeitig nutzbar!**

Statt einen Filter auszuwählen, sind jetzt **alle Parameter immer sichtbar**. Jeder Filter wird automatisch angewendet, sobald du seinen Wert vom Standard abweichst.

**Beispiel:** 
- Helligkeit +20
- Kontrast +15
- Sättigung +10
- Sepia 30%
- Vignette 0.5

→ Alle werden in optimaler Reihenfolge kombiniert!

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

## Build-Output

```bash
✅ Megafilter.dll (24 KB)
✅ FilterDefinitions.json (12.8 KB)
✅ 0 Fehler, 2 unkritische Warnungen
✅ 27 Filter - alle gleichzeitig kombinierbar!
✅ Pipeline-Architektur mit intelligenter Filter-Reihenfolge
```

## 🆕 Was ist neu in v3.0 - PIPELINE EDITION?

### ⚡ Hauptfeature: Multi-Filter-Pipeline

**Vorher (v2.0):** Dropdown-Menü → Einen Filter auswählen → Parameter einstellen
**Jetzt (v3.0):** Alle 27 Filter gleichzeitig verfügbar → Beliebig kombinieren!

### 🎯 Intelligente Filter-Reihenfolge

Die Filter werden automatisch in optimaler Reihenfolge angewendet:

1. **Farbkorrekturen** (Helligkeit, Kontrast, Belichtung, Gamma...)
2. **Farbeffekte** (Hue-Shift, Graustufen, Sepia, Invertieren)
3. **Stilisierung** (Posterize, Pixelate, Ölgemälde)
4. **Weichzeichner & Schärfe** (Blur, Sharpen, Motion Blur...)
5. **Spezialeffekte** (Emboss, Edge Detect, Vignette, Glow...)

### 🔧 Technische Verbesserungen

- ❌ **Entfernt:** FilterType Dropdown
- ❌ **Entfernt:** ReadOnly Property Rules
- ✅ **Neu:** `ApplyFilterPipeline()` - Intelligente Multi-Filter-Engine
- ✅ **Neu:** Automatische Default-Wert-Erkennung (Filter wird nur bei Änderung angewendet)
- ✅ **Neu:** Bessere UI-Labels mit Filter-Präfix (z.B. "Helligkeit - Stärke")

### 💡 Workflow-Beispiele

**Professionelle Bildbearbeitung:**
```
1. Belichtung: +10
2. Kontrast: +20
3. Sättigung: +5
4. Schärfen: 25
5. Vignette: Strength 0.3, Radius 0.8
```

**Künstlerischer Look:**
```
1. Farbtemperatur: +30 (wärmer)
2. Sepia: 40%
3. Ölgemälde: Brush 3, Intensity 20
4. Vignette: Strength 0.5
```

**S/W-Portrait mit Drama:**
```
1. Graustufen: Luminosity
2. Kontrast: +40
3. Schatten/Lichter: Shadows +30, Highlights -20
4. Vignette: Strength 0.6
```

## Installation

```powershell
# Paint.NET MUSS geschlossen sein!
Copy-Item "bin\Release\net9.0-windows\Megafilter.dll" "C:\Program Files\paint.net\Effects\" -Force
Copy-Item "bin\Release\net9.0-windows\FilterDefinitions.json" "C:\Program Files\paint.net\Effects\" -Force

# Starte Paint.NET neu
# Gehe zu Effects > Effects > Megafilter
# Alle 27 Filter-Parameter sind sichtbar!
# Verändere beliebige Werte - Filter werden automatisch kombiniert!
```

## 🎨 Wie nutzt man die Pipeline?

1. **Öffne Paint.NET** und wähle Effects > Effects > Megafilter
2. **Scrolle durch alle 27 Filter** - sie sind nach Kategorie sortiert
3. **Stelle beliebige Parameter ein** - Werte ungleich Default werden angewendet
4. **Live-Preview** zeigt die Kombination aller aktiven Filter
5. **Experimentiere!** Kombinationen sind unbegrenzt

### Tipps für beste Ergebnisse

✅ **Do:**
- Starte mit Farbkorrekturen (Helligkeit, Kontrast)
- Füge dann Stilisierung hinzu
- Nutze Vignette am Ende für Finishing

⚠️ **Don't:**
- Zu viele intensive Filter kombinieren (z.B. Ölgemälde + Edge Detect)
- Extreme Werte bei mehreren Filtern gleichzeitig

## Filter-Kategorien im Plugin

**Farbkorrektur:** Helligkeit, Kontrast, Sättigung, Farbtemperatur, Kanalmixer, Gamma, Tonwertkorrektur, Belichtung, Schatten/Lichter

**Schwarz/Weiß:** Graustufen, Invertieren, Sepia

**Weichzeichner:** Weichzeichner, Bewegungsunschärfe, Glühen

**Schärfe:** Schärfen, Unscharf maskieren

**Künstlerisch:** Posterisieren, Verpixeln, Prägen, Ölgemälde, Kantenerkennung

**Effekte:** Vignette, Farbton verschieben, Rauschen, Chromatische Aberration, Linsenverzerrung

## Performance-Hinweise

### Pipeline-Performance

**Kombination von Filtern:**
- Nur Filter mit **veränderten Werten** werden angewendet
- Filter werden in **optimaler Reihenfolge** ausgeführt
- Pro Pixel werden alle aktiven Filter sequenziell angewendet

⚠️ **Rechenintensiv:** Ölgemälde, Kantenerkennung, Unscharf maskieren, Bewegungsunschärfe
⚡ **Schnell:** Helligkeit, Kontrast, Invertieren, Graustufen, Gamma
🖼️ **Mittel:** Alle anderen Filter

### Empfehlungen

- **Kleine Bilder (<2000px):** Alle Kombinationen problemlos
- **Mittelgroße Bilder (2000-4000px):** Vermeide mehr als 3-4 rechenintensive Filter
- **Große Bilder (>4000px):** Nutze hauptsächlich schnelle Filter, wenige intensive

**Tipp:** Änderungen werden live berechnet - bei langsamen Kombinationen kurz warten bis Preview fertig ist.

Bei großen Bildern (>4000px) können einige Filter mehrere Sekunden benötigen. Progress-Feedback wird durch Paint.NET angezeigt.

## Technische Details

- **Architektur:** PropertyBasedEffect (Classic API) mit Pipeline-Processing
- **Filter-Pipeline:** JSON-basierte Konfiguration + intelligente Reihenfolge
- **Pipeline-Engine:** `ApplyFilterPipeline()` - wendet alle aktiven Filter sequenziell an
- **Automatische Optimierung:** Filter nur bei Werten ≠ Default aktiv
- **Pixel-Processing:** Direct Surface-Access mit Multi-Pass-Pipeline
- **Multi-Threading:** Automatisch durch Paint.NET Rectangle-Rendering
- **UI-Verbesserung:** Filter-Namen als Präfix für bessere Übersicht

## Neue Filter hinzufügen

1. Filter in `FilterDefinitions.json` definieren
2. Case in `ProcessPixel()` hinzufügen (gleich wie vorher)
3. Filter-Methode implementieren (Signatur: `ColorBgra Apply...(ColorBgra pixel, ...)`)
4. Optional: Reihenfolge in `ApplyFilterPipeline()` anpassen
5. Kompilieren und testen!

Beispiel:

```csharp
// In ProcessPixel():
"myfilter" => ApplyMyFilter(pixel, GetParameterValue(filter, "strength")),

private ColorBgra ApplyMyFilter(ColorBgra pixel, double strength)
{
    // Ihre Logik hier
    return pixel;
}

// In ApplyFilterPipeline() - Reihenfolge anpassen:
string[] filterOrder = new[]
{
    "brightness", "contrast", ..., "myfilter", ...
};
```

## Was kommt als nächstes?

Mögliche Erweiterungen:

- **Presets:** Vordefinierte Filter-Kombinationen speichern/laden
- **Filter-Gruppen:** Collapsible Sections im UI für bessere Übersicht
- **GPU-Acceleration:** Für rechenintensive Filter-Kombinationen
- **Reihenfolge-Editor:** Benutzer kann Filter-Reihenfolge selbst bestimmen
- **A/B-Vergleich:** Original vs. Pipeline-Ergebnis Vergleichsansicht
