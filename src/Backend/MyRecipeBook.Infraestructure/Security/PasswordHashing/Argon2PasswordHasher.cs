using Konscious.Security.Cryptography;
using MyRecipeBook.Domain.Security.PasswordHashing;
using System.Security.Cryptography;
using System.Text;

namespace MyRecipeBook.Infraestructure.Security.PasswordHashing;

public sealed class Argon2PasswordHasher : IPasswordHasher
{
    private const int DEGREE_OF_PARALLELISM = 1;
    private const int ITERATIONS = 2;
    private const int MEMORY_SIZE = 20 * 1024;
    private const int SALT_SIZE = 16;
    private const int HASH_SIZE = 32;

    public string HashPassword(string password)
    {
        var salt = RandomNumberGenerator.GetBytes(SALT_SIZE);
        var hash = HashPassword(salt, password);

        var combinedBytes = new byte[hash.Length + salt.Length];

        salt.CopyTo(combinedBytes.AsSpan(0, salt.Length));
        hash.CopyTo(combinedBytes.AsSpan(salt.Length, hash.Length));

        return Convert.ToBase64String(combinedBytes);
    }

    public bool VerifyPassword(string password, string hashedPassword)
    {
        var hashedPasswordBytes = Convert.FromBase64String(hashedPassword);

        var salt = new byte[SALT_SIZE];
        var hash = new byte[HASH_SIZE];

        Array.Copy(hashedPasswordBytes, 0, salt, 0, SALT_SIZE);
        Array.Copy(hashedPasswordBytes, SALT_SIZE, hash, 0, HASH_SIZE);

        var newHash = HashPassword(salt, password);

        return CryptographicOperations.FixedTimeEquals(hash, newHash);
    }

    private static byte[] HashPassword(byte[] salt, string password)
    {
        byte[] bytesPassword = Encoding.UTF8.GetBytes(password);

        Argon2id hashAlgorithm = new(bytesPassword)
        {
            DegreeOfParallelism = DEGREE_OF_PARALLELISM,
            Iterations = ITERATIONS,
            MemorySize = MEMORY_SIZE,
            Salt = salt
        };

        return hashAlgorithm.GetBytes(HASH_SIZE);
    }
}
