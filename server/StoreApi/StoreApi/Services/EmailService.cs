using StoreApi.Repositories;
using StoreApi.DTOs;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;

namespace StoreApi.Services
{
    /// <summary>
    /// SERVICE LAYER - Handles email sending logic for raffle winners
    /// This service communicates with the EmailRepository (Data Layer) and is called by GiftService
    /// </summary>
    public class EmailService : IEmailService
    {
        private readonly IEmailRepository _repository;
        private readonly IConfiguration _config;

        public EmailService(IEmailRepository repository, IConfiguration config)
        {
            _repository = repository;
            _config = config;
        }

        /// <summary>
        /// Sends a congratulation email to the raffle winner
        /// Called automatically when someone wins a raffle in GiftService
        /// </summary>
        /// <param name="giftId">The ID of the gift that was won</param>
        public async Task SendWinnerNotificationAsync(int giftId)
        {
            Console.WriteLine($"[EMAIL] Starting email for gift {giftId}");

            var details = await _repository.GetWinnerDetailsAsync(giftId);
            Console.WriteLine($"[EMAIL] Winner details: name={details?.WinnerName}, email={details?.Email}, gift={details?.GiftName}");

            if (details == null)
                throw new Exception("Winner details not found for gift ID: " + giftId);

            var senderEmail = _config["EmailSettings:SenderEmail"]!;
            var appPassword = _config["EmailSettings:AppPassword"]!;
            Console.WriteLine($"[EMAIL] Sender: {senderEmail}, Password length: {appPassword?.Length}");

            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("Raffle System", senderEmail));
            message.To.Add(new MailboxAddress(details.WinnerName, details.Email));
            message.Subject = "🎉 מזל טוב! זכית בהגרלה!";

            string htmlBody = $@"
<!DOCTYPE html>
<html dir=""rtl"" lang=""he"">
<head>
  <meta charset=""UTF-8"" />
  <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"" />
</head>
<body style=""margin:0;padding:0;background:#0a0a1a;font-family:Arial,sans-serif;"">
  <table width=""100%"" cellpadding=""0"" cellspacing=""0"" style=""background:#0a0a1a;padding:40px 0;"">
    <tr>
      <td align=""center"">
        <table width=""600"" cellpadding=""0"" cellspacing=""0"" style=""background:#12122a;border-radius:16px;overflow:hidden;border:1px solid #6c63ff33;"">

          <!-- Header -->
          <tr>
            <td style=""background:linear-gradient(135deg,#6c63ff,#a855f7);padding:40px 30px;text-align:center;"">
              <div style=""font-size:56px;margin-bottom:12px;"">🏆</div>
              <h1 style=""color:#fff;margin:0;font-size:32px;font-weight:bold;letter-spacing:1px;"">מזל טוב!</h1>
              <p style=""color:#e0d7ff;margin:8px 0 0;font-size:16px;"">המכירה הסינית 2026</p>
            </td>
          </tr>

          <!-- Body -->
          <tr>
            <td style=""padding:40px 40px 30px;text-align:right;"">
              <p style=""color:#c0b8f0;font-size:17px;margin:0 0 10px;"">שלום,</p>
              <h2 style=""color:#fff;font-size:24px;margin:0 0 24px;"">{details.WinnerName}</h2>

              <p style=""color:#a09ac8;font-size:15px;line-height:1.7;margin:0 0 28px;"">
                אנחנו שמחים לבשר לך כי <strong style=""color:#a855f7;"">זכית</strong> בהגרלת המכירה הסינית!
              </p>

              <!-- Gift Card -->
              <table width=""100%"" cellpadding=""0"" cellspacing=""0"" style=""background:#1e1b4b;border-radius:12px;margin-bottom:32px;border:1px solid #6c63ff55;"">
                <tr>
                  <td style=""padding:24px 28px;"">
                    <p style=""color:#a09ac8;font-size:13px;margin:0 0 6px;text-transform:uppercase;letter-spacing:1px;"">המתנה שזכית בה</p>
                    <p style=""color:#fff;font-size:22px;font-weight:bold;margin:0;"">🎁 {details.GiftName}</p>
                  </td>
                </tr>
              </table>

              <p style=""color:#a09ac8;font-size:15px;line-height:1.7;margin:0 0 8px;"">
                אנא צור/י איתנו קשר בהקדם לתיאום קבלת המתנה.
              </p>
            </td>
          </tr>

          <!-- Footer -->
          <tr>
            <td style=""background:#0d0d20;padding:24px 40px;text-align:center;border-top:1px solid #6c63ff22;"">
              <p style=""color:#6c63ff;font-size:20px;font-weight:bold;margin:0 0 4px;"">✨ המכירה הסינית 2026 ✨</p>
              <p style=""color:#4a4570;font-size:12px;margin:0;"">מייל זה נשלח אוטומטית — אין להשיב אליו</p>
            </td>
          </tr>

        </table>
      </td>
    </tr>
  </table>
</body>
</html>";

            var bodyBuilder = new BodyBuilder { HtmlBody = htmlBody };
            message.Body = bodyBuilder.ToMessageBody();

            Console.WriteLine("[EMAIL] Connecting to SMTP...");
            using var client = new SmtpClient();
            await client.ConnectAsync("smtp.gmail.com", 587, SecureSocketOptions.StartTls);
            Console.WriteLine("[EMAIL] Connected. Authenticating...");
            await client.AuthenticateAsync(senderEmail, appPassword);
            Console.WriteLine("[EMAIL] Authenticated. Sending...");
            await client.SendAsync(message);
            await client.DisconnectAsync(true);
            Console.WriteLine("[EMAIL] Sent successfully!");
        }

        /// <summary>
        /// Gets the winner details needed for composing the email
        /// </summary>
        public async Task<WinnerDetailsDto> GetWinnerDetailsAsync(int giftId)
        {
            return await _repository.GetWinnerDetailsAsync(giftId);
        }
    }
}