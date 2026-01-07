using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using PizzaOrderApp.Data;

#nullable disable

namespace PizzaOrderapp.Migrations
{
    [DbContext(typeof(PizzaStoreContext))]
    [Migration("20251229135920_UpdatedModels")]
    partial class UpdatedModels
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "9.0.1");

            modelBuilder.Entity("PizzaOrderApp.Models.CustomerInfo", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Address")
                        .IsRequired()
                        .HasMaxLength(250)
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("CreatedDate")
                        .HasColumnType("TEXT");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("TEXT");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("Email")
                        .IsUnique();

                    b.ToTable("CustomerInfos");
                });

            modelBuilder.Entity("PizzaOrderApp.Models.Order", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int>("CustomerInfoId")
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("OrderDate")
                        .HasColumnType("TEXT");

                    b.Property<string>("OrderNumber")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("TEXT");

                    b.Property<string>("SelectedPizzaId")
                        .HasColumnType("TEXT");

                    b.Property<string>("SelectedSizeId")
                        .HasColumnType("TEXT");

                    b.Property<string>("SelectedToppingsJson")
                        .IsRequired()
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT")
                        .HasDefaultValue("[]");

                    b.Property<decimal>("TotalPrice")
                        .HasPrecision(10, 2)
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("CustomerInfoId");

                    b.HasIndex("OrderNumber")
                        .IsUnique();

                    b.HasIndex("SelectedPizzaId");

                    b.HasIndex("SelectedSizeId");

                    b.ToTable("Orders");
                });

            modelBuilder.Entity("PizzaOrderApp.Models.Pizza", b =>
                {
                    b.Property<string>("Id")
                        .HasMaxLength(50)
                        .HasColumnType("TEXT");

                    b.Property<decimal>("BasePrice")
                        .HasPrecision(10, 2)
                        .HasColumnType("TEXT");

                    b.Property<string>("ImageUrl")
                        .HasMaxLength(500)
                        .HasColumnType("TEXT");

                    b.Property<string>("IngredientsJson")
                        .IsRequired()
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT")
                        .HasDefaultValue("[]");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("Pizzas");
                });

            modelBuilder.Entity("PizzaOrderApp.Models.PizzaSize", b =>
                {
                    b.Property<string>("Id")
                        .HasMaxLength(50)
                        .HasColumnType("TEXT");

                    b.Property<double>("Multiplier")
                        .HasColumnType("REAL");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("TEXT");

                    b.Property<string>("PizzaId")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("PizzaId");

                    b.ToTable("PizzaSizes");
                });

            modelBuilder.Entity("PizzaOrderApp.Models.Order", b =>
                {
                    b.HasOne("PizzaOrderApp.Models.CustomerInfo", "CustomerInfo")
                        .WithMany("Orders")
                        .HasForeignKey("CustomerInfoId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("PizzaOrderApp.Models.Pizza", "SelectedPizza")
                        .WithMany("Orders")
                        .HasForeignKey("SelectedPizzaId")
                        .OnDelete(DeleteBehavior.SetNull);

                    b.HasOne("PizzaOrderApp.Models.PizzaSize", "SelectedSize")
                        .WithMany("Orders")
                        .HasForeignKey("SelectedSizeId")
                        .OnDelete(DeleteBehavior.SetNull);

                    b.Navigation("CustomerInfo");

                    b.Navigation("SelectedPizza");

                    b.Navigation("SelectedSize");
                });

            modelBuilder.Entity("PizzaOrderApp.Models.PizzaSize", b =>
                {
                    b.HasOne("PizzaOrderApp.Models.Pizza", "Pizza")
                        .WithMany("Sizes")
                        .HasForeignKey("PizzaId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Pizza");
                });

            modelBuilder.Entity("PizzaOrderApp.Models.CustomerInfo", b =>
                {
                    b.Navigation("Orders");
                });

            modelBuilder.Entity("PizzaOrderApp.Models.Pizza", b =>
                {
                    b.Navigation("Orders");

                    b.Navigation("Sizes");
                });

            modelBuilder.Entity("PizzaOrderApp.Models.PizzaSize", b =>
                {
                    b.Navigation("Orders");
                });
#pragma warning restore 612, 618
        }
    }
}
