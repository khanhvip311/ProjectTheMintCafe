using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace ManagementCafe.Models;

public partial class ManagementCafeContext : DbContext
{
    public ManagementCafeContext()
    {
    }

    public ManagementCafeContext(DbContextOptions<ManagementCafeContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Bill> Bills { get; set; }

    public virtual DbSet<BillDetail> BillDetails { get; set; }

    public virtual DbSet<Booking> Bookings { get; set; }

    public virtual DbSet<BookingDetail> BookingDetails { get; set; }

    public virtual DbSet<Category> Categories { get; set; }

    public virtual DbSet<PartyTable> PartyTables { get; set; }

    public virtual DbSet<Product> Products { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseSqlServer("Data Source=eren\\eren;Initial Catalog=ManagementCafe;Integrated Security=True;Trust Server Certificate=True");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Bill>(entity =>
        {
            entity.HasKey(e => e.BillId).HasName("PK__Bill__11F2FC4A26C03A3B");

            entity.ToTable("Bill");

            entity.Property(e => e.BillId)
                .ValueGeneratedNever()
                .HasColumnName("BillID");
            entity.Property(e => e.TotalPrice).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.UserId).HasColumnName("UserID");

            entity.HasOne(d => d.User).WithMany(p => p.Bills)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK__Bill__UserID__45F365D3");
        });

        modelBuilder.Entity<BillDetail>(entity =>
        {
            entity.HasKey(e => e.BillDetailId).HasName("PK__BillDeta__793CAF75FA4B8CCC");

            entity.ToTable("BillDetail");

            entity.Property(e => e.BillDetailId)
                .ValueGeneratedNever()
                .HasColumnName("BillDetailID");
            entity.Property(e => e.BillId).HasColumnName("BillID");
            entity.Property(e => e.Note).HasColumnType("text");
            entity.Property(e => e.ProductId).HasColumnName("ProductID");

            entity.HasOne(d => d.Bill).WithMany(p => p.BillDetails)
                .HasForeignKey(d => d.BillId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK__BillDetai__BillI__46E78A0C");

            entity.HasOne(d => d.Product).WithMany(p => p.BillDetails)
                .HasForeignKey(d => d.ProductId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK__BillDetai__Produ__47DBAE45");
        });

        modelBuilder.Entity<Booking>(entity =>
        {
            entity.HasKey(e => e.BookingId).HasName("PK__Booking__73951ACD8480BC67");

            entity.ToTable("Booking");

            entity.Property(e => e.BookingId)
                .ValueGeneratedNever()
                .HasColumnName("BookingID");
            entity.Property(e => e.BookingTime).HasColumnType("datetime");
            entity.Property(e => e.Note).HasColumnType("text");
            entity.Property(e => e.TableId).HasColumnName("TableID");
            entity.Property(e => e.UserId).HasColumnName("UserID");

            entity.HasOne(d => d.Table).WithMany(p => p.Bookings)
                .HasForeignKey(d => d.TableId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK__Booking__TableID__48CFD27E");

            entity.HasOne(d => d.User).WithMany(p => p.Bookings)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK__Booking__UserID__49C3F6B7");
        });

        modelBuilder.Entity<BookingDetail>(entity =>
        {
            entity.HasKey(e => e.BookingDetailId).HasName("PK__BookingD__8136D47AD9870057");

            entity.ToTable("BookingDetail");

            entity.Property(e => e.BookingDetailId)
                .ValueGeneratedNever()
                .HasColumnName("BookingDetailID");
            entity.Property(e => e.BookingId).HasColumnName("BookingID");
            entity.Property(e => e.ProductId).HasColumnName("ProductID");

            entity.HasOne(d => d.Booking).WithMany(p => p.BookingDetails)
                .HasForeignKey(d => d.BookingId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK__BookingDe__Booki__4AB81AF0");

            entity.HasOne(d => d.Product).WithMany(p => p.BookingDetails)
                .HasForeignKey(d => d.ProductId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK__BookingDe__Produ__4BAC3F29");
        });

        modelBuilder.Entity<Category>(entity =>
        {
            entity.HasKey(e => e.CateId).HasName("PK__Category__27638D7455263785");

            entity.ToTable("Category");

            entity.Property(e => e.CateId)
                .ValueGeneratedNever()
                .HasColumnName("CateID");
            entity.Property(e => e.Name).HasMaxLength(255);
        });

        modelBuilder.Entity<PartyTable>(entity =>
        {
            entity.HasKey(e => e.TableId).HasName("PK__PartyTab__7D5F018E5B79CBE9");

            entity.ToTable("PartyTable");

            entity.Property(e => e.TableId)
                .ValueGeneratedNever()
                .HasColumnName("TableID");
            entity.Property(e => e.Status).HasMaxLength(50);
        });

        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasKey(e => e.ProductId).HasName("PK__Product__B40CC6ED2E4C4CB8");

            entity.ToTable("Product");

            entity.Property(e => e.ProductId)
                .ValueGeneratedNever()
                .HasColumnName("ProductID");
            entity.Property(e => e.CateId).HasColumnName("CateID");
            entity.Property(e => e.Description).HasColumnType("text");
            entity.Property(e => e.Image)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.Name).HasMaxLength(255);
            entity.Property(e => e.Price).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.Status).HasMaxLength(50);

            entity.HasOne(d => d.Cate).WithMany(p => p.Products)
                .HasForeignKey(d => d.CateId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK__Product__CateID__4CA06362");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK__Users__1788CCACD5BD18E6");

            entity.HasIndex(e => e.Email, "UQ__Users__A9D105349FC33F9F").IsUnique();

            entity.Property(e => e.UserId)
                .ValueGeneratedNever()
                .HasColumnName("UserID");
            entity.Property(e => e.Address).HasColumnType("text");
            entity.Property(e => e.Email)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.Name).HasMaxLength(255);
            entity.Property(e => e.Pass)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.Phone)
                .HasMaxLength(15)
                .IsUnicode(false);
            entity.Property(e => e.Role).HasMaxLength(50);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
