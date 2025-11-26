using ExpenseService.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace ExpenseService.Migrations
{
    [DbContext(typeof(ExpenseDbContext))]
    partial class ExpenseDbContextModelSnapshot : ModelSnapshot
    {
        /// <inheritdoc />
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.0")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("ExpenseService.Models.Expense", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasColumnName("id");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<decimal>("Amount")
                        .HasColumnType("numeric(10,2)")
                        .HasColumnName("amount");

                    b.Property<string>("Category")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)")
                        .HasColumnName("category");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("created_at");

                    b.Property<DateTime>("Date")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("date");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasMaxLength(1000)
                        .HasColumnType("character varying(1000)")
                        .HasColumnName("description");

                    b.HasKey("Id");

                    b.ToTable("expenses", (string)null);
                });
#pragma warning restore 612, 618
        }
    }
}
