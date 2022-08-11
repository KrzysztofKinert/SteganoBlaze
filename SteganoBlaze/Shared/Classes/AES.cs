using SteganoBlaze.Shared.Classes.Types;
using System.Security.Cryptography;

namespace SteganoBlaze.Shared.Classes
{
    public class AES
    {
        public static Message Encrypt(Message message, byte[] Key, byte[] IV)
        {
            // Check arguments
            if (Key == null || Key.Length <= 0)
                throw new ArgumentNullException("Key");
            if (IV == null || IV.Length <= 0)
                throw new ArgumentNullException("IV");
            //byte[] encrypted;

            // Create an Aes object
            // with the specified key and IV.
            using (Aes aes = Aes.Create())
            {
                aes.Mode = CipherMode.CBC;
                aes.KeySize = Key.Length;
                aes.BlockSize = 128;
                aes.Padding = PaddingMode.Zeros;
                aes.Key = Key;
                aes.IV = IV;

                // Create an encryptor to perform the stream transform.
                ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);

                // Create the streams used for encryption.
                using (MemoryStream msEncrypt = new MemoryStream())
                {
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        csEncrypt.WriteAsync(message.header, 0, message.header.Length);
                        csEncrypt.FlushFinalBlockAsync();
                        message.header = msEncrypt.ToArray();
                    }
                }
                using (MemoryStream msEncrypt = new MemoryStream())
                {
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        csEncrypt.WriteAsync(message.metadata, 0, message.metadata.Length);
                        csEncrypt.FlushFinalBlockAsync();
                        message.metadata = msEncrypt.ToArray();
                    }
                }
                using (MemoryStream msEncrypt = new MemoryStream())
                {
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        csEncrypt.WriteAsync(message.message, 0, message.message.Length);
                        csEncrypt.FlushFinalBlockAsync();
                        message.message = msEncrypt.ToArray();
                    }
                }
            }
            return message;
        }
        public static byte[] Decrypt(byte[] data, byte[] key, byte[] iv)
        {
            using (Aes aes = Aes.Create())
            {
                aes.KeySize = 128;
                aes.BlockSize = 128;
                aes.Padding = PaddingMode.Zeros;

                aes.Key = key;
                aes.IV = iv;

                ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);

                using (MemoryStream msEncrypt = new MemoryStream())
                {
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, decryptor, CryptoStreamMode.Write))
                    {
                        csEncrypt.WriteAsync(data, 0, data.Length);
                        csEncrypt.FlushFinalBlockAsync();

                        //using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                        //{
                        //    //Write all data to the stream.
                        //    swEncrypt.Write(plainText);
                        //}
                        return msEncrypt.ToArray();
                    }
                }
            }
        }
    }
}
