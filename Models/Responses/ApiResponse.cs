using System.Text.Json.Serialization;

namespace TodoApi.Models.Responses;

public class ApiResponse
{
    [JsonPropertyName("status")]
    public int Status {get; set;} = 200;

    [JsonPropertyName("message")]
    public string Message {get;set;} = "Success";

    [JsonPropertyName("data")]
    public object? Data {get;set;}
}