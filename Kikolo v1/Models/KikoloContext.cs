using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Kikolo_v1.Models
{
    public partial class KikoloContext : DbContext
    {
        public KikoloContext()
        {
        }

        public KikoloContext(DbContextOptions<KikoloContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Fragenpack> Fragenpacks { get; set; } = null!;
        public virtual DbSet<Question> Questions { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
                optionsBuilder.UseSqlServer("Data Source=DESKTOP-CQGSRN6;Initial Catalog=Kikolo;Integrated Security=True;Connect Timeout=30;Encrypt=False;Trust Server Certificate=False;Application Intent=ReadWrite;Multi Subnet Failover=False");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Fragenpack>(entity =>
            {
                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.Name).HasMaxLength(255);

                entity.HasMany(d => d.Questions)
                    .WithMany(p => p.Fragenpacks)
                    .UsingEntity<Dictionary<string, object>>(
                        "FragenpacksQuestion",
                        l => l.HasOne<Question>().WithMany().HasForeignKey("QuestionId").OnDelete(DeleteBehavior.ClientSetNull).HasConstraintName("FK__Fragenpac__Quest__3E52440B"),
                        r => r.HasOne<Fragenpack>().WithMany().HasForeignKey("FragenpackId").OnDelete(DeleteBehavior.ClientSetNull).HasConstraintName("FK__Fragenpac__Frage__3D5E1FD2"),
                        j =>
                        {
                            j.HasKey("FragenpackId", "QuestionId").HasName("PK__Fragenpa__23E53155D3324B72");

                            j.ToTable("FragenpacksQuestions");

                            j.IndexerProperty<int>("FragenpackId").HasColumnName("FragenpackID");

                            j.IndexerProperty<int>("QuestionId").HasColumnName("QuestionID");
                        });
            });

            modelBuilder.Entity<Question>(entity =>
            {
                entity.Property(e => e.Category).HasMaxLength(255);

                entity.Property(e => e.CreatedAt)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
