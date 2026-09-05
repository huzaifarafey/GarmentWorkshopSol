using GarmentWorkshop.Models;
using Microsoft.EntityFrameworkCore;

namespace GarmentWorkshop.EF
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Worker> Workers { get; set; }
        public DbSet<Party> Parties { get; set; }
        public DbSet<Garment> Garments { get; set; }
        public DbSet<WorkerRate> WorkerRates { get; set; }
        public DbSet<WorkOrder> WorkOrders { get; set; }
        public DbSet<Production> Productions { get; set; }
        public DbSet<Machine> Machines { get; set; }
        public DbSet<MachineMaintenance> MachineMaintenances { get; set; }
        public DbSet<ExpenseCategory> ExpenseCategories { get; set; }
        public DbSet<Expense> Expenses { get; set; }
        public DbSet<ThreadStock> ThreadStocks { get; set; }
        public DbSet<ThreadTransaction> ThreadTransactions { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // ---------- Worker ----------
            modelBuilder.Entity<Worker>(entity =>
            {
                entity.Property(w => w.Status)
                    .HasConversion<string>()
                    .HasMaxLength(20);
            });

            // ---------- Party ----------
            modelBuilder.Entity<Party>(entity =>
            {
                entity.Property(p => p.Status)
                    .HasConversion<string>()
                    .HasMaxLength(20);
            });

            // ---------- Garment ----------
            modelBuilder.Entity<Garment>(entity =>
            {
                entity.Property(g => g.Status)
                    .HasConversion<string>()
                    .HasMaxLength(20);
            });

            // ---------- WorkerRate ----------
            modelBuilder.Entity<WorkerRate>(entity =>
            {
                entity.HasOne(wr => wr.Worker)
                    .WithMany(w => w.WorkerRates)
                    .HasForeignKey(wr => wr.WorkerId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(wr => wr.Garment)
                    .WithMany(g => g.WorkerRates)
                    .HasForeignKey(wr => wr.GarmentId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.Property(wr => wr.RatePerPiece)
                    .HasColumnType("decimal(10,2)");

                // Speeds up "latest rate for worker+garment as of date" lookups
                entity.HasIndex(wr => new { wr.WorkerId, wr.GarmentId, wr.EffectiveFrom });
            });

            // ---------- WorkOrder ----------
            modelBuilder.Entity<WorkOrder>(entity =>
            {
                entity.HasOne(wo => wo.Party)
                    .WithMany(p => p.WorkOrders)
                    .HasForeignKey(wo => wo.PartyId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(wo => wo.Garment)
                    .WithMany(g => g.WorkOrders)
                    .HasForeignKey(wo => wo.GarmentId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.Property(wo => wo.PartyRatePerPiece)
                    .HasColumnType("decimal(10,2)");

                entity.Property(wo => wo.Status)
                    .HasConversion<string>()
                    .HasMaxLength(20);
            });

            // ---------- Production ----------
            modelBuilder.Entity<Production>(entity =>
            {
                entity.HasOne(pr => pr.Worker)
                    .WithMany(w => w.Productions)
                    .HasForeignKey(pr => pr.WorkerId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(pr => pr.WorkOrder)
                    .WithMany(wo => wo.Productions)
                    .HasForeignKey(pr => pr.WorkOrderId)
                    .OnDelete(DeleteBehavior.Restrict);

                // Speeds up date-range reports (daily/weekly/monthly)
                entity.HasIndex(pr => pr.Date);
                entity.HasIndex(pr => new { pr.WorkerId, pr.Date });
            });

            // ---------- Machine ----------
            modelBuilder.Entity<Machine>(entity =>
            {
                entity.Property(m => m.Type)
                    .HasConversion<string>()
                    .HasMaxLength(20);

                entity.Property(m => m.Status)
                    .HasConversion<string>()
                    .HasMaxLength(20);
            });

            // ---------- MachineMaintenance ----------
            modelBuilder.Entity<MachineMaintenance>(entity =>
            {
                entity.HasOne(mm => mm.Machine)
                    .WithMany(m => m.MaintenanceRecords)
                    .HasForeignKey(mm => mm.MachineId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.Property(mm => mm.Amount)
                    .HasColumnType("decimal(10,2)");

                entity.HasIndex(mm => mm.Date);
                entity.HasIndex(mm => mm.MachineId);
            });
            // ---------- Expense ----------
            modelBuilder.Entity<Expense>(entity =>
            {
                entity.HasOne(e => e.ExpenseCategory)
                    .WithMany(ec => ec.Expenses)
                    .HasForeignKey(e => e.ExpenseCategoryId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.Property(e => e.Amount)
                    .HasColumnType("decimal(10,2)");

                entity.HasIndex(e => e.Date);
            });

            // ---------- ThreadStock ----------
            modelBuilder.Entity<ThreadStock>(entity =>
            {
                entity.Property(ts => ts.CurrentQuantity)
                    .HasColumnType("decimal(10,2)");
            });
            // ---------- Thread ----------
            modelBuilder.Entity<ThreadTransaction>(entity =>
            {
                entity.HasOne(tt => tt.ThreadStock)
                    .WithMany(ts => ts.Transactions)
                    .HasForeignKey(tt => tt.ThreadStockId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.Property(tt => tt.Type)
                    .HasConversion<string>()
                    .HasMaxLength(20);

                entity.Property(tt => tt.Quantity)
                    .HasColumnType("decimal(10,2)");

                entity.HasIndex(tt => tt.Date);
            });

        }
    }
}