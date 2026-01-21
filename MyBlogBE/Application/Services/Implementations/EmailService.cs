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

    public async Task SendRegisterEmailAsync(string email, string username, string confirmCode)
    {
        var emailTokenTimeout = _settings.TokenExpiryMinutes;

        await SendEmailAsync(
            to: email,
            subject: "Xác nhận đăng ký tài khoản MyBlog",
            body: $"<h2>Xin chào {username}</h2><br/>"
                + "<p>Bạn đã đăng ký tài khoản thành công trên MyBlog.</p>"
                + "<p>Vui lòng hãy nhấn vào liên kết này để xác thực email:</p>"
                + $"<a href='https://myblog.example.com/confirm?type=register&token={confirmCode}'>Xác nhận tài khoản</a><br/><br/>"
                + $"<p>Đường link này chỉ tồn tại trong {emailTokenTimeout} phút.</p><br/>"
                + "<p>Nếu bạn không đăng ký tài khoản này, vui lòng bỏ qua email này.</p>"
                + "<br/><p>Trân trọng,<br/>Đội ngũ MyBlog</p>"
        );
    }

    public async Task SendForgotPasswordEmailAsync(
        string email,
        string username,
        string confirmCode
    )
    {
        var emailTokenTimeout = _settings.TokenExpiryMinutes;

        await SendEmailAsync(
            to: email,
            subject: "Xác nhận quên mật khẩu tài khoản MyBlog",
            body: $"<h2>Xin chào {username}</h2><br/>"
                + "<p>Bạn đã quên mật khẩu trên MyBlog.</p>"
                + "<p>Vui lòng hãy nhấn vào liên kết này để xác thực email:</p>"
                + $"<a href='https://myblog.example.com/confirm?type=forgotPassword&token={confirmCode}'>Xác nhận tài khoản</a><br/><br/>"
                + $"<p>Đường link này chỉ tồn tại trong {emailTokenTimeout} phút.</p><br/>"
                + "<p>Nếu bạn không sử dụng quên mật khẩu, vui lòng bỏ qua email này.</p>"
                + "<br/><p>Trân trọng,<br/>Đội ngũ MyBlog</p>"
        );
    }

    public async Task SendAccountRemovalEmailAsync(string email)
    {
        var selfRemoveDurationDays = _settings.SelfRemoveDurationDays;
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
}
