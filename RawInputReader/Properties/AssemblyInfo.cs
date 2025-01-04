using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;

#if DEBUG
[assembly: AssemblyConfiguration("DEBUG")]
#else
[assembly: AssemblyConfiguration("RELEASE")]
#endif
[assembly: AssemblyTitle("Raw Input Reader")]
[assembly: AssemblyDescription("Raw Input Reader")]
[assembly: AssemblyCompany("Simon Mourier")]
[assembly: AssemblyProduct("Raw Input Reader")]
[assembly: AssemblyCopyright("Copyright (C) 2022-2025 Simon Mourier. All rights reserved.")]
[assembly: AssemblyCulture("")]
[assembly: ComVisible(false)]
[assembly: Guid("fbef5d65-8595-4d99-ab2d-7c7980a1de45")]
[assembly: SupportedOSPlatform("windows")]

[assembly: AssemblyVersion("1.0.0.0")]
[assembly: AssemblyFileVersion("1.0.0.0")]
