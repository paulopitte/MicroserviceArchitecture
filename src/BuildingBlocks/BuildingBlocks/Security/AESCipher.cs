using System.Security.Cryptography;
using System.Text;

namespace BuildingBlocks.Security;
public class AESCipher
{
    private static readonly string Key = "your-32-char-secret-key1234567890"; // 32 caracteres (256 bits)
    private static readonly string IV = "your-16-char-iv123";                 // 16 caracteres (128 bits)


    public static string Cipher(string plainText)
    {
        if (string.IsNullOrEmpty(plainText))
            throw new ArgumentNullException(nameof(plainText));

        using (var aes = Aes.Create())
        {
            aes.Key = Encoding.UTF8.GetBytes(Key);
            aes.IV = Encoding.UTF8.GetBytes(IV);

            using (var encryptor = aes.CreateEncryptor(aes.Key, aes.IV))
            {
                using (var ms = new MemoryStream())
                {
                    using (var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
                    {
                        using (var writer = new StreamWriter(cs))
                        {
                            writer.Write(plainText);
                        }
                    }
                    return Convert.ToBase64String(ms.ToArray());
                }
            }
        }
    }


    public static string Decipher(string cipherText)
    {
        if (string.IsNullOrEmpty(cipherText))
            throw new ArgumentNullException(nameof(cipherText));

        using (var aes = Aes.Create())
        {
            aes.Key = Encoding.UTF8.GetBytes(Key);
            aes.IV = Encoding.UTF8.GetBytes(IV);

            using (var decryptor = aes.CreateDecryptor(aes.Key, aes.IV))
            {
                using (var ms = new MemoryStream(Convert.FromBase64String(cipherText)))
                {
                    using (var cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read))
                    {
                        using (var reader = new StreamReader(cs))
                        {
                            return reader.ReadToEnd();
                        }
                    }
                }
            }
        }
    }

    // Método para verificar se uma string está no formato Base64
    public static bool IsBase64String(string base64)
    {
        if (string.IsNullOrEmpty(base64))
            return false;

        Span<byte> buffer = new Span<byte>(new byte[base64.Length]);
        return Convert.TryFromBase64String(base64, buffer, out _);
    }
}
