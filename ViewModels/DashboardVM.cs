namespace AcxiomCRM.ViewModels;

public class DashboardVM
{
    public int Customers { get; set; }
    public int Leads { get; set; }
    public int OpenLeads { get; set; }
    public int Opportunities { get; set; }
    public int OpenOpportunities { get; set; }
    public int Won { get; set; }
    public int Lost { get; set; }
    public decimal Pipeline { get; set; }
    public Dictionary<string, int> LeadStatus { get; set; } = new();
    public Dictionary<string, decimal> OpportunityPipeline { get; set; } = new();
}

public class LoginVM
{
    [System.ComponentModel.DataAnnotations.Required, System.ComponentModel.DataAnnotations.EmailAddress]
    public string Email { get; set; } = "";
    [System.ComponentModel.DataAnnotations.Required, System.ComponentModel.DataAnnotations.DataType(System.ComponentModel.DataAnnotations.DataType.Password)]
    public string Password { get; set; } = "";
    public bool RememberMe { get; set; }
}

public class RegisterVM
{
    [System.ComponentModel.DataAnnotations.Required, System.ComponentModel.DataAnnotations.MaxLength(120)] public string Name { get; set; } = "";
    [System.ComponentModel.DataAnnotations.Required, System.ComponentModel.DataAnnotations.EmailAddress] public string Email { get; set; } = "";
    [System.ComponentModel.DataAnnotations.Required, System.ComponentModel.DataAnnotations.DataType(System.ComponentModel.DataAnnotations.DataType.Password)] public string Password { get; set; } = "";
    [System.ComponentModel.DataAnnotations.Required, System.ComponentModel.DataAnnotations.Compare(nameof(Password)), System.ComponentModel.DataAnnotations.DataType(System.ComponentModel.DataAnnotations.DataType.Password)] public string ConfirmPassword { get; set; } = "";
}
