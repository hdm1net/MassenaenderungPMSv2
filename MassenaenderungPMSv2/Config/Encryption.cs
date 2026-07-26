using System.Security.Cryptography;
using System.Text;

namespace MassenaenderungPMSv2.Enryption
{
    public class AesOperation
    {
        // Konstanter Key für die AES-Verschlüsselung innerhalb dieser Klasse
        private const string AesEncryptionKey = "abc17875896LiHeDoCdinBre09meABC8";

        /// <summary>
        /// Verschlüsselung einer Zeichenfolge mittels AES 
        /// </summary>
        /// <param name="plainText"></param>
        /// <returns>Verschlüsselte Zeichenfolge</returns>
        public static string EncryptString(string plainText)
        {
            if (String.IsNullOrEmpty(plainText)) { return String.Empty; }

            //
            // Copilot:
            // You’re using an all-zero IV every time.This makes the encryption deterministic: the same plaintext always produces the same ciphertext. That defeats the purpose of using an IV and weakens security.
            // Best practice: generate a random IV for each encryption using aes.GenerateIV() or RandomNumberGenerator.Fill(iv).Store the IV alongside the ciphertext(e.g., prepend it before Base64 encoding).
            //
            // Das ist Absicht hier, da die verschlüsselte Zeichenfolge (Passwort) auch wieder entschlüsselt werden muss.

            //byte[] array;

            using (Aes aes = Aes.Create())
            {

                aes.Key = Encoding.UTF8.GetBytes(AesEncryptionKey);
                aes.IV = new byte[16];

                ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);

                using (MemoryStream memoryStream = new MemoryStream())
                {
                    using (CryptoStream cryptoStream = new CryptoStream((Stream)memoryStream, encryptor, CryptoStreamMode.Write))
                    using (StreamWriter streamWriter = new StreamWriter((Stream)cryptoStream))
                        {
                            streamWriter.Write(plainText);
                        }

                    return Convert.ToBase64String(memoryStream.ToArray());
                }
            }

        }

        /// <summary>
        /// Entschlüsselung einer Zeichenfolge die zuvor mittels EncryptString verschlüsselt wurde
        /// </summary>
        /// <param name="cipherText"></param>
        /// <returns>Entschlüsselte Zeichenfolge</returns>
        public static string DecryptString(string cipherText)
        {
            if (String.IsNullOrEmpty(cipherText)) { return String.Empty; }

            //
            // Copilot:
            // You’re using an all-zero IV every time.This makes the encryption deterministic: the same plaintext always produces the same ciphertext. That defeats the purpose of using an IV and weakens security.
            // Best practice: generate a random IV for each encryption using aes.GenerateIV() or RandomNumberGenerator.Fill(iv).Store the IV alongside the ciphertext(e.g., prepend it before Base64 encoding).
            //
            // Das ist Absicht hier, da die verschlüsselte Zeichenfolge (Passwort) auch wieder entschlüsselt werden muss.

            byte[] buffer = Convert.FromBase64String(cipherText);

            using (Aes aes = Aes.Create())
            {
                aes.Key = Encoding.UTF8.GetBytes(AesEncryptionKey);
                aes.IV = new byte[16];

                ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);

                using (MemoryStream memoryStream = new MemoryStream(buffer))
                {
                    using (CryptoStream cryptoStream = new CryptoStream((Stream)memoryStream, decryptor, CryptoStreamMode.Read))
                    using (StreamReader streamReader = new StreamReader((Stream)cryptoStream))
                    {
                        return streamReader.ReadToEnd();
                    }
                }
            }
        }
    }
}
