using Microsoft.EntityFrameworkCore;
using MyRecipeBook.Domain.Entities;
using MyRecipeBook.Domain.Repositories;
using MyRecipeBook.Domain.Repositories.User;

namespace MyRecipeBook.Infraestructure.DataAcess.Repositories;

internal sealed class UserRepository : IUserWriteOnlyRepository, IUserReadOnlyRepository, IUserUpdateOnlyRepository
{
    private readonly MyRecipeBookDbContext _dbContext;
    private readonly IUnitOfWork _unitOfWork;

    public UserRepository(MyRecipeBookDbContext dbContext, IUnitOfWork unitOfWork)
    {
        _dbContext = dbContext;
        _unitOfWork = unitOfWork;
    }

    public async Task Add(User user) => await _dbContext.Users.AddAsync(user);

    public async Task<bool> ExistActiveUserWithEmail(string email) => await _dbContext.Users.AnyAsync(u => u.Email == email && u.Active);

    public async Task<bool> ExistActiveUserWithId(Guid guid) => await _dbContext.Users.AnyAsync(u => u.Id == guid && u.Active);

    public async Task<User?> GetByEmail(string email) => await _dbContext.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Active && u.Email.Equals(email));

    public async Task UpdatePassword(Guid userId, string passwordHash) => await _dbContext.Users.Where(user => user.Id == userId).ExecuteUpdateAsync(setter => setter.SetProperty(user => user.Password, passwordHash));
    
    public void UpdateProfile(User user)
    {
        _dbContext.Users.Attach(user);
        _dbContext.Entry(user).Property(user => user.Name).IsModified = true;
        _dbContext.Entry(user).Property(user => user.Email).IsModified = true;
    }
}
