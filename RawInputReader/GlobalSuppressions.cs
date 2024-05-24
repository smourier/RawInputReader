using System.Diagnostics.CodeAnalysis;

[assembly: SuppressMessage("Style", "IDE0066:Convert switch statement to expression", Justification = "Don't like it")]
[assembly: SuppressMessage("Interoperability", "SYSLIB1054:Use 'LibraryImportAttribute' instead of 'DllImportAttribute' to generate P/Invoke marshalling code at compile time", Justification = "Don't want to go unsafe")]
[assembly: SuppressMessage("Interoperability", "CA1401:P/Invokes should not be visible", Justification = "Dumb")]
[assembly: SuppressMessage("Design", "CA1069:Enums values should not be duplicated", Justification = "So what")]
