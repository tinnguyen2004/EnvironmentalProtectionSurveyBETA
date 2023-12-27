using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace EnvironmentalProtectionSurvey.Models;

public partial class SurveyProjectContext : DbContext
{
    public SurveyProjectContext()
    {
    }

    public SurveyProjectContext(DbContextOptions<SurveyProjectContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Award> Awards { get; set; }

    public virtual DbSet<Contest> Contests { get; set; }

    public virtual DbSet<Faq> Faqs { get; set; }

    public virtual DbSet<FilledSurvey> FilledSurveys { get; set; }

    public virtual DbSet<ForgotPassword> ForgotPasswords { get; set; }

    public virtual DbSet<News> News { get; set; }

    public virtual DbSet<Option> Options { get; set; }

    public virtual DbSet<PasswordReset> PasswordResets { get; set; }

    public virtual DbSet<Question> Questions { get; set; }

    public virtual DbSet<QuestionContest> QuestionContests { get; set; }

    public virtual DbSet<Support> Supports { get; set; }

    public virtual DbSet<Survey> Surveys { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<Winner> Winners { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseSqlServer("server=LAPTOP-BVN4O9HO\\SQLEXPRESS;database=SurveyProject;uid=sa;pwd=123;TrustServerCertificate=true");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Award>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Award__3214EC07FFA5B69C");

            entity.ToTable("Award");

            entity.Property(e => e.Bonus)
                .HasMaxLength(255)
                .IsUnicode(false);

            entity.HasOne(d => d.Survey).WithMany(p => p.Awards)
                .HasForeignKey(d => d.SurveyId)
                .HasConstraintName("FK__Award__SurveyId__619B8048");

            entity.HasOne(d => d.User).WithMany(p => p.Awards)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__Award__UserId__628FA481");
        });

        modelBuilder.Entity<Contest>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Contest__3214EC0764BDAEA2");

            entity.ToTable("Contest");

            entity.Property(e => e.EndTime).HasColumnType("datetime");
            entity.Property(e => e.StartTime).HasColumnType("datetime");
            entity.Property(e => e.Title).HasMaxLength(255);
        });

        modelBuilder.Entity<Faq>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__FAQ__3214EC07797CA2EB");

            entity.ToTable("FAQ");

            entity.Property(e => e.Answer)
                .HasMaxLength(5000)
                .IsUnicode(false);
            entity.Property(e => e.Title)
                .HasMaxLength(5000)
                .IsUnicode(false);
        });

        modelBuilder.Entity<FilledSurvey>(entity =>
        {
            entity.ToTable("FilledSurvey");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CreatedAt)
                .HasColumnType("datetime")
                .HasColumnName("Created_At");

            entity.HasOne(d => d.Option).WithMany(p => p.FilledSurveys)
                .HasForeignKey(d => d.OptionId)
                .HasConstraintName("FK__FilledSur__Optio__5CD6CB2B");

            entity.HasOne(d => d.Survey).WithMany(p => p.FilledSurveys)
                .HasForeignKey(d => d.SurveyId)
                .HasConstraintName("FK__FilledSur__Surve__5BE2A6F2");

            entity.HasOne(d => d.User).WithMany(p => p.FilledSurveys)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__FilledSur__UserI__5AEE82B9");
        });

        modelBuilder.Entity<ForgotPassword>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__ForgotPa__3214EC073ED07446");

            entity.ToTable("ForgotPassword");

            entity.Property(e => e.ExpiryTime).HasColumnType("datetime");
            entity.Property(e => e.IsUsed).HasDefaultValueSql("((0))");
            entity.Property(e => e.Token)
                .HasMaxLength(255)
                .IsUnicode(false);

            entity.HasOne(d => d.User).WithMany(p => p.ForgotPasswords)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__ForgotPas__IsUse__5070F446");
        });

        modelBuilder.Entity<News>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__News__3214EC0707259BE4");

            entity.Property(e => e.Content)
                .HasMaxLength(5000)
                .IsUnicode(false);
            entity.Property(e => e.Image)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.Title)
                .HasMaxLength(255)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Option>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Option__3214EC0775B83A36");

            entity.ToTable("Option");

            //entity.Property(e => e.Answer)
            //    .HasMaxLength(255)
            //    .IsUnicode(false);
            entity.Property(e => e.Title)
                .HasMaxLength(255)
                .IsUnicode(false);

            entity.HasOne(d => d.Question).WithMany(p => p.Options)
                .HasForeignKey(d => d.QuestionId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK__Option__Question__59063A47");
        });

        modelBuilder.Entity<PasswordReset>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Password__3214EC078571871D");

            entity.ToTable("PasswordReset");

            entity.Property(e => e.ExpiryTime).HasColumnType("datetime");
            entity.Property(e => e.Token)
                .HasMaxLength(255)
                .IsUnicode(false);

            entity.HasOne(d => d.User).WithMany(p => p.PasswordResets)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__PasswordR__Expir__4CA06362");
        });

        modelBuilder.Entity<Question>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Question__3214EC0766EF2ED5");

            entity.ToTable("Question");

            entity.Property(e => e.CorrectAnswer)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.Title)
                .HasMaxLength(255)
                .IsUnicode(false);

            entity.HasOne(d => d.Survey).WithMany(p => p.Questions)
                .HasForeignKey(d => d.SurveyId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK__Question__Survey__5629CD9C");
        });

        modelBuilder.Entity<QuestionContest>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Question__3214EC07E40A4E56");

            entity.ToTable("QuestionContest");

            entity.Property(e => e.CorrectAnswer).HasMaxLength(255);

            // Sử dụng một chuỗi để lưu trữ các lựa chọn, phân tách bằng dấu phẩy
            entity.Property(e => e.AnswerOptions)
            .IsRequired()
            .HasMaxLength(1000)
             .HasConversion(
                v => string.Join(',', v),
                v => v.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList()
            );

            entity.HasOne(d => d.Contest).WithMany(p => p.QuestionContests)
                .HasForeignKey(d => d.ContestId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK__QuestionC__Conte__04E4BC85");
        });

        modelBuilder.Entity<Support>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Support__3214EC079B64E75E");

            entity.ToTable("Support");

            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.PhoneNumber)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.Reply)
                .HasMaxLength(5000)
                .IsUnicode(false);
            entity.Property(e => e.TextMessage)
                .HasMaxLength(5000)
                .IsUnicode(false);

            entity.HasOne(d => d.User).WithMany(p => p.Supports)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__Support__UserId__6754599E");
        });

        modelBuilder.Entity<Survey>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Survey__3214EC079802BCED");

            entity.ToTable("Survey");

            entity.Property(e => e.CreatedAt)
                .HasColumnType("datetime")
                .HasColumnName("Created_At");
            entity.Property(e => e.EndAt)
                .HasColumnType("datetime")
                .HasColumnName("End_At");
            entity.Property(e => e.Form)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.Title)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.UserPost).HasDefaultValueSql("((0))");
            entity.Property(e => e.UserType)
                .HasMaxLength(255)
                .IsUnicode(false);
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__User__3214EC07B0CBA98A");

            entity.ToTable("User");

            entity.Property(e => e.Active).HasDefaultValueSql("((0))");
            entity.Property(e => e.Class)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.Email)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.ExpiryTime).HasColumnType("datetime");
            entity.Property(e => e.JoinDate).HasColumnType("datetime");
            entity.Property(e => e.NumberCode)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.Password)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.Role)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.Section)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.Specification)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.Token)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.UserName)
                .HasMaxLength(255)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Winner>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Winner__3214EC0749017F20");

            entity.ToTable("Winner");

            entity.HasOne(d => d.Contest).WithMany(p => p.Winners)
                .HasForeignKey(d => d.ContestId)
                .HasConstraintName("FK__Winner__ContestI__07C12930");

            entity.HasOne(d => d.User).WithMany(p => p.Winners)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__Winner__UserId__08B54D69");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
