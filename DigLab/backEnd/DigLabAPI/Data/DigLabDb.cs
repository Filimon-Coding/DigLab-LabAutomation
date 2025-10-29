using DigLabAPI.Models;
using Microsoft.EntityFrameworkCore;
using BCrypt.Net; 

namespace DigLabAPI.Data;

public class DigLabDb : DbContext
{
    public DigLabDb(DbContextOptions<DigLabDb> options) : base(options) { }

    public DbSet<Person> Persons => Set<Person>();
    public DbSet<Order>  Orders  => Set<Order>();
    public DbSet<OrderResult> OrderResults => Set<OrderResult>();

    public DbSet<User> Users => Set<User>(); 

    

    protected override void OnModelCreating(ModelBuilder b)
    {
        base.OnModelCreating(b);

        // --- Person config + seed ---
        b.Entity<Person>(e =>
        {
            e.Property(x => x.Personnummer).HasMaxLength(11);
            e.Property(x => x.FirstName).HasMaxLength(100);
            e.Property(x => x.MiddleName).HasMaxLength(100);
            e.Property(x => x.LastName).HasMaxLength(100);
            e.Property(x => x.AddressLine1).HasMaxLength(200);
            e.Property(x => x.AddressLine2).HasMaxLength(200);
            e.Property(x => x.PostalCode).HasMaxLength(4);
            e.Property(x => x.City).HasMaxLength(100);
            e.Property(x => x.Email).HasMaxLength(100);
            e.Property(x => x.Phone).HasMaxLength(20);
        });

        // (your existing Person seeds kept)
        b.Entity<Person>().HasData(
            new Person { Id = 1,  Personnummer = "01010112345", FirstName = "Ola",     LastName = "Nordmann",  AddressLine1 = "Storgata 1",   PostalCode = "0001", City = "Oslo",       Email = "ola@example.com",     Phone = "90000001", CreatedAtUtc = DateTime.UtcNow },
            new Person { Id = 2,  Personnummer = "02020223456", FirstName = "Kari",    LastName = "Nordmann",  AddressLine1 = "Parkveien 22", PostalCode = "5003", City = "Bergen",     Email = "kari@example.com",    Phone = "90000002", CreatedAtUtc = DateTime.UtcNow },
            new Person { Id = 3,  Personnummer = "03030334567", FirstName = "Per",     LastName = "Hansen",    AddressLine1 = "Markveien 12", PostalCode = "7010", City = "Trondheim",  Email = "per@example.com",     Phone = "90000003", CreatedAtUtc = DateTime.UtcNow },
            new Person { Id = 4,  Personnummer = "04040445678", FirstName = "Anne",    LastName = "Larsen",    AddressLine1 = "Solbergveien 7", PostalCode = "9008", City = "Tromsø",    Email = "anne@example.com",    Phone = "90000004", CreatedAtUtc = DateTime.UtcNow },
            new Person { Id = 5,  Personnummer = "05050556789", FirstName = "Marius",  LastName = "Bakke",     AddressLine1 = "Skogveien 14", PostalCode = "2815", City = "Gjøvik",     Email = "marius@example.com",  Phone = "90000005", CreatedAtUtc = DateTime.UtcNow },
            new Person { Id = 6,  Personnummer = "06060667890", FirstName = "Ingrid",  LastName = "Lie",       AddressLine1 = "Havnegata 3",  PostalCode = "1606", City = "Fredrikstad", Email = "ingrid@example.com", Phone = "90000006", CreatedAtUtc = DateTime.UtcNow },
            new Person { Id = 7,  Personnummer = "07070778901", FirstName = "Jonas",   LastName = "Moen",      AddressLine1 = "Kirkeveien 15", PostalCode = "2004", City = "Lillestrøm", Email = "jonas@example.com",  Phone = "90000007", CreatedAtUtc = DateTime.UtcNow },
            new Person { Id = 8,  Personnummer = "08080889012", FirstName = "Camilla", LastName = "Johansen",  AddressLine1 = "Elveveien 2",   PostalCode = "4005", City = "Stavanger",  Email = "camilla@example.com", Phone = "90000008", CreatedAtUtc = DateTime.UtcNow },
            new Person { Id = 9,  Personnummer = "09090990123", FirstName = "Andreas", LastName = "Solheim",   AddressLine1 = "Fjordgata 9",  PostalCode = "6411", City = "Molde",      Email = "andreas@example.com", Phone = "90000009", CreatedAtUtc = DateTime.UtcNow },
            new Person { Id = 10, Personnummer = "10101001234", FirstName = "Sofie",   LastName = "Berg",      AddressLine1 = "Torget 8",     PostalCode = "2317", City = "Hamar",      Email = "sofie@example.com",   Phone = "90000010", CreatedAtUtc = DateTime.UtcNow }
        );

        // --- Order / OrderResult config (+ a bit of seed you already had) ---
        b.Entity<Order>(e =>
        {
            e.Property(x => x.LabNumber).HasMaxLength(32);
            e.Property(x => x.Name).HasMaxLength(200);
            e.Property(x => x.Personnummer).HasMaxLength(11);
        });

        b.Entity<Order>()
            .HasMany(o => o.Results)
            .WithOne(r => r.Order)
            .HasForeignKey(r => r.OrderId)
            .OnDelete(DeleteBehavior.Cascade);

        // existing Order seed preserved
        b.Entity<Order>().HasData(
            new Order { Id = 1, LabNumber = "LAB-20250909-AAA11111", Name = "Ola Nordmann", Personnummer = "01010112345", Date = new DateOnly(2025, 09, 09), Time = new TimeOnly(9, 15), DiagnosesJson = "[\"Dengue\"]", CreatedAtUtc = DateTime.UtcNow },
            new Order { Id = 2, LabNumber = "LAB-20250909-BBB22222", Name = "Kari Nordmann", Personnummer = "02020223456", Date = new DateOnly(2025, 09, 09), Time = new TimeOnly(9, 30), DiagnosesJson = "[\"Malaria\"]", CreatedAtUtc = DateTime.UtcNow },
            new Order { Id = 3, LabNumber = "LAB-20250909-CCC33333", Name = "Per Hansen", Personnummer = "03030334567", Date = new DateOnly(2025, 09, 09), Time = new TimeOnly(10, 00), DiagnosesJson = "[\"TBE\",\"Dengue\"]", CreatedAtUtc = DateTime.UtcNow },
            new Order { Id = 4, LabNumber = "LAB-20250909-DDD44444", Name = "Anne Larsen", Personnummer = "04040445678", Date = new DateOnly(2025, 09, 09), Time = new TimeOnly(10, 30), DiagnosesJson = "[\"Hantavirus – Puumalavirus (PuV)\"]", CreatedAtUtc = DateTime.UtcNow },
            new Order { Id = 5, LabNumber = "LAB-20250909-EEE55555", Name = "Marius Bakke", Personnummer = "05050556789", Date = new DateOnly(2025, 09, 09), Time = new TimeOnly(11, 00), DiagnosesJson = "[\"Dengue\"]", CreatedAtUtc = DateTime.UtcNow },
            new Order { Id = 6, LabNumber = "LAB-20250909-FFF66666", Name = "Ingrid Lie", Personnummer = "06060667890", Date = new DateOnly(2025, 09, 09), Time = new TimeOnly(11, 15), DiagnosesJson = "[\"Malaria\",\"TBE\"]", CreatedAtUtc = DateTime.UtcNow },
            new Order { Id = 7, LabNumber = "LAB-20250909-GGG77777", Name = "Jonas Moen", Personnummer = "07070778901", Date = new DateOnly(2025, 09, 09), Time = new TimeOnly(11, 45), DiagnosesJson = "[\"TBE\"]", CreatedAtUtc = DateTime.UtcNow },
            new Order { Id = 8, LabNumber = "LAB-20250909-HHH88888", Name = "Camilla Johansen", Personnummer = "08080889012", Date = new DateOnly(2025, 09, 09), Time = new TimeOnly(12, 00), DiagnosesJson = "[\"Dengue\",\"Malaria\"]", CreatedAtUtc = DateTime.UtcNow },
            new Order { Id = 9, LabNumber = "LAB-20250909-III99999", Name = "Andreas Solheim", Personnummer = "09090990123", Date = new DateOnly(2025, 09, 09), Time = new TimeOnly(12, 30), DiagnosesJson = "[\"Hantavirus – Puumalavirus (PuV)\"]", CreatedAtUtc = DateTime.UtcNow },
            new Order { Id = 10, LabNumber = "LAB-20250909-JJJ00000", Name = "Sofie Berg", Personnummer = "10101001234", Date = new DateOnly(2025, 09, 09), Time = new TimeOnly(13, 00), DiagnosesJson = "[\"Malaria\"]", CreatedAtUtc = DateTime.UtcNow });

        b.Entity<User>(e =>
            {
                e.HasIndex(x => x.Username).IsUnique();
                e.Property(x => x.Username).HasMaxLength(50);
                e.Property(x => x.Role).HasMaxLength(20);
            });

        b.Entity<User>().HasData(new User
        {
            Id = 1,
            Username = "admin",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("admin123"),
            Role = "admin",
            CreatedAtUtc = DateTime.UtcNow
        });
        
    }
}
