using System.Reflection;
using System.Runtime.InteropServices;
using Vintagestory.API.Common;

// Assembly (DLL) stuff
[assembly: AssemblyTitle("SVGPoc")]
[assembly: AssemblyDescription("game needs dragons")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("SeReptilians")]
[assembly: AssemblyProduct("SVGPoc")]
[assembly: AssemblyCopyright("Nexrem, Lyrthras - 2021")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]

[assembly: AssemblyVersion("1.0.0.0")]
[assembly: AssemblyFileVersion("1.0.0.0")]

[assembly: ComVisible(false)]

// Mod information stuff
[assembly: ModDependency("game")]
[assembly: ModInfo("SVGPoc", "svgpoc0aaaaaaaaa", 
    Version = "1.5.0", 
    Authors = new[] { "Nexrem", "Lyrthras" },
    Website = "", 
    Description = "SVG PoC for VintageStory", 
    RequiredOnClient = true )]
