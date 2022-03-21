using System.Text.Json.Serialization;

namespace Api.Sql.Models.User
{
    public class UserViewModel
    {
        [JsonPropertyName("_id")]
        public long Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
    }
}
