using AcxiomCRM.Data;
using AcxiomCRM.Models;
using AcxiomCRM.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AcxiomCRM.Controllers;

[Authorize]
public class FollowUpsController : Controller
{
    private readonly ApplicationDbContext _db; private readonly AuditService _audit; private readonly ScopeService _scope;
    public FollowUpsController(ApplicationDbContext db,AuditService audit,ScopeService scope){_db=db;_audit=audit;_scope=scope;}
    public async Task<IActionResult>Index(){var q=_db.FollowUps.AsNoTracking();if(_scope.IsSalesExecutive)q=q.Where(x=>x.AssignedTo==_scope.UserName);return View(await q.OrderBy(x=>x.FollowUpDate).ToListAsync());}
    public IActionResult Create()=>View(new FollowUp());
    [HttpPost,ValidateAntiForgeryToken] public async Task<IActionResult>Create(FollowUp m){if(m.FollowUpDate.Date<DateTime.Today)ModelState.AddModelError(nameof(m.FollowUpDate),"Follow-up date cannot be earlier than today.");if(!ModelState.IsValid)return View(m);m.AssignedTo=User.Identity?.Name;m.Status="Planned";_db.Add(m);await _db.SaveChangesAsync();await _audit.LogAsync("Create","FollowUp",m.FollowUpId.ToString(),newValue:m);return RedirectToAction(nameof(Index));}
    [HttpPost,ValidateAntiForgeryToken] public async Task<IActionResult>Complete(int id){var m=await _db.FollowUps.FindAsync(id);if(m is null)return NotFound();if(_scope.IsSalesExecutive&&m.AssignedTo!=_scope.UserName)return Forbid();m.Status="Completed";await _db.SaveChangesAsync();await _audit.LogAsync("Update","FollowUp",id.ToString(),new{Status="Planned"},m);return RedirectToAction(nameof(Index));}
}
