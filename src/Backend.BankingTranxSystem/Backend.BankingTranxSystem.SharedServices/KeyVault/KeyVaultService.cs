using Azure.Security.KeyVault.Certificates;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

namespace Backend.BankingTranxSystem.SharedServices.KeyVault;

public class KeyVaultService(CertificateClient certificateClient)
{
    private readonly CertificateClient _certificateClient = certificateClient;

    public Task<X509Certificate> GetX509Certificate(string certificateName)
    {
        KeyVaultCertificateWithPolicy certificateWithPolicy = _certificateClient.GetCertificate(certificateName);
        return Task.FromResult(new X509Certificate(certificateWithPolicy.Cer, (string)null));
    }
}