using System.Security.Cryptography;

namespace Core.Miscellaneous
{
    public interface ICryptography
    {
        string GetHash(string input);
        string GetHash(HashAlgorithm hashAlgorithm, string input);
        bool VerifyHash(string input, string hash);
        bool VerifyHash(HashAlgorithm hashAlgorithm, string input, string hash);
    }
}