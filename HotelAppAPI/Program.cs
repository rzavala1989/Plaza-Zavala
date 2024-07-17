using HotelApp.DataAccess.Context;
using HotelAppDataAccess.Models;
using HotelAppLibrary;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HotelApp
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();

            // Seed the database
            SeedDatabase(host);

            host.Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });

        private static void SeedDatabase(IHost host)
        {
            using (var scope = host.Services.CreateScope())
            {
                var services = scope.ServiceProvider;

                try
                {
                    var context = services.GetRequiredService<HotelContext>();
                    context.Database.Migrate();

                    if (context.Hotels.Any())
                    {
                        // Database has been seeded
                        Console.WriteLine("Database has already been seeded...");
                    }
                    else
                    {
                        // Seed the database with sample data
                        AddSampleData(context);
                        Console.WriteLine("Database seeded with sample data!!!");
                    }
                    
                }
                catch (Exception ex)
                {
                    // Log any errors that occur during database seeding
                    var logger = services.GetRequiredService<ILogger<Program>>();
                    logger.LogError(ex, "An error occurred seeding the DB.");
                }
            }
        }

        private static void AddSampleData(HotelContext context)
        {
            // Add Hotels
            if (!context.Hotels.Any())
            {
                var hotels = new List<HotelModel>
                {
                    new HotelModel { Name = "Grand Plaza", Address = "123 Grand Ave" },
                    new HotelModel { Name = "Ocean View", Address = "456 Ocean Drive" }
                };
                context.Hotels.AddRange(hotels);
            }

            // Add Room Types
            if (!context.RoomTypes.Any())
            {
                var roomTypes = new List<RoomTypeModel>
                {
                    new RoomTypeModel { TypeName = "Standard", Description = "A standard room with basic amenities.", Price = 100 },
                    new RoomTypeModel { TypeName = "Deluxe", Description = "A deluxe room with ocean views and a minibar.", Price = 200 },
                    new RoomTypeModel { TypeName = "Suite", Description = "A spacious suite with a separate living area and premium amenities.", Price = 300 }
                };
                context.RoomTypes.AddRange(roomTypes);
            }

            context.SaveChanges();

            // Add Rooms
            if (!context.Rooms.Any())
            {
                var standardRoomTypeId = context.RoomTypes.First(rt => rt.TypeName == "Standard").RoomTypeId;
                var deluxeRoomTypeId = context.RoomTypes.First(rt => rt.TypeName == "Deluxe").RoomTypeId;
                
                var firstHotelId = context.Hotels.First().HotelId;
                var secondHotelId = context.Hotels.Skip(1).First().HotelId;

                var rooms = new List<RoomModel>
                {
                    new RoomModel { RoomNumber = "101", RoomTypeId = standardRoomTypeId, HotelId = firstHotelId },
                    new RoomModel { RoomNumber = "102", RoomTypeId = standardRoomTypeId, HotelId = firstHotelId },
                    new RoomModel { RoomNumber = "103", RoomTypeId = standardRoomTypeId, HotelId = firstHotelId },
                    new RoomModel { RoomNumber = "201", RoomTypeId = deluxeRoomTypeId, HotelId = firstHotelId },
                    new RoomModel { RoomNumber = "202", RoomTypeId = deluxeRoomTypeId, HotelId = firstHotelId },
                    new RoomModel { RoomNumber = "203", RoomTypeId = deluxeRoomTypeId, HotelId = secondHotelId},
                    new RoomModel { RoomNumber = "301", RoomTypeId = deluxeRoomTypeId, HotelId = secondHotelId},
                    new RoomModel { RoomNumber = "302", RoomTypeId = deluxeRoomTypeId, HotelId = secondHotelId},
                    new RoomModel { RoomNumber = "303", RoomTypeId = deluxeRoomTypeId, HotelId = secondHotelId},
                    new RoomModel { RoomNumber = "304", RoomTypeId = deluxeRoomTypeId, HotelId = secondHotelId},
                };
                context.Rooms.AddRange(rooms);
            }

            // Add Guests
            if (!context.Guests.Any())
            {
                var guests = new List<GuestModel>
                {
                    new GuestModel { FirstName = "Mickey", LastName = "Mouse", Phone = "123-456-7890", Email = "mickthetrick@gmail.com" },
                    new GuestModel { FirstName = "John", LastName = "Doe", Phone = "987-654-3210", Email = "john.doe@example.com" },
                    new GuestModel { FirstName = "Jane", LastName = "Doe", Phone = "555-555-5555", Email = "jane.doe@example.com" },
                    new GuestModel { FirstName = "Jim", LastName = "Beam", Phone = "666-666-6666", Email = "jim.beam@example.com" }
                };
                context.Guests.AddRange(guests);
            }

            context.SaveChanges();

            // Add Bookings
            if (!context.Bookings.Any())
            {
                var guestId = context.Guests.First(g => g.Email == "mickthetrick@gmail.com").GuestModelId;
                var roomStandardId = context.Rooms.First(r => r.RoomNumber == "101").RoomId;
                var roomDeluxeId = context.Rooms.First(r => r.RoomNumber == "201").RoomId;

                var bookings = new List<BookingModel>
                {
                    new BookingModel { GuestId = guestId, RoomId = roomStandardId, RoomTypeId = context.Rooms.First(r => r.RoomId == roomStandardId).RoomTypeId, StartDate = DateTime.UtcNow, EndDate = DateTime.UtcNow.AddDays(2) },
                    new BookingModel { GuestId = guestId, RoomId = roomDeluxeId, RoomTypeId = context.Rooms.First(r => r.RoomId == roomDeluxeId).RoomTypeId, StartDate = DateTime.UtcNow.AddDays(5), EndDate = DateTime.UtcNow.AddDays(7) },
                    new BookingModel { GuestId = guestId, RoomId = roomStandardId, RoomTypeId = context.Rooms.First(r => r.RoomId == roomStandardId).RoomTypeId, StartDate = DateTime.UtcNow.AddDays(10), EndDate = DateTime.UtcNow.AddDays(12) }
                };
                context.Bookings.AddRange(bookings);
            }

            context.SaveChanges();

            // Add Reviews
            if (!context.Reviews.Any())
            {
                var bookingId = context.Bookings.First().BookingId;

                var reviews = new List<ReviewModel>
                {
                    new ReviewModel { BookingId = bookingId, Rating = 5, Content = "Amazing stay!" }
                };
                context.Reviews.AddRange(reviews);
            }

            context.SaveChanges();
        }
    }
}
