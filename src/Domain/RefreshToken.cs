namespace Domain;

public class RefreshToken
{
    public long Id { get; set; }

    public required string UserId { get; set; }
    public User? User { get; set; }

    public required string TokenHash { get; set; }
    public required DateTime CreatedAt { get; set; }
    public required DateTime Expiration { get; set; }
    public bool IsRevoked { get; set; } = false;
    public DateTime RevokedAt { get; set; }

    public long? ReplacesTokenId { get; set; }
    public RefreshToken? ReplacesToken { get; set; }
    public long? ReplacedByTokenId { get; set; }
    public RefreshToken? ReplacedByToken { get; set; }
}