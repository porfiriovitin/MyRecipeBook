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

        byte[] bytesPassword = Encoding.UTF8.GetBytes(password);

        Argon2id hashAlgorithm = new(bytesPassword)
        {
            DegreeOfParallelism = DEGREE_OF_PARALLELISM,
            Iterations = ITERATIONS,
            MemorySize = MEMORY_SIZE,
            Salt = salt
        };

        var hash = hashAlgorithm.GetBytes(HASH_SIZE);

        var combinedBytes = new byte[hash.Length + salt.Length];

        salt.CopyTo(combinedBytes.AsSpan(0, salt.Length));
        hash.CopyTo(combinedBytes.AsSpan(salt.Length, hash.Length));

        return Convert.ToBase64String(combinedBytes);
    }

    public bool VerifyPassword(string password, string hashedPassword)
    {
        throw new NotImplementedException();
    }
}
