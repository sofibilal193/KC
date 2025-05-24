using System.Threading.Tasks;

namespace KC.Identity.API.ManageOtp
{
    public interface IOtpManager
    {
        Task<bool> VerifyOTPAsync(int otp, string phoneNumber);

        Task<int> GenerateOTPAsync(string phoneNumber);
    }
}
