public interface IPasswordHashProvider
{
    string Hash(string password);
    bool Verify(string password, string hashed);
}
