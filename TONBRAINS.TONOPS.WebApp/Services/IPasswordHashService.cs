namespace TONBRAINS.TONOPS.WebApp
{
    public interface IPasswordHashService
    {

        string Hash(string input, string salt = null);

        public bool IsValid(string input, string validInput, string salt = null);

    }
}
