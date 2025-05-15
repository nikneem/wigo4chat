using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Wigo4it.Chat.Client.OllamaClient.Services;

public class OllamaService : IOllamaService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<OllamaService> _logger;
    private readonly OllamaSettings _settings;

    public OllamaService(
        HttpClient httpClient,
        ILogger<OllamaService> logger,
        IOptions<OllamaSettings> options)
    {
        _httpClient = httpClient;
        _logger = logger;
        _settings = options.Value;
        
        // Set base address from config
        _httpClient.BaseAddress = new Uri(_settings.ApiEndpoint);
    }

    public async Task<string> GetCompletionAsync(string prompt)
    {
        try
        {
            _logger.LogInformation("Sending prompt to Ollama: {Prompt}", prompt);
            
            var request = new OllamaRequest
            {
                Model = _settings.Model,
                Prompt = prompt,
                Stream = false
            };
            
            var requestJson = JsonSerializer.Serialize(request);
            var content = new StringContent(requestJson, Encoding.UTF8, "application/json");
            
            var response = await _httpClient.PostAsync("/api/generate", content);
            response.EnsureSuccessStatusCode();
            
            var responseBody = await response.Content.ReadAsStringAsync();
            var ollamaResponse = JsonSerializer.Deserialize<OllamaResponse>(responseBody, 
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            
            return ollamaResponse?.Response ?? "No response from Ollama";
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error calling Ollama API");
            return $"Sorry, I encountered an error: {ex.Message}";
        }
    }
}

public interface IOllamaService
{
    Task<string> GetCompletionAsync(string prompt);
}

public class OllamaSettings
{
    public string ApiEndpoint { get; set; } = "http://localhost:11434";
    public string Model { get; set; } = "llama3";
}

public class OllamaRequest
{
    public string Model { get; set; } = "";
    public string Prompt { get; set; } = "";
    public bool Stream { get; set; } = false;
}

public class OllamaResponse
{
    public string Model { get; set; } = "";
    public string Response { get; set; } = "";
}