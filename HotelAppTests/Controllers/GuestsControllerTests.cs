using HotelApp.DataAccess.Context;
using HotelAppAPI.Controllers;
using HotelAppDataAccess.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace HotelAppTests.Controllers
{
    public class GuestsControllerTests
    {
        private readonly DbContextOptions<HotelContext> _dbContextOptions;

        public GuestsControllerTests()
        {
            _dbContextOptions = new DbContextOptionsBuilder<HotelContext>()
                .UseInMemoryDatabase(databaseName: "HotelAppTest")
                .Options;
        }

        private GuestsController CreateController(HotelContext context)
        {
            var mockLogger = new Mock<ILogger<GuestsController>>();
            return new GuestsController(context, mockLogger.Object);
        }

        private void ClearDatabase(HotelContext context)
        {
            context.Guests.RemoveRange(context.Guests);
            context.SaveChanges();
        }

        private void SeedData(HotelContext context)
        {
            var guests = new List<GuestModel>
            {
                new GuestModel { FirstName = "Mickey", LastName = "Mouse", Phone = "123-456-7890", Email = "mickthetrick@gmail.com" },
                new GuestModel { FirstName = "John", LastName = "Doe", Phone = "987-654-3210", Email = "john.doe@example.com" },
                new GuestModel { FirstName = "Jane", LastName = "Doe", Phone = "555-555-5555", Email = "jane.doe@example.com" },
                new GuestModel { FirstName = "Jim", LastName = "Beam", Phone = "666-666-6666", Email = "jim.beam@example.com" }
            };
            context.Guests.AddRange(guests);
            context.SaveChanges();
        }

        [Fact]
        public async Task GetGuests_ReturnsAllGuests()
        {
            using (var context = new HotelContext(_dbContextOptions, new Mock<IConfiguration>().Object))
            {
                ClearDatabase(context);
                SeedData(context);

                var controller = CreateController(context);

                var result = await controller.GetGuests();

                var okResult = Assert.IsType<OkObjectResult>(result.Result);
                var guests = Assert.IsType<List<GuestModel>>(okResult.Value);

                Assert.Equal(4, guests.Count);
            }
        }

        [Fact]
        public async Task GetGuest_ReturnsGuest()
        {
            int guestId;

            using (var context = new HotelContext(_dbContextOptions, new Mock<IConfiguration>().Object))
            {
                ClearDatabase(context);
                SeedData(context);

                var guest = context.Guests.First();
                guestId = guest.GuestModelId;
            }

            using (var context = new HotelContext(_dbContextOptions, new Mock<IConfiguration>().Object))
            {
                var controller = CreateController(context);

                var result = await controller.GetGuest(guestId);

                var okResult = Assert.IsType<OkObjectResult>(result.Result);
                var guest = Assert.IsType<GuestModel>(okResult.Value);

                Assert.Equal(guestId, guest.GuestModelId);
            }
        }

        [Fact]
        public async Task PostGuest_CreatesGuest()
        {
            using (var context = new HotelContext(_dbContextOptions, new Mock<IConfiguration>().Object))
            {
                ClearDatabase(context);

                var controller = CreateController(context);

                var newGuest = new GuestModel { FirstName = "Donald", LastName = "Duck", Phone = "777-777-7777", Email = "donald.duck@example.com" };

                var result = await controller.PostGuest(newGuest);

                var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result.Result);
                var guest = Assert.IsType<GuestModel>(createdAtActionResult.Value);

                Assert.Equal(newGuest.FirstName, guest.FirstName);
                Assert.Equal(newGuest.LastName, guest.LastName);
                Assert.Equal(newGuest.Phone, guest.Phone);
                Assert.Equal(newGuest.Email, guest.Email);
            }
        }

        [Fact]
        public async Task PutGuest_UpdatesGuest()
        {
            int guestId;

            using (var context = new HotelContext(_dbContextOptions, new Mock<IConfiguration>().Object))
            {
                ClearDatabase(context);
                SeedData(context);

                var guest = context.Guests.First();
                guestId = guest.GuestModelId;
            }

            using (var context = new HotelContext(_dbContextOptions, new Mock<IConfiguration>().Object))
            {
                var controller = CreateController(context);

                var updatedGuest = new GuestModel
                {
                    GuestModelId = guestId,
                    FirstName = "Updated",
                    LastName = "Doe",
                    Phone = "123-456-7890",
                    Email = "updated.doe@example.com"
                };

                var result = await controller.PutGuest(guestId, updatedGuest);

                Assert.IsType<NoContentResult>(result);

                var guest = await context.Guests.FindAsync(guestId);
                Assert.Equal(updatedGuest.FirstName, guest.FirstName);
                Assert.Equal(updatedGuest.LastName, guest.LastName);
                Assert.Equal(updatedGuest.Phone, guest.Phone);
                Assert.Equal(updatedGuest.Email, guest.Email);
            }
        }

        [Fact]
        public async Task DeleteGuest_DeletesGuest()
        {
            int guestId;

            using (var context = new HotelContext(_dbContextOptions, new Mock<IConfiguration>().Object))
            {
                ClearDatabase(context);
                SeedData(context);

                var guest = context.Guests.First();
                guestId = guest.GuestModelId;
            }

            using (var context = new HotelContext(_dbContextOptions, new Mock<IConfiguration>().Object))
            {
                var controller = CreateController(context);

                var result = await controller.DeleteGuest(guestId);

                Assert.IsType<NoContentResult>(result);

                var guest = await context.Guests.FindAsync(guestId);
                Assert.Null(guest);
            }
        }
    }
}
