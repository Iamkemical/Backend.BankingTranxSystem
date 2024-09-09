using Backend.BankingTranxSystem.SharedServices.Domain;
using System.ComponentModel.DataAnnotations;

namespace Backend.BankingTranxSystem.SharedServices.Models;
public class SharedBase : BaseEntity<Guid>
{
    public DateTime CreatedAt { get; set; }

    [MaxLength(200)]
    public string CreatedBy { get; set; }
    public DateTime UpdatedAt { get; set; } = DateTime.Now.ToUniversalTime();

    [MaxLength(200)]
    public string UpdatedBy { get; set; }


}
