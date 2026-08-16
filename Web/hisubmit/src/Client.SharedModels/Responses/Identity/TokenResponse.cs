using System;

namespace Hisubmit.Client.SharedModels.Responses.Identity
{
    public class TokenResponse
    {
        public string Token { get; set; }
        public string RefreshToken { get; set; }
        public string UserImageURL { get; set; }
        public  DateTime TokenExpiryTime { get; set; }
        public DateTime RefreshTokenExpiryTime { get; set; }
        public bool GoToVerification { get; set; }
    }
}