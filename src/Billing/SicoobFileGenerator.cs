using System.Globalization;
using System.Text;
using Sphera.API.Billing.BilletConfigurations;
using Sphera.API.Billing.Billets;

namespace Sphera.API.Billing;

public class SicoobFileGenerator
{
    public static Stream GenerateRemmitanceFile(List<Billet> billets)
    {
        string data = "";
        
        var stream = new MemoryStream();
        var writer = new StreamWriter(stream);
        writer.Write(data);
        writer.Flush();
        stream.Position = 0;

        return stream;
    }
    
    public static string Text(object input, int length, char padding = ' ')
    {
        var strInput = input?.ToString() ?? string.Empty;
        if (strInput.Length > length)
            return strInput.Substring(0, length);
        return strInput.PadLeft(length, padding);
    }
}