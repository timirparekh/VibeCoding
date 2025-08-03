namespace VibeCoding.Models;

public class FinalReport
{
    public bool IsInitialLoad { get; set; } = true;
    public string Client { get; set; }
    public string AuditType { get; set; }
    public string AuditPeriod { get; set; }
    public string AuditTeamLead { get; set; }
    public string AuditManager { get; set; }
    public List<string> EngagementAndOnboarding { get; set; } = new List<string>();
    public List<string> AuditPlanning { get; set; } = new List<string>();
    public List<string> FieldworkExecution { get; set; } = new List<string>();
    public List<string> Reporting { get; set; } = new List<string>();
    public List<string> ComplianceAndConfidentiality { get; set; } = new List<string>();
    public List<string> PostAuditReview { get; set; } = new List<string>();
    public List<string> Attachments { get; set; } = new List<string>();
    public List<string> FollowUp { get; set; } = new List<string>();
    public List<string> MajorObservations { get; set; } = new List<string>();
}