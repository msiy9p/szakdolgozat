namespace Libellus.Application.ViewModels;

public sealed class TwoFactorSummaryVm
{
    public bool HasAuthenticator { get; init; }
    public int RecoveryCodesLeft { get; init; }
    public bool Is2FaEnabled { get; init; }
    public bool IsMachineRemembered { get; init; }

    public TwoFactorSummaryVm(bool hasAuthenticator, int recoveryCodesLeft, bool is2FaEnabled, bool isMachineRemembered)
    {
        HasAuthenticator = hasAuthenticator;
        RecoveryCodesLeft = recoveryCodesLeft;
        Is2FaEnabled = is2FaEnabled;
        IsMachineRemembered = isMachineRemembered;
    }
}