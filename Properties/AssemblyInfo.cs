using System.Reflection;
using MelonLoader;

[assembly: AssemblyTitle(CVRBilibiliVideoResolver.BuildInfo.Description)]
[assembly: AssemblyDescription(CVRBilibiliVideoResolver.BuildInfo.Description)]
[assembly: AssemblyCompany(CVRBilibiliVideoResolver.BuildInfo.Company)]
[assembly: AssemblyProduct(CVRBilibiliVideoResolver.BuildInfo.Name)]
[assembly: AssemblyCopyright("Created by " + CVRBilibiliVideoResolver.BuildInfo.Author)]
[assembly: AssemblyTrademark(CVRBilibiliVideoResolver.BuildInfo.Company)]
[assembly: AssemblyVersion(CVRBilibiliVideoResolver.BuildInfo.Version)]
[assembly: AssemblyFileVersion(CVRBilibiliVideoResolver.BuildInfo.Version)]
[assembly: MelonInfo(typeof(CVRBilibiliVideoResolver.CVRBilibiliVideoResolver), CVRBilibiliVideoResolver.BuildInfo.Name, CVRBilibiliVideoResolver.BuildInfo.Version, CVRBilibiliVideoResolver.BuildInfo.Author, CVRBilibiliVideoResolver.BuildInfo.DownloadLink)]
[assembly: MelonColor()]

// Create and Setup a MelonGame Attribute to mark a Melon as Universal or Compatible with specific Games.
// If no MelonGame Attribute is found or any of the Values for any MelonGame Attribute on the Melon is null or empty it will be assumed the Melon is Universal.
// Values for MelonGame Attribute can be found in the Game's app.info file or printed at the top of every log directly beneath the Unity version.
[assembly: MelonGame(null, null)]