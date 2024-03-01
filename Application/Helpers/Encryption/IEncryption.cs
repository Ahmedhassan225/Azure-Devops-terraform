namespace Application.Helpers.Encryption
{
    public interface IEncryption
    {
        static string Encryptkey { get; set; }
        string EncryptData(string textData);
        string DecryptData(string EncryptText);
    }
}
