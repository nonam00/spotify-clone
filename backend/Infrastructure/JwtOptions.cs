namespace Infrastructure
{
    public class JwtOptions
    {
        public string SecretKey { get; init; } = string.Empty;
        public int ExpiresHours { get; init; }
    }
}