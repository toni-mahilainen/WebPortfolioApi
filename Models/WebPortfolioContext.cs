using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace WebPortfolioCoreApi.Models
{
    public partial class WebPortfolioContext : DbContext
    {
        public WebPortfolioContext()
        {
        }

        public WebPortfolioContext(DbContextOptions<WebPortfolioContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Emails> Emails { get; set; }
        public virtual DbSet<ImageTypes> ImageTypes { get; set; }
        public virtual DbSet<ImageUrls> ImageUrls { get; set; }
        public virtual DbSet<PortfolioContent> PortfolioContent { get; set; }
        public virtual DbSet<Projects> Projects { get; set; }
        public virtual DbSet<QuestbookMessages> QuestbookMessages { get; set; }
        public virtual DbSet<Skills> Skills { get; set; }
        public virtual DbSet<SocialMediaLinks> SocialMediaLinks { get; set; }
        public virtual DbSet<SocialMediaServices> SocialMediaServices { get; set; }
        public virtual DbSet<Users> Users { get; set; }
        public virtual DbSet<Visitors> Visitors { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer("Server=ASUS-LAPTOP\\SQLDEVTONIMA;Initial Catalog=WebPortfolio;Persist Security Info=False;User ID=sa;Password=866462Tm;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Emails>(entity =>
            {
                entity.HasKey(e => e.EmailId);

                entity.Property(e => e.EmailId).HasColumnName("EmailID");

                entity.Property(e => e.EmailAddress)
                    .IsRequired()
                    .HasMaxLength(150);

                entity.Property(e => e.PortfolioId).HasColumnName("PortfolioID");

                entity.HasOne(d => d.Portfolio)
                    .WithMany(p => p.Emails)
                    .HasForeignKey(d => d.PortfolioId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Emails_PortfolioContent");
            });

            modelBuilder.Entity<ImageTypes>(entity =>
            {
                entity.HasKey(e => e.TypeId);

                entity.Property(e => e.TypeId).HasColumnName("TypeID");

                entity.Property(e => e.Type)
                    .IsRequired()
                    .HasMaxLength(50);
            });

            modelBuilder.Entity<ImageUrls>(entity =>
            {
                entity.HasKey(e => e.UrlId);

                entity.Property(e => e.UrlId).HasColumnName("UrlID");

                entity.Property(e => e.TypeId).HasColumnName("TypeID");

                entity.Property(e => e.Url)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.UserId).HasColumnName("UserID");

                entity.HasOne(d => d.Type)
                    .WithMany(p => p.ImageUrls)
                    .HasForeignKey(d => d.TypeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ImageUrls_ImageTypes");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.ImageUrls)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ImageUrls_Users");
            });

            modelBuilder.Entity<PortfolioContent>(entity =>
            {
                entity.HasKey(e => e.PortfolioId);

                entity.Property(e => e.PortfolioId).HasColumnName("PortfolioID");

                entity.Property(e => e.Birthdate).HasColumnType("date");

                entity.Property(e => e.City).HasMaxLength(50);

                entity.Property(e => e.Country).HasMaxLength(50);

                entity.Property(e => e.Firstname)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.Lastname)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.Phonenumber).HasMaxLength(30);

                entity.Property(e => e.UserId).HasColumnName("UserID");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.PortfolioContent)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_PortfolioContent_Users");
            });

            modelBuilder.Entity<Projects>(entity =>
            {
                entity.HasKey(e => e.ProjectId);

                entity.Property(e => e.ProjectId).HasColumnName("ProjectID");

                entity.Property(e => e.Link).HasMaxLength(150);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.SkillId).HasColumnName("SkillID");

                entity.HasOne(d => d.Skill)
                    .WithMany(p => p.Projects)
                    .HasForeignKey(d => d.SkillId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Projects_Skills");
            });

            modelBuilder.Entity<QuestbookMessages>(entity =>
            {
                entity.HasKey(e => e.MessageId);

                entity.Property(e => e.MessageId).HasColumnName("MessageID");

                entity.Property(e => e.PortfolioId).HasColumnName("PortfolioID");

                entity.Property(e => e.VisitationTimestamp).HasColumnType("datetime");

                entity.Property(e => e.VisitorId).HasColumnName("VisitorID");

                entity.HasOne(d => d.Portfolio)
                    .WithMany(p => p.QuestbookMessages)
                    .HasForeignKey(d => d.PortfolioId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_QuestbookMessages_PortfolioContent");

                entity.HasOne(d => d.Visitor)
                    .WithMany(p => p.QuestbookMessages)
                    .HasForeignKey(d => d.VisitorId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_QuestbookMessages_Visitors");
            });

            modelBuilder.Entity<Skills>(entity =>
            {
                entity.HasKey(e => e.SkillId);

                entity.Property(e => e.SkillId).HasColumnName("SkillID");

                entity.Property(e => e.Skill)
                    .IsRequired()
                    .HasMaxLength(150);

                entity.Property(e => e.UserId).HasColumnName("UserID");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.Skills)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Skills_Users");
            });

            modelBuilder.Entity<SocialMediaLinks>(entity =>
            {
                entity.HasKey(e => e.LinkId);

                entity.Property(e => e.LinkId).HasColumnName("LinkID");

                entity.Property(e => e.Link).HasMaxLength(200);

                entity.Property(e => e.ServiceId).HasColumnName("ServiceID");

                entity.Property(e => e.UserId).HasColumnName("UserID");

                entity.HasOne(d => d.Service)
                    .WithMany(p => p.SocialMediaLinks)
                    .HasForeignKey(d => d.ServiceId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_SocialMediaLinks_SocialMediaServices");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.SocialMediaLinks)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_SocialMediaLinks_Users");
            });

            modelBuilder.Entity<SocialMediaServices>(entity =>
            {
                entity.HasKey(e => e.ServiceId);

                entity.Property(e => e.ServiceId).HasColumnName("ServiceID");

                entity.Property(e => e.Service)
                    .IsRequired()
                    .HasMaxLength(50);
            });

            modelBuilder.Entity<Users>(entity =>
            {
                entity.HasKey(e => e.UserId);

                entity.Property(e => e.UserId).HasColumnName("UserID");

                entity.Property(e => e.Password)
                    .IsRequired()
                    .HasMaxLength(32)
                    .IsUnicode(false)
                    .IsFixedLength();

                entity.Property(e => e.Username)
                    .IsRequired()
                    .HasMaxLength(50);
            });

            modelBuilder.Entity<Visitors>(entity =>
            {
                entity.HasKey(e => e.VisitorId);

                entity.Property(e => e.VisitorId).HasColumnName("VisitorID");

                entity.Property(e => e.Company).HasMaxLength(50);

                entity.Property(e => e.Firstname)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.Lastname)
                    .IsRequired()
                    .HasMaxLength(50);
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
