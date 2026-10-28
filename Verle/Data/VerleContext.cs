using Microsoft.EntityFrameworkCore;
using Verle.Models;

namespace Verle.Data
{
    public class VerleContext : DbContext
    {
        public VerleContext(DbContextOptions<VerleContext> options)
            : base(options)
        {
        }

        public DbSet<Nomenclatura> Nomenclaturas =>
            Set<Nomenclatura>();
        public DbSet<NomenclaturaClass> NomenclaturaClasses =>
            Set<NomenclaturaClass>();
        public DbSet<NomenclaturaClassLink> NomenclaturaClassLinks =>
            Set<NomenclaturaClassLink>();
        public DbSet<SkuClass> SkuClasses =>
            Set<SkuClass>();
        public DbSet<ProductType> ProductTypes =>
            Set<ProductType>();
        public DbSet<Brand> Brands =>
            Set<Brand>();
        public DbSet<NomenclaturaSkuClassLink> NomenclaturaSkuClassLinks =>
            Set<NomenclaturaSkuClassLink>();
        public DbSet<NomenclaturaProductTypeLink> NomenclaturaProductTypeLinks =>
                Set<NomenclaturaProductTypeLink>();
        public DbSet<NomenclaturaBrandLink> NomenclaturaBrandLinks =>
            Set<NomenclaturaBrandLink>();
        public DbSet<BrandGroup> BrandGroups =>
            Set<BrandGroup>();
        public DbSet<BrandBrandGroupLink> BrandBrandGroupLinks =>
            Set<BrandBrandGroupLink>();
        public DbSet<Counterpart> Counterparts =>
            Set<Counterpart>();
        public DbSet<CounterpartGroup> CounterpartGroups =>
            Set<CounterpartGroup>();
        public DbSet<CounterpartCounterpartGroupLink> CounterpartCounterpartGroupLinks =>
            Set<CounterpartCounterpartGroupLink>();
        public DbSet<SalesChannel> SalesChannels => 
            Set<SalesChannel>();
        public DbSet<CounterpartGroupSalesChannelLink> CounterpartGroupSalesChannelLinks =>
            Set<CounterpartGroupSalesChannelLink>(); 
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Справочники.

            modelBuilder.Entity<Nomenclatura>(entity =>
            {
                entity.ToTable("nomenclatura");

                entity.HasKey(x => x.Id);

                entity.Property(x => x.Id)
                    .HasColumnName("id")
                    .ValueGeneratedOnAdd();

                entity.Property(x => x.Id1C)
                    .HasColumnName("id1C")
                    .HasMaxLength(100)
                    .IsRequired();

                entity.HasIndex(x => x.Id1C)
                    .IsUnique()
                    .HasDatabaseName("ux_nomenclatura_id1c");
            });
            modelBuilder.Entity<SkuClass>(entity =>
            {
                entity.ToTable("skuClass");

                entity.HasKey(x => x.Id);

                entity.Property(x => x.Id)
                    .HasColumnName("id");

                entity.Property(x => x.Name)
                    .HasColumnName("name")
                    .HasMaxLength(255)
                    .IsRequired();
            });
            modelBuilder.Entity<ProductType>(entity =>
            {
                entity.ToTable("productType");

                entity.HasKey(x => x.Id);

                entity.Property(x => x.Id)
                    .HasColumnName("id");

                entity.Property(x => x.Name)
                    .HasColumnName("name")
                    .HasMaxLength(255)
                    .IsRequired();
            });
            modelBuilder.Entity<Brand>(entity =>
            {
                entity.ToTable("brand");

                entity.HasKey(x => x.Id);

                entity.Property(x => x.Id)
                    .HasColumnName("id");

                entity.Property(x => x.Name)
                    .HasColumnName("name")
                    .HasMaxLength(255)
                    .IsRequired();
            });
            modelBuilder.Entity<BrandGroup>(entity =>
            {
                entity.ToTable("brandGroup");

                entity.HasKey(x => x.Id);

                entity.Property(x => x.Id)
                    .HasColumnName("id")
                    .ValueGeneratedOnAdd();

                entity.Property(x => x.Name)
                    .HasColumnName("name")
                    .HasMaxLength(255)
                    .IsRequired();
            });
            modelBuilder.Entity<Counterpart>(entity =>
            {
                entity.ToTable("counterpart");

                entity.HasKey(x => x.Id);

                entity.Property(x => x.Id)
                    .HasColumnName("id")
                    .ValueGeneratedOnAdd();

                entity.Property(x => x.Id1C)
                    .HasColumnName("id1C")
                    .HasMaxLength(100)
                    .IsRequired();

                entity.Property(x => x.INN)
                    .HasColumnName("INN")
                    .HasMaxLength(12)
                    .IsRequired(false);

                entity.Property(x => x.Name)
                    .HasColumnName("name")
                    .HasMaxLength(255)
                    .IsRequired();

                entity.HasIndex(x => x.Id1C)
                    .IsUnique()
                    .HasDatabaseName("ux_counterpart_id1c");
            });
            modelBuilder.Entity<CounterpartGroup>(entity =>
            {
                entity.ToTable("counterpartGroup");

                entity.HasKey(x => x.Id);

                entity.Property(x => x.Id)
                    .HasColumnName("id")
                    .ValueGeneratedOnAdd();

                entity.Property(x => x.Name)
                    .HasColumnName("name")
                    .HasMaxLength(255)
                    .IsRequired();
            });
            modelBuilder.Entity<SalesChannel>(entity =>
            {
                entity.ToTable("salesChannel");

                entity.HasKey(x => x.Id);

                entity.Property(x => x.Id)
                    .HasColumnName("id")
                    .ValueGeneratedOnAdd();

                entity.Property(x => x.Name)
                    .HasColumnName("name")
                    .HasMaxLength(255)
                    .IsRequired();
            });
            // Основной класс номенклатуры.
            modelBuilder.Entity<NomenclaturaClassLink>(entity =>
            {
                entity.ToTable("nomenclaturaClassLink");

                entity.HasKey(x => x.NomenclaturaId);

                entity.Property(x => x.NomenclaturaId)
                    .HasColumnName("nomenclaturaId")
                    .ValueGeneratedNever();

                entity.Property(x => x.NomenclaturaClassId)
                    .HasColumnName("nomenclaturaClassId");

                entity.HasOne(x => x.Nomenclatura)
                    .WithOne(x => x.ClassLink)
                    .HasForeignKey<NomenclaturaClassLink>(
                        x => x.NomenclaturaId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(x => x.NomenclaturaClass)
                    .WithMany(x => x.NomenclaturaLinks)
                    .HasForeignKey(x => x.NomenclaturaClassId)
                    .OnDelete(DeleteBehavior.Restrict);
            });
            // Класс SKU.
            modelBuilder.Entity<NomenclaturaSkuClassLink>(entity =>
            {
                entity.ToTable("nomenclaturaSkuClass");

                entity.HasKey(x => x.NomenclaturaId);

                entity.Property(x => x.NomenclaturaId)
                    .HasColumnName("nomenclaturaId")
                    .ValueGeneratedNever();

                entity.Property(x => x.SkuClassId)
                    .HasColumnName("skuClassId");

                entity.HasOne(x => x.Nomenclatura)
                    .WithOne(x => x.SkuClassLink)
                    .HasForeignKey<NomenclaturaSkuClassLink>(
                        x => x.NomenclaturaId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(x => x.SkuClass)
                    .WithMany(x => x.NomenclaturaLinks)
                    .HasForeignKey(x => x.SkuClassId)
                    .OnDelete(DeleteBehavior.Restrict);
            });
            // Тип продукта.
            modelBuilder.Entity<NomenclaturaProductTypeLink>(entity =>
            {
                entity.ToTable("nomenclaturaProductType");

                entity.HasKey(x => x.NomenclaturaId);

                entity.Property(x => x.NomenclaturaId)
                    .HasColumnName("nomenclaturaId")
                    .ValueGeneratedNever();

                entity.Property(x => x.ProductTypeId)
                    .HasColumnName("productTypeId");

                entity.HasOne(x => x.Nomenclatura)
                    .WithOne(x => x.ProductTypeLink)
                    .HasForeignKey<NomenclaturaProductTypeLink>(
                        x => x.NomenclaturaId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(x => x.ProductType)
                    .WithMany(x => x.NomenclaturaLinks)
                    .HasForeignKey(x => x.ProductTypeId)
                    .OnDelete(DeleteBehavior.Restrict);
            });
            // Бренд.
            modelBuilder.Entity<NomenclaturaBrandLink>(entity =>
            {
                entity.ToTable("nomenclaturaBrand");

                entity.HasKey(x => x.NomenclaturaId);

                entity.Property(x => x.NomenclaturaId)
                    .HasColumnName("nomenclaturaId")
                    .ValueGeneratedNever();

                entity.Property(x => x.BrandId)
                    .HasColumnName("brandId");

                entity.HasOne(x => x.Nomenclatura)
                    .WithOne(x => x.BrandLink)
                    .HasForeignKey<NomenclaturaBrandLink>(
                        x => x.NomenclaturaId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(x => x.Brand)
                    .WithMany(x => x.NomenclaturaLinks)
                    .HasForeignKey(x => x.BrandId)
                    .OnDelete(DeleteBehavior.Restrict);
            });
            // Группа бренда
            modelBuilder.Entity<BrandBrandGroupLink>(entity =>
            {
                entity.ToTable("brandBrandGroup");

                // Один бренд может иметь не более одной группы.
                entity.HasKey(x => x.BrandId);

                entity.Property(x => x.BrandId)
                    .HasColumnName("brandId")
                    .ValueGeneratedNever();

                entity.Property(x => x.BrandGroupId)
                    .HasColumnName("brandGroupId");

                // Brand 1 -> 0..1 BrandBrandGroupLink.
                entity.HasOne(x => x.Brand)
                    .WithOne(x => x.BrandGroupLink)
                    .HasForeignKey<BrandBrandGroupLink>(
                        x => x.BrandId)
                    .OnDelete(DeleteBehavior.Cascade);

                // BrandGroup 1 -> много BrandBrandGroupLink.
                entity.HasOne(x => x.BrandGroup)
                    .WithMany(x => x.BrandLinks)
                    .HasForeignKey(x => x.BrandGroupId)
                    .OnDelete(DeleteBehavior.Restrict);
            });
            // Каналы продаж
            modelBuilder.Entity<CounterpartGroupSalesChannelLink>(entity =>
            {
                entity.ToTable("counterpartGroupSalesChannel");

                // Один counterpartGroup может иметь максимум один канал.
                entity.HasKey(x => x.CounterpartGroupId);

                entity.Property(x => x.CounterpartGroupId)
                    .HasColumnName("counterpartGroupId")
                    .ValueGeneratedNever();

                entity.Property(x => x.SalesChannelId)
                    .HasColumnName("salesChannelId")
                    .IsRequired();

                // CounterpartGroup 1 -> 0..1 link.
                entity.HasOne(x => x.CounterpartGroup)
                    .WithOne(x => x.SalesChannelLink)
                    .HasForeignKey<CounterpartGroupSalesChannelLink>(
                        x => x.CounterpartGroupId)
                    .OnDelete(DeleteBehavior.Cascade);

                // SalesChannel 1 -> много link.
                entity.HasOne(x => x.SalesChannel)
                    .WithMany(x => x.CounterpartGroupLinks)
                    .HasForeignKey(x => x.SalesChannelId)
                    .OnDelete(DeleteBehavior.Restrict);
            });
        }
    }
}