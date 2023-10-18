using System;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;

namespace RawInputReader.Utilities
{
    public static class AssemblyUtilities
    {
        public static string? GetCompany() => GetCompany(null);
        public static string? GetCompany(this Assembly? assembly) => (assembly ?? Assembly.GetEntryAssembly())?.GetCustomAttribute<AssemblyCompanyAttribute>()?.Company;

        public static string? GetDescription() => GetDescription(null);
        public static string? GetDescription(this Assembly? assembly) => (assembly ?? Assembly.GetEntryAssembly())?.GetCustomAttribute<AssemblyDescriptionAttribute>()?.Description;

        public static string? GetProduct() => GetProduct(null);
        public static string? GetProduct(this Assembly? assembly) => (assembly ?? Assembly.GetEntryAssembly())?.GetCustomAttribute<AssemblyProductAttribute>()?.Product;

        public static string? GetConfiguration() => GetConfiguration(null);
        public static string? GetConfiguration(this Assembly? assembly) => (assembly ?? Assembly.GetEntryAssembly())?.GetCustomAttribute<AssemblyConfigurationAttribute>()?.Configuration;

        public static string? GetTitle() => GetTitle(null);
        public static string? GetTitle(this Assembly? assembly) => (assembly ?? Assembly.GetEntryAssembly())?.GetCustomAttribute<AssemblyTitleAttribute>()?.Title;

        public static string? GetInformationalVersion() => GetInformationalVersion(null);
        public static string? GetInformationalVersion(this Assembly? assembly) => (assembly ?? Assembly.GetEntryAssembly())?.GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion;

        public static string? GetFileVersion() => GetFileVersion(null);
        public static string? GetFileVersion(this Assembly? assembly) => (assembly ?? Assembly.GetEntryAssembly())?.GetCustomAttribute<AssemblyFileVersionAttribute>()?.Version;

        public static string? GetCopyright() => GetCopyright(null);
        public static string? GetCopyright(this Assembly? assembly) => (assembly ?? Assembly.GetEntryAssembly())?.GetCustomAttribute<AssemblyCopyrightAttribute>()?.Copyright;

        public static Guid GetGuid() => GetGuid(null);
        public static Guid GetGuid(this Assembly? assembly)
        {
            // note we use executing assembly here
            var text = (assembly ?? Assembly.GetExecutingAssembly())?.GetCustomAttribute<GuidAttribute>()?.Value;
            return text != null ? new Guid(text) : Guid.Empty;
        }

        public static string? GetMetatadaAttribute(string key) => GetMetatadaAttribute(null, key);
        public static string? GetMetatadaAttribute(this Assembly? assembly, string key)
        {
            ArgumentNullException.ThrowIfNull(key);
            return (assembly ?? Assembly.GetEntryAssembly())?.GetCustomAttributes<AssemblyMetadataAttribute>()?.FirstOrDefault(a => a.Key.EqualsIgnoreCase(key))?.Value;
        }
    }
}
