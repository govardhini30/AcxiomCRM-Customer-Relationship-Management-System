using AcxiomCRM.Data;
using AcxiomCRM.ViewModels;
using AcxiomCRM.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AcxiomCRM.Controllers;

[Authorize]
public class DashboardController : Controller
{
    private readonly ApplicationDbContext _db; private readonly ScopeService _scope;
    public DashboardController(ApplicationDbContext db, ScopeService scope) { _db = db; _scope = scope; }

    public async Task<IActionResult> Index()
    {
        var customers = _db.Customers.AsQueryable();
        var leads = _db.Leads.AsQueryable();
        var opportunities = _db.Opportunities.AsQueryable();
        if (_scope.IsSalesExecutive) { customers = customers.Where(x => x.CreatedBy == _scope.UserName); leads = leads.Where(x => x.AssignedTo == _scope.UserName); opportunities = opportunities.Where(x => x.AssignedTo == _scope.UserName); }
        var vm = new DashboardVM
        {
            Customers = await customers.CountAsync(),
            Leads = await leads.CountAsync(),
            OpenLeads = await leads.CountAsync(x => x.Status != "Lost" && x.Status != "Converted"),
            Opportunities = await opportunities.CountAsync(),
            OpenOpportunities = await opportunities.CountAsync(x => x.Status == "Open"),
            Won = await opportunities.CountAsync(x => x.Status == "Won"),
            Lost = await opportunities.CountAsync(x => x.Status == "Lost"),
            Pipeline = await opportunities.Where(x => x.Status == "Open").SumAsync(x => (decimal?)x.Amount * x.Probability / 100) ?? 0
        };
        vm.LeadStatus = await leads.GroupBy(x => x.Status).ToDictionaryAsync(g => g.Key, g => g.Count());
        vm.OpportunityPipeline = await opportunities.GroupBy(x => x.Stage).ToDictionaryAsync(g => g.Key, g => g.Sum(x => x.Amount));
        return View(vm);
    }
}
