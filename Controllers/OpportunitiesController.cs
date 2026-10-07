using AcxiomCRM.Data;
using AcxiomCRM.Models;
using AcxiomCRM.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AcxiomCRM.Controllers;

[Authorize]
public class OpportunitiesController : Controller
{
    private readonly ApplicationDbContext _db; private readonly AuditService _audit; private readonly ScopeService _scope;
    public OpportunitiesController(ApplicationDbContext db,AuditService audit,ScopeService scope){_db=db;_audit=audit;_scope=scope;}
    public async Task<IActionResult>Index(){var q=_db.Opportunities.AsNoTracking();if(_scope.IsSalesExecutive)q=q.Where(x=>x.AssignedTo==_scope.UserName);return View(await q.OrderByDescending(x=>x.CreatedDate).ToListAsync());}
    public IActionResult Create()=>View(new Opportunity());
    private void ValidateBusiness(Opportunity m){if(m.Amount<=0)ModelState.AddModelError(nameof(m.Amount),"Opportunity Amount must be greater than 0.");if(m.Probability<0||m.Probability>100)ModelState.AddModelError(nameof(m.Probability),"Probability must be between 0 and 100.");if(m.Status=="Open"&&m.ExpectedCloseDate.Date<DateTime.Today)ModelState.AddModelError(nameof(m.ExpectedCloseDate),"Expected Close Date cannot be in the past.");}
    [HttpPost,ValidateAntiForgeryToken] public async Task<IActionResult>Create(Opportunity m){ValidateBusiness(m);if(!ModelState.IsValid)return View(m);m.CreatedDate=DateTime.UtcNow;m.AssignedTo=User.Identity?.Name;_db.Add(m);await _db.SaveChangesAsync();await _audit.LogAsync("Create","Opportunity",m.OpportunityId.ToString(),newValue:m);return RedirectToAction(nameof(Index));}
    public async Task<IActionResult>Edit(int id){var m=await _db.Opportunities.FindAsync(id);if(m is null)return NotFound();if(_scope.IsSalesExecutive&&m.AssignedTo!=_scope.UserName)return Forbid();return View(m);}
    [HttpPost,ValidateAntiForgeryToken] public async Task<IActionResult>Edit(int id,Opportunity m){if(id!=m.OpportunityId)return BadRequest();var existing=await _db.Opportunities.FindAsync(id);if(existing is null)return NotFound();if(_scope.IsSalesExecutive&&existing.AssignedTo!=_scope.UserName)return Forbid();ValidateBusiness(m);if(!ModelState.IsValid)return View(m);var old=new{existing.OpportunityName,existing.Amount,existing.Probability,existing.Stage,existing.Status,existing.ExpectedCloseDate};existing.OpportunityName=m.OpportunityName;existing.Amount=m.Amount;existing.Probability=m.Probability;existing.Stage=m.Stage;existing.Status=m.Status;existing.ExpectedCloseDate=m.ExpectedCloseDate;existing.CustomerId=m.CustomerId;existing.LeadId=m.LeadId;existing.Notes=m.Notes;await _db.SaveChangesAsync();await _audit.LogAsync("Update","Opportunity",id.ToString(),old,existing);return RedirectToAction(nameof(Index));}
}
