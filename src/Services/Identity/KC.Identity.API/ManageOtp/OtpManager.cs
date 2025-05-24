using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using KC.Application.Common.Cacheing;
using KC.Utils.Common;

namespace KC.Identity.API.ManageOtp
{
    public class OtpManager : IOtpManager
    {
        private readonly ICache _cache;

        public OtpManager(ICache cache)
        {
            _cache = cache;
        }

        public async Task<int> GenerateOTPAsync(string phoneNumber)
        {
            //Generate 6 digit random number

            int otpNumber = NumberUtil.GenerateRandomNumber(100000, 999999);

            //Setting up timeout to 30 minutes
            var offset = DateTimeOffset.UtcNow.AddMinutes(30);

            string cacheKey = $"MFAOtp_{Regex.Replace(phoneNumber, "[^0-9]", "", RegexOptions.NonBacktracking)}";

            await _cache.SetAsync(cacheKey, otpNumber, offset);

            return otpNumber;
        }

        public async Task<bool> VerifyOTPAsync(int otp, string phoneNumber)
        {
            string cacheKey = $"MFAOtp_{Regex.Replace(phoneNumber, "[^0-9]", "", RegexOptions.NonBacktracking)}";

            var otpNumber = await _cache.GetAsync<int>(cacheKey);

            return otpNumber > 0 && otpNumber == otp;
        }
    }
}
