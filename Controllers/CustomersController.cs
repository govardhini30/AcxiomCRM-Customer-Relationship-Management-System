using AcxiomCRM.Data;
using AcxiomCRM.Models;
using AcxiomCRM.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AcxiomCRM.Controllers;

[Authorize]
public class CustomersController : Controller
{
    private readonly ApplicationDbContext _db; private readonly AuditService _audit; private readonly ScopeService _scope;
    public CustomersController(ApplicationDbContext db, AuditService audit, ScopeService scope) { _db = db; _audit = audit; _scope = scope; }
    public async Task<IActionResult> Index(string? q) { var x = _db.Customers.AsNoTracking(); if (_scope.IsSalesExecutive) x = x.Where(c => c.CreatedBy == _scope.UserName); if (!string.IsNullOrWhiteSpace(q)) x = x.Where(c => c.CustomerName.Contains(q) || c.Email.Contains(q) || c.Phone.Contains(q) || c.CompanyName.Contains(q)); return View(await x.OrderByDescending(c => c.CreatedDate).ToListAsync()); }
    public IActionResult Create() => View(new Customer());
    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Customer m) { if (await _db.Customers.AnyAsync(c => c.Email == m.Email || c.Phone == m.Phone)) ModelState.AddModelError("", "Customer email or phone already exists."); if (!ModelState.IsValid) return View(m); m.CreatedBy = User.Identity?.Name; m.CreatedDate = DateTime.UtcNow; _db.Add(m); await _db.SaveChangesAsync(); await _audit.LogAsync("Create", "Customer", m.CustomerId.ToString(), newValue: m); return RedirectToAction(nameof(Index)); }
    public async Task<IActionResult> Edit(int id) { var m = await _db.Customers.FindAsync(id); if (m is not null && _scope.IsSalesExecutive && m.CreatedBy != _scope.UserName) return Forbid(); return m is null ? NotFound() : View(m); }
    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Customer m) { if (id != m.CustomerId) return BadRequest(); var existing = await _db.Customers.FindAsync(id); if (existing is null) return NotFound(); if (_scope.IsSalesExecutive && existing.CreatedBy != _scope.UserName) return Forbid(); if (await _db.Customers.AnyAsync(c => c.CustomerId != id && (c.Email == m.Email || c.Phone == m.Phone))) ModelState.AddModelError("", "Customer email or phone already exists."); if (!ModelState.IsValid) return View(m); var old = new { existing.CustomerName, existing.Email, existing.Phone, existing.CompanyName, existing.Status }; existing.CustomerName=m.CustomerName; existing.Email=m.Email; existing.Phone=m.Phone; existing.CompanyName=m.CompanyName; existing.Address=m.Address; existing.City=m.City; existing.State=m.State; existing.Status=m.Status; await _db.SaveChangesAsync(); await _audit.LogAsync("Update", "Customer", id.ToString(), old, existing); return RedirectToAction(nameof(Index)); }
    [HttpPost, ValidateAntiForgeryToken, Authorize(Roles = "Admin,Manager")]
    public async Task<IActionResult> Delete(int id) { var m = await _db.Customers.FindAsync(id); if (m is null) return NotFound(); _db.Remove(m); await _db.SaveChangesAsync(); await _audit.LogAsync("Delete", "Customer", id.ToString(), m); return RedirectToAction(nameof(Index)); }
}
