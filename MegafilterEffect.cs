using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text.Json;
using PaintDotNet;
using PaintDotNet.Effects;
using PaintDotNet.IndirectUI;
using PaintDotNet.PropertySystem;

namespace Megafilter;

[PluginSupportInfo(typeof(PluginSupportInfo))]
public sealed class MegafilterEffect : PropertyBasedEffect
{
    private FilterDefinitions? _filterDefinitions;
    
#pragma warning disable CS0618 // Type or member is obsolete - using classic API for stability
    public MegafilterEffect()
        : base("Megafilter", (System.Drawing.Image)null!, "Effects", new EffectOptions { Flags = EffectFlags.Configurable })
    {
        LoadFilterDefinitions();
    }
#pragma warning restore CS0618

    private void LoadFilterDefinitions()
    {
        try
        {
            string jsonPath = Path.Combine(Path.GetDirectoryName(GetType().Assembly.Location) ?? "", "FilterDefinitions.json");
            if (File.Exists(jsonPath))
            {
                string jsonContent = File.ReadAllText(jsonPath);
                _filterDefinitions = JsonSerializer.Deserialize<FilterDefinitions>(jsonContent);
            }
        }
        catch (Exception)
        {
            _filterDefinitions = new FilterDefinitions();
        }
    }

    protected override PropertyCollection OnCreatePropertyCollection()
    {
        var properties = new List<Property>();

        if (_filterDefinitions != null && _filterDefinitions.Filters.Count > 0)
        {
            // Erstelle Properties für ALLE Filter-Parameter (keine Rules mehr!)
            foreach (var filter in _filterDefinitions.Filters)
            {
                foreach (var param in filter.Parameters)
                {
                    string propName = $"{filter.Id}_{param.Name}";
                    
                    if (param.Type == "double")
                    {
                        properties.Add(new DoubleProperty(propName, param.Default, param.Min, param.Max));
                    }
                    else
                    {
                        properties.Add(new Int32Property(propName, (int)param.Default, (int)param.Min, (int)param.Max));
                    }
                }
            }
        }

        return new PropertyCollection(properties);
    }

    protected override ControlInfo OnCreateConfigUI(PropertyCollection props)
    {
        ControlInfo configUI = CreateDefaultConfigUI(props);
        
        if (_filterDefinitions != null && _filterDefinitions.Filters.Count > 0)
        {
            // Setze Display-Namen mit Filter-Präfix für bessere Übersicht
            foreach (var filter in _filterDefinitions.Filters)
            {
                foreach (var param in filter.Parameters)
                {
                    string propName = $"{filter.Id}_{param.Name}";
                    string displayName = $"{filter.Name} - {param.DisplayName}";
                    configUI.SetPropertyControlValue(propName, ControlInfoPropertyNames.DisplayName, displayName);
                }
            }
        }
        
        return configUI;
    }

    protected override void OnSetRenderInfo(PropertyBasedEffectConfigToken? newToken, RenderArgs dstArgs, RenderArgs srcArgs)
    {
        base.OnSetRenderInfo(newToken, dstArgs, srcArgs);
    }

    protected override void OnRender(Rectangle[] renderRects, int startIndex, int length)
    {
        if (_filterDefinitions == null || _filterDefinitions.Filters.Count == 0)
            return;

        Surface srcSurface = SrcArgs.Surface;
        Surface dstSurface = DstArgs.Surface;

        for (int i = startIndex; i < startIndex + length; i++)
        {
            Rectangle rect = renderRects[i];

            for (int y = rect.Top; y < rect.Bottom; y++)
            {
                if (IsCancelRequested)
                    return;

                for (int x = rect.Left; x < rect.Right; x++)
                {
                    ColorBgra pixel = srcSurface[x, y];
                    
                    // Filter-Pipeline: Wende alle Filter nacheinander an
                    // Nur wenn Wert != Default wird der Filter angewendet
                    pixel = ApplyFilterPipeline(pixel, x, y, srcSurface.Width, srcSurface.Height);

                    dstSurface[x, y] = pixel;
                }
            }
        }
    }

    private ColorBgra ApplyFilterPipeline(ColorBgra pixel, int x, int y, int width, int height)
    {
        if (_filterDefinitions == null) return pixel;

        // Definiere sinnvolle Filter-Reihenfolge
        string[] filterOrder = new[]
        {
            // 1. Farbkorrekturen (Basis)
            "brightness", "contrast", "saturation", "exposure", "gamma",
            "temperature", "channelmixer", "levels", "shadowhighlight",
            
            // 2. Farbeffekte
            "hueshift", "grayscale", "sepia", "invert",
            
            // 3. Stilisierung
            "posterize", "pixelate", "oilpainting",
            
            // 4. Weichzeichner & Schärfe
            "blur", "sharpen", "unsharpmask", "motionblur",
            
            // 5. Spezialeffekte
            "emboss", "edgedetect", "glow", "vignette",
            "noise", "chromatic", "lensdistortion"
        };

        foreach (var filterId in filterOrder)
        {
            var filter = _filterDefinitions.Filters.FirstOrDefault(f => f.Id == filterId);
            if (filter == null) continue;

            // Prüfe ob mindestens ein Parameter != Default ist
            bool shouldApply = false;
            foreach (var param in filter.Parameters)
            {
                double currentValue = GetFilterParamValue(filterId, param.Name);
                if (Math.Abs(currentValue - param.Default) > 0.001)
                {
                    shouldApply = true;
                    break;
                }
            }

            if (shouldApply)
            {
                pixel = ProcessPixel(pixel, filter, x, y, width, height);
            }
        }

        return pixel;
    }

    private double GetFilterParamValue(string filterId, string paramName)
    {
        string propName = $"{filterId}_{paramName}";
        var property = Token.GetProperty(propName);
        
        if (property is Int32Property intProp)
            return intProp.Value;
        else if (property is DoubleProperty doubleProp)
            return doubleProp.Value;
        
        return 0;
    }

    private ColorBgra ProcessPixel(ColorBgra pixel, FilterDefinition filter, int x, int y, int width, int height)
    {
        return filter.Id switch
        {
            "brightness" => ApplyBrightness(pixel, GetParameterValue(filter, "amount")),
            "contrast" => ApplyContrast(pixel, GetParameterValue(filter, "amount")),
            "saturation" => ApplySaturation(pixel, GetParameterValue(filter, "amount")),
            "sharpen" => ApplySharpen(pixel, GetParameterValue(filter, "amount")),
            "hueshift" => ApplyHueShift(pixel, GetParameterValue(filter, "degrees")),
            "vignette" => ApplyVignette(pixel, x, y, width, height, GetParameterValue(filter, "strength"), GetParameterValue(filter, "radius")),
            "sepia" => ApplySepia(pixel, GetParameterValue(filter, "intensity")),
            "grayscale" => ApplyGrayscale(pixel, GetParameterValue(filter, "method")),
            "invert" => ApplyInvert(pixel, GetParameterValue(filter, "intensity")),
            "temperature" => ApplyTemperature(pixel, GetParameterValue(filter, "temp"), GetParameterValue(filter, "tint")),
            "channelmixer" => ApplyChannelMixer(pixel, GetParameterValue(filter, "red"), GetParameterValue(filter, "green"), GetParameterValue(filter, "blue")),
            "motionblur" => ApplyMotionBlur(pixel, x, y, GetParameterValue(filter, "angle"), GetParameterValue(filter, "distance")),
            "edgedetect" => ApplyEdgeDetect(pixel, x, y, GetParameterValue(filter, "threshold")),
            "unsharpmask" => ApplyUnsharpMask(pixel, x, y, GetParameterValue(filter, "amount"), GetParameterValue(filter, "radius"), GetParameterValue(filter, "threshold")),
            "posterize" => ApplyPosterize(pixel, GetParameterValue(filter, "levels")),
            "pixelate" => ApplyPixelate(pixel, x, y, GetParameterValue(filter, "blocksize")),
            "emboss" => ApplyEmboss(pixel, x, y, GetParameterValue(filter, "angle"), GetParameterValue(filter, "depth")),
            "oilpainting" => ApplyOilPainting(pixel, x, y, GetParameterValue(filter, "brushsize"), GetParameterValue(filter, "intensity")),
            "exposure" => ApplyExposure(pixel, GetParameterValue(filter, "ev")),
            "shadowhighlight" => ApplyShadowHighlight(pixel, GetParameterValue(filter, "shadows"), GetParameterValue(filter, "highlights")),
            "gamma" => ApplyGamma(pixel, GetParameterValue(filter, "value")),
            "levels" => ApplyLevels(pixel, GetParameterValue(filter, "blackpoint"), GetParameterValue(filter, "whitepoint"), GetParameterValue(filter, "midpoint")),
            "glow" => ApplyGlow(pixel, x, y, GetParameterValue(filter, "intensity"), GetParameterValue(filter, "radius")),
            "noise" => ApplyNoise(pixel, GetParameterValue(filter, "amount"), GetParameterValue(filter, "type")),
            "chromatic" => ApplyChromaticAberration(pixel, x, y, GetParameterValue(filter, "strength")),
            "lensdistortion" => ApplyLensDistortion(pixel, x, y, width, height, GetParameterValue(filter, "amount")),
            _ => pixel
        };
    }

    private double GetParameterValue(FilterDefinition filter, params string[] paramNames)
    {
        foreach (var paramName in paramNames)
        {
            string propName = $"{filter.Id}_{paramName}";
            var property = Token.GetProperty(propName);
            
            if (property is Int32Property intProp)
                return intProp.Value;
            else if (property is DoubleProperty doubleProp)
                return doubleProp.Value;
        }
        return 0;
    }

    private ColorBgra ApplyBrightness(ColorBgra pixel, double amount)
    {
        int adjustment = (int)(amount * 2.55);
        return ColorBgra.FromBgra(
            (byte)Math.Clamp(pixel.B + adjustment, 0, 255),
            (byte)Math.Clamp(pixel.G + adjustment, 0, 255),
            (byte)Math.Clamp(pixel.R + adjustment, 0, 255),
            pixel.A);
    }

    private ColorBgra ApplyContrast(ColorBgra pixel, double amount)
    {
        double contrast = (amount + 100.0) / 100.0;
        contrast *= contrast;

        int r = (int)Math.Clamp((int)(((pixel.R / 255.0 - 0.5) * contrast + 0.5) * 255.0), 0, 255);
        int g = (int)Math.Clamp((int)(((pixel.G / 255.0 - 0.5) * contrast + 0.5) * 255.0), 0, 255);
        int b = (int)Math.Clamp((int)(((pixel.B / 255.0 - 0.5) * contrast + 0.5) * 255.0), 0, 255);

        return ColorBgra.FromBgra((byte)b, (byte)g, (byte)r, pixel.A);
    }

    private ColorBgra ApplySaturation(ColorBgra pixel, double amount)
    {
        double saturation = (amount + 100.0) / 100.0;
        int gray = (int)(pixel.R * 0.299 + pixel.G * 0.587 + pixel.B * 0.114);

        int r = (int)Math.Clamp((int)(gray + (pixel.R - gray) * saturation), 0, 255);
        int g = (int)Math.Clamp((int)(gray + (pixel.G - gray) * saturation), 0, 255);
        int b = (int)Math.Clamp((int)(gray + (pixel.B - gray) * saturation), 0, 255);

        return ColorBgra.FromBgra((byte)b, (byte)g, (byte)r, pixel.A);
    }

    private ColorBgra ApplyBlur(ColorBgra[,] source, int x, int y, int width, int height, FilterDefinition filter)
    {
        double radius = GetParameterValue(filter, "radius");
        int kernelSize = (int)Math.Max(1, radius / 10.0);
        
        if (kernelSize == 0)
            return source[x, y];

        int r = 0, g = 0, b = 0, count = 0;
        
        for (int dy = -kernelSize; dy <= kernelSize; dy++)
        {
            for (int dx = -kernelSize; dx <= kernelSize; dx++)
            {
                int nx = x + dx;
                int ny = y + dy;
                
                if (nx >= 0 && nx < width && ny >= 0 && ny < height)
                {
                    var pixel = source[nx, ny];
                    r += pixel.R;
                    g += pixel.G;
                    b += pixel.B;
                    count++;
                }
            }
        }
        
        if (count > 0)
        {
            return ColorBgra.FromBgra((byte)(b / count), (byte)(g / count), (byte)(r / count), source[x, y].A);
        }
        
        return source[x, y];
    }

    private ColorBgra ApplySharpen(ColorBgra pixel, double amount)
    {
        double factor = 1.0 + (amount / 100.0);
        
        int r = (int)Math.Clamp(pixel.R * factor, 0, 255);
        int g = (int)Math.Clamp(pixel.G * factor, 0, 255);
        int b = (int)Math.Clamp(pixel.B * factor, 0, 255);
        
        return ColorBgra.FromBgra((byte)b, (byte)g, (byte)r, pixel.A);
    }

    private ColorBgra ApplyHueShift(ColorBgra pixel, double degrees)
    {
        double r = pixel.R / 255.0;
        double g = pixel.G / 255.0;
        double b = pixel.B / 255.0;
        
        double max = Math.Max(r, Math.Max(g, b));
        double min = Math.Min(r, Math.Min(g, b));
        double delta = max - min;
        
        double h = 0, s = 0, v = max;
        
        if (delta > 0)
        {
            s = delta / max;
            
            if (r == max)
                h = (g - b) / delta + (g < b ? 6 : 0);
            else if (g == max)
                h = (b - r) / delta + 2;
            else
                h = (r - g) / delta + 4;
            
            h /= 6;
        }
        
        h += degrees / 360.0;
        while (h < 0) h += 1;
        while (h > 1) h -= 1;
        
        double nr, ng, nb;
        
        if (s == 0)
        {
            nr = ng = nb = v;
        }
        else
        {
            h *= 6;
            int i = (int)h;
            double f = h - i;
            double p = v * (1 - s);
            double q = v * (1 - s * f);
            double t = v * (1 - s * (1 - f));
            
            switch (i % 6)
            {
                case 0: nr = v; ng = t; nb = p; break;
                case 1: nr = q; ng = v; nb = p; break;
                case 2: nr = p; ng = v; nb = t; break;
                case 3: nr = p; ng = q; nb = v; break;
                case 4: nr = t; ng = p; nb = v; break;
                default: nr = v; ng = p; nb = q; break;
            }
        }
        
        return ColorBgra.FromBgra((byte)(nb * 255), (byte)(ng * 255), (byte)(nr * 255), pixel.A);
    }

    private ColorBgra ApplyVignette(ColorBgra pixel, int x, int y, int width, int height, double strength, double radius)
    {
        double centerX = width / 2.0;
        double centerY = height / 2.0;
        double dx = (x - centerX) / centerX;
        double dy = (y - centerY) / centerY;
        double distance = Math.Sqrt(dx * dx + dy * dy);
        
        double vignette = 1.0 - Math.Pow(distance * radius, 2) * (strength / 100.0);
        vignette = Math.Max(0, Math.Min(1, vignette));
        
        return ColorBgra.FromBgra(
            (byte)(pixel.B * vignette),
            (byte)(pixel.G * vignette),
            (byte)(pixel.R * vignette),
            pixel.A);
    }

    private ColorBgra ApplySepia(ColorBgra pixel, double intensity)
    {
        double factor = intensity / 100.0;
        
        int tr = (int)(pixel.R * 0.393 + pixel.G * 0.769 + pixel.B * 0.189);
        int tg = (int)(pixel.R * 0.349 + pixel.G * 0.686 + pixel.B * 0.168);
        int tb = (int)(pixel.R * 0.272 + pixel.G * 0.534 + pixel.B * 0.131);
        
        int r = (int)(pixel.R * (1 - factor) + tr * factor);
        int g = (int)(pixel.G * (1 - factor) + tg * factor);
        int b = (int)(pixel.B * (1 - factor) + tb * factor);
        
        return ColorBgra.FromBgra(
            (byte)Math.Clamp(b, 0, 255),
            (byte)Math.Clamp(g, 0, 255),
            (byte)Math.Clamp(r, 0, 255),
            pixel.A);
    }

    private ColorBgra ApplyGrayscale(ColorBgra pixel, double method)
    {
        byte gray = (int)method switch
        {
            0 => (byte)((pixel.R + pixel.G + pixel.B) / 3), // Average
            1 => (byte)(pixel.R * 0.299 + pixel.G * 0.587 + pixel.B * 0.114), // Luminosity
            2 => (byte)((Math.Max(pixel.R, Math.Max(pixel.G, pixel.B)) + Math.Min(pixel.R, Math.Min(pixel.G, pixel.B))) / 2), // Desaturation
            _ => pixel.R
        };
        return ColorBgra.FromBgra(gray, gray, gray, pixel.A);
    }

    private ColorBgra ApplyInvert(ColorBgra pixel, double intensity)
    {
        double factor = intensity / 100.0;
        int r = (int)(pixel.R * (1 - factor) + (255 - pixel.R) * factor);
        int g = (int)(pixel.G * (1 - factor) + (255 - pixel.G) * factor);
        int b = (int)(pixel.B * (1 - factor) + (255 - pixel.B) * factor);
        
        return ColorBgra.FromBgra((byte)b, (byte)g, (byte)r, pixel.A);
    }

    private ColorBgra ApplyTemperature(ColorBgra pixel, double temp, double tint)
    {
        double tempFactor = temp / 100.0;
        double tintFactor = tint / 100.0;
        
        int r = (int)Math.Clamp(pixel.R + tempFactor * 50, 0, 255);
        int g = (int)Math.Clamp(pixel.G + tintFactor * 50, 0, 255);
        int b = (int)Math.Clamp(pixel.B - tempFactor * 50, 0, 255);
        
        return ColorBgra.FromBgra((byte)b, (byte)g, (byte)r, pixel.A);
    }

    private ColorBgra ApplyChannelMixer(ColorBgra pixel, double redBalance, double greenBalance, double blueBalance)
    {
        double rFactor = 1.0 + redBalance / 100.0;
        double gFactor = 1.0 + greenBalance / 100.0;
        double bFactor = 1.0 + blueBalance / 100.0;
        
        return ColorBgra.FromBgra(
            (byte)Math.Clamp(pixel.B * bFactor, 0, 255),
            (byte)Math.Clamp(pixel.G * gFactor, 0, 255),
            (byte)Math.Clamp(pixel.R * rFactor, 0, 255),
            pixel.A);
    }

    private ColorBgra ApplyMotionBlur(ColorBgra pixel, int x, int y, double angle, double distance)
    {
        if (SrcArgs.Surface == null) return pixel;
        
        double angleRad = angle * Math.PI / 180.0;
        double dx = Math.Cos(angleRad);
        double dy = Math.Sin(angleRad);
        int steps = Math.Max(1, (int)distance);
        
        int r = 0, g = 0, b = 0, count = 0;
        for (int i = 0; i <= steps; i++)
        {
            int sx = (int)(x + dx * i * distance / steps);
            int sy = (int)(y + dy * i * distance / steps);
            
            if (sx >= 0 && sx < SrcArgs.Surface.Width && sy >= 0 && sy < SrcArgs.Surface.Height)
            {
                var p = SrcArgs.Surface[sx, sy];
                r += p.R;
                g += p.G;
                b += p.B;
                count++;
            }
        }
        
        if (count == 0) return pixel;
        return ColorBgra.FromBgra((byte)(b / count), (byte)(g / count), (byte)(r / count), pixel.A);
    }

    private ColorBgra ApplyEdgeDetect(ColorBgra pixel, int x, int y, double threshold)
    {
        if (SrcArgs.Surface == null) return pixel;
        
        int width = SrcArgs.Surface.Width;
        int height = SrcArgs.Surface.Height;
        
        // Sobel operator
        int gx = 0, gy = 0;
        for (int dy = -1; dy <= 1; dy++)
        {
            for (int dx = -1; dx <= 1; dx++)
            {
                int sx = Math.Clamp(x + dx, 0, width - 1);
                int sy = Math.Clamp(y + dy, 0, height - 1);
                var p = SrcArgs.Surface[sx, sy];
                int intensity = (p.R + p.G + p.B) / 3;
                
                // Sobel kernels
                int kx = dx * (dy == 0 ? 2 : 1);
                int ky = dy * (dx == 0 ? 2 : 1);
                gx += intensity * kx;
                gy += intensity * ky;
            }
        }
        
        int magnitude = (int)Math.Sqrt(gx * gx + gy * gy);
        byte value = (byte)(magnitude > threshold ? 255 : 0);
        return ColorBgra.FromBgra(value, value, value, pixel.A);
    }

    private ColorBgra ApplyUnsharpMask(ColorBgra pixel, int x, int y, double amount, double radius, double threshold)
    {
        if (SrcArgs.Surface == null) return pixel;
        
        // Simple unsharp mask: original + (original - blurred) * amount
        var blurred = ApplySimpleBlur(x, y, (int)radius);
        
        int diffR = pixel.R - blurred.R;
        int diffG = pixel.G - blurred.G;
        int diffB = pixel.B - blurred.B;
        
        // Apply threshold
        if (Math.Abs(diffR) < threshold) diffR = 0;
        if (Math.Abs(diffG) < threshold) diffG = 0;
        if (Math.Abs(diffB) < threshold) diffB = 0;
        
        double factor = amount / 100.0;
        return ColorBgra.FromBgra(
            (byte)Math.Clamp(pixel.B + diffB * factor, 0, 255),
            (byte)Math.Clamp(pixel.G + diffG * factor, 0, 255),
            (byte)Math.Clamp(pixel.R + diffR * factor, 0, 255),
            pixel.A);
    }

    private ColorBgra ApplySimpleBlur(int x, int y, int radius)
    {
        int width = SrcArgs.Surface.Width;
        int height = SrcArgs.Surface.Height;
        int r = 0, g = 0, b = 0, count = 0;
        
        for (int dy = -radius; dy <= radius; dy++)
        {
            for (int dx = -radius; dx <= radius; dx++)
            {
                int sx = Math.Clamp(x + dx, 0, width - 1);
                int sy = Math.Clamp(y + dy, 0, height - 1);
                var p = SrcArgs.Surface[sx, sy];
                r += p.R;
                g += p.G;
                b += p.B;
                count++;
            }
        }
        
        return ColorBgra.FromBgra((byte)(b / count), (byte)(g / count), (byte)(r / count), 255);
    }

    private ColorBgra ApplyPosterize(ColorBgra pixel, double levels)
    {
        int numLevels = Math.Max(2, (int)levels);
        double step = 255.0 / (numLevels - 1);
        
        byte r = (byte)(Math.Round(pixel.R / step) * step);
        byte g = (byte)(Math.Round(pixel.G / step) * step);
        byte b = (byte)(Math.Round(pixel.B / step) * step);
        
        return ColorBgra.FromBgra(b, g, r, pixel.A);
    }

    private ColorBgra ApplyPixelate(ColorBgra pixel, int x, int y, double blockSize)
    {
        if (SrcArgs.Surface == null) return pixel;
        
        int size = Math.Max(2, (int)blockSize);
        int blockX = (x / size) * size;
        int blockY = (y / size) * size;
        
        // Average color in block
        int r = 0, g = 0, b = 0, count = 0;
        for (int dy = 0; dy < size; dy++)
        {
            for (int dx = 0; dx < size; dx++)
            {
                int sx = Math.Min(blockX + dx, SrcArgs.Surface.Width - 1);
                int sy = Math.Min(blockY + dy, SrcArgs.Surface.Height - 1);
                var p = SrcArgs.Surface[sx, sy];
                r += p.R;
                g += p.G;
                b += p.B;
                count++;
            }
        }
        
        return ColorBgra.FromBgra((byte)(b / count), (byte)(g / count), (byte)(r / count), pixel.A);
    }

    private ColorBgra ApplyEmboss(ColorBgra pixel, int x, int y, double angle, double depth)
    {
        if (SrcArgs.Surface == null) return pixel;
        
        int width = SrcArgs.Surface.Width;
        int height = SrcArgs.Surface.Height;
        
        double angleRad = angle * Math.PI / 180.0;
        int dx = (int)Math.Round(Math.Cos(angleRad));
        int dy = (int)Math.Round(Math.Sin(angleRad));
        
        int x1 = Math.Clamp(x + dx, 0, width - 1);
        int y1 = Math.Clamp(y + dy, 0, height - 1);
        int x2 = Math.Clamp(x - dx, 0, width - 1);
        int y2 = Math.Clamp(y - dy, 0, height - 1);
        
        var p1 = SrcArgs.Surface[x1, y1];
        var p2 = SrcArgs.Surface[x2, y2];
        
        int diff = ((p1.R + p1.G + p1.B) - (p2.R + p2.G + p2.B)) / 3;
        int value = (int)Math.Clamp(128 + diff * depth / 100.0, 0, 255);
        byte gray = (byte)value;
        
        return ColorBgra.FromBgra(gray, gray, gray, pixel.A);
    }

    private ColorBgra ApplyOilPainting(ColorBgra pixel, int x, int y, double brushSize, double intensity)
    {
        if (SrcArgs.Surface == null) return pixel;
        
        int size = Math.Max(1, (int)brushSize);
        int width = SrcArgs.Surface.Width;
        int height = SrcArgs.Surface.Height;
        
        // Find most frequent intensity level
        int[] intensityCount = new int[256];
        int[] sumR = new int[256];
        int[] sumG = new int[256];
        int[] sumB = new int[256];
        
        for (int dy = -size; dy <= size; dy++)
        {
            for (int dx = -size; dx <= size; dx++)
            {
                int sx = Math.Clamp(x + dx, 0, width - 1);
                int sy = Math.Clamp(y + dy, 0, height - 1);
                var p = SrcArgs.Surface[sx, sy];
                
                int intensityLevel = (p.R + p.G + p.B) / 3;
                intensityCount[intensityLevel]++;
                sumR[intensityLevel] += p.R;
                sumG[intensityLevel] += p.G;
                sumB[intensityLevel] += p.B;
            }
        }
        
        int maxCount = 0, maxIndex = 0;
        for (int i = 0; i < 256; i++)
        {
            if (intensityCount[i] > maxCount)
            {
                maxCount = intensityCount[i];
                maxIndex = i;
            }
        }
        
        if (maxCount == 0) return pixel;
        
        double factor = intensity / 100.0;
        int avgR = sumR[maxIndex] / maxCount;
        int avgG = sumG[maxIndex] / maxCount;
        int avgB = sumB[maxIndex] / maxCount;
        
        return ColorBgra.FromBgra(
            (byte)Math.Clamp(pixel.B * (1 - factor) + avgB * factor, 0, 255),
            (byte)Math.Clamp(pixel.G * (1 - factor) + avgG * factor, 0, 255),
            (byte)Math.Clamp(pixel.R * (1 - factor) + avgR * factor, 0, 255),
            pixel.A);
    }

    private ColorBgra ApplyExposure(ColorBgra pixel, double ev)
    {
        double exposure = Math.Pow(2, ev / 100.0);
        
        return ColorBgra.FromBgra(
            (byte)Math.Clamp(pixel.B * exposure, 0, 255),
            (byte)Math.Clamp(pixel.G * exposure, 0, 255),
            (byte)Math.Clamp(pixel.R * exposure, 0, 255),
            pixel.A);
    }

    private ColorBgra ApplyShadowHighlight(ColorBgra pixel, double shadows, double highlights)
    {
        int luminance = (pixel.R + pixel.G + pixel.B) / 3;
        double shadowFactor = (128 - luminance) / 128.0;
        double highlightFactor = (luminance - 128) / 128.0;
        
        shadowFactor = Math.Clamp(shadowFactor, 0, 1) * shadows / 100.0;
        highlightFactor = Math.Clamp(highlightFactor, 0, 1) * highlights / 100.0;
        
        int adjustment = (int)((shadowFactor - highlightFactor) * 50);
        
        return ColorBgra.FromBgra(
            (byte)Math.Clamp(pixel.B + adjustment, 0, 255),
            (byte)Math.Clamp(pixel.G + adjustment, 0, 255),
            (byte)Math.Clamp(pixel.R + adjustment, 0, 255),
            pixel.A);
    }

    private ColorBgra ApplyGamma(ColorBgra pixel, double gammaValue)
    {
        double gamma = gammaValue / 100.0;
        
        return ColorBgra.FromBgra(
            (byte)(Math.Pow(pixel.B / 255.0, gamma) * 255),
            (byte)(Math.Pow(pixel.G / 255.0, gamma) * 255),
            (byte)(Math.Pow(pixel.R / 255.0, gamma) * 255),
            pixel.A);
    }

    private ColorBgra ApplyLevels(ColorBgra pixel, double blackPoint, double whitePoint, double midPoint)
    {
        double gamma = midPoint / 100.0;
        
        double r = Math.Clamp((pixel.R - blackPoint) / (whitePoint - blackPoint), 0, 1);
        double g = Math.Clamp((pixel.G - blackPoint) / (whitePoint - blackPoint), 0, 1);
        double b = Math.Clamp((pixel.B - blackPoint) / (whitePoint - blackPoint), 0, 1);
        
        r = Math.Pow(r, gamma);
        g = Math.Pow(g, gamma);
        b = Math.Pow(b, gamma);
        
        return ColorBgra.FromBgra(
            (byte)(b * 255),
            (byte)(g * 255),
            (byte)(r * 255),
            pixel.A);
    }

    private ColorBgra ApplyGlow(ColorBgra pixel, int x, int y, double intensity, double radius)
    {
        if (SrcArgs.Surface == null) return pixel;
        
        // Apply blur to bright areas
        int luminance = (pixel.R + pixel.G + pixel.B) / 3;
        if (luminance < 128) return pixel;
        
        var blurred = ApplySimpleBlur(x, y, (int)radius);
        double factor = (luminance / 255.0) * (intensity / 100.0);
        
        return ColorBgra.FromBgra(
            (byte)Math.Clamp(pixel.B + blurred.B * factor, 0, 255),
            (byte)Math.Clamp(pixel.G + blurred.G * factor, 0, 255),
            (byte)Math.Clamp(pixel.R + blurred.R * factor, 0, 255),
            pixel.A);
    }

    private Random _random = new Random();
    
    private ColorBgra ApplyNoise(ColorBgra pixel, double amount, double noiseType)
    {
        int type = (int)noiseType;
        double factor = amount / 100.0;
        
        int noise = type switch
        {
            0 => (int)(((_random.NextDouble() - 0.5) * 2) * factor * 128), // Gaussian
            1 => (int)((_random.NextDouble() - 0.5) * factor * 255), // Uniform
            2 => _random.NextDouble() < factor ? (_random.Next(2) == 0 ? -255 : 255) : 0, // Salt & Pepper
            _ => 0
        };
        
        return ColorBgra.FromBgra(
            (byte)Math.Clamp(pixel.B + noise, 0, 255),
            (byte)Math.Clamp(pixel.G + noise, 0, 255),
            (byte)Math.Clamp(pixel.R + noise, 0, 255),
            pixel.A);
    }

    private ColorBgra ApplyChromaticAberration(ColorBgra pixel, int x, int y, double strength)
    {
        if (SrcArgs.Surface == null) return pixel;
        
        int offset = (int)strength;
        int width = SrcArgs.Surface.Width;
        int height = SrcArgs.Surface.Height;
        
        int xR = Math.Clamp(x + offset, 0, width - 1);
        int xB = Math.Clamp(x - offset, 0, width - 1);
        
        var pR = SrcArgs.Surface[xR, y];
        var pB = SrcArgs.Surface[xB, y];
        
        return ColorBgra.FromBgra(pB.B, pixel.G, pR.R, pixel.A);
    }

    private ColorBgra ApplyLensDistortion(ColorBgra pixel, int x, int y, int width, int height, double amount)
    {
        if (SrcArgs.Surface == null) return pixel;
        
        double centerX = width / 2.0;
        double centerY = height / 2.0;
        double maxRadius = Math.Sqrt(centerX * centerX + centerY * centerY);
        
        double dx = x - centerX;
        double dy = y - centerY;
        double distance = Math.Sqrt(dx * dx + dy * dy);
        double factor = 1.0 + (amount / 100.0) * (distance / maxRadius);
        
        int sx = (int)(centerX + dx / factor);
        int sy = (int)(centerY + dy / factor);
        
        if (sx >= 0 && sx < width && sy >= 0 && sy < height)
            return SrcArgs.Surface[sx, sy];
        
        return pixel;
    }
}

