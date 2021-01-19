using Core.Miscellaneous;
using System;
using System.Security.Cryptography;
using System.Text;

namespace Miscellaneous
{
    public class Cryptography: ICryptography
    {
        public string GetHash(string input)
        {
            string result = string.Empty;

            using (SHA256 sha256Hash = SHA256.Create())
            {
                result = GetHash(sha256Hash, input);
            }

            return result;
        }

        public string GetHash(HashAlgorithm hashAlgorithm, string input)
        {
            // Convert the input string to a byte array and compute the hash.
            byte[] data = hashAlgorithm.ComputeHash(Encoding.UTF8.GetBytes(input));

            // Create a new Stringbuilder to collect the bytes
            // and create a string.
            var sBuilder = new StringBuilder();

            // Loop through each byte of the hashed data
            // and format each one as a hexadecimal string.
            for (int i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("x2"));
            }

            // Return the hexadecimal string.
            return sBuilder.ToString();
        }

        public bool VerifyHash(string input, string hash)
        {
            bool result = false;

            using (SHA256 sha256Hash = SHA256.Create())
            {
                result = VerifyHash(sha256Hash, input, hash);
            }

            return result;
        }


        // Verify a hash against a string.
        public bool VerifyHash(HashAlgorithm hashAlgorithm, string input, string hash)
        {
            // Hash the input.
            var hashOfInput = GetHash(hashAlgorithm, input);

            // Create a StringComparer an compare the hashes.
            StringComparer comparer = StringComparer.OrdinalIgnoreCase;

            return comparer.Compare(hashOfInput, hash) == 0;
        }
    }
}
