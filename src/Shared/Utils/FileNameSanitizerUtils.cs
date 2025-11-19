using System.Text.RegularExpressions;

namespace Sphera.API.Shared.Utils;

public class FileNameSanitizerUtils
{
    public static string SanitizeName(string name, bool allowSlashes = false)
    {
        if (string.IsNullOrWhiteSpace(name))
            return string.Empty;
        
        var pattern = allowSlashes ? "[^a-zA-Z0-9-/.]" : "[^a-zA-Z0-9-.]";
        name = Regex.Replace(name, pattern, "_");
        
        if (!char.IsLetterOrDigit(name[0]))
            name = "_" + name;
        
        if (!char.IsLetterOrDigit(name[name.Length - 1]))
            name += "_";

        return name.ToLowerInvariant();
    }
}