using Coronation.Infrastructure.Services;
using Backend.BankingTranxSystem.SharedServices.Enums;
using System;

namespace Backend.BankingTranxSystem.SharedServices.Helper;

public static class InfowareUtility
{

    public static int GetFileLimit()
    {
        if (int.TryParse(Environment.GetEnvironmentVariable(Constants.FileSizeLimit), out var FileSizeLimit))
            return FileSizeLimit;

//#if DEBUG
        FileSizeLimit = 1572864;
//#endif
        return FileSizeLimit;
    }

    public static bool GetFileLimitReached(long length)
    {
        return length > GetFileLimit();
    }

    public static ServiceType GetEnvironmentStateManager(Platform platform, OnboardingStep step, bool isProcessMaker = false)
    {
        if (IsLiveEnvironmentActive())
        {
            if (platform == Platform.Cam && step == OnboardingStep.Create) return ServiceType.AssetManagementLiveOnboardingCreate;
            if (platform == Platform.Cam && step == OnboardingStep.Involvement) return ServiceType.AssetManagementLiveOnboardingInvolvement;
            if (platform == Platform.Cam && step == OnboardingStep.CustomFields) return ServiceType.AssetManagementLiveOnboardingCustomFields;
            if (platform == Platform.Cam && step == OnboardingStep.None) return ServiceType.AssetManagementLive;
            if (platform == Platform.Cosec && step == OnboardingStep.Create) return ServiceType.SecuritiesLiveOnboarding;
            if (platform == Platform.Cam && step == OnboardingStep.None && isProcessMaker) return ServiceType.AssetManagementProcessMakerLive;
            if (platform == Platform.Cosec && step == OnboardingStep.None && isProcessMaker) return ServiceType.SecuritiesProcessMakerLive;
            if (platform == Platform.Cosec && step == OnboardingStep.None && !isProcessMaker) return ServiceType.SecuritiesLive;
        }
        else
        {
            if (platform == Platform.Cam && step == OnboardingStep.Create) return ServiceType.AssetManagementTestOnboardingCreate;
            if (platform == Platform.Cam && step == OnboardingStep.Involvement) return ServiceType.AssetManagementTestOnboardingInvolvement;
            if (platform == Platform.Cam && step == OnboardingStep.CustomFields) return ServiceType.AssetManagementTestOnboardingCustomFields;
            if (platform == Platform.Cam && step == OnboardingStep.None && !isProcessMaker) return ServiceType.AssetManagementTest;
            if (platform == Platform.Cam && step == OnboardingStep.None && isProcessMaker) return ServiceType.AssetManagementProcessMakerTest;
            if (platform == Platform.Cosec && step == OnboardingStep.Create) return ServiceType.SecuritiesTestOnboarding;
            if (platform == Platform.Cosec && step == OnboardingStep.None && !isProcessMaker) return ServiceType.SecuritiesTest;
            if ((platform == Platform.Cosec && step == OnboardingStep.None && isProcessMaker)) return ServiceType.SecuritiesProcessMakerTest;
        }
        return ServiceType.Undefined;
    }

    public static bool IsTestEnvironmentActive()
    {
        return Environment.GetEnvironmentVariable(Constants.Mode) != Constants.EnviromentType.Live;
    }

    public static bool IsLiveEnvironmentActive()
    {
        return Environment.GetEnvironmentVariable(Constants.Mode) == Constants.EnviromentType.Live;
    }
}