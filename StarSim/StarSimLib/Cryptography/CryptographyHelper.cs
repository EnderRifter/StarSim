using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace StarSimLib.Cryptography
{
    /// <summary>
    /// Provides helper methods for manipulating password salts and hashes.
    /// </summary>
    public static class CryptographyHelper
    {
        /// <summary>
        /// The cryptographically secure random number generator to use to generate cryptographically strong sequences
        /// of bytes.
        /// </summary>
        private static readonly RNGCryptoServiceProvider cryptoServiceProvider = new RNGCryptoServiceProvider();

        /// <summary>
        /// The number of iterations used when deriving the password hash from the salted password.
        /// </summary>
        public const int HashIterations = 10_000;

        /// <summary>
        /// The number of bytes of hash to return
        /// </summary>
        public const int HashLength = 2048;

        /// <summary>
        /// The number of bytes per password salt.
        /// </summary>
        public const int SaltByteCount = 2048;

        /// <summary>
        /// Returns the string representation of the given bytes.
        /// </summary>
        /// <param name="contents">The byte contents to convert.</param>
        /// <returns>The string that represents the given bytes.</returns>
        public static string BytesToString(byte[] contents)
        {
            StringBuilder contentsStringBuilder = new StringBuilder();

            foreach (byte contentByte in contents)
            {
                contentsStringBuilder.Append(Convert.ToChar(contentByte));
            }

            return contentsStringBuilder.ToString();
        }

        /// <summary>
        /// Returns the hash of the given salted password (password with salt prepended) with the given number of iterations.
        /// </summary>
        /// <param name="password">The password to hash.</param>
        /// <param name="salt">The salt to prepend to the password.</param>
        /// <param name="hashLength">The number of bytes of hash to return.</param>
        /// <param name="iterations">The number of iterations of hashing algorithm to perform.</param>
        /// <returns>The given number of bytes of hashed salted password.</returns>
        public static byte[] GenerateHash(byte[] password, byte[] salt, int hashLength = HashLength, int iterations = HashIterations)
        {
            using (Rfc2898DeriveBytes pbkdf = new Rfc2898DeriveBytes(password, salt, iterations))
            {
                return pbkdf.GetBytes(hashLength);
            }
        }

        /// <summary>
        /// Returns a cryptographically strong sequence of bytes of the given length to function as a password salt.
        /// </summary>
        /// <param name="length">The number of bytes that should make up the salt.</param>
        /// <returns>The generated salt.</returns>
        public static byte[] GenerateSalt(int length = SaltByteCount)
        {
            byte[] saltBytes = new byte[length];

            cryptoServiceProvider.GetBytes(saltBytes);

            return saltBytes;
        }

        /// <summary>
        /// Determines whether the two hashes given are equal.
        /// </summary>
        /// <param name="a">The first hash to compare.</param>
        /// <param name="b">The second hash to compare.</param>
        /// <returns>Whether the two hashes are equal.</returns>
        public static bool HashesEqual(byte[] a, byte[] b)
        {
            if (a.Length != b.Length)
            {
                return false;
            }

            for (int i = 0; i < a.Length; i++)
            {
                // bytewise XOR will return a 0 if, and only if, both bytes are the same. if they are not, the hashes
                // are not identical
                if ((a[i] ^ b[i]) != 0)
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Returns the bytes of the given string.
        /// </summary>
        /// <param name="contents">The string contents to convert.</param>
        /// <returns>The bytes that make up the given string.</returns>
        public static byte[] StringToBytes(string contents)
        {
            return contents.Select(Convert.ToByte).ToArray();
        }
    }
}