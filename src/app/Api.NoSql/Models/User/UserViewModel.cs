using System.Text.Json.Serialization;

namespace Api.Models.User
{
    public class UserViewModel
    {
        [JsonPropertyName("_id")]
        public string Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
    }
}
