namespace Backend.BankingTranxSystem.SharedServices.Cache;

public class RedisCacheSettings
{
    public bool Enabled { get; set; }

    public string ConnectionString { get; set; }
}