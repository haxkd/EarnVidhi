using System.Net.Mail;
using System.Net;

public class MailLogic
{
    public IConfiguration _configuration;

    private readonly IHttpContextAccessor _httpContextAccessor;

    public MailLogic(IConfiguration config, IHttpContextAccessor httpContextAccessor)
    {
        _configuration = config;
        _httpContextAccessor = httpContextAccessor;
    }
    public void SendOtpMail(string Verifycode, string ToEmail)
    {

        string VerifyLink = GetDomainName() + "/Auth/Verify?token=" + Verifycode;
        string body = string.Empty;
        using (StreamReader reader = new StreamReader(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "MailTemplates", "VerifyEmail.html")))
        {
            body = reader.ReadToEnd();
        }

        body = body.Replace("{VerifyLink}", VerifyLink);
        body = body.Replace("{HOST}", GetDomainName());

        string? smtpHost = _configuration["AdminEmail:smtpHost"];
        int smtpPort = int.Parse(_configuration["AdminEmail:smtpPort"]);
        string? smtpUserName = _configuration["AdminEmail:smtpUserName"];
        string? smtpPassword = _configuration["AdminEmail:smtpPassword"];
        bool enableSsl = bool.Parse(_configuration["AdminEmail:enableSsl"]);

        SendMail("Verify Email", body, smtpHost, smtpPort, smtpUserName, smtpPassword, enableSsl, ToEmail);
    }
    public void SendMail(string subject, string msg, string smtpHost, int smtpPort, string smtpUserName, string smtpPassword, bool enableSsl, string toEmail)
    {

        SmtpClient smtpClient = new SmtpClient(smtpHost, smtpPort);
        smtpClient.Credentials = new NetworkCredential(smtpUserName, smtpPassword);
        smtpClient.EnableSsl = enableSsl;
        MailMessage mailMessage = new MailMessage();
        mailMessage.From = new MailAddress(smtpUserName);
        mailMessage.To.Add(toEmail);
        mailMessage.Subject = subject;
        mailMessage.Body = msg;
        mailMessage.IsBodyHtml = true;
        smtpClient.Send(mailMessage);
    }


    private string GetDomainName()
    {
        var request = _httpContextAccessor.HttpContext.Request;
        string domainName = request.Host.Host;
        return domainName;
    }
}