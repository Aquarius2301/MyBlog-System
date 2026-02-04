using Application.Services.Interfaces;
using Application.Settings;
using MailKit.Net.Smtp;
using Microsoft.Extensions.Options;
using MimeKit;

namespace Application.Services.Implementations;

/// <summary>
/// Service class for sending emails such as registration and password reset emails.
/// </summary>
public class EmailService : IEmailService
{
    private readonly BaseSettings _settings;

    /// <summary>
    /// Initializes a new instance of the <see cref="EmailService"/> class.
    /// </summary>
    /// <param name="options">The configured settings.</param>
    public EmailService(IOptions<BaseSettings> options)
    {
        _settings = options.Value;
    }

    public async Task SendEmailAsync(string to, string subject, string body, bool isHtml = true)
    {
        var message = new MimeMessage();
        message.From.Add(
            new MailboxAddress(
                _settings.EmailSettings.SenderName,
                _settings.EmailSettings.SenderEmail
            )
        );
        message.To.Add(new MailboxAddress("", to));
        message.Subject = subject;

        var builder = new BodyBuilder();
        if (isHtml)
            builder.HtmlBody = body;
        else
            builder.TextBody = body;

        message.Body = builder.ToMessageBody();

        using var client = new SmtpClient();
        await client.ConnectAsync(
            _settings.EmailSettings.SmtpServer,
            _settings.EmailSettings.Port,
            MailKit.Security.SecureSocketOptions.StartTls
        );
        await client.AuthenticateAsync(
            _settings.EmailSettings.Username,
            _settings.EmailSettings.Password
        );
        await client.SendAsync(message);
        await client.DisconnectAsync(true);
    }

    public async Task SendRegisterEmailAsync(
        string email,
        string username,
        string confirmCode,
        string lang
    )
    {
        var emailTokenTimeout = _settings.TokenExpiryMinutes;
        var frontendUrl = _settings.FrontendUrl;

        if (lang == "vn")
        {
            await SendEmailAsync(
                to: email,
                subject: "Xác nhận đăng ký tài khoản MyBlog",
                body: $"<h2>Xin chào {username}</h2><br/>"
                    + "<p>Bạn đã đăng ký tài khoản thành công trên MyBlog.</p>"
                    + "<p>Vui lòng hãy nhấn vào liên kết này để xác thực email:</p>"
                    + $"<a href='{frontendUrl}/confirm?type=register&token={confirmCode}'>Xác nhận tài khoản</a><br/><br/>"
                    + $"<p>Đường link này chỉ tồn tại trong {emailTokenTimeout} phút.</p><br/>"
                    + "<p>Nếu bạn không đăng ký tài khoản này, vui lòng bỏ qua email này.</p>"
                    + "<br/><p>Trân trọng,<br/>Đội ngũ MyBlog</p>"
            );
        }
        else if (lang == "ja")
        {
            await SendEmailAsync(
                to: email,
                subject: "MyBlogアカウント登録確認",
                body: $"<h2>こんにちは、{username}さん</h2><br/>"
                    + "<p>MyBlogへのアカウント登録が完了しました。</p>"
                    + "<p>以下のリンクをクリックして、メールを確認してください:</p>"
                    + $"<a href='{frontendUrl}/confirm?type=register&token={confirmCode}'>アカウントを確認する</a><br/><br/>"
                    + $"<p>このリンクは{emailTokenTimeout}分間有効です。</p><br/>"
                    + "<p>このアカウント登録をリクエストしなかった場合は、このメールを無視してください。</p>"
                    + "<br/><p>よろしくお願いいたします。<br/>MyBlogチーム</p>"
            );
        }
        else
        {
            await SendEmailAsync(
                to: email,
                subject: "MyBlog Account Registration Confirmation",
                body: $"<h2>Hello {username}</h2><br/>"
                    + "<p>You have successfully registered an account on MyBlog.</p>"
                    + "<p>Please click the link below to verify your email:</p>"
                    + $"<a href='{frontendUrl}/confirm?type=register&token={confirmCode}'>Confirm Account</a><br/><br/>"
                    + $"<p>This link will expire in {emailTokenTimeout} minutes.</p><br/>"
                    + "<p>If you did not request this account registration, please ignore this email.</p>"
                    + "<br/><p>Best regards,<br/>The MyBlog Team</p>"
            );
        }
    }

    public async Task SendForgotPasswordEmailAsync(
        string email,
        string username,
        string confirmCode,
        string lang
    )
    {
        var emailTokenTimeout = _settings.TokenExpiryMinutes;
        var frontendUrl = _settings.FrontendUrl;

        if (lang == "vn")
        {
            await SendEmailAsync(
                to: email,
                subject: "Xác nhận quên mật khẩu tài khoản MyBlog",
                body: $"<h2>Xin chào {username}</h2><br/>"
                    + "<p>Bạn đã quên mật khẩu trên MyBlog.</p>"
                    + "<p>Vui lòng hãy nhấn vào liên kết này để xác thực email:</p>"
                    + $"<a href='{frontendUrl}/confirm?type=forgotPassword&token={confirmCode}'>Xác nhận tài khoản</a><br/><br/>"
                    + $"<p>Đường link này chỉ tồn tại trong {emailTokenTimeout} phút.</p><br/>"
                    + "<p>Nếu bạn không sử dụng quên mật khẩu, vui lòng bỏ qua email này.</p>"
                    + "<br/><p>Trân trọng,<br/>Đội ngũ MyBlog</p>"
            );
        }
        else if (lang == "ja")
        {
            await SendEmailAsync(
                to: email,
                subject: "MyBlogアカウントのパスワードリセット確認",
                body: $"<h2>こんにちは、{username}さん</h2><br/>"
                    + "<p>MyBlogでパスワードをお忘れになった場合の対応です。</p>"
                    + "<p>以下のリンクをクリックして、メールを確認してください:</p>"
                    + $"<a href='{frontendUrl}/confirm?type=forgotPassword&token={confirmCode}'>アカウントを確認する</a><br/><br/>"
                    + $"<p>このリンクは{emailTokenTimeout}分間有効です。</p><br/>"
                    + "<p>パスワードリセットをリクエストしなかった場合は、このメールを無視してください。</p>"
                    + "<br/><p>よろしくお願いいたします。<br/>MyBlogチーム</p>"
            );
        }
        else // default to English
        {
            await SendEmailAsync(
                to: email,
                subject: "MyBlog Account Password Reset Confirmation",
                body: $"<h2>Hello {username}</h2><br/>"
                    + "<p>You have requested to reset your password on MyBlog.</p>"
                    + "<p>Please click the link below to verify your email:</p>"
                    + $"<a href='{frontendUrl}/confirm?type=forgotPassword&token={confirmCode}'>Confirm Account</a><br/><br/>"
                    + $"<p>This link will expire in {emailTokenTimeout} minutes.</p><br/>"
                    + "<p>If you did not request a password reset, please ignore this email.</p>"
                    + "<br/><p>Best regards,<br/>The MyBlog Team</p>"
            );
        }
    }

    public async Task SendAccountRemovalEmailAsync(string email, string lang)
    {
        var selfRemoveDurationDays = _settings.SelfRemoveDurationDays;

        if (lang == "vn")
        {
            await SendEmailAsync(
                to: email,
                subject: "Xác nhận xóa tài khoản MyBlog",
                body: "<h2>Xin chào</h2><br/>"
                    + "<p>Bạn đã yêu cầu xóa tài khoản trên MyBlog.</p>"
                    + $"<p>Tài khoản của bạn sẽ bị xóa vĩnh viễn sau {selfRemoveDurationDays} ngày kể từ hôm nay.</p><br/>"
                    + "<p>Nếu bạn không yêu cầu xóa tài khoản này, vui lòng đăng nhập lại để hủy yêu cầu xóa tài khoản.</p>"
                    + "<br/><p>Trân trọng,<br/>Đội ngũ MyBlog</p>"
            );
        }
        else if (lang == "ja")
        {
            await SendEmailAsync(
                to: email,
                subject: "MyBlogアカウント削除確認",
                body: "<h2>こんにちは</h2><br/>"
                    + "<p>MyBlogでアカウントの削除をリクエストしました。</p>"
                    + $"<p>あなたのアカウントは今日から{selfRemoveDurationDays}日後に完全に削除されます。</p><br/>"
                    + "<p>このアカウント削除をリクエストしなかった場合は、再度ログインしてリクエストをキャンセルしてください。</p>"
                    + "<br/><p>よろしくお願いいたします。<br/>MyBlogチーム</p>"
            );
        }
        else // default to English
        {
            await SendEmailAsync(
                to: email,
                subject: "MyBlog Account Removal Confirmation",
                body: "<h2>Hello</h2><br/>"
                    + "<p>You have requested to delete your MyBlog account.</p>"
                    + $"<p>Your account will be permanently deleted in {selfRemoveDurationDays} days from today.</p><br/>"
                    + "<p>If you did not request this account deletion, please log in again to cancel the deletion request.</p>"
                    + "<br/><p>Best regards,<br/>The MyBlog Team</p>"
            );
        }
    }
}
