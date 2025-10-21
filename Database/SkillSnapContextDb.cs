
using Microsoft.EntityFrameworkCore;
using Shared.Models;

public class SkillSnapContext : DbContext
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

            // Add indexes for better performance
            entity.HasIndex(e => e.Name)
                .HasDatabaseName("IX_PortfolioUsers_Name");
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

        // Seed initial data
        SeedData(modelBuilder);
    }

    private void SeedData(ModelBuilder modelBuilder)
    {
        // Seed PortfolioUsers
        modelBuilder.Entity<PortfolioUser>().HasData(
            new PortfolioUser
            {
                Id = 1,
                Name = "Alex Johnson",
                Bio = "Full-stack developer passionate about creating innovative web applications and mobile solutions.",
                ProfileImageUrl = "https://images.unsplash.com/photo-1507003211169-0a1dd7228f2d?w=150&h=150&fit=crop&crop=face"
            },
            new PortfolioUser
            {
                Id = 2,
                Name = "Sarah Chen",
                Bio = "UI/UX Designer and Frontend Developer specializing in creating beautiful, user-centered digital experiences.",
                ProfileImageUrl = "https://images.unsplash.com/photo-1494790108755-2616b612b786?w=150&h=150&fit=crop&crop=face"
            },
            new PortfolioUser
            {
                Id = 3,
                Name = "Michael Rodriguez",
                Bio = "Backend developer with expertise in cloud architecture and microservices. Passionate about scalable solutions.",
                ProfileImageUrl = "https://images.unsplash.com/photo-1472099645785-5658abf4ff4e?w=150&h=150&fit=crop&crop=face"
            }
        );

        // Seed Projects
        modelBuilder.Entity<Project>().HasData(
            new Project { Id = 1, Title = "E-Commerce Platform", Description = "A modern e-commerce solution built with React and Node.js featuring real-time inventory management and payment processing.", ImageUrl = "https://images.unsplash.com/photo-1556742049-0cfed4f6a45d?w=400&h=300&fit=crop", PortfolioUserId = 1 },
            new Project { Id = 2, Title = "Task Management App", Description = "Collaborative task management tool with real-time updates, drag-and-drop functionality, and team collaboration features.", ImageUrl = "https://images.unsplash.com/photo-1611224923853-80b023f02d71?w=400&h=300&fit=crop", PortfolioUserId = 1 },
            new Project { Id = 3, Title = "Weather Dashboard", Description = "Beautiful weather application with location-based forecasts, interactive maps, and detailed weather analytics.", ImageUrl = "https://images.unsplash.com/photo-1504608524841-42fe6f032b4b?w=400&h=300&fit=crop", PortfolioUserId = 1 },
            new Project { Id = 4, Title = "Design System", Description = "Comprehensive design system for a fintech startup including components, patterns, and accessibility guidelines.", ImageUrl = "https://images.unsplash.com/photo-1558655146-9f40138edfeb?w=400&h=300&fit=crop", PortfolioUserId = 2 },
            new Project { Id = 5, Title = "Mobile Banking App", Description = "Intuitive mobile banking application with advanced security features, biometric authentication, and real-time transactions.", ImageUrl = "https://images.unsplash.com/photo-1563013544-824ae1b704d3?w=400&h=300&fit=crop", PortfolioUserId = 2 },
            new Project { Id = 6, Title = "Cloud Infrastructure", Description = "Scalable cloud infrastructure setup using AWS with auto-scaling, load balancing, and monitoring solutions.", ImageUrl = "https://images.unsplash.com/photo-1451187580459-43490279c0fa?w=400&h=300&fit=crop", PortfolioUserId = 3 }
        );

        // Seed Skills
        modelBuilder.Entity<Skill>().HasData(
            // Alex Johnson's skills
            new Skill { Id = 1, Name = "React", Level = "Expert", PortfolioUserId = 1 },
            new Skill { Id = 2, Name = "Node.js", Level = "Advanced", PortfolioUserId = 1 },
            new Skill { Id = 3, Name = "TypeScript", Level = "Advanced", PortfolioUserId = 1 },
            new Skill { Id = 4, Name = "MongoDB", Level = "Intermediate", PortfolioUserId = 1 },
            new Skill { Id = 5, Name = "AWS", Level = "Intermediate", PortfolioUserId = 1 },
            new Skill { Id = 6, Name = "Docker", Level = "Novice", PortfolioUserId = 1 },
            
            // Sarah Chen's skills
            new Skill { Id = 7, Name = "Figma", Level = "Expert", PortfolioUserId = 2 },
            new Skill { Id = 8, Name = "Vue.js", Level = "Advanced", PortfolioUserId = 2 },
            new Skill { Id = 9, Name = "CSS/SCSS", Level = "Expert", PortfolioUserId = 2 },
            new Skill { Id = 10, Name = "Adobe Creative Suite", Level = "Advanced", PortfolioUserId = 2 },
            new Skill { Id = 11, Name = "JavaScript", Level = "Advanced", PortfolioUserId = 2 },
            
            // Michael Rodriguez's skills
            new Skill { Id = 12, Name = "C#", Level = "Expert", PortfolioUserId = 3 },
            new Skill { Id = 13, Name = ".NET Core", Level = "Expert", PortfolioUserId = 3 },
            new Skill { Id = 14, Name = "Azure", Level = "Advanced", PortfolioUserId = 3 },
            new Skill { Id = 15, Name = "SQL Server", Level = "Advanced", PortfolioUserId = 3 },
            new Skill { Id = 16, Name = "Microservices", Level = "Advanced", PortfolioUserId = 3 },
            new Skill { Id = 17, Name = "Docker", Level = "Advanced", PortfolioUserId = 3 }
        );
    }
}