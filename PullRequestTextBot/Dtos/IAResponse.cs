using System.Text.Json.Serialization;

namespace PullRequestTextBot.Dtos
{
    public record IAResponse
    {
        [JsonPropertyName("response")]
        public string Response { get; set; } = string.Empty;

        [JsonPropertyName("done")]
        public bool Done { get; set; }
    }
}
