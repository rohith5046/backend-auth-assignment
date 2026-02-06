using System.Security.Cryptography;
using System.Text;

namespace BackendAuthAssignment.Helper;

public static class Crypto
{
    public static string Sha256(string input)
    {
        using var sha = SHA256.Create();
        var bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(input));
        return Convert.ToHexString(bytes);
    }

    public static string RandomToken(int bytes = 32)
    {
        var data = RandomNumberGenerator.GetBytes(bytes);
        return Convert.ToBase64String(data);
    }
}
