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
        public DbSet<Sale> Sales =>
            Set<Sale>();
        public DbSet<Unit> Units =>
            Set<Unit>();
        public DbSet<Fuso> Fusos =>
            Set<Fuso>();
        public DbSet<Shift> Shifts => 
            Set<Shift>();
        public DbSet<LegalEntity> LegalEntities =>
            Set<LegalEntity>();
        public DbSet<OfficeLocation> OfficeLocations =>
            Set<OfficeLocation>();
        public DbSet<Department> Departments =>
            Set<Department>();
        public DbSet<Division> Divisions =>
            Set<Division>();
        public DbSet<Position> Positions => 
            Set<Position>();
        public DbSet<Employee> Employees => Set<Employee>();
        public DbSet<DripSemiproductOrder> DripSemiproductOrders =>
            Set<DripSemiproductOrder>();
        public DbSet<DripSemiproductOrderExecution> DripSemiproductOrderExecutions =>
            Set<DripSemiproductOrderExecution>();
        public DbSet<ProductionSpecification> ProductionSpecifications =>
            Set<ProductionSpecification>();
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
            modelBuilder.Entity<Sale>(entity =>
            {
                entity.ToTable("sales");

                entity.HasKey(x => x.Id);

                entity.Property(x => x.Id)
                    .HasColumnName("id")
                    .ValueGeneratedOnAdd();

                entity.Property(x => x.CounterpartId)
                    .HasColumnName("counterpartId")
                    .IsRequired();

                entity.Property(x => x.NomenclaturaId)
                    .HasColumnName("nomenclaturaId")
                    .IsRequired();

                entity.Property(x => x.CounterpartId1C)
                    .HasColumnName("counterpartId1C")
                    .HasMaxLength(100)
                    .IsRequired();

                entity.Property(x => x.CounterpartName1C)
                    .HasColumnName("counterpartName1C")
                    .HasMaxLength(255)
                    .IsRequired();

                entity.Property(x => x.Date)
                    .HasColumnName("date")
                    .HasColumnType("date")
                    .IsRequired();

                entity.Property(x => x.Document)
                    .HasColumnName("document")
                    .HasMaxLength(500)
                    .IsRequired();

                entity.Property(x => x.NomenclaturaId1C)
                    .HasColumnName("nomenclaturaId1C")
                    .HasMaxLength(100)
                    .IsRequired();

                entity.Property(x => x.NomenclaturaName1C)
                    .HasColumnName("nomenclaturaName1C")
                    .HasMaxLength(255)
                    .IsRequired();

                entity.Property(x => x.Quantity)
                    .HasColumnName("quantity")
                    .HasColumnType("double precision")
                    .IsRequired();

                entity.Property(x => x.Revenue)
                    .HasColumnName("revenue")
                    .HasColumnType("double precision")
                    .IsRequired();

                entity.Property(x => x.Cost)
                    .HasColumnName("cost")
                    .HasColumnType("double precision")
                    .IsRequired();

                entity.Property(x => x.GrossProfit)
                    .HasColumnName("grossProfit")
                    .HasColumnType("double precision")
                    .IsRequired();

                entity.Property(x => x.Margin)
                    .HasColumnName("margin")
                    .HasColumnType("double precision")
                    .IsRequired();

                entity.HasOne(x => x.Counterpart)
                    .WithMany()
                    .HasForeignKey(x => x.CounterpartId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(x => x.Nomenclatura)
                    .WithMany()
                    .HasForeignKey(x => x.NomenclaturaId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasIndex(x => x.CounterpartId);

                entity.HasIndex(x => x.NomenclaturaId);

                entity.HasIndex(x => x.Date);
                // Уникальное сочетание:
                // контрагент из 1С + документ + номенклатура из 1С.
                entity.HasIndex(x => new
                {
                    x.CounterpartId1C,
                    x.Document,
                    x.NomenclaturaId1C
                })
                    .IsUnique()
                    .HasDatabaseName(
                        "UX_sales_counterpartId1C_document_nomenclaturaId1C");
            });
            modelBuilder.Entity<Unit>(entity =>
            {
                entity.ToTable("units");

                entity.HasKey(x => x.Id);

                entity.Property(x => x.Id)
                    .HasColumnName("id")
                    .ValueGeneratedOnAdd();

                entity.Property(x => x.Name)
                    .HasColumnName("name")
                    .HasMaxLength(100)
                    .IsRequired();
            });
            modelBuilder.Entity<Fuso>(entity =>
            {
                entity.ToTable("FUSO");

                entity.HasKey(x => x.Id);

                entity.Property(x => x.Id)
                    .HasColumnName("id")
                    .ValueGeneratedOnAdd();

                entity.Property(x => x.FusoNumber)
                    .HasColumnName("fusoNumber")
                    .IsRequired();

                entity.Property(x => x.CommissioningDate)
                    .HasColumnName("commissioningDate")
                    .HasColumnType("date")
                    .IsRequired();

                entity.Property(x => x.Capacity)
                    .HasColumnName("capacity")
                    .HasColumnType("double precision")
                    .IsRequired();

                entity.Property(x => x.UnitId)
                    .HasColumnName("unit")
                    .IsRequired();

                entity.HasOne(x => x.Unit)
                    .WithMany()
                    .HasForeignKey(x => x.UnitId)
                    .OnDelete(DeleteBehavior.Restrict);
            });
            modelBuilder.Entity<Shift>(entity =>
            {
                entity.ToTable("shifts");

                entity.HasKey(x => x.Id);

                entity.Property(x => x.Id)
                    .HasColumnName("id")
                    .ValueGeneratedOnAdd();

                entity.Property(x => x.Name)
                    .HasColumnName("name")
                    .HasMaxLength(50)
                    .IsRequired();

                entity.HasData(
                    new Shift
                    {
                        Id = 1,
                        Name = "день"
                    },
                    new Shift
                    {
                        Id = 2,
                        Name = "ночь"
                    }
                );
            });
            modelBuilder.Entity<LegalEntity>(entity =>
            {
                entity.ToTable("legalEntity");

                entity.HasKey(x => x.Id);

                entity.Property(x => x.Id)
                    .HasColumnName("id")
                    .ValueGeneratedOnAdd();

                entity.Property(x => x.FullName)
                    .HasColumnName("fullName")
                    .HasMaxLength(300)
                    .IsRequired();

                entity.Property(x => x.ShortName)
                    .HasColumnName("shortName")
                    .HasMaxLength(100)
                    .IsRequired();

                entity.Property(x => x.INN)
                    .HasColumnName("INN")
                    .HasMaxLength(10)
                    .IsRequired();
            });
            modelBuilder.Entity<OfficeLocation>(entity =>
            {
                entity.ToTable("officeLocation");

                entity.HasKey(x => x.Id);

                entity.Property(x => x.Id)
                    .HasColumnName("id")
                    .ValueGeneratedOnAdd();

                entity.Property(x => x.Name)
                    .HasColumnName("name")
                    .HasMaxLength(100)
                    .IsRequired();

                entity.Property(x => x.ShortName)
                    .HasColumnName("shortName")
                    .HasMaxLength(20)
                    .IsRequired();
            });
            modelBuilder.Entity<Department>(entity =>
            {
                entity.ToTable("Department");

                entity.HasKey(x => x.Id);

                entity.Property(x => x.Id)
                    .HasColumnName("id")
                    .ValueGeneratedOnAdd();

                entity.Property(x => x.Name)
                    .HasColumnName("name")
                    .HasMaxLength(100)
                    .IsRequired();
            });
            modelBuilder.Entity<Division>(entity =>
            {
                entity.ToTable("division");

                entity.HasKey(x => x.Id);

                entity.Property(x => x.Id)
                    .HasColumnName("id")
                    .ValueGeneratedOnAdd();

                entity.Property(x => x.Name)
                    .HasColumnName("name")
                    .HasMaxLength(100)
                    .IsRequired();

                entity.Property(x => x.DepartmentId)
                    .HasColumnName("departmentId")
                    .IsRequired();

                entity.HasOne(x => x.Department)
                    .WithMany()
                    .HasForeignKey(x => x.DepartmentId)
                    .IsRequired()
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasIndex(x => new { x.DepartmentId, x.Name })
                    .IsUnique()
                    .HasDatabaseName("UX_Division_DepartmentId_Name");
            });
            modelBuilder.Entity<Position>(entity =>
            {
                entity.ToTable("Position");

                entity.HasKey(x => x.Id);

                entity.Property(x => x.Id)
                    .HasColumnName("id")
                    .ValueGeneratedOnAdd();

                entity.Property(x => x.Name)
                    .HasColumnName("name")
                    .HasMaxLength(100)
                    .IsRequired();

                entity.Property(x => x.DivisionId)
                    .HasColumnName("divisionId")
                    .IsRequired();

                entity.HasOne(x => x.Division)
                    .WithMany()
                    .HasForeignKey(x => x.DivisionId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasIndex(x => new { x.DivisionId, x.Name })
                    .IsUnique()
                    .HasDatabaseName("UX_Position_DivisionId_Name");
            });
            modelBuilder.Entity<Employee>(entity =>
            {
                entity.ToTable("employees");

                entity.HasKey(x => x.Id);

                entity.Property(x => x.Id)
                    .HasColumnName("id")
                    .ValueGeneratedOnAdd();

                entity.Property(x => x.Surname)
                    .HasColumnName("surname")
                    .HasMaxLength(100)
                    .IsRequired();

                entity.Property(x => x.Name)
                    .HasColumnName("name")
                    .HasMaxLength(100)
                    .IsRequired();

                entity.Property(x => x.Patronymic)
                    .HasColumnName("patronymic")
                    .HasMaxLength(100)
                    .IsRequired(false);

                entity.Property(x => x.HireDate)
                    .HasColumnName("hireDate")
                    .HasColumnType("date")
                    .IsRequired();

                entity.Property(x => x.PositionId)
                    .HasColumnName("positionId")
                    .IsRequired();

                entity.HasOne(x => x.Position)
                    .WithMany()
                    .HasForeignKey(x => x.PositionId)
                    .OnDelete(DeleteBehavior.Restrict);
            });
            // Заказы на производство полуфабрикатов дрип-пакетов.
            modelBuilder.Entity<DripSemiproductOrder>(entity =>
            {
                entity.ToTable("DripSemiproductOrders");

                entity.HasKey(x => x.Id);

                entity.Property(x => x.Id)
                    .HasColumnName("id")
                    .ValueGeneratedOnAdd();

                entity.Property(x => x.Id1C)
                    .HasColumnName("id1C")
                    .HasColumnType("text")
                    .IsRequired();

                entity.HasIndex(x => x.Id1C)
                    .IsUnique()
                    .HasDatabaseName("UX_DripSemiproductOrders_id1C");

                entity.Property(x => x.OrderDate)
                    .HasColumnName("orderDate")
                    .HasColumnType("date")
                    .IsRequired();

                entity.Property(x => x.NomenclaturaId)
                    .HasColumnName("nomenclaturaId")
                    .IsRequired();

                entity.Property(x => x.ShiftId)
                    .HasColumnName("shift")
                    .HasDefaultValue(1)
                    .IsRequired();

                entity.Property(x => x.FusoId)
                    .HasColumnName("fusoId")
                    .IsRequired();

                entity.Property(x => x.Comment)
                    .HasColumnName("comment")
                    .HasColumnType("text")
                    .IsRequired(false);

                entity.Property(x => x.Plan)
                    .HasColumnName("plan")
                    .HasPrecision(18, 3)
                    .IsRequired();

                entity.HasOne(x => x.Nomenclatura)
                    .WithMany()
                    .HasForeignKey(x => x.NomenclaturaId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(x => x.Shift)
                    .WithMany()
                    .HasForeignKey(x => x.ShiftId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(x => x.Fuso)
                    .WithMany()
                    .HasForeignKey(x => x.FusoId)
                    .OnDelete(DeleteBehavior.Restrict);
            });
            modelBuilder.Entity<DripSemiproductOrderExecution>(entity =>
            {
                entity.ToTable("DripSemiproductOrderExecutions");

                entity.HasKey(x => x.OrderId);

                entity.Property(x => x.OrderId)
                    .HasColumnName("orderId")
                    .ValueGeneratedNever();

                entity.Property(x => x.Actual)
                    .HasColumnName("actual")
                    .HasPrecision(18, 3)
                    .IsRequired();

                entity.Property(x => x.Comment)
                    .HasColumnName("comment")
                    .HasColumnType("text")
                    .IsRequired(false);

                entity.HasOne(x => x.Order)
                    .WithOne(x => x.Execution)
                    .HasForeignKey<DripSemiproductOrderExecution>(x => x.OrderId)
                    .OnDelete(DeleteBehavior.Cascade);
            });
            // Производственные спецификации.
            modelBuilder.Entity<ProductionSpecification>(entity =>
            {
                entity.ToTable("productionSpecifications", table =>
                {
                    table.HasCheckConstraint(
                        "CK_productionSpecifications_quantityInput_positive",
                        "\"quantityInput\" > 0");

                    table.HasCheckConstraint(
                        "CK_productionSpecifications_quantityOutput_positive",
                        "\"quantityOutput\" > 0");
                });

                entity.HasKey(x => x.Id);

                entity.Property(x => x.Id)
                    .HasColumnName("id")
                    .ValueGeneratedOnAdd();

                entity.Property(x => x.NomenclaturaIdOutput)
                    .HasColumnName("nomenclaturaIdOutput")
                    .IsRequired();

                entity.Property(x => x.NomenclaturaIdInput)
                    .HasColumnName("nomenclaturaIdInput")
                    .IsRequired();

                entity.Property(x => x.QuantityInput)
                    .HasColumnName("quantityInput")
                    .HasPrecision(18, 6)
                    .IsRequired();

                entity.Property(x => x.QuantityOutput)
                    .HasColumnName("quantityOutput")
                    .HasPrecision(18, 6)
                    .IsRequired();

                entity.HasOne(x => x.NomenclaturaOutput)
                    .WithMany()
                    .HasForeignKey(x => x.NomenclaturaIdOutput)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(x => x.NomenclaturaInput)
                    .WithMany()
                    .HasForeignKey(x => x.NomenclaturaIdInput)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasIndex(x => x.NomenclaturaIdOutput)
                    .HasDatabaseName(
                        "IX_productionSpecifications_nomenclaturaIdOutput");

                entity.HasIndex(x => x.NomenclaturaIdInput)
                    .HasDatabaseName(
                        "IX_productionSpecifications_nomenclaturaIdInput");
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
            // Класс номенклатуры
            modelBuilder.Entity<NomenclaturaClass>(entity =>
            {
                entity.ToTable("nomenclaturaClass");

                entity.HasKey(x => x.Id);

                entity.Property(x => x.Id)
                    .HasColumnName("id")
                    .ValueGeneratedOnAdd();

                entity.Property(x => x.Name)
                    .HasColumnName("name")
                    .HasMaxLength(255)
                    .IsRequired();
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
            // Группа контрагента.
            modelBuilder.Entity<CounterpartCounterpartGroupLink>(entity =>
            {
                entity.ToTable("counterpartCounterpartGroup");

                // Один контрагент может иметь не более одной группы.
                entity.HasKey(x => x.CounterpartId);

                entity.Property(x => x.CounterpartId)
                    .HasColumnName("counterpartId")
                    .ValueGeneratedNever();

                entity.Property(x => x.CounterpartGroupId)
                    .HasColumnName("counterpartGroupId")
                    .IsRequired();

                // Counterpart 1 -> 0..1 связующая запись.
                entity.HasOne(x => x.Counterpart)
                    .WithOne(x => x.CounterpartGroupLink)
                    .HasForeignKey<CounterpartCounterpartGroupLink>(
                        x => x.CounterpartId)
                    .OnDelete(DeleteBehavior.Cascade);

                // CounterpartGroup 1 -> много связующих записей.
                entity.HasOne(x => x.CounterpartGroup)
                    .WithMany(x => x.CounterpartLinks)
                    .HasForeignKey(x => x.CounterpartGroupId)
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