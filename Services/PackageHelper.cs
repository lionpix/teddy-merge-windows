using System;
using System.Reflection;
using Windows.ApplicationModel;

namespace TeddyMerge.Services;

public static class PackageHelper
{
    public static bool IsPackaged
    {
        get
        {
            try
            {
                return Package.Current.Id != null;
            }
            catch (InvalidOperationException)
            {
                return false;
            }
        }
    }

    public static string GetAppVersion()
    {
        if (IsPackaged)
        {
            var version = Package.Current.Id.Version;
            return $"{version.Major}.{version.Minor}.{version.Build}.{version.Revision}";
        }
        else
        {
            var version = Assembly.GetExecutingAssembly().GetName().Version;
            return version?.ToString() ?? "1.0.0.0";
        }
    }
}
