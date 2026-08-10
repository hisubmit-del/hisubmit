using Hisubmit.Client.SharedModels.Requests.Identity;

namespace Hisubmit.Client.SharedModels.Features.Users.Commands.Register;
    public class RegisterUserCommand:RegisterRequest
    {
        public string Origin { get; set; }
        public bool IsFestivalUser { get; set; }
    }
