namespace Api.Core.Services.Infrastructure.Models
{
    public class GoogleAuthModel
    {
        public bool IsValid { get; set; }
        public PayloadModel Payload { get; set; }

        public class PayloadModel
        {
            public string Email { get; set; }
            public string GivenName { get; set; }
            public string FamilyName { get; set; }
        }
    }
}
