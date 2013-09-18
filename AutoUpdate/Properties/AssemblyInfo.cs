using System.Reflection;
using System.Runtime.InteropServices;

// General Information about an assembly is controlled through the following 
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.
[assembly: AssemblyTitle("AutoUpdate")]
[assembly: AssemblyDescription("A package that helps to build self-updating web applications using NuGet.")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("Phil Haack")]
[assembly: AssemblyProduct("AutoUpdate")]
[assembly: AssemblyCopyright("Copyright © Phil Haack 2013")]
[assembly: AssemblyCulture("")]

// Setting ComVisible to false makes the types in this assembly not visible 
// to COM components.  If you need to access a type in this assembly from 
// COM, set the ComVisible attribute to true on that type.
[assembly: ComVisible(false)]

// The following GUID is for the ID of the typelib if this project is exposed to COM
[assembly: Guid("9976df45-898a-4936-bdc2-262ea8ac5b54")]

[assembly: AssemblyVersion(AssemblyConstants.AssemblyVersion)]
[assembly: AssemblyFileVersion(AssemblyConstants.AssemblyVersion)]
[assembly: AssemblyInformationalVersion(AssemblyConstants.PackageVersion)]

internal static class AssemblyConstants
{
    internal const string AssemblyVersion = PackageVersion + ".0";
    internal const string PackageVersion = "0.0.3";
    internal const string PrereleaseVersion = ""; // Until we ship 1.0, this isn't necessary.
}