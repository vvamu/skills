
using EmailProvider.Interfaces;
using EmailProvider.Models;
using EmailProvider.Options;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using MimeKit;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace EmailProvider;

public class MailService : IMailService
{
    private readonly MailOptions _mailOptions;
    public MailService(IOptionsSnapshot<MailOptions> options)
    {
        _mailOptions = options.Value;
    }

    #region Common Send Message

    public async Task SendEmailAsync(Mailrequest mailrequest)
    {
        //CheckMessage(mailrequest);
        var email = await CreateMessage(mailrequest);
        var builder = new BodyBuilder();

        if (mailrequest.File != null)
            await AddFileToMessage(builder, mailrequest.File);

        builder.TextBody = mailrequest.TextBody ?? "";
        builder.HtmlBody = mailrequest.HtmlBody ?? "";

        email.Body = builder.ToMessageBody();

        var smtp = new SmtpClient();
        smtp.Connect(_mailOptions.Host, _mailOptions.Port, SecureSocketOptions.StartTls);
        smtp.Authenticate(_mailOptions.Email, _mailOptions.Password);
        await smtp.SendAsync(email);
        smtp.Disconnect(true);
    }
    public async Task<MimeMessage> CreateMessage(Mailrequest mailrequest)
    {
        var email = new MimeMessage();

        mailrequest.ToEmail = mailrequest.ToEmail ?? "6183866@gmail.com";
        mailrequest.Subject = mailrequest.Subject ?? "Welcome to NihiraTechiees";


        email.Sender = MailboxAddress.Parse(_mailOptions.Email);
        email.To.Add(MailboxAddress.Parse(mailrequest.ToEmail));
        email.Subject = mailrequest.Subject;
        Task.CompletedTask.Wait();

        return email;
    }

    #endregion

    #region SkillsHub

    public async Task SendEmailAsync(SendingMessage mailrequest)
    {

        var email = await CreateMessageForSkillsHub(mailrequest);

        var smtp = new SmtpClient();
        smtp.CheckCertificateRevocation = false;
        //smtp.ServerCertificateValidationCallback = (sender, certificate, chain, errors) => true;

        smtp.Connect(_mailOptions.Host, _mailOptions.Port, SecureSocketOptions.SslOnConnect);
        smtp.Authenticate(_mailOptions.Email, _mailOptions.Password);  
        await smtp.SendAsync(email);
        smtp.Disconnect(true);
    }
    public async Task<MimeMessage> CreateMessageForSkillsHub(SendingMessage msg)
    {
        msg.ToEmail = "skillshubinfo7@gmail.com";


        var builder = new BodyBuilder();
        builder.TextBody = $"Name: {msg.Name}\nEmail: {msg.Email}\nPhone: {msg.Phone}\nMessage: {msg.Data}";


        var email = new MimeMessage();
        email.Subject = "Message from website - " + msg.Date;
        email.Sender = MailboxAddress.Parse(_mailOptions.Email);
        email.To.Add(MailboxAddress.Parse(msg.ToEmail));
        email.Body = builder.ToMessageBody();

        return email;

    }

    #endregion


    private async Task AddFileToMessage(BodyBuilder builder, IFormFile file)
    {
        var fileExtension = Path.GetExtension(file.FileName);

        string currentDirectory = Directory.GetCurrentDirectory();
        string parentDirectory = Directory.GetParent(currentDirectory).FullName;

        var localPath = "\\EmailProvider\\Attachments\\";
        string filePath = parentDirectory + localPath + file.FileName;

        using (var fileStream = new FileStream(filePath, FileMode.Create))
        {
            await file.CopyToAsync(fileStream);
        }
        byte[] fileBytes = await File.ReadAllBytesAsync(filePath);
        builder.Attachments.Add("attachment" + builder.Attachments.Count + fileExtension, fileBytes, ContentType.Parse("application/octet-stream"));



        Task.CompletedTask.Wait();

    }
    private static void CheckMessage(Mailrequest mailrequest)
    {
        if (IsHtmlString(mailrequest.HtmlBody) && mailrequest.HtmlBody != null)
            throw new System.Exception("Not correct html");
    }
    private static bool IsHtmlString(string input)
    {
        string pattern = @"<[^>]+?>";
        return Regex.IsMatch(input, pattern);
    }
}