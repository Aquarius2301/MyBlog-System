namespace Application.Services.Interfaces;

/// <summary>
/// Email Service Interface
/// </summary>
public interface IEmailService
{
    /// <summary>
    /// Sends an email to the specified recipient.
    /// </summary>
    /// <param name="to">Recipient email address.</param>
    /// <param name="subject">Subject of the email.</param>
    /// <param name="body">Body content of the email.</param>
    /// <param name="isHtml">Indicates whether the body is HTML. Defaults to true.</param>
    /// <returns>A task representing the asynchronous email sending operation.</returns>
    Task SendEmailAsync(string to, string subject, string body, bool isHtml = true);

    /// <summary>
    /// Sends a registration confirmation email to a new user.
    /// </summary>
    /// <param name="email">Recipient email address.</param>
    /// <param name="username">Username of the new account.</param>
    /// <param name="confirmCode">Confirmation code for email verification.</param>
    /// <returns>A task representing the asynchronous email sending operation.</returns>
    /// <remarks>
    /// The email includes a link to confirm the account.
    /// The confirmation link expires after <see cref="TokenTimeOut"/> minutes.
    /// </remarks>
    Task SendRegisterEmailAsync(string email, string username, string confirmCode);

    /// <summary>
    /// Sends a password reset email to the user.
    /// </summary>
    /// <param name="email">Recipient email address.</param>
    /// <param name="username">Username of the account.</param>
    /// <param name="confirmCode">Confirmation code for password reset.</param>
    /// <returns>A task representing the asynchronous email sending operation.</returns>
    /// <remarks>
    /// The email includes a link to reset the password.
    /// The confirmation link expires after <see cref="TokenTimeOut"/> minutes.
    /// </remarks>
    Task SendForgotPasswordEmailAsync(string email, string username, string confirmCode);

    /// <summary>
    /// Sends an account removal notification email to the user.
    /// </summary>
    /// <param name="email">Recipient email address.</param>
    /// <returns>A task representing the asynchronous email sending operation.</returns>
    /// <remarks>
    /// The email notifies the user that their account has been removed.
    /// </remarks>
    Task SendAccountRemovalEmailAsync(string email);
}
