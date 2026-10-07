using AcxiomCRM.Data;
using AcxiomCRM.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AcxiomCRM.Controllers.Api;

public record CustomerDto(int CustomerId,string CustomerName,string Email,string Phone,string CompanyName,string Status);
public record LeadDto(int LeadId,string LeadName,string Email,string Phone,string CompanyName,string Source,string Status,string Priority,decimal ExpectedValue);
public record OpportunityDto(int OpportunityId,string OpportunityName,int? CustomerId,int? LeadId,decimal Amount,string Stage,int Probability,DateTime ExpectedCloseDate,string Status);

[ApiController,Route("api/customers"),Authorize]
public class CustomersApiController(ApplicationDbContext db, AcxiomCRM.Services.ScopeService scope) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<CustomerDto>>> Get()
    {
        var q=db.Customers.AsNoTracking();
        if(scope.IsSalesExecutive) q=q.Where(x=>x.CreatedBy==scope.UserName);
        return Ok(await q.Select(x=>new CustomerDto(x.CustomerId,x.CustomerName,x.Email,x.Phone,x.CompanyName,x.Status)).ToListAsync());
    }
    [HttpGet("{id:int}")]
    public async Task<ActionResult<CustomerDto>> Get(int id)
    {
        var x=await db.Customers.AsNoTracking().FirstOrDefaultAsync(c=>c.CustomerId==id);
        if(x is null)return NotFound();
        if(scope.IsSalesExecutive&&x.CreatedBy!=scope.UserName)return Forbid();
        return Ok(new CustomerDto(x.CustomerId,x.CustomerName,x.Email,x.Phone,x.CompanyName,x.Status));
    }
    [HttpPost]
    public async Task<ActionResult<CustomerDto>> Post(CustomerDto dto)
    {
        if(!ModelState.IsValid)return ValidationProblem(ModelState);
        if(await db.Customers.AnyAsync(c=>c.Email==dto.Email||c.Phone==dto.Phone))return Conflict(new{message="Duplicate customer email or phone."});
        var x=new Customer{CustomerName=dto.CustomerName,Email=dto.Email,Phone=dto.Phone,CompanyName=dto.CompanyName,Status=dto.Status,CreatedBy=User.Identity?.Name};
        db.Customers.Add(x);await db.SaveChangesAsync();
        var result=new CustomerDto(x.CustomerId,x.CustomerName,x.Email,x.Phone,x.CompanyName,x.Status);
        return CreatedAtAction(nameof(Get),new{id=x.CustomerId},result);
    }
    [HttpPut("{id:int}")]
    public async Task<ActionResult<CustomerDto>> Put(int id,CustomerDto dto)
    {
        if(id!=dto.CustomerId)return BadRequest();
        var x=await db.Customers.FindAsync(id);if(x is null)return NotFound();
        if(scope.IsSalesExecutive&&x.CreatedBy!=scope.UserName)return Forbid();
        if(await db.Customers.AnyAsync(c=>c.CustomerId!=id&&(c.Email==dto.Email||c.Phone==dto.Phone)))return Conflict(new{message="Duplicate customer email or phone."});
        x.CustomerName=dto.CustomerName;x.Email=dto.Email;x.Phone=dto.Phone;x.CompanyName=dto.CompanyName;x.Status=dto.Status;
        await db.SaveChangesAsync();
        return Ok(new CustomerDto(x.CustomerId,x.CustomerName,x.Email,x.Phone,x.CompanyName,x.Status));
    }
    [HttpDelete("{id:int}"),Authorize(Roles="Admin,Manager")]
    public async Task<IActionResult> Delete(int id){var x=await db.Customers.FindAsync(id);if(x is null)return NotFound();db.Remove(x);await db.SaveChangesAsync();return NoContent();}
}

[ApiController,Route("api/leads"),Authorize]
public class LeadsApiController(ApplicationDbContext db, AcxiomCRM.Services.ScopeService scope):ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<LeadDto>>> Get(){var q=db.Leads.AsNoTracking();if(scope.IsSalesExecutive)q=q.Where(x=>x.AssignedTo==scope.UserName);return Ok(await q.Select(x=>new LeadDto(x.LeadId,x.LeadName,x.Email,x.Phone,x.CompanyName,x.Source,x.Status,x.Priority,x.ExpectedValue)).ToListAsync());}
    [HttpPost]
    public async Task<ActionResult<LeadDto>> Post(LeadDto dto){if(!ModelState.IsValid)return ValidationProblem(ModelState);var x=new Lead{LeadName=dto.LeadName,Email=dto.Email,Phone=dto.Phone,CompanyName=dto.CompanyName,Source=dto.Source,Status=dto.Status,Priority=dto.Priority,ExpectedValue=dto.ExpectedValue,AssignedTo=User.Identity?.Name};db.Leads.Add(x);await db.SaveChangesAsync();var result=new LeadDto(x.LeadId,x.LeadName,x.Email,x.Phone,x.CompanyName,x.Source,x.Status,x.Priority,x.ExpectedValue);return Created($"/api/leads/{x.LeadId}",result);}
}

[ApiController,Route("api/opportunities"),Authorize]
public class OpportunitiesApiController(ApplicationDbContext db, AcxiomCRM.Services.ScopeService scope):ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<OpportunityDto>>> Get(){var q=db.Opportunities.AsNoTracking();if(scope.IsSalesExecutive)q=q.Where(x=>x.AssignedTo==scope.UserName);return Ok(await q.Select(x=>new OpportunityDto(x.OpportunityId,x.OpportunityName,x.CustomerId,x.LeadId,x.Amount,x.Stage,x.Probability,x.ExpectedCloseDate,x.Status)).ToListAsync());}
    [HttpPost]
    public async Task<ActionResult<OpportunityDto>> Post(OpportunityDto dto){if(dto.Amount<=0)return BadRequest(new{message="Opportunity Amount must be greater than 0."});if(dto.Probability<0||dto.Probability>100)return BadRequest(new{message="Probability must be between 0 and 100."});if(dto.Status=="Open"&&dto.ExpectedCloseDate.Date<DateTime.Today)return BadRequest(new{message="Expected Close Date cannot be in the past."});var x=new Opportunity{OpportunityName=dto.OpportunityName,CustomerId=dto.CustomerId,LeadId=dto.LeadId,Amount=dto.Amount,Stage=dto.Stage,Probability=dto.Probability,ExpectedCloseDate=dto.ExpectedCloseDate,Status=dto.Status,AssignedTo=User.Identity?.Name};db.Opportunities.Add(x);await db.SaveChangesAsync();var result=new OpportunityDto(x.OpportunityId,x.OpportunityName,x.CustomerId,x.LeadId,x.Amount,x.Stage,x.Probability,x.ExpectedCloseDate,x.Status);return Created($"/api/opportunities/{x.OpportunityId}",result);}
}
