namespace CamMgt.Api.Contracts;

public class ApiRoutes
{
    public const string Root = "api";

    public const string Version = "v1";

    public const string Base = Root + "/" + Version;

    public const string AdminBase = Root + "/" + Version + "/" + "admin";

    public static class Customer
    {
        public const string CreateCustomer = Base + "/customer";
        public const string GetOnboardingStatus = Base + "/customer/get-onboarding-status/{userId}";
        public const string GetCustomerCreationStatus = Base + "/customer/get-customer-status/{bvn}";
    }
}