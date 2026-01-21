namespace BusinessObject.Enums;

public enum VerificationType
{
    /// <summary>
    /// Verification type for account registration.
    /// </summary>
    Register,

    /// <summary>
    /// Verification type for password reset.
    /// </summary>
    ForgotPassword,

    /// <summary>
    /// Verification type for changing password.
    /// </summary>
    ChangePassword,
}
