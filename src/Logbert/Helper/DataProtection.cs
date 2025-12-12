using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using System.Security.Cryptography;
using System.Text;

namespace Logbert.Helper
{
  /// <summary>
  /// Class to protect data like credentials.
  /// Uses DPAPI on Windows, AES encryption on other platforms.
  /// </summary>
  public sealed class DataProtection
  {
    #region Private Fields

    /// <summary>
    /// Static entropy used for additional encryption strength.
    /// </summary>
    private static readonly byte[] Entropy = Encoding.Unicode.GetBytes("{472242CB-00B0-47F0-B2FA-72591E9419E0}");

    /// <summary>
    /// Machine-specific key derivation salt for non-Windows platforms.
    /// </summary>
    private static readonly byte[] Salt = Encoding.UTF8.GetBytes("Logbert.DataProtection.Salt.v1");

    #endregion

    #region Private Methods

    /// <summary>
    /// Gets a byte array with additional entropy to improve the strength of the encryption algorithm.
    /// </summary>
    private static byte[] GetAditionalEntropy()
    {
      return Entropy;
    }

    /// <summary>
    /// Encrypts the data using DataProtectionScope.CurrentUser (Windows only).
    /// </summary>
    [SupportedOSPlatform("windows")]
    private static byte[] ProtectWindows(byte[] data)
    {
      return ProtectedData.Protect(data, GetAditionalEntropy(), DataProtectionScope.CurrentUser);
    }

    /// <summary>
    /// Decrypts the data using DataProtectionScope.CurrentUser (Windows only).
    /// </summary>
    [SupportedOSPlatform("windows")]
    private static byte[] UnprotectWindows(byte[] data)
    {
      return ProtectedData.Unprotect(data, GetAditionalEntropy(), DataProtectionScope.CurrentUser);
    }

    /// <summary>
    /// Derives an encryption key from user-specific data for cross-platform encryption.
    /// </summary>
    private static byte[] DeriveKey()
    {
      // Use environment-specific data to derive a user-specific key
      string userKey = Environment.UserName + Environment.MachineName + Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
      byte[] passwordBytes = Encoding.UTF8.GetBytes(userKey);
      return Rfc2898DeriveBytes.Pbkdf2(passwordBytes, Salt, 10000, HashAlgorithmName.SHA256, 32); // 256-bit key for AES
    }

    /// <summary>
    /// Encrypts data using AES for cross-platform support.
    /// </summary>
    private static byte[] ProtectCrossPlatform(byte[] data)
    {
      using var aes = Aes.Create();
      aes.Key = DeriveKey();
      aes.GenerateIV();

      using var encryptor = aes.CreateEncryptor();
      using var ms = new MemoryStream();

      // Write IV first
      ms.Write(aes.IV, 0, aes.IV.Length);

      using (var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
      {
        cs.Write(data, 0, data.Length);
        cs.FlushFinalBlock();
      }

      return ms.ToArray();
    }

    /// <summary>
    /// Decrypts data using AES for cross-platform support.
    /// </summary>
    private static byte[] UnprotectCrossPlatform(byte[] data)
    {
      using var aes = Aes.Create();
      aes.Key = DeriveKey();

      // Read IV from beginning of data
      byte[] iv = new byte[16];
      Array.Copy(data, 0, iv, 0, 16);
      aes.IV = iv;

      using var decryptor = aes.CreateDecryptor();
      using var ms = new MemoryStream(data, 16, data.Length - 16);
      using var cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read);
      using var resultStream = new MemoryStream();

      cs.CopyTo(resultStream);
      return resultStream.ToArray();
    }

    /// <summary>
    /// Encrypts the data using platform-appropriate method.
    /// </summary>
    private static byte[] Protect(byte[] data)
    {
      if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
      {
        return ProtectWindows(data);
      }
      return ProtectCrossPlatform(data);
    }

    /// <summary>
    /// Decrypts the data using platform-appropriate method.
    /// </summary>
    private static byte[] Unprotect(byte[] data)
    {
      if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
      {
        return UnprotectWindows(data);
      }
      return UnprotectCrossPlatform(data);
    }

    #endregion

    #region Public Methods

    /// <summary>
    /// Encrypts a string using platform-appropriate encryption.
    /// Uses DPAPI on Windows, AES on macOS/Linux.
    /// </summary>
    /// <param name="stringToEngrypt">The string to encrypt.</param>
    /// <returns>The encrypted string as Base64.</returns>
    public static string EncryptString(string stringToEngrypt)
    {
      if (string.IsNullOrEmpty(stringToEngrypt))
      {
        return string.Empty;
      }

      try
      {
        byte[] stringAsByteArray = Encoding.Unicode.GetBytes(stringToEngrypt);
        byte[] encodedData = Protect(stringAsByteArray);

        return Convert.ToBase64String(encodedData);
      }
      catch (Exception ex)
      {
        Logger.Error($"Encryption failed: {ex.Message}");
      }

      return string.Empty;
    }

    /// <summary>
    /// Decrypts a string using platform-appropriate decryption.
    /// Uses DPAPI on Windows, AES on macOS/Linux.
    /// </summary>
    /// <param name="stringToDegrypt">The Base64 encoded encrypted string to decrypt.</param>
    /// <returns>The decrypted string.</returns>
    public static string DecryptString(string stringToDegrypt)
    {
      if (string.IsNullOrEmpty(stringToDegrypt))
      {
        return string.Empty;
      }

      try
      {
        byte[] encodedData = Convert.FromBase64String(stringToDegrypt);
        byte[] decodedData = Unprotect(encodedData);

        return Encoding.Unicode.GetString(decodedData);
      }
      catch (Exception ex)
      {
        Logger.Error($"Decryption failed: {ex.Message}");
      }

      return string.Empty;
    }

    #endregion
  }
}
