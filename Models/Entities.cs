using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace AcxiomCRM.Models;

public class ApplicationUser : IdentityUser
{
    [Required, MaxLength(120)]
    public string Name { get; set; } = "";
    public bool IsActive { get; set; } = true;
}

public class Customer
{
    public int CustomerId { get; set; }
    [Required, MaxLength(120)] public string CustomerName { get; set; } = "";
    [Required, EmailAddress, MaxLength(160)] public string Email { get; set; } = "";
    [Required, Phone, MaxLength(20)] public string Phone { get; set; } = "";
    [MaxLength(200)] public string CompanyName { get; set; } = "";
    [MaxLength(300)] public string Address { get; set; } = "";
    [MaxLength(80)] public string City { get; set; } = "";
    [MaxLength(80)] public string State { get; set; } = "";
    [Required, MaxLength(30)] public string Status { get; set; } = "Active";
    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
    public string? CreatedBy { get; set; }
}

public class Lead
{
    public int LeadId { get; set; }
    [Required, MaxLength(120)] public string LeadName { get; set; } = "";
    [Required, EmailAddress, MaxLength(160)] public string Email { get; set; } = "";
    [Required, Phone, MaxLength(20)] public string Phone { get; set; } = "";
    [MaxLength(200)] public string CompanyName { get; set; } = "";
    [Required, MaxLength(80)] public string Source { get; set; } = "Website";
    [Required, MaxLength(30)] public string Status { get; set; } = "New";
    [Required, MaxLength(30)] public string Priority { get; set; } = "Medium";
    [Range(0, double.MaxValue)] public decimal ExpectedValue { get; set; }
    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
    public string? AssignedTo { get; set; }
}

public class Opportunity
{
    public int OpportunityId { get; set; }
    [Required, MaxLength(160)] public string OpportunityName { get; set; } = "";
    public int? CustomerId { get; set; }
    public int? LeadId { get; set; }
    [Range(0.01, double.MaxValue)] public decimal Amount { get; set; }
    [Required, MaxLength(30)] public string Stage { get; set; } = "Qualification";
    [Range(0, 100)] public int Probability { get; set; } = 20;
    [DataType(DataType.Date)] public DateTime ExpectedCloseDate { get; set; } = DateTime.Today.AddDays(30);
    [Required, MaxLength(20)] public string Status { get; set; } = "Open";
    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
    public string? AssignedTo { get; set; }
    [MaxLength(1000)] public string? Notes { get; set; }
}

public class FollowUp
{
    public int FollowUpId { get; set; }
    public int? CustomerId { get; set; }
    public int? LeadId { get; set; }
    [DataType(DataType.Date), Required] public DateTime FollowUpDate { get; set; } = DateTime.Today;
    [Required, MaxLength(30)] public string FollowUpType { get; set; } = "Call";
    [MaxLength(1000)] public string Remarks { get; set; } = "";
    [Required, MaxLength(30)] public string Status { get; set; } = "Planned";
    public string? AssignedTo { get; set; }
}

public class Activity
{
    public int ActivityId { get; set; }
    [Required, MaxLength(30)] public string ActivityType { get; set; } = "Call";
    [Required, MaxLength(160)] public string Subject { get; set; } = "";
    [MaxLength(2000)] public string Description { get; set; } = "";
    public DateTime ActivityDate { get; set; } = DateTime.UtcNow;
    public int? CustomerId { get; set; }
    public int? LeadId { get; set; }
    public string? AssignedTo { get; set; }
    [Required, MaxLength(30)] public string Status { get; set; } = "Open";
}

public class AuditLog
{
    public int AuditLogId { get; set; }
    public string? UserId { get; set; }
    [Required, MaxLength(60)] public string Action { get; set; } = "";
    [Required, MaxLength(80)] public string EntityName { get; set; } = "";
    public string? RecordId { get; set; }
    public string? OldValue { get; set; }
    public string? NewValue { get; set; }
    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
    [MaxLength(64)] public string? IpAddress { get; set; }
    [MaxLength(30)] public string Result { get; set; } = "Success";
    [MaxLength(1000)] public string? Details { get; set; }
}
