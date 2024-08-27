﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using TicketSystem.DataAccess.Context;

#nullable disable

namespace TicketSystem.DataAccess.Migrations
{
    [DbContext(typeof(TicketSystemDbContext))]
    [Migration("20240808184323_tt")]
    partial class tt
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.7")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("TicketSystem.DataAccess.Models.AppUser", b =>
                {
                    b.Property<string>("UserId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("Address")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<string>("CreatedById")
                        .HasColumnType("nvarchar(450)");

                    b.Property<DateTime>("DateCreated")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("DateOfBirth")
                        .HasColumnType("datetime2");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<bool>("IsActive")
                        .HasColumnType("bit");

                    b.Property<string>("MobileNumber")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Password")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("RefreshToken")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("RefreshTokenExpiryTime")
                        .HasColumnType("datetime2");

                    b.Property<string>("UpdateById")
                        .HasColumnType("nvarchar(450)");

                    b.Property<DateTime>("UpdateDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("UserImageURL")
                        .IsRequired()
                        .HasMaxLength(500)
                        .HasColumnType("nvarchar(500)");

                    b.Property<string>("UserName")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<int>("UserType")
                        .HasColumnType("int");

                    b.HasKey("UserId");

                    b.HasIndex("CreatedById");

                    b.HasIndex("UpdateById");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("TicketSystem.DataAccess.Models.Comment", b =>
                {
                    b.Property<int>("CommentId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("CommentId"));

                    b.Property<string>("CreatedById")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.Property<DateTime>("CreatedDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("Text")
                        .IsRequired()
                        .HasMaxLength(500)
                        .HasColumnType("nvarchar(500)");

                    b.Property<int>("TicketId")
                        .HasColumnType("int");

                    b.Property<string>("UpdateById")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.Property<DateTime>("UpdateDate")
                        .HasColumnType("datetime2");

                    b.HasKey("CommentId");

                    b.HasIndex("CreatedById");

                    b.HasIndex("TicketId");

                    b.HasIndex("UpdateById");

                    b.ToTable("Comments");
                });

            modelBuilder.Entity("TicketSystem.DataAccess.Models.Product", b =>
                {
                    b.Property<int>("ProductId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("ProductId"));

                    b.Property<string>("CreatedById")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.Property<DateTime>("CreatedDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasMaxLength(500)
                        .HasColumnType("nvarchar(500)");

                    b.Property<string>("NameAr")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<string>("NameEn")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<string>("UpdateById")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.Property<DateTime>("UpdateDate")
                        .HasColumnType("datetime2");

                    b.HasKey("ProductId");

                    b.HasIndex("CreatedById");

                    b.HasIndex("UpdateById");

                    b.ToTable("Products");
                });

            modelBuilder.Entity("TicketSystem.DataAccess.Models.Ticket", b =>
                {
                    b.Property<int>("TicketId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("TicketId"));

                    b.Property<string>("AssignedToId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("CreatedById")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.Property<DateTime>("CreatedDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("ProblemDescription")
                        .IsRequired()
                        .HasMaxLength(500)
                        .HasColumnType("nvarchar(500)");

                    b.Property<int>("ProductId")
                        .HasColumnType("int");

                    b.Property<int>("Status")
                        .HasColumnType("int");

                    b.Property<int>("TicketTypeId")
                        .HasColumnType("int");

                    b.Property<string>("UpdateById")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.Property<DateTime>("UpdateDate")
                        .HasColumnType("datetime2");

                    b.HasKey("TicketId");

                    b.HasIndex("AssignedToId");

                    b.HasIndex("CreatedById");

                    b.HasIndex("ProductId");

                    b.HasIndex("TicketTypeId");

                    b.HasIndex("UpdateById");

                    b.ToTable("Tickets");
                });

            modelBuilder.Entity("TicketSystem.DataAccess.Models.TicketAttachment", b =>
                {
                    b.Property<int>("AttachmentId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("AttachmentId"));

                    b.Property<string>("CreatedById")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.Property<DateTime>("CreatedDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("FileName")
                        .IsRequired()
                        .HasMaxLength(200)
                        .HasColumnType("nvarchar(200)");

                    b.Property<string>("FilePath")
                        .IsRequired()
                        .HasMaxLength(500)
                        .HasColumnType("nvarchar(500)");

                    b.Property<int>("TicketId")
                        .HasColumnType("int");

                    b.Property<string>("UpdateById")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.Property<DateTime>("UpdateDate")
                        .HasColumnType("datetime2");

                    b.HasKey("AttachmentId");

                    b.HasIndex("CreatedById");

                    b.HasIndex("TicketId");

                    b.HasIndex("UpdateById");

                    b.ToTable("Attachments");
                });

            modelBuilder.Entity("TicketSystem.DataAccess.Models.TicketCategory", b =>
                {
                    b.Property<int>("TicketCategoryId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("TicketCategoryId"));

                    b.Property<string>("CreatedById")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.Property<DateTime>("CreatedDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("NameAr")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<string>("NameEn")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<string>("UpdateById")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.Property<DateTime>("UpdateDate")
                        .HasColumnType("datetime2");

                    b.HasKey("TicketCategoryId");

                    b.HasIndex("CreatedById");

                    b.HasIndex("UpdateById");

                    b.ToTable("TicketCategories");
                });

            modelBuilder.Entity("TicketSystem.DataAccess.Models.TicketDetail", b =>
                {
                    b.Property<int>("TicketDetailId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("TicketDetailId"));

                    b.Property<string>("CreateById")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.Property<DateTime>("CreatedDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("Status")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<int>("TicketId")
                        .HasColumnType("int");

                    b.HasKey("TicketDetailId");

                    b.HasIndex("CreateById");

                    b.HasIndex("TicketId");

                    b.ToTable("TicketDetails");
                });

            modelBuilder.Entity("TicketSystem.DataAccess.Models.TicketType", b =>
                {
                    b.Property<int>("TicketTypeId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("TicketTypeId"));

                    b.Property<string>("CreatedById")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.Property<DateTime>("CreatedDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("NameAr")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<string>("NameEn")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<int>("TicketCategoryId")
                        .HasColumnType("int");

                    b.Property<string>("UpdateById")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.Property<DateTime>("UpdateDate")
                        .HasColumnType("datetime2");

                    b.HasKey("TicketTypeId");

                    b.HasIndex("CreatedById");

                    b.HasIndex("TicketCategoryId");

                    b.HasIndex("UpdateById");

                    b.ToTable("TicketTypes");
                });

            modelBuilder.Entity("TicketSystem.DataAccess.Models.AppUser", b =>
                {
                    b.HasOne("TicketSystem.DataAccess.Models.AppUser", "CreatedBy")
                        .WithMany()
                        .HasForeignKey("CreatedById")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.HasOne("TicketSystem.DataAccess.Models.AppUser", "UpdateBy")
                        .WithMany()
                        .HasForeignKey("UpdateById")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.Navigation("CreatedBy");

                    b.Navigation("UpdateBy");
                });

            modelBuilder.Entity("TicketSystem.DataAccess.Models.Comment", b =>
                {
                    b.HasOne("TicketSystem.DataAccess.Models.AppUser", "CreatedBy")
                        .WithMany("CreatedComments")
                        .HasForeignKey("CreatedById")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("TicketSystem.DataAccess.Models.Ticket", "Ticket")
                        .WithMany("Comments")
                        .HasForeignKey("TicketId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("TicketSystem.DataAccess.Models.AppUser", "UpdateBy")
                        .WithMany("UpdatedComments")
                        .HasForeignKey("UpdateById")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("CreatedBy");

                    b.Navigation("Ticket");

                    b.Navigation("UpdateBy");
                });

            modelBuilder.Entity("TicketSystem.DataAccess.Models.Product", b =>
                {
                    b.HasOne("TicketSystem.DataAccess.Models.AppUser", "CreatedBy")
                        .WithMany("CreatedProducts")
                        .HasForeignKey("CreatedById")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("TicketSystem.DataAccess.Models.AppUser", "UpdateBy")
                        .WithMany("UpdatedProducts")
                        .HasForeignKey("UpdateById")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("CreatedBy");

                    b.Navigation("UpdateBy");
                });

            modelBuilder.Entity("TicketSystem.DataAccess.Models.Ticket", b =>
                {
                    b.HasOne("TicketSystem.DataAccess.Models.AppUser", "AssignedTo")
                        .WithMany("AssignedTickets")
                        .HasForeignKey("AssignedToId")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.HasOne("TicketSystem.DataAccess.Models.AppUser", "CreatedBy")
                        .WithMany("CreatedTickets")
                        .HasForeignKey("CreatedById")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("TicketSystem.DataAccess.Models.Product", "Product")
                        .WithMany("Tickets")
                        .HasForeignKey("ProductId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("TicketSystem.DataAccess.Models.TicketType", "TicketType")
                        .WithMany("Tickets")
                        .HasForeignKey("TicketTypeId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("TicketSystem.DataAccess.Models.AppUser", "UpdateBy")
                        .WithMany("UpdatedTickets")
                        .HasForeignKey("UpdateById")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("AssignedTo");

                    b.Navigation("CreatedBy");

                    b.Navigation("Product");

                    b.Navigation("TicketType");

                    b.Navigation("UpdateBy");
                });

            modelBuilder.Entity("TicketSystem.DataAccess.Models.TicketAttachment", b =>
                {
                    b.HasOne("TicketSystem.DataAccess.Models.AppUser", "CreatedBy")
                        .WithMany("CreatedAttachments")
                        .HasForeignKey("CreatedById")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("TicketSystem.DataAccess.Models.Ticket", "Ticket")
                        .WithMany("Attachments")
                        .HasForeignKey("TicketId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("TicketSystem.DataAccess.Models.AppUser", "UpdateBy")
                        .WithMany("UpdatedAttachments")
                        .HasForeignKey("UpdateById")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("CreatedBy");

                    b.Navigation("Ticket");

                    b.Navigation("UpdateBy");
                });

            modelBuilder.Entity("TicketSystem.DataAccess.Models.TicketCategory", b =>
                {
                    b.HasOne("TicketSystem.DataAccess.Models.AppUser", "CreatedBy")
                        .WithMany("CreatedCategories")
                        .HasForeignKey("CreatedById")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("TicketSystem.DataAccess.Models.AppUser", "UpdateBy")
                        .WithMany("UpdatedCategories")
                        .HasForeignKey("UpdateById")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("CreatedBy");

                    b.Navigation("UpdateBy");
                });

            modelBuilder.Entity("TicketSystem.DataAccess.Models.TicketDetail", b =>
                {
                    b.HasOne("TicketSystem.DataAccess.Models.AppUser", "CreateBy")
                        .WithMany("TicketDetails")
                        .HasForeignKey("CreateById")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("TicketSystem.DataAccess.Models.Ticket", "Ticket")
                        .WithMany("TicketDetails")
                        .HasForeignKey("TicketId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("CreateBy");

                    b.Navigation("Ticket");
                });

            modelBuilder.Entity("TicketSystem.DataAccess.Models.TicketType", b =>
                {
                    b.HasOne("TicketSystem.DataAccess.Models.AppUser", "CreatedBy")
                        .WithMany("CreatedTypes")
                        .HasForeignKey("CreatedById")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("TicketSystem.DataAccess.Models.TicketCategory", "TicketCategory")
                        .WithMany("TicketTypes")
                        .HasForeignKey("TicketCategoryId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("TicketSystem.DataAccess.Models.AppUser", "UpdateBy")
                        .WithMany("UpdatedTypes")
                        .HasForeignKey("UpdateById")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("CreatedBy");

                    b.Navigation("TicketCategory");

                    b.Navigation("UpdateBy");
                });

            modelBuilder.Entity("TicketSystem.DataAccess.Models.AppUser", b =>
                {
                    b.Navigation("AssignedTickets");

                    b.Navigation("CreatedAttachments");

                    b.Navigation("CreatedCategories");

                    b.Navigation("CreatedComments");

                    b.Navigation("CreatedProducts");

                    b.Navigation("CreatedTickets");

                    b.Navigation("CreatedTypes");

                    b.Navigation("TicketDetails");

                    b.Navigation("UpdatedAttachments");

                    b.Navigation("UpdatedCategories");

                    b.Navigation("UpdatedComments");

                    b.Navigation("UpdatedProducts");

                    b.Navigation("UpdatedTickets");

                    b.Navigation("UpdatedTypes");
                });

            modelBuilder.Entity("TicketSystem.DataAccess.Models.Product", b =>
                {
                    b.Navigation("Tickets");
                });

            modelBuilder.Entity("TicketSystem.DataAccess.Models.Ticket", b =>
                {
                    b.Navigation("Attachments");

                    b.Navigation("Comments");

                    b.Navigation("TicketDetails");
                });

            modelBuilder.Entity("TicketSystem.DataAccess.Models.TicketCategory", b =>
                {
                    b.Navigation("TicketTypes");
                });

            modelBuilder.Entity("TicketSystem.DataAccess.Models.TicketType", b =>
                {
                    b.Navigation("Tickets");
                });
#pragma warning restore 612, 618
        }
    }
}
