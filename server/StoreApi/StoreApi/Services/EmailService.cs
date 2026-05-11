using StoreApi.Repositories;
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
            try
            {
                // STEP 1: Get winner and gift details from the database (Repository Layer)
                var details = await _repository.GetWinnerDetailsAsync(giftId);

                if (details == null) 
                    throw new Exception("Winner details not found for gift ID: " + giftId);

                var senderEmail = _config["EmailSettings:SenderEmail"]!;
                var appPassword = _config["EmailSettings:AppPassword"]!;

                // STEP 2: Create the email message with winner details
                var message = new MimeMessage();
                message.From.Add(new MailboxAddress("Raffle System", senderEmail));
                message.To.Add(new MailboxAddress(details.WinnerName, details.Email));
                message.Subject = "Congratulations! You Won the Raffle";

                string emailBody = $@"
Congratulations {details.WinnerName}!

We wanted to let you know that you won the raffle for gift: {details.GiftName}

Please contact us to receive the gift.

Best regards,
Raffle System";

                message.Body = new TextPart("plain")
                {
                    Text = emailBody
                };

                // STEP 3: Send the email via SMTP
                using var client = new SmtpClient();
                await client.ConnectAsync("smtp.gmail.com", 587, SecureSocketOptions.StartTls);
                await client.AuthenticateAsync(senderEmail, appPassword);
                await client.SendAsync(message);
                await client.DisconnectAsync(true);
            }
            catch (Exception ex)
            {
                // Log the error but don't fail the lottery process
                Console.WriteLine($"Email sending failed for gift {giftId}: {ex.Message}");
                // TODO: Add proper logging here
            }
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