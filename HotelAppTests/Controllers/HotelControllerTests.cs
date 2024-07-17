using HotelApp.DataAccess.Context;
using HotelAppAPI.Controllers;
using HotelAppDataAccess.Models;
using HotelAppLibrary;
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
    public class HotelsControllerTests
    {
        private readonly DbContextOptions<HotelContext> _dbContextOptions;

        public HotelsControllerTests()
        {
            _dbContextOptions = new DbContextOptionsBuilder<HotelContext>()
                .UseInMemoryDatabase(databaseName: "HotelAppTest")
                .Options;
        }

        private HotelsController CreateController(HotelContext context)
        {
            var mockLogger = new Mock<ILogger<HotelsController>>();
            return new HotelsController(context, mockLogger.Object);
        }

        private void ClearDatabase(HotelContext context)
        {
            context.Hotels.RemoveRange(context.Hotels);
            context.SaveChanges();
        }

        private void SeedData(HotelContext context)
        {
            var hotels = new List<HotelModel>
            {
                new HotelModel { Name = "Grand Plaza", Address = "123 Grand Ave" },
                new HotelModel { Name = "Ocean View", Address = "456 Ocean Drive" }
            };
            context.Hotels.AddRange(hotels);
            context.SaveChanges();
        }

        [Fact]
        public async Task GetHotels_ReturnsAllHotels()
        {
            using (var context = new HotelContext(_dbContextOptions, new Mock<IConfiguration>().Object))
            {
                ClearDatabase(context);
                SeedData(context);

                var controller = CreateController(context);

                var result = await controller.GetHotels();

                var okResult = Assert.IsType<OkObjectResult>(result.Result);
                var hotels = Assert.IsType<List<HotelModel>>(okResult.Value);

                Assert.Equal(2, hotels.Count);
            }
        }

        [Fact]
        public async Task GetHotel_ReturnsHotel()
        {
            int hotelId;

            using (var context = new HotelContext(_dbContextOptions, new Mock<IConfiguration>().Object))
            {
                ClearDatabase(context);
                SeedData(context);

                var hotel = context.Hotels.First();
                hotelId = hotel.HotelId;
            }

            using (var context = new HotelContext(_dbContextOptions, new Mock<IConfiguration>().Object))
            {
                var controller = CreateController(context);

                var result = await controller.GetHotel(hotelId);

                var okResult = Assert.IsType<OkObjectResult>(result.Result);
                var hotel = Assert.IsType<HotelModel>(okResult.Value);

                Assert.Equal(hotelId, hotel.HotelId);
            }
        }

        [Fact]
        public async Task PostHotel_CreatesHotel()
        {
            using (var context = new HotelContext(_dbContextOptions, new Mock<IConfiguration>().Object))
            {
                ClearDatabase(context);

                var controller = CreateController(context);

                var newHotel = new HotelModel { Name = "Sunset Resort", Address = "789 Sunset Blvd" };

                var result = await controller.PostHotel(newHotel);

                var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result.Result);
                var hotel = Assert.IsType<HotelModel>(createdAtActionResult.Value);

                Assert.Equal(newHotel.Name, hotel.Name);
                Assert.Equal(newHotel.Address, hotel.Address);
            }
        }

        [Fact]
        public async Task PutHotel_UpdatesHotel()
        {
            int hotelId;

            using (var context = new HotelContext(_dbContextOptions, new Mock<IConfiguration>().Object))
            {
                ClearDatabase(context);
                SeedData(context);

                var hotel = context.Hotels.First();
                hotelId = hotel.HotelId;
            }

            using (var context = new HotelContext(_dbContextOptions, new Mock<IConfiguration>().Object))
            {
                var controller = CreateController(context);

                var updatedHotel = new HotelModel { HotelId = hotelId, Name = "Updated Plaza", Address = "789 Updated Ave" };

                var result = await controller.PutHotel(hotelId, updatedHotel);

                Assert.IsType<NoContentResult>(result);

                var hotel = await context.Hotels.FindAsync(hotelId);
                Assert.Equal(updatedHotel.Name, hotel.Name);
                Assert.Equal(updatedHotel.Address, hotel.Address);
            }
        }

        [Fact]
        public async Task DeleteHotel_DeletesHotel()
        {
            int hotelId;

            using (var context = new HotelContext(_dbContextOptions, new Mock<IConfiguration>().Object))
            {
                ClearDatabase(context);
                SeedData(context);

                var hotel = context.Hotels.First();
                hotelId = hotel.HotelId;
            }

            using (var context = new HotelContext(_dbContextOptions, new Mock<IConfiguration>().Object))
            {
                var controller = CreateController(context);

                var result = await controller.DeleteHotel(hotelId);

                Assert.IsType<NoContentResult>(result);

                var hotel = await context.Hotels.FindAsync(hotelId);
                Assert.Null(hotel);
            }
        }
    }
}
