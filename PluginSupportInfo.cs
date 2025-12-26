using System;
using System.Reflection;
using PaintDotNet;

namespace Megafilter;

public class PluginSupportInfo : IPluginSupportInfo
{
    public string DisplayName => "Megafilter";
    
    public string Author => "Your Name";
    
    public string Copyright => $"Copyright © {DateTime.Now.Year}";
    
    public Version Version => Assembly.GetExecutingAssembly().GetName().Version ?? new Version(1, 0, 0, 0);
    
    public Uri? WebsiteUri => null;
}
