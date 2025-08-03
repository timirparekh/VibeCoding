using System.Threading.Tasks;
using Azure;
using Azure.AI.OpenAI;
using OpenAI.Chat;

namespace VibeCoding.LLM;

public class OpenAI
{
    private readonly IConfiguration configuration;
    private readonly string endPoint;
    private readonly string apiKey;
    private readonly string model;

    public OpenAI(IConfiguration configuration)
    {
        this.configuration = configuration;
        this.endPoint = configuration["OpenAI:EndPoint"] ?? string.Empty;
        this.apiKey = configuration["OpenAI:ApiKey"] ?? string.Empty;
        this.model = configuration["OpenAI:Model"] ?? string.Empty;
    }

    public async Task<string> GetSummary(string docxText)
    {
        AzureOpenAIClient azureClient = new AzureOpenAIClient(new Uri(endPoint), new AzureKeyCredential(apiKey));
        ChatClient chatClient = azureClient.GetChatClient(model);

        var requestOptions = new ChatCompletionOptions
        {
            Temperature = 1.0f,
            TopP = 1.0f,
            FrequencyPenalty = 0.0f,
            PresencePenalty = 0.0f,
        };

        List<ChatMessage> messages = new List<ChatMessage>
        {
            new SystemChatMessage("You are a large language model trained by OpenAI. You can provide answers to questions as per the output format."),
            new UserChatMessage(@"Output format:
            ```json
            {
            ""Client"": """",
            ""Audit Type"": """",
            ""Audit Period"": """",
            ""Audit Team Lead"": """",
            ""Audit Manager"": """",
            ""Engagement & Onboarding"": [],
            ""Audit Planning"": [],
            ""Fieldwork Execution"": [],
            ""Reporting"": [],
            ""Compliance and Confidentiality"": [],
            ""Post-Audit Review"": [],
            ""Attachments"": [],
            ""Follow up"": [],
            ""Any major observations"": [],
            }
            ```
            Text to summarize is here.
            
            Output:"
            ),
        };

        var response = await chatClient.CompleteChatAsync(messages, requestOptions);
        var jsonResponse = response.Value.Content[0].Text;
        return jsonResponse;
    }
}
