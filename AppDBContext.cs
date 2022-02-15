using Microsoft.EntityFrameworkCore;
using TodoApi.Models;

namespace TodoApi {
    public class AppDBContext : DbContext {
        public AppDBContext(DbContextOptions<AppDBContext> options)
            : base (options) 
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //Configure domain classes using modelBuilder here   
            modelBuilder.Entity<NilaiModel>()
                .HasKey(m => new {m.nrp, m.matkul});
        }

        public DbSet<MhsModel> mhs {get; set;} = null!;
        public DbSet<NilaiModel> nilai {get; set;} = null!;
    }
}