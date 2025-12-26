# Build-Anleitung

## Voraussetzungen

1. .NET 9.0 SDK installiert
2. Paint.NET 5 installiert

## Build-Schritte

### 1. DLL-Pfade anpassen

Öffne `Megafilter.csproj` und passe die Pfade zu deiner Paint.NET Installation an:

```xml
<Reference Include="PaintDotNet.Base">
  <HintPath>C:\Program Files\paint.net\PaintDotNet.Base.dll</HintPath>
</Reference>
```

### 2. Projekt kompilieren

```powershell
# Im Projektverzeichnis ausführen
dotnet restore
dotnet build -c Release
```

### 3. Plugin installieren

Die kompilierten Dateien befinden sich in:

```bash
bin\Release\net9.0-windows\
```

Kopiere folgende Dateien in den Paint.NET Effects-Ordner:

- `Megafilter.dll`
- `FilterDefinitions.json`

**Ziel-Ordner:**

- `C:\Program Files\paint.net\Effects\` (benötigt Admin-Rechte)
- ODER `%USERPROFILE%\Documents\paint.net App Files\Effects\`

### 4. Paint.NET neu starten

## Entwicklung

### Debug-Build erstellen

```powershell
dotnet build -c Debug
```

### Tests durchführen

Da dies ein Plugin ist, teste es direkt in Paint.NET:

1. Kompiliere das Plugin
2. Kopiere es in den Effects-Ordner
3. Starte Paint.NET
4. Öffne ein Testbild
5. Wende den Effekt an: `Effekte > Filter > Megafilter`

## Fehlerbehebung

### "Assembly konnte nicht geladen werden"

- Überprüfe, ob alle Paint.NET DLLs korrekt referenziert sind
- Stelle sicher, dass die Pfade in der .csproj korrekt sind

### "FilterDefinitions.json nicht gefunden"

- Stelle sicher, dass die JSON-Datei im gleichen Ordner wie die DLL liegt
- Überprüfe die Build-Action der JSON-Datei (sollte "Content" sein)

### Plugin erscheint nicht in Paint.NET

- Stelle sicher, dass du Paint.NET neu gestartet hast
- Überprüfe das Event-Log von Windows auf Fehler
- Stelle sicher, dass das Plugin für die richtige .NET Version kompiliert wurde
