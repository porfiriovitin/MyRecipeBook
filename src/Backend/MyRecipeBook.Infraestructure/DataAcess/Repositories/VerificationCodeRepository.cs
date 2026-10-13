using Microsoft.EntityFrameworkCore;
using MyRecipeBook.Domain.Entities;
using MyRecipeBook.Domain.Enums;
using MyRecipeBook.Domain.Repositories.VerificationCode;

namespace MyRecipeBook.Infraestructure.DataAcess.Repositories;

internal sealed class VerificationCodeRepository : IVerificationCodeWriteOnlyRepository, IVerificationCodeReadOnlyRepository
{
    private readonly MyRecipeBookDbContext _dbContext;

    public VerificationCodeRepository(MyRecipeBookDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task Replace(VerificationCode verificationCode)
    {
        var existingCodes = await _dbContext.VerificationCodes.Where(vc => vc.UserId == verificationCode.UserId && vc.Type == verificationCode.Type).ToListAsync();

        _dbContext.VerificationCodes.RemoveRange(existingCodes);

        await _dbContext.VerificationCodes.AddAsync(verificationCode);
    }

    public async Task Delete(VerificationCode verificationCode) => await _dbContext.VerificationCodes.Where(vc => vc.Id == verificationCode.Id).ExecuteDeleteAsync();

    public async Task<VerificationCode?> GetVerificationCodeAsync(Guid UserId, string Code, VerificationCodeType type) => await _dbContext.VerificationCodes.FirstOrDefaultAsync(vc => vc.UserId == UserId && vc.Code == Code && vc.Type == type);
    
}
