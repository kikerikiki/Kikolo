﻿// <auto-generated />
using System;
using Kikolo_v1.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace Kikolo_v1.Migrations
{
    [DbContext(typeof(KikoloContext))]
    partial class KikoloContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.14")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("FragenpacksQuestion", b =>
                {
                    b.Property<int>("FragenpackId")
                        .HasColumnType("int")
                        .HasColumnName("FragenpackID");

                    b.Property<int>("QuestionId")
                        .HasColumnType("int")
                        .HasColumnName("QuestionID");

                    b.HasKey("FragenpackId", "QuestionId")
                        .HasName("PK__Fragenpa__23E53155D3324B72");

                    b.HasIndex("QuestionId");

                    b.ToTable("FragenpacksQuestions", (string)null);
                });

            modelBuilder.Entity("Kikolo_v1.Models.Fragenpack", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("ID");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Name")
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.HasKey("Id");

                    b.ToTable("Fragenpacks");
                });

            modelBuilder.Entity("Kikolo_v1.Models.Question", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Category")
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<DateTime?>("CreatedAt")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetime")
                        .HasDefaultValueSql("(getdate())");

                    b.Property<string>("FragenPack")
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<string>("Text")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Questions");
                });

            modelBuilder.Entity("FragenpacksQuestion", b =>
                {
                    b.HasOne("Kikolo_v1.Models.Fragenpack", null)
                        .WithMany()
                        .HasForeignKey("FragenpackId")
                        .IsRequired()
                        .HasConstraintName("FK__Fragenpac__Frage__3D5E1FD2");

                    b.HasOne("Kikolo_v1.Models.Question", null)
                        .WithMany()
                        .HasForeignKey("QuestionId")
                        .IsRequired()
                        .HasConstraintName("FK__Fragenpac__Quest__3E52440B");
                });
#pragma warning restore 612, 618
        }
    }
}
