using StackExchange.Redis;
using Infrastructure.Email;

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

    public async Task SetConfirmationCode(string email, string code, TimeSpan expiry)
    {
        await _redisDb.StringSetAsync($"{ConfirmationPrefix}:{email}", code, expiry).ConfigureAwait(false);
    }

    public async Task<string?> GetConfirmationCode(string email)
    {
        var code = await _redisDb.StringGetAsync($"{ConfirmationPrefix}:{email}").ConfigureAwait(false);
        return code;
    }

    public async Task SetRestoreCode(string email, string code, TimeSpan expiry)
    {
        await _redisDb.StringSetAsync($"{RestorePrefix}:{email}", code, expiry).ConfigureAwait(false);
    }

    public async Task<string?> GetRestoreCode(string email)
    {
        var code = await _redisDb.StringGetAsync($"{RestorePrefix}:{email}").ConfigureAwait(false);
        return code;
    }
}