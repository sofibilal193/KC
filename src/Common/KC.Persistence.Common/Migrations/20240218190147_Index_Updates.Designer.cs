﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using KC.Persistence.Common.Events;

#nullable disable

namespace KC.Persistence.Common.Migrations
{
    [DbContext(typeof(EventSqlDbContext))]
    [Migration("20240218190147_Index_Updates")]
    partial class Index_Updates
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasDefaultSchema("event")
                .HasAnnotation("ProductVersion", "8.0.1")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("KC.Domain.Common.Entities.Events.SqlEventLog", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnOrder(1);

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<DateTime>("DateTimeUtc")
                        .HasColumnType("datetime2")
                        .HasColumnOrder(3);

                    b.Property<string>("Description")
                        .HasColumnType("nvarchar(max)")
                        .HasColumnOrder(5);

                    b.Property<string>("Event")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)")
                        .HasColumnOrder(2);

                    b.Property<int?>("OrgId")
                        .HasColumnType("int")
                        .HasColumnOrder(7);

                    b.Property<int?>("RecordId")
                        .HasColumnType("int")
                        .HasColumnOrder(8);

                    b.Property<string>("Source")
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)")
                        .HasColumnOrder(4);

                    b.Property<byte[]>("Timestamp")
                        .IsConcurrencyToken()
                        .ValueGeneratedOnAddOrUpdate()
                        .HasColumnType("rowversion")
                        .HasColumnOrder(99);

                    b.Property<int?>("UserId")
                        .HasColumnType("int")
                        .HasColumnOrder(6);

                    b.HasKey("Id");

                    b.ToTable("EventLogs", "event");
                });

            modelBuilder.Entity("KC.Domain.Common.Entities.IntegrationEvents.SqlIntegrationEventLog", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnOrder(1);

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<DateTime>("CreateDateTimeUtc")
                        .HasColumnType("datetime2")
                        .HasColumnOrder(21);

                    b.Property<string>("EventData")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)")
                        .HasColumnOrder(11);

                    b.Property<Guid>("EventId")
                        .HasColumnType("uniqueidentifier")
                        .HasColumnOrder(2);

                    b.Property<string>("EventTypeName")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)")
                        .HasColumnOrder(4);

                    b.Property<string>("HostName")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)")
                        .HasColumnOrder(3);

                    b.Property<DateTime?>("ModifyDateTimeUtc")
                        .IsRequired()
                        .HasColumnType("datetime2")
                        .HasColumnOrder(22);

                    b.Property<int?>("OrgId")
                        .HasColumnType("int")
                        .HasColumnOrder(9);

                    b.Property<int>("RetryCount")
                        .HasColumnType("int")
                        .HasColumnOrder(6);

                    b.Property<string>("Source")
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)")
                        .HasColumnOrder(10);

                    b.Property<string>("State")
                        .IsRequired()
                        .HasMaxLength(25)
                        .HasColumnType("nvarchar(25)")
                        .HasColumnOrder(5);

                    b.Property<byte[]>("Timestamp")
                        .IsConcurrencyToken()
                        .ValueGeneratedOnAddOrUpdate()
                        .HasColumnType("rowversion")
                        .HasColumnOrder(99);

                    b.Property<Guid?>("TransactionId")
                        .HasColumnType("uniqueidentifier")
                        .HasColumnOrder(7);

                    b.Property<int?>("UserId")
                        .HasColumnType("int")
                        .HasColumnOrder(8);

                    b.HasKey("Id");

                    b.HasIndex("EventId")
                        .IsUnique();

                    b.HasIndex("HostName", "State", "CreateDateTimeUtc");

                    b.ToTable("IntegrationEventLogs", "event");
                });
#pragma warning restore 612, 618
        }
    }
}
