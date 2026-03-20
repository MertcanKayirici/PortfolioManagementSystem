using System;
using System.Security.Cryptography;

/// <summary>
/// Provides helper methods for securely hashing and verifying passwords
/// using PBKDF2 with a random salt.
/// </summary>
public static class PasswordHelper
{
    /// <summary>
    /// Creates a salted PBKDF2 hash for the given plain-text password.
    /// The returned value stores both the salt and the hash in a single Base64 string.
    /// </summary>
    /// <param name="password">The plain-text password to hash.</param>
    /// <returns>A Base64-encoded string containing the salt and derived hash.</returns>
    public static string HashPassword(string password)
    {
        // Generate a cryptographically secure random salt
        using (var rng = new RNGCryptoServiceProvider())
        {
            byte[] salt = new byte[16];
            rng.GetBytes(salt);

            // Derive a hash from the password using PBKDF2
            using (var pbkdf2 = new Rfc2898DeriveBytes(password, salt, 10000))
            {
                byte[] hash = pbkdf2.GetBytes(20);

                // Combine salt + hash into a single byte array
                byte[] hashBytes = new byte[36];
                Array.Copy(salt, 0, hashBytes, 0, 16);
                Array.Copy(hash, 0, hashBytes, 16, 20);

                // Return the combined value as a Base64 string
                return Convert.ToBase64String(hashBytes);
            }
        }
    }

    /// <summary>
    /// Verifies whether a plain-text password matches a previously stored hash.
    /// </summary>
    /// <param name="password">The plain-text password to verify.</param>
    /// <param name="storedHash">The stored Base64-encoded salt+hash value.</param>
    /// <returns>True if the password matches; otherwise, false.</returns>
    public static bool VerifyPassword(string password, string storedHash)
    {
        try
        {
            // Return false if input values are missing
            if (string.IsNullOrWhiteSpace(password) || string.IsNullOrWhiteSpace(storedHash))
                return false;

            // Decode the stored Base64 string
            byte[] hashBytes = Convert.FromBase64String(storedHash);

            // Validate expected byte length (16-byte salt + 20-byte hash)
            if (hashBytes.Length != 36)
                return false;

            // Extract the original salt from the stored hash
            byte[] salt = new byte[16];
            Array.Copy(hashBytes, 0, salt, 0, 16);

            // Recompute the hash using the provided password and extracted salt
            using (var pbkdf2 = new Rfc2898DeriveBytes(password, salt, 10000))
            {
                byte[] hash = pbkdf2.GetBytes(20);

                // Compare each byte of the stored hash and computed hash
                for (int i = 0; i < 20; i++)
                {
                    if (hashBytes[i + 16] != hash[i])
                        return false;
                }
            }

            // Password is valid if all bytes match
            return true;
        }
        catch
        {
            // Return false if the stored hash is invalid or any error occurs
            return false;
        }
    }
}