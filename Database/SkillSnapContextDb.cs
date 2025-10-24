
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Shared.Models;

public class SkillSnapContext : IdentityDbContext<ApplicationUser>
{
    public SkillSnapContext(DbContextOptions<SkillSnapContext> options) : base(options) {}
    
    public DbSet<PortfolioUser> PortfolioUsers { get; set; }
    public DbSet<Project> Projects { get; set; }
    public DbSet<Skill> Skills { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure PortfolioUser entity
        modelBuilder.Entity<PortfolioUser>(entity =>
        {
            entity.HasKey(e => e.Id);
            
            entity.Property(e => e.Name)
                .IsRequired()
                .HasMaxLength(100)
                .HasComment("Full name of the portfolio user");
            
            entity.Property(e => e.Bio)
                .IsRequired()
                .HasMaxLength(500)
                .HasComment("Biography or description of the user");
            
            entity.Property(e => e.ProfileImageUrl)
                .HasMaxLength(500)
                .HasComment("URL to the user's profile image");

            // Configure relationships
            entity.HasMany(e => e.Projects)
                .WithOne(e => e.PortfolioUser)
                .HasForeignKey(e => e.PortfolioUserId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasMany(e => e.Skills)
                .WithOne(e => e.PortfolioUser)
                .HasForeignKey(e => e.PortfolioUserId)
                .OnDelete(DeleteBehavior.Cascade);

            // Configure relationship with ApplicationUser
            entity.HasOne(e => e.User)
                .WithMany()
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.SetNull);

            // Add indexes for better performance
            entity.HasIndex(e => e.Name)
                .HasDatabaseName("IX_PortfolioUsers_Name");
            
            entity.HasIndex(e => e.UserId)
                .HasDatabaseName("IX_PortfolioUsers_UserId");
        });

        // Configure Project entity
        modelBuilder.Entity<Project>(entity =>
        {
            entity.HasKey(e => e.Id);
            
            entity.Property(e => e.Title)
                .IsRequired()
                .HasMaxLength(200)
                .HasComment("Title of the project");
            
            entity.Property(e => e.Description)
                .IsRequired()
                .HasMaxLength(1000)
                .HasComment("Detailed description of the project");
            
            entity.Property(e => e.ImageUrl)
                .HasMaxLength(500)
                .HasComment("URL to the project's featured image");
            
            entity.Property(e => e.PortfolioUserId)
                .IsRequired()
                .HasComment("Foreign key to the portfolio user");

            // Configure relationship
            entity.HasOne(e => e.PortfolioUser)
                .WithMany(e => e.Projects)
                .HasForeignKey(e => e.PortfolioUserId)
                .OnDelete(DeleteBehavior.Cascade);

            // Add indexes
            entity.HasIndex(e => e.Title)
                .HasDatabaseName("IX_Projects_Title");
            
            entity.HasIndex(e => e.PortfolioUserId)
                .HasDatabaseName("IX_Projects_PortfolioUserId");
        });

        // Configure Skill entity
        modelBuilder.Entity<Skill>(entity =>
        {
            entity.HasKey(e => e.Id);
            
            entity.Property(e => e.Name)
                .IsRequired()
                .HasMaxLength(100)
                .HasComment("Name of the skill (e.g., React, C#, Python)");
            
            entity.Property(e => e.Level)
                .IsRequired()
                .HasMaxLength(50)
                .HasComment("Skill level (Beginner, Intermediate, Advanced, Expert, Master)");
            
            entity.Property(e => e.PortfolioUserId)
                .IsRequired()
                .HasComment("Foreign key to the portfolio user");

            // Configure relationship
            entity.HasOne(e => e.PortfolioUser)
                .WithMany(e => e.Skills)
                .HasForeignKey(e => e.PortfolioUserId)
                .OnDelete(DeleteBehavior.Cascade);

            // Add indexes
            entity.HasIndex(e => e.Name)
                .HasDatabaseName("IX_Skills_Name");
            
            entity.HasIndex(e => e.Level)
                .HasDatabaseName("IX_Skills_Level");
            
            entity.HasIndex(e => e.PortfolioUserId)
                .HasDatabaseName("IX_Skills_PortfolioUserId");

            // Add unique constraint to prevent duplicate skills for the same user
            entity.HasIndex(e => new { e.Name, e.PortfolioUserId })
                .IsUnique()
                .HasDatabaseName("IX_Skills_Name_PortfolioUserId_Unique");
        });

    }

}