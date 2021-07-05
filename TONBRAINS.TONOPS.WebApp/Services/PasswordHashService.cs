namespace TONBRAINS.TONOPS.WebApp.Services
{
    public class PasswordHashService : IPasswordHashService
    {
        public string Hash(string input, string salt = null)
        {
            if (!string.IsNullOrWhiteSpace(salt)) return BCrypt.Net.BCrypt.HashPassword(input + salt, BCrypt.Net.BCrypt.GenerateSalt());

            return BCrypt.Net.BCrypt.HashPassword(input, BCrypt.Net.BCrypt.GenerateSalt());
        }

        public bool IsValid(string input, string validInput, string salt = null)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(salt)) return BCrypt.Net.BCrypt.Verify(input + salt, validInput);

                return BCrypt.Net.BCrypt.Verify(input, validInput);
            }
            catch
            {
                return false;
            }
        }
    }
}
