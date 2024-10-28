using MailKit.Net.Smtp;
using MimeKit;
using NETCore.MailKit.Core;
using Project_BDS.Application.HandleEmail;
using Project_BDS.Application.InterfaceService;
using Project_BDS.Application.Payloads.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IEmailService = Project_BDS.Application.InterfaceService.IEmailService;

namespace Project_BDS.Application.ImplementService
{
    public class EmailService : IEmailService
    {
        private readonly EmailConfiguration _configuration;
        public EmailService(EmailConfiguration configuration)
        {
            _configuration = configuration;
        }
        public string SendEmail(EmailMessage emailMessage)
        {
            var message = CreateEmailMessage(emailMessage);
            Send(message);
            var recipients = string.Join(", ", message.To);
            return ResponseMessage.GetEmailSuccessMessage(recipients);
        }

        public async Task<string> SendEmailAsync(EmailMessage emailMessage)
        {
            var message = CreateEmailMessage(emailMessage);
            await SendAsync(message);
            var recipients = string.Join(", ", message.To);
            return ResponseMessage.GetEmailSuccessMessage(recipients);
        }

        private MimeMessage CreateEmailMessage(EmailMessage message)
        {
            var emailMessage = new MimeMessage();
            emailMessage.From.Add(new MailboxAddress("email", _configuration.From));
            emailMessage.To.AddRange(message.To);
            emailMessage.Subject = message.Subject;
            emailMessage.Body = new TextPart(MimeKit.Text.TextFormat.Text) { Text = message.Content };
            return emailMessage;
        }
        private void Send(MimeMessage message)
        {
            using var client = new SmtpClient();
            try
            {
                client.Connect(_configuration.SmtpServer, _configuration.Port, true);
                client.AuthenticationMechanisms.Remove("XOAUTH2");
                client.Authenticate(_configuration.UserName, _configuration.Password);
                client.Send(message);
            }
            catch (Exception ex)
            {
                // Ghi log lỗi hoặc thông báo lỗi chi tiết
                Console.WriteLine($"Error sending email: {ex.Message}");
                throw;
            }
            finally
            {
                client.Disconnect(true);
                client.Dispose();
            }
        }

        private async Task SendAsync(MimeMessage message)
        {
            using var client = new SmtpClient();
            try
            {
                await client.ConnectAsync(_configuration.SmtpServer, _configuration.Port, true);
                client.AuthenticationMechanisms.Remove("XOAUTH2");
                await client.AuthenticateAsync(_configuration.UserName, _configuration.Password);
                await client.SendAsync(message);
            }
            catch (Exception ex)
            {
                // Ghi log lỗi hoặc thông báo lỗi chi tiết
                Console.WriteLine($"Error sending email: {ex.Message}");
                throw;
            }
            finally
            {
                await client.DisconnectAsync(true);
                client.Dispose();
            }
        }
    }
}
