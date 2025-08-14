﻿using StackExchange.Redis;

using Application.Users.Interfaces;

namespace Persistence.Repositories;

public class ConfirmationCodesRepository : IConfirmationCodesRepository
{
    private readonly IDatabaseAsync _redisDb;
    private const string Prefix = "confirmation";

    public ConfirmationCodesRepository(IConnectionMultiplexer redisDb)
    {
        _redisDb = redisDb.GetDatabase();
    }

    public async Task SetConfirmationCode(string email, string code, TimeSpan expiry)
    {
        await _redisDb.StringSetAsync($"{Prefix}:{email}", code, expiry);
    }

    public async Task<string?> GetConfirmationCode(string email)
    {
        var code = await _redisDb.StringGetAsync($"{Prefix}:{email}");
        return code;
    }
}