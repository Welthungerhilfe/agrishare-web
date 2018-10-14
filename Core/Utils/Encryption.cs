/* Title: Gloo Framework
 * Author: Bradley Searle (C2 Digital)
 * Source: www.c2.co.zw
 * License: CC BY 4.0 (https://creativecommons.org/licenses/by/4.0/legalcode) */

using Agrishare.Core.Encryption;
using Agrishare.Core.Entities;
using System;
using System.Data;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace Agrishare.Core.Utils
{
    /// <summary>
    /// Provides access to multiple encryption methods
    /// </summary>
    public class Encryption
    {
        #region HEX

        /// <summary>
        /// Convert a normal string to HEX characters
        /// </summary>
        /// <param name="Original">String literal</param>
        /// <returns>HEX encoded string</returns>
        public static string ConvertToHexString(string Original)
        {
            string Hex = String.Empty;
            byte[] encodedChars = Encoding.Unicode.GetBytes(Original);
            for (int i = 0; i < encodedChars.Length; i++)
                Hex += encodedChars[i].ToString("X2"); //.PadLeft(2, '0');            }
            return Hex;
        }

        /// <summary>
        /// Convert a HEX string back to the original string
        /// </summary>
        /// <param name="HexString">The HEX string literal</param>
        /// <returns>Original string</returns>
        public static string ConvertFromHexString(string HexString)
        {
            int Discarded;
            return Encoding.Unicode.GetString(HexEncoding.GetBytes(HexString, out Discarded));
        }

        #endregion

        #region SHA-512

        /// <summary>
        /// Hash string using SHA-512 algorithm
        /// </summary>
        /// <param name="Data">Original string</param>
        /// <param name="Salt">Random data</param>
        /// <returns>Hashed string - 88 characters long</returns>
        public static string GetSHAHash(string Data, string Salt)
        {
            UnicodeEncoding ue = new UnicodeEncoding();

            // Mix 1 part data, 1 part Salt and 1 part of a little something I like to call... well... salt.
            // The idea is that they could get the password and salt from the Db, but would then also need 
            // the extra salt from the code to retrieve the password using a rainbow table attack.
            string password = Data + Salt + Config.EncryptionSalt;

            byte[] data = ue.GetBytes(password);

            SHA512 shaM = new SHA512Managed();

            byte[] result = shaM.ComputeHash(data);

            return Convert.ToBase64String(result);
        }

        #endregion

        #region MD5

        /// <summary>
        /// Hash string using MD5 algorithm
        /// </summary>
        /// <param name="Data">Original string</param>
        /// <returns>Hashed string</returns>
        public static string GetMD5Hash(string Data)
        {
            MD5 md5 = MD5.Create();
            byte[] inputBytes = Encoding.ASCII.GetBytes(Data);
            byte[] hash = md5.ComputeHash(inputBytes);

            return string.Concat(hash.Select(b => b.ToString("X2")).ToArray());
        }

        #endregion

        #region RC4

        /// <summary>
        /// Encrypt a string using RC4 algorithm
        /// </summary>
        /// <param name="Data">Original string</param>
        /// <param name="Salt">Random data</param>
        /// <returns>HEX encoded encrypted string literal</returns>
        public static string EncryptWithRC4(string Data, string Salt = "")
        {
            // URL encode string to preserve foriegn characters and NON-ANSI characters
            Data = System.Net.WebUtility.UrlEncode(Data);

            // Encrypt string
            RC4Engine RC4 = new RC4Engine();
            RC4.EncryptionKey = Config.EncryptionPassword + Salt + Config.EncryptionSalt;
            RC4.InClearText = Data;
            RC4.Encrypt();
            string Encrypted = RC4.CryptedText;

            // Convert to HEX
            return ConvertToHexString(Encrypted);
        }

        /// <summary>
        /// Decrypt a string using RC4 algorithm
        /// </summary>
        /// <param name="Data">HEX encoded encrypted string</param>
        /// <param name="Salt">Random data</param>
        /// <returns>Decrypted string literal</returns>
        public static string DecryptWithRC4(string Data, string Salt = "")
        {
            // Convert from HEX
            Data = ConvertFromHexString(Data);

            // Decrypt string
            RC4Engine RC4 = new RC4Engine();
            RC4.EncryptionKey = Config.EncryptionPassword + Salt + Config.EncryptionSalt;
            RC4.CryptedText = Data;
            RC4.Decrypt();
            string Decrypted = RC4.InClearText;

            // URL decode
            return System.Net.WebUtility.UrlDecode(Decrypted);
        }

        #endregion

        #region Utils

        /// <summary>
        /// Get a random string for use in salts.  Uses the RNG Cryptographic service provider.  Note: string is [A-Za-z0-9/=]
        /// </summary>
        /// <param name="Length"></param>
        /// <returns></returns>
        public static string GetSalt(int Length = 64)
        {
            RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
            byte[] SaltyBytes = new byte[Length];

            rng.GetBytes(SaltyBytes);

            return Convert.ToBase64String(SaltyBytes);
        }

        #endregion
    }
}
