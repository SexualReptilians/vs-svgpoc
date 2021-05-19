using System.Reflection;
using System.Runtime.InteropServices;
using Vintagestory.API.Common;

// Assembly (DLL) stuff
[assembly: AssemblyTitle("SVGPoc")]
[assembly: AssemblyDescription("")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("SexualReptilians")]
[assembly: AssemblyProduct("SVGPoc")]
[assembly: AssemblyCopyright("Nexrem, Lyrthras - 2021")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]

[assembly: AssemblyVersion("1.0.0")]
[assembly: AssemblyFileVersion("1.0.0")]

[assembly: ComVisible(false)]

// Mod information stuff
[assembly: ModDependency("game")]
[assembly: ModInfo("SVGPoc", "svgpoc",
    Version = "1.0.0",
    Authors = new[] { "Nexrem", "Lyrthras" },
    Website = "",
    Description = "SVG rendering PoC for VintageStory",
    RequiredOnClient = true )]
