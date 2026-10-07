using System.Security.Claims;

namespace AcxiomCRM.Services;

public class ScopeService
{
    private readonly ClaimsPrincipal _user;
    public ScopeService(IHttpContextAccessor accessor) => _user = accessor.HttpContext?.User ?? new ClaimsPrincipal();
    public bool IsAdmin => _user.IsInRole("Admin");
    public bool IsManager => _user.IsInRole("Manager");
    public bool IsSalesExecutive => _user.IsInRole("SalesExecutive");
    public string UserName => _user.Identity?.Name ?? "";
}
