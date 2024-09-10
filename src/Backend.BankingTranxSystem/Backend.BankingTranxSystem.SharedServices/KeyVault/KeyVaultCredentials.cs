namespace Backend.BankingTranxSystem.SharedServices.KeyVault;

public record KeyVaultCredentials(string ClientId, string ClientSecret, string TenantId, string VaultUri);