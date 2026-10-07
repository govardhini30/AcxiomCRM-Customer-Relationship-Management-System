using AcxiomCRM.Data;
using AcxiomCRM.Models;
using AcxiomCRM.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AcxiomCRM.Controllers;

[Authorize]
public class LeadsController : Controller
{
    private readonly ApplicationDbContext _db; private readonly AuditService _audit; private readonly ScopeService _scope;
    public LeadsController(ApplicationDbContext db, AuditService audit, ScopeService scope) { _db=db; _audit=audit; _scope=scope; }
    public async Task<IActionResult> Index(string? q) { var x=_db.Leads.AsNoTracking(); if (_scope.IsSalesExecutive) x=x.Where(l=>l.AssignedTo==_scope.UserName); if(!string.IsNullOrWhiteSpace(q)) x=x.Where(l=>l.LeadName.Contains(q)||l.CompanyName.Contains(q)||l.Status.Contains(q)||l.AssignedTo!.Contains(q)); return View(await x.OrderByDescending(l=>l.CreatedDate).ToListAsync()); }
    public IActionResult Create()=>View(new Lead());
    [HttpPost,ValidateAntiForgeryToken] public async Task<IActionResult>Create(Lead m){if(!ModelState.IsValid)return View(m);m.CreatedDate=DateTime.UtcNow;m.AssignedTo=User.Identity?.Name;_db.Add(m);await _db.SaveChangesAsync();await _audit.LogAsync("Create","Lead",m.LeadId.ToString(),newValue:m);return RedirectToAction(nameof(Index));}
    public async Task<IActionResult>Edit(int id){var m=await _db.Leads.FindAsync(id);if(m is null)return NotFound();if(_scope.IsSalesExecutive&&m.AssignedTo!=_scope.UserName)return Forbid();return View(m);}
    [HttpPost,ValidateAntiForgeryToken] public async Task<IActionResult>Edit(int id,Lead m){if(id!=m.LeadId)return BadRequest();var existing=await _db.Leads.FindAsync(id);if(existing is null)return NotFound();if(_scope.IsSalesExecutive&&existing.AssignedTo!=_scope.UserName)return Forbid();if(!ModelState.IsValid)return View(m);var old=new{existing.LeadName,existing.Email,existing.Phone,existing.Status,existing.Priority,existing.ExpectedValue};existing.LeadName=m.LeadName;existing.Email=m.Email;existing.Phone=m.Phone;existing.CompanyName=m.CompanyName;existing.Source=m.Source;existing.Status=m.Status;existing.Priority=m.Priority;existing.ExpectedValue=m.ExpectedValue;await _db.SaveChangesAsync();await _audit.LogAsync("Update","Lead",id.ToString(),old,existing);return RedirectToAction(nameof(Index));}
    [HttpPost,ValidateAntiForgeryToken,Authorize(Roles="Admin,Manager")]
    public async Task<IActionResult>Delete(int id){var m=await _db.Leads.FindAsync(id);if(m is null)return NotFound();_db.Remove(m);await _db.SaveChangesAsync();await _audit.LogAsync("Delete","Lead",id.ToString(),m);return RedirectToAction(nameof(Index));}
}
