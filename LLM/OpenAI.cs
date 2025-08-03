using System.Text.Json;
using Azure;
using Azure.AI.OpenAI;
using OpenAI.Chat;
using VibeCoding.Models;

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

    public async Task<FinalReport> GetSummary(string docxText)
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
        jsonResponse = jsonResponse.Replace("```json", string.Empty)
                                   .Replace("```", string.Empty)
                                   .Trim();

        return ParseJsonToFinalReport(jsonResponse);
    }

    public async Task<FinalReport> GetSummaryTest(string docxText)
    {
        return new FinalReport
        {
            IsInitialLoad = false,
            Client = "XYZ Manufacturing Ltd.",
            AuditType = "Financial Audit",
            AuditPeriod = "July 1, 2025 - December 31, 2025",
            AuditTeamLead = "Test Team Lead",
            AuditManager = "Test Manager",
            EngagementAndOnboarding = new List<string> { "Conducted KYC (Know Your Customer) procedures; documentation collected and verified",
                "KYC completed successfully", "Performed conflict-of-interest checks: no issues found.",
                "Obtained engagement letter signed by client on July 5, 2025." },
            AuditPlanning = new List<string> { "Audit plan prepared and approved on July 10, 2025",
                "Procedures to be audited identified.", "Risk assessment conducted - high risk noted in inventory reporting",
                "Roles and responsibilities assigned to team members.",
                "Time estimates and schedules determined; audit scheduled to start August 1." },
            FieldworkExecution = new List<string> { "Standard audit checklist followed.",
            "All supporting doucmentation (invoices, receipts, payroll logs) collected and verified.",
            "Weekly internal review meetings held; additional procedures discussed, planned and execute.",
            "Findings logged in audit workpapers."},
            Reporting = new List<string> { "Draft audit repot with executive summary, findings and recommendations prepared in January 2026.",
                "Quality Assurance Officer (QAO) reviewed report on January 10." },
            ComplianceAndConfidentiality = new List<string> { "Team signed confidentiality agreement with the client.",
                "All client data stored securely with encryption." },
            PostAuditReview = new List<string> { "Debrief meeting held on August 20.",
                "Key lesson: more rigorous checks needed in inventory management" },
            Attachments = new List<string> { "Engagement letter", "Audit Plan", "Risk assessment document", "Checklists and workpapers",
            "Correspondence logs", "Final reports", "Review notes and feedback" },
            FollowUp = new List<string> { "SOPs to be reviewed annually or after major/mandatory regulartory changes" },
            MajorObservations = new List<string> { "High risk identified in inventory reporting; more rigorous inventory management checks recommended" }
        };
    }

    private FinalReport ParseJsonToFinalReport(string json)
    {
        FinalReport report = new FinalReport();
        report.IsInitialLoad = false;
        var jsonOption = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            WriteIndented = true
        };
        var jsonResponse = JsonSerializer.Deserialize<Dictionary<string, object>>(json, jsonOption);

        if (jsonResponse != null)
        {
            if (jsonResponse.TryGetValue("Client", out var client)) report.Client = client?.ToString() ?? string.Empty;
            if (jsonResponse.TryGetValue("Audit Type", out var auditType)) report.AuditType = auditType?.ToString() ?? string.Empty;
            if (jsonResponse.TryGetValue("Audit Period", out var auditPeriod)) report.AuditPeriod = auditPeriod?.ToString() ?? string.Empty;
            if (jsonResponse.TryGetValue("Audit Team Lead", out var auditTeamLead)) report.AuditTeamLead = auditTeamLead?.ToString() ?? string.Empty;
            if (jsonResponse.TryGetValue("Audit Manager", out var auditManager)) report.AuditManager = auditManager?.ToString() ?? string.Empty;
            if (jsonResponse.TryGetValue("Engagement & Onboarding", out var engagementAndOnboarding) && engagementAndOnboarding is JsonElement onboardingElement)
            {
                report.EngagementAndOnboarding = onboardingElement.Deserialize<List<string>>() ?? new List<string>();
            }
            if (jsonResponse.TryGetValue("Audit Planning", out var auditPlanning) && auditPlanning is JsonElement planningElement)
            {
                report.AuditPlanning = planningElement.Deserialize<List<string>>() ?? new List<string>();
            }
            if (jsonResponse.TryGetValue("Fieldwork Execution", out var fieldworkExecution) && fieldworkExecution is JsonElement executionElement)
            {
                report.FieldworkExecution = executionElement.Deserialize<List<string>>() ?? new List<string>();
            }
            if (jsonResponse.TryGetValue("Reporting", out var reporting) && reporting is JsonElement reportingElement)
            {
                report.Reporting = reportingElement.Deserialize<List<string>>() ?? new List<string>();
            }
            if (jsonResponse.TryGetValue("Compliance and Confidentiality", out var complianceAndConfidentiality) && complianceAndConfidentiality is JsonElement complianceElement)
            {
                report.ComplianceAndConfidentiality = complianceElement.Deserialize<List<string>>() ?? new List<string>();
            }
            if (jsonResponse.TryGetValue("Post-Audit Review", out var postAuditReview) && postAuditReview is JsonElement reviewElement)
            {
                report.PostAuditReview = reviewElement.Deserialize<List<string>>() ?? new List<string>();
            }
            if (jsonResponse.TryGetValue("Attachments", out var attachments) && attachments is JsonElement attachmentsElement)
            {
                report.Attachments = attachmentsElement.Deserialize<List<string>>() ?? new List<string>();
            }
            if (jsonResponse.TryGetValue("Follow up", out var followUp) && followUp is JsonElement followUpElement)
            {
                report.FollowUp = followUpElement.Deserialize<List<string>>() ?? new List<string>();
            }
            if (jsonResponse.TryGetValue("Any major observations", out var majorObservations) && majorObservations is JsonElement observationsElement)
            {
                report.MajorObservations = observationsElement.Deserialize<List<string>>() ?? new List<string>();
            }
        }

        return report;
    }
}
