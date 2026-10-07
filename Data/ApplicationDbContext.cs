using AcxiomCRM.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace AcxiomCRM.Data;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

    public DbSet<Customer> Customers => Set<Customer>();
    public DbSet<Lead> Leads => Set<Lead>();
    public DbSet<Opportunity> Opportunities => Set<Opportunity>();
    public DbSet<FollowUp> FollowUps => Set<FollowUp>();
    public DbSet<Activity> Activities => Set<Activity>();
    public DbSet<AuditLog> AuditLogs => Set<AuditLog>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.Entity<Customer>().HasIndex(x => x.Email).IsUnique();
        builder.Entity<Customer>().HasIndex(x => x.Phone).IsUnique();
        builder.Entity<Lead>().Property(x => x.ExpectedValue).HasPrecision(18, 2);
        builder.Entity<Opportunity>().Property(x => x.Amount).HasPrecision(18, 2);
        builder.Entity<AuditLog>().Property(x => x.OldValue).HasColumnType("TEXT");
        builder.Entity<AuditLog>().Property(x => x.NewValue).HasColumnType("TEXT");
    }
}

public static class SeedData
{
    public static async Task InitializeAsync(IServiceProvider services)
    {
        var roleManager = services.GetRequiredService<RoleManager<Microsoft.AspNetCore.Identity.IdentityRole>>();
        foreach (var role in new[] { "Admin", "Manager", "SalesExecutive" })
        {
            if (!await roleManager.RoleExistsAsync(role))
                await roleManager.CreateAsync(new Microsoft.AspNetCore.Identity.IdentityRole(role));
        }

        var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
        const string email = "admin@acxiomcrm.local";
        var admin = await userManager.FindByEmailAsync(email);
        if (admin is null)
        {
            admin = new ApplicationUser { Name = "System Admin", UserName = email, Email = email, EmailConfirmed = true };
            var result = await userManager.CreateAsync(admin, "Admin@123");
            if (!result.Succeeded)
                throw new InvalidOperationException(string.Join("; ", result.Errors.Select(e => e.Description)));
        }

        if (!await userManager.IsInRoleAsync(admin, "Admin"))
            await userManager.AddToRoleAsync(admin, "Admin");
    }
}
