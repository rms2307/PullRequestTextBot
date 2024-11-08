using System.Security.Cryptography;
using System.Text;

namespace PullRequestTextBot
{
    public static class LoginManager
    {
        private static readonly string FilePath = "apikey.dat";

        public static bool CheckIfIsLoggedIn()
        {
            if (!File.Exists(FilePath))
            {
                return false;
            }

            return true;
        }

        public static void Login()
        {
            Console.WriteLine("Necessário login.");
            Console.WriteLine("Informe a sua ApiKey: ");
            string? apiKey = Console.ReadLine();

            while (string.IsNullOrEmpty(apiKey))
            {
                Console.WriteLine("Informe a sua ApiKey: ");
                apiKey = Console.ReadLine();
            }

            byte[] apiKeyBytes = Encoding.UTF8.GetBytes(apiKey);
            byte[] encryptedApiKey = ProtectedData.Protect(apiKeyBytes, null, DataProtectionScope.CurrentUser);

            File.WriteAllBytes(FilePath, encryptedApiKey);
        }

        public static void Logout()
        {
            if (File.Exists(FilePath))
            {
                File.Delete(FilePath);
            }
        }

        public static string GetApiKey()
        {
            if (!CheckIfIsLoggedIn())
            {
                Login();
            }

            byte[] encryptedApiKey = File.ReadAllBytes(FilePath);
            byte[] decryptedApiKey = ProtectedData.Unprotect(encryptedApiKey, null, DataProtectionScope.CurrentUser);

            return Encoding.UTF8.GetString(decryptedApiKey);
        }
    }
}
