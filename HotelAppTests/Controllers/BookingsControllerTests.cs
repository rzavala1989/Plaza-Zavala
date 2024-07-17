using HotelApp.DataAccess.Context;
using HotelAppAPI.Controllers;
using HotelAppDataAccess.Models;
using HotelAppLibrary;
using HotelAppAPI.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace HotelAppTests.Controllers
{
    public class BookingsControllerTests
    {
        private readonly DbContextOptions<HotelContext> _dbContextOptions;

        public BookingsControllerTests()
        {
            _dbContextOptions = new DbContextOptionsBuilder<HotelContext>()
                .UseInMemoryDatabase(databaseName: "HotelAppTest")
                .Options;
        }

        private BookingsController CreateController(HotelContext context)
        {
            var mockBookingService = new Mock<IBookingService>();
            var mockEmailService = new Mock<IEmailService>();
            var mockLogger = new Mock<ILogger<BookingsController>>();
            return new BookingsController(mockBookingService.Object, mockEmailService.Object, mockLogger.Object, context);
        }

        private void ClearDatabase(HotelContext context)
        {
            context.Bookings.RemoveRange(context.Bookings);
            context.Rooms.RemoveRange(context.Rooms);
            context.RoomTypes.RemoveRange(context.RoomTypes);
            context.Guests.RemoveRange(context.Guests);
            context.Hotels.RemoveRange(context.Hotels);
            context.SaveChanges();
        }

        private void SeedData(HotelContext context)
        {
            var hotel = new HotelModel { Name = "Test Hotel", Address = "123 Test Ave" };
            context.Hotels.Add(hotel);

            var roomType = new RoomTypeModel { TypeName = "Standard", Description = "Standard Room", Price = 100 };
            context.RoomTypes.Add(roomType);

            var room = new RoomModel { RoomNumber = "101", RoomTypeId = roomType.RoomTypeId, HotelId = hotel.HotelId };
            context.Rooms.Add(room);

            var guest = new GuestModel { FirstName = "John", LastName = "Doe", Phone = "123-456-7890", Email = "john.doe@example.com" };
            context.Guests.Add(guest);

            context.SaveChanges();
        }

        [Fact]
        public async Task GetBookings_ReturnsAllBookings()
        {
            using (var context = new HotelContext(_dbContextOptions, new Mock<IConfiguration>().Object))
            {
                ClearDatabase(context);
                SeedData(context);

                var guestId = context.Guests.First().GuestModelId;
                var roomId = context.Rooms.First().RoomId;
                var roomTypeId = context.RoomTypes.First().RoomTypeId;

                context.Bookings.Add(new BookingModel
                {
                    GuestId = guestId,
                    RoomId = roomId,
                    RoomTypeId = roomTypeId,
                    StartDate = DateTime.UtcNow,
                    EndDate = DateTime.UtcNow.AddDays(2)
                });
                context.SaveChanges();
            }

            using (var context = new HotelContext(_dbContextOptions, new Mock<IConfiguration>().Object))
            {
                var controller = CreateController(context);

                var result = await controller.GetBookings();

                var okResult = Assert.IsType<OkObjectResult>(result.Result);
                var bookings = Assert.IsType<List<BookingModel>>(okResult.Value);

                Assert.Single(bookings);
            }
        }

        [Fact]
        public async Task GetBooking_ReturnsBooking()
        {
            int bookingId;

            using (var context = new HotelContext(_dbContextOptions, new Mock<IConfiguration>().Object))
            {
                ClearDatabase(context);
                SeedData(context);

                var guestId = context.Guests.First().GuestModelId;
                var roomId = context.Rooms.First().RoomId;
                var roomTypeId = context.RoomTypes.First().RoomTypeId;

                var booking = new BookingModel
                {
                    GuestId = guestId,
                    RoomId = roomId,
                    RoomTypeId = roomTypeId,
                    StartDate = DateTime.UtcNow,
                    EndDate = DateTime.UtcNow.AddDays(2)
                };
                context.Bookings.Add(booking);
                context.SaveChanges();
                bookingId = booking.BookingId;
            }

            using (var context = new HotelContext(_dbContextOptions, new Mock<IConfiguration>().Object))
            {
                var controller = CreateController(context);

                var result = await controller.GetBooking(bookingId);

                var okResult = Assert.IsType<OkObjectResult>(result.Result);
                var booking = Assert.IsType<BookingModel>(okResult.Value);

                Assert.Equal(bookingId, booking.BookingId);
            }
        }

        [Fact]
        public async Task PostBooking_CreatesNewBooking()
        {
            using (var context = new HotelContext(_dbContextOptions, new Mock<IConfiguration>().Object))
            {
                ClearDatabase(context);
                SeedData(context);

                var guestId = context.Guests.First().GuestModelId;
                var roomId = context.Rooms.First().RoomId;
                var roomTypeId = context.RoomTypes.First().RoomTypeId;

                var booking = new BookingModel
                {
                    GuestId = guestId,
                    RoomId = roomId,
                    RoomTypeId = roomTypeId,
                    StartDate = DateTime.UtcNow,
                    EndDate = DateTime.UtcNow.AddDays(2)
                };

                var mockBookingService = new Mock<IBookingService>();
                mockBookingService.Setup(s => s.IsRoomAvailable(It.IsAny<int>(), It.IsAny<DateTime>(), It.IsAny<DateTime>())).ReturnsAsync(true);
                mockBookingService.Setup(s => s.CreateBooking(It.IsAny<BookingModel>())).ReturnsAsync(booking);

                var mockEmailService = new Mock<IEmailService>();
                var mockLogger = new Mock<ILogger<BookingsController>>();

                var controller = new BookingsController(mockBookingService.Object, mockEmailService.Object, mockLogger.Object, context);
                var result = await controller.PostBooking(booking);

                var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result.Result);
                var createdBooking = Assert.IsType<BookingModel>(createdAtActionResult.Value);

                Assert.Equal(booking.GuestId, createdBooking.GuestId);
                Assert.Equal(booking.RoomId, createdBooking.RoomId);
                Assert.Equal(booking.RoomTypeId, createdBooking.RoomTypeId);
            }
        }

        [Fact]
        public async Task PostBooking_DoesNotAllowDoubleBooking()
        {
            using (var context = new HotelContext(_dbContextOptions, new Mock<IConfiguration>().Object))
            {
                ClearDatabase(context);
                SeedData(context);

                var guestId = context.Guests.First().GuestModelId;
                var roomId = context.Rooms.First().RoomId;
                var roomTypeId = context.RoomTypes.First().RoomTypeId;

                context.Bookings.Add(new BookingModel
                {
                    GuestId = guestId,
                    RoomId = roomId,
                    RoomTypeId = roomTypeId,
                    StartDate = DateTime.UtcNow,
                    EndDate = DateTime.UtcNow.AddDays(2)
                });
                context.SaveChanges();

                var overlappingBooking = new BookingModel
                {
                    GuestId = guestId,
                    RoomId = roomId,
                    RoomTypeId = roomTypeId,
                    StartDate = DateTime.UtcNow.AddDays(1),
                    EndDate = DateTime.UtcNow.AddDays(3)
                };

                var mockBookingService = new Mock<IBookingService>();
                mockBookingService.Setup(s => s.IsRoomAvailable(It.IsAny<int>(), It.IsAny<DateTime>(), It.IsAny<DateTime>())).ReturnsAsync(false);

                var mockEmailService = new Mock<IEmailService>();
                var mockLogger = new Mock<ILogger<BookingsController>>();

                var controller = new BookingsController(mockBookingService.Object, mockEmailService.Object, mockLogger.Object, context);
                var result = await controller.PostBooking(overlappingBooking);

                var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
                Assert.Equal("The room is not available for the selected dates or the guest has overlapping bookings.", badRequestResult.Value);
            }
        }

        [Fact]
        public async Task DeleteBooking_DeletesBooking()
        {
            int bookingId;

            using (var context = new HotelContext(_dbContextOptions, new Mock<IConfiguration>().Object))
            {
                ClearDatabase(context);
                SeedData(context);

                var guestId = context.Guests.First().GuestModelId;
                var roomId = context.Rooms.First().RoomId;
                var roomTypeId = context.RoomTypes.First().RoomTypeId;

                var booking = new BookingModel
                {
                    GuestId = guestId,
                    RoomId = roomId,
                    RoomTypeId = roomTypeId,
                    StartDate = DateTime.UtcNow,
                    EndDate = DateTime.UtcNow.AddDays(2)
                };
                context.Bookings.Add(booking);
                context.SaveChanges();
                bookingId = booking.BookingId;
            }

            using (var context = new HotelContext(_dbContextOptions, new Mock<IConfiguration>().Object))
            {
                var controller = CreateController(context);

                var result = await controller.DeleteBooking(bookingId);

                Assert.IsType<NoContentResult>(result);

                var booking = await context.Bookings.FindAsync(bookingId);
                Assert.Null(booking);
            }
        }
    }
}
