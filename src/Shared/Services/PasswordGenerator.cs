using System.Text;

namespace Sphera.API.Shared.Services;

public static class PasswordGenerator
{
    private const string Upper = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
    private const string Lower = "abcdefghijklmnopqrstuvwxyz";
    private const string Digits = "0123456789";
    private const string Special = "!@#$%^&*()-_=+[]{}|;:,.<>?";

    public static string Generate(int length = 8)
    {
        if (length < 8)
            throw new ArgumentException("Minimum length is 8.");

        var rand = new Random();
        var password = new StringBuilder();
        password.Append(Upper[rand.Next(Upper.Length)]);
        password.Append(Lower[rand.Next(Lower.Length)]);
        password.Append(Digits[rand.Next(Digits.Length)]);
        password.Append(Special[rand.Next(Special.Length)]);

        var allChars = Upper + Lower + Digits + Special;
        for (int i = password.Length; i < length; i++)
            password.Append(allChars[rand.Next(allChars.Length)]);
        
        return new string(password.ToString().OrderBy(_ => rand.Next()).ToArray());
    }
}