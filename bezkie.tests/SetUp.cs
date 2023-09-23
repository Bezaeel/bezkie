using bezkie.core.Entities;
using bezkie.infrastructure.Persistence;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using System.Configuration;

namespace bezkie.tests
{
    public class SetUp : IDisposable
    {
        public readonly ApplicationDbContext dbContext;

        public SetUp()
        {
            var services = new ServiceCollection();
            services.AddEntityFrameworkInMemoryDatabase()
                .AddDbContext<ApplicationDbContext>(options =>
                {
                    options.UseInMemoryDatabase("bezloft");
                    options.ConfigureWarnings(x => x.Ignore(InMemoryEventId.TransactionIgnoredWarning));
                });
            var serviceProvider = services.BuildServiceProvider();
            dbContext = serviceProvider.GetRequiredService<ApplicationDbContext>();
            dbContext.Database.EnsureCreated();
        }

        public void SeedBook()
        {
            Book book = new Book
            {
                Name = "Test"
            };

            dbContext.Books.Add(book);
            dbContext.SaveChanges();
        }

        public void SeedCustomer()
        {
            User user = new User
            {
                Name = "talabi",
                Email = "talabi@mail.com",
                UserName = "talabi@mail.com"
            };

            dbContext.Users.Add(user);
            dbContext.SaveChanges();
        }

        public void SeedRSVP()
        {
            //Book book = new Book
            //{
            //    Name = "Test"
            //};

            //User user = new User
            //{
            //    Name = "talabi",
            //    Email = "talabi@mail.com",
            //    UserName = "talabi@mail.com"
            //};

            //dbContext.Books.Add(book);
            //dbContext.Users.Add(user);

            RSVP rsvp = new RSVP
            {
                BookId = 1,
                CustomerId = 1,
                Status = core.Enums.RSVPStatus.RESERVED
            };

            dbContext.RSVPs.Add(rsvp);
            dbContext.SaveChanges();
        }

        public void Before()
        {
            dbContext.Database.EnsureDeleted();
        }

        public void Dispose()
        {
            dbContext.Database.EnsureDeleted();
        }
    }
}