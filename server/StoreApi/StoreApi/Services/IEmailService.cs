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

        /// <summary>
        /// Gets the winner details needed for the email
        /// </summary>
        /// <param name="giftId">The ID of the gift</param>
        /// <returns>Winner details including name, email, and gift name</returns>
        Task<WinnerDetailsDto> GetWinnerDetailsAsync(int giftId);
    }
}
