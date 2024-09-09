namespace Backend.BankingTranxSystem.SharedServices.Helper;
public enum RepositoryActionStatus
{
    Ok,
    Created,
    Updated,
    Deleted,
    NotFound,
    NothingModified,
    Error,
    ValidationError,
    Forbidden,
    Disabled
}