using VMS.TPS.Common.Model.API;
using System.Reflection;
using System.Runtime.InteropServices;

// Tämä on tärkein rivi täällä
[assembly: ESAPIScript(IsWriteable = false)]

// Muut tiedot voivat olla oletusarvoisia
[assembly: AssemblyTitle("EQD2_DVH")]
[assembly: AssemblyProduct("EQD2_DVH")]
[assembly: ComVisible(false)]
[assembly: Guid("814C8D26-0C88-44ED-86AE-9518726C034A")] // Voit luoda uuden GUID:n (Tools -> Create GUID)
[assembly: AssemblyVersion("1.0.0.0")]
[assembly: AssemblyFileVersion("1.0.0.0")]