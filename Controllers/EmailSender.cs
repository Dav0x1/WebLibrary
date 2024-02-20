using System.Net.Mail;
using System.Net;
using System.Text;

namespace WebLibrary.Controllers
{
    public class EmailSender
    {
        private readonly IConfiguration _configuration;
        SmtpClient client;

        public EmailSender(IConfiguration configuration) {
            _configuration = configuration;

            client = new SmtpClient(_configuration.GetValue<string>("EmailService:host"), _configuration.GetValue<int>("EmailService:port"));
            client.EnableSsl = true;
            client.UseDefaultCredentials = false;
            client.Credentials = new NetworkCredential(_configuration.GetValue<string>("EmailService:login"), _configuration.GetValue<string>("EmailService:password"));
        }

        public void remindPassword(string toEmail, string password)
        {
            // Create email message
            MailMessage mailMessage = new MailMessage();
            mailMessage.From = new MailAddress(_configuration.GetValue<string>("EmailService:login"));
            mailMessage.To.Add(toEmail);
            mailMessage.Subject = "WebLibrary - Password";
            mailMessage.IsBodyHtml = true;

            StringBuilder mailBody = new StringBuilder();
            mailBody.AppendFormat("<p>Dear User,</p>");
            mailBody.AppendFormat("<p>We have sent your login information for your WebLibrary account. Please use the following password to log in:</p>");
            mailBody.AppendFormat($"<p><strong>Password:</strong> {password}</p>");
            mailBody.AppendFormat("<p>Thank you for using WebLibrary.</p>");
            mailMessage.Body = mailBody.ToString();

            // Send email
            client.Send(mailMessage);
        }
    }
}
