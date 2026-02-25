using StackExchange.Redis;
using Application.Users.Interfaces;

namespace Persistence.Repositories;

public class CodesRepository : ICodesRepository
{
    private readonly IDatabaseAsync _redisDb;
    private const string ConfirmationPrefix = "confirmation";
    private const string RestorePrefix = "restore";
    
    public CodesRepository(IConnectionMultiplexer redisDb)
    {
        _redisDb = redisDb.GetDatabase(0);
    }

    public Task<bool> SetConfirmationCode(string email, string code, TimeSpan expiry) 
        => _redisDb.StringSetAsync($"{ConfirmationPrefix}:{email}", code, expiry);

    public Task<string> GetConfirmationCode(string email)
        => _redisDb.StringGetAsync($"{ConfirmationPrefix}:{email}")
            .ContinueWith(t => t.Result.ToString());

    public Task<bool> SetRestoreCode(string email, string code, TimeSpan expiry)
        => _redisDb.StringSetAsync($"{RestorePrefix}:{email}", code, expiry);

    public Task<string> GetRestoreCode(string email)
        => _redisDb.StringGetAsync($"{RestorePrefix}:{email}")
            .ContinueWith(t => t.Result.ToString());
}