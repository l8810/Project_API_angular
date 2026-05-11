using StoreApi.DTOs;

namespace StoreApi.Services
{
    /// <summary>
    /// SERVICE LAYER - Interface for email notification service
    /// Defines the contract for sending emails to raffle winners
    /// </summary>
    public interface IEmailService
    {
        /// <summary>
        /// Sends a congratulation email to the raffle winner
        /// </summary>
        /// <param name="giftId">The ID of the gift that was won</param>
        /// <returns>Task representing the asynchronous email sending operation</returns>
        Task SendWinnerNotificationAsync(int giftId);

        Task<WinnerDetailsDto> GetWinnerDetailsAsync(int giftId);

        Task SendPasswordResetEmailAsync(string toEmail, string userName, string resetLink);
    }
}
