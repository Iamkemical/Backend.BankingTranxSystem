using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Backend.BankingTranxSystem.SharedServices.Domain;
public abstract class BaseEntity<TId>
{
    public List<BaseDomainEvent> Events = [];

    [Key]
    [Column(Order = 0)]
    public TId Id { get; set; }
}
