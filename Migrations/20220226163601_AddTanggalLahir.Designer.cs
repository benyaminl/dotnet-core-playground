﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using TodoApi;

#nullable disable

namespace TodoApi.Migrations
{
    [DbContext(typeof(AppDBContext))]
    [Migration("20220226163601_AddTanggalLahir")]
    partial class AddTanggalLahir
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.1")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder, 1L, 1);

            modelBuilder.Entity("TodoApi.Models.MhsModel", b =>
                {
                    b.Property<string>("nrp")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("alamat")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("nama")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("telepon")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("tglLahir")
                        .HasColumnType("datetime2");

                    b.HasKey("nrp");

                    b.ToTable("mhs");
                });

            modelBuilder.Entity("TodoApi.Models.NilaiModel", b =>
                {
                    b.Property<string>("nrp")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("matkul")
                        .HasColumnType("nvarchar(450)");

                    b.Property<byte>("nilai")
                        .HasColumnType("tinyint");

                    b.HasKey("nrp", "matkul");

                    b.ToTable("nilai");
                });

            modelBuilder.Entity("TodoApi.Models.NilaiModel", b =>
                {
                    b.HasOne("TodoApi.Models.MhsModel", "mhs")
                        .WithMany("nilais")
                        .HasForeignKey("nrp")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("mhs");
                });

            modelBuilder.Entity("TodoApi.Models.MhsModel", b =>
                {
                    b.Navigation("nilais");
                });
#pragma warning restore 612, 618
        }
    }
}
