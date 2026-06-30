using AppointmentManagement.Application.Interfaces;
using Microsoft.Extensions.Configuration;
using System.Security.Cryptography;
using System.Text;

namespace AppointmentManagement.Infrastructure.Services;

public class EncryptionService(IConfiguration config) : IEncryptionService
{
    private readonly byte[] _key = SHA256.HashData(
        Encoding.UTF8.GetBytes(config["Encryption:Key"]!));

    public string Encrypt(string plainText)
    {
        using var aes = Aes.Create();
        aes.Key = _key;
        aes.GenerateIV();
        using var enc = aes.CreateEncryptor();
        var plain = Encoding.UTF8.GetBytes(plainText);
        var cipher = enc.TransformFinalBlock(plain, 0, plain.Length);
        var result = new byte[aes.IV.Length + cipher.Length];
        aes.IV.CopyTo(result, 0);
        cipher.CopyTo(result, aes.IV.Length);
        return Convert.ToBase64String(result);
    }

    public string Decrypt(string cipherText)
    {
        var data = Convert.FromBase64String(cipherText);
        using var aes = Aes.Create();
        aes.Key = _key;
        var iv = data[..16];
        var cipher = data[16..];
        aes.IV = iv;
        using var dec = aes.CreateDecryptor();
        var plain = dec.TransformFinalBlock(cipher, 0, cipher.Length);
        return Encoding.UTF8.GetString(plain);
    }
}