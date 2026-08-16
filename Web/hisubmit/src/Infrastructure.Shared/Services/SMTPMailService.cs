using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using HiSubmit.Application.Configurations;
using HiSubmit.Application.Interfaces.Services;
using HiSubmit.Application.Requests.Mail;
using MailKit.Security;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;
using System.Threading.Tasks;
using MailKit.Net.Imap;


namespace HiSubmit.Infrastructure.Shared.Services
{
    public class SMTPMailService : IMailService
    {
        private readonly MailConfiguration _config;
        private readonly ILogger<SMTPMailService> _logger;

        public SMTPMailService(IOptions<MailConfiguration> config, ILogger<SMTPMailService> logger)
        {
            _config = config.Value;
            _logger = logger;
        }

        public async Task SendAsync(MailRequest request)
        {
            try
            {
                var fromAddress = request.From ?? _config.From;
                var client = new SmtpClient(_config.Host, _config.Port);
                client.EnableSsl = true;
                client.Credentials = new NetworkCredential(_config.UserName, _config.Password);
                var from = new MailAddress(fromAddress,"HiSubmit");
                var to = new MailAddress(request.To);
                var t = GenerateAttach(request.Attachments);
                var message = new MailMessage(from, to)
                {
                    
                    Body = request.Body,
                    BodyEncoding = Encoding.UTF8,
                    Subject = request.Subject,
                    SubjectEncoding = Encoding.UTF8,
                    IsBodyHtml = true,
                };
                foreach (var attach in t)
                {
                    message.Attachments.Add(attach);
                }
                
                await client.SendMailAsync(message);
                
                message.Dispose();
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex.Message, ex);
            }
        }

        private List<Attachment> GenerateAttach(List<MailAttachment> mailAttachment)
        {
            return mailAttachment.Select(attachMail => new Attachment(new MemoryStream(attachMail.File), attachMail.Name)).ToList();
        }
    }
   
}