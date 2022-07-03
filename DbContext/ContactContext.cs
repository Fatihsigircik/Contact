using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Entities.Abstract;
using Entities.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Configuration;
using msContext = Microsoft.EntityFrameworkCore;

namespace DbContext
{
    public class ContactContext : msContext.DbContext
    {

        public msContext.DbSet<Contact> Contacts { get; set; }
        public msContext.DbSet<ContactInfo> ContactInfos { get; set; }
        public msContext.DbSet<ReportStatus> ReportStatuses { get; set; }

        public ContactContext(msContext.DbContextOptions options) : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder builder)
        {
            var basePath = Directory.GetCurrentDirectory() + string.Format("{0}..{0}ContactApi", Path.DirectorySeparatorChar);
            IConfiguration configuration = new ConfigurationBuilder()
                .SetBasePath(basePath)
                .AddJsonFile("appsettings.json")
                .Build();
            builder.UseNpgsql(configuration.GetConnectionString("DefaultConnection"));
            //base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(msContext.ModelBuilder modelBuilder)
        {
            
            

            //contact
            modelBuilder.Entity<Contact>().HasKey(m => m.Id);
            modelBuilder.Entity<Contact>().Property(m => m.FirstName).HasMaxLength(30).IsRequired();
            modelBuilder.Entity<Contact>().Property(m => m.LastName).HasMaxLength(30).IsRequired();
            modelBuilder.Entity<Contact>().Property(m => m.CompanyName).HasMaxLength(150);
            modelBuilder.Entity<Contact>().Property(m => m.CreationDate).IsRequired();
            modelBuilder.Entity<Contact>().Property(m => m.ModifiedDate);
            //modelBuilder.Entity<Contact>().Ignore(m => m.PhoneInfos);
            //modelBuilder.Entity<Contact>().Ignore(m => m.EmailInfos);
            //modelBuilder.Entity<Contact>().Ignore(m => m.LocationInfos);



            //contactInfo
            modelBuilder.Entity<ContactInfo>().HasKey(m => m.Id);
            modelBuilder.Entity<ContactInfo>().Property(m => m.Content).HasMaxLength(300).IsRequired();
            modelBuilder.Entity<ContactInfo>().Property(m => m.InformationType).HasConversion<short>();
            modelBuilder.Entity<ContactInfo>().Property(m => m.CreationDate).IsRequired();
            modelBuilder.Entity<ContactInfo>().Property(m => m.ModifiedDate);
            modelBuilder.Entity<ContactInfo>().HasOne(m => m.Contact).WithMany(m => m.ContactInfos);


            //reportStatus
            modelBuilder.Entity<ReportStatus>().HasKey(m => m.Id);
            modelBuilder.Entity<ReportStatus>().Property(m => m.Status).IsRequired();
            modelBuilder.Entity<ReportStatus>().Property(m => m.CreationDate).IsRequired();
            modelBuilder.Entity<ReportStatus>().Property(m => m.ModifiedDate);
           
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            foreach (var entry in ChangeTracker.Entries<BaseEntity>())
            {
                switch (entry.State)
                {
                    case msContext.EntityState.Added:
                        entry.Entity.Id=Guid.NewGuid();
                        entry.Entity.CreationDate = DateTime.Now;
                        break;
                    case msContext.EntityState.Modified:
                        entry.Entity.ModifiedDate = DateTime.Now;
                        break;
                    default:
                        break;
                }
            }

            return await base.SaveChangesAsync(cancellationToken);
        }
        
    }
}
