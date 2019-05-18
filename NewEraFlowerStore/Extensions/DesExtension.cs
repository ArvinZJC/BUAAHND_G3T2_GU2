// csharp file that performs as a DES encryptor/decryptor

#region Using Directives
using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
#endregion Using Directives

namespace NewEraFlowerStore.Extensions
{
    /// <summary>
    /// The class <see cref="DesExtension"/> encrypts/decrypts a string value with DES.
    /// </summary>
    public static class DesExtension
    {
        private const string encryptionKey = "LOVEROSE";
        private static readonly Encoding defaultEncoding = Encoding.UTF8;

        /// <summary>
        /// Encrypt a string value with DES, as an asynchronous operation.
        /// </summary>
        /// <param name="value">a specified string value</param>
        /// <returns>an encrypted string value</returns>
        public static async Task<string> EncryptAsync(this string value)
        {
            if (string.IsNullOrEmpty(value))
                return string.Empty;

            using (var provider = new DESCryptoServiceProvider())
            {
                provider.IV = defaultEncoding.GetBytes(encryptionKey); // the initialisation vector
                provider.Key = defaultEncoding.GetBytes(encryptionKey); // the secret key

                using (var encryptor = provider.CreateEncryptor(provider.IV, provider.Key))
                {
                    var temp = defaultEncoding.GetBytes(value);

                    using (var memoryStream = new MemoryStream())
                    {
                        // encrypt the value
                        using (var cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write))
                        {
                            await cryptoStream.WriteAsync(temp, 0, temp.Length);
                            await cryptoStream.FlushAsync();
                            cryptoStream.Close();
                        }

                        return Convert.ToBase64String(memoryStream.ToArray());
                    }
                }
            }
        } // end method EncryptAsync

        /// <summary>
        /// Decrypt a string value, as an asynchronous operation.
        /// </summary>
        /// <param name="value">a specified string value</param>
        /// <returns>a decrypted string value</returns>
        public static async Task<string> DecryptAsync(this string value)
        {
            if (string.IsNullOrEmpty(value))
                return string.Empty;

            using (var provider = new DESCryptoServiceProvider())
            {
                provider.IV = defaultEncoding.GetBytes(encryptionKey);
                provider.Key = defaultEncoding.GetBytes(encryptionKey);

                using (var decryptor = provider.CreateDecryptor(provider.IV, provider.Key))
                {
                    var temp = Convert.FromBase64String(value);

                    using (var memoryStream = new MemoryStream())
                    {
                        // decrypt the value
                        using (var cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Write))
                        {
                            await cryptoStream.WriteAsync(temp, 0, temp.Length);
                            await cryptoStream.FlushAsync();
                            cryptoStream.Close();
                        }

                        return defaultEncoding.GetString(memoryStream.ToArray());
                    }
                }
            }
        } // end method DecryptAsync
    } // end static class DesExtension
} // end namespace NewEraFlowerStore.Extensions