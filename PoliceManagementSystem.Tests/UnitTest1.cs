using Microsoft.EntityFrameworkCore;
using PoliceManagementSystem.Data;
using PoliceManagementSystem.DTOs.PoliceStations;
using PoliceManagementSystem.Models;
using PoliceManagementSystem.Services;
using Xunit;

namespace PoliceManagementSystem.Tests
{
    /// <summary>Unit tests for PoliceStationService (REQ-1 to REQ-8).</summary>
    public class PoliceStationServiceTests
    {
        private AppDbContext CreateInMemoryContext()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            return new AppDbContext(options);
        }

        /// <summary>REQ-1: Creating a station with valid data should succeed.</summary>
        [Fact]
        public async Task CreateAsync_ValidRequest_ReturnsStation()
        {
            // Arrange
            var context = CreateInMemoryContext();
            var service = new PoliceStationService(context);
            var request = new CreatePoliceStationRequest
            {
                Name = "Sectia 1 Craiova",
                Address = "Str. Unirii nr. 1",
                Latitude = 44.3302,
                Longitude = 23.7949
            };

            // Act
            var result = await service.CreateAsync(request);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Sectia 1 Craiova", result.Name);
            Assert.Equal(44.3302, result.Latitude);
        }

        /// <summary>REQ-7: Creating a station without a name should throw exception.</summary>
        [Fact]
        public async Task CreateAsync_EmptyName_ThrowsArgumentException()
        {
            // Arrange
            var context = CreateInMemoryContext();
            var service = new PoliceStationService(context);
            var request = new CreatePoliceStationRequest
            {
                Name = "",
                Latitude = 44.3302,
                Longitude = 23.7949
            };

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => service.CreateAsync(request));
        }

        /// <summary>REQ-9: GetAllAsync should return all stations.</summary>
        [Fact]
        public async Task GetAllAsync_ReturnsAllStations()
        {
            // Arrange
            var context = CreateInMemoryContext();
            context.PoliceStations.AddRange(
                new PoliceStation { Name = "Sectia 1", Address = "Addr 1", Latitude = 44.1, Longitude = 23.1 },
                new PoliceStation { Name = "Sectia 2", Address = "Addr 2", Latitude = 44.2, Longitude = 23.2 }
            );
            await context.SaveChangesAsync();
            var service = new PoliceStationService(context);

            // Act
            var result = await service.GetAllAsync();

            // Assert
            Assert.Equal(2, result.Count());
        }

        /// <summary>REQ-3: UpdateAsync should update station name.</summary>
        [Fact]
        public async Task UpdateAsync_ValidRequest_UpdatesStation()
        {
            // Arrange
            var context = CreateInMemoryContext();
            var station = new PoliceStation { Name = "Old Name", Address = "Addr", Latitude = 44.1, Longitude = 23.1 };
            context.PoliceStations.Add(station);
            await context.SaveChangesAsync();
            var service = new PoliceStationService(context);
            var request = new UpdatePoliceStationRequest
            {
                Name = "New Name",
                Address = "New Addr",
                Latitude = 44.2,
                Longitude = 23.2
            };

            // Act
            var result = await service.UpdateAsync(station.Id, request);

            // Assert
            Assert.True(result);
            var updated = await context.PoliceStations.FindAsync(station.Id);
            Assert.Equal("New Name", updated!.Name);
        }

        /// <summary>REQ-3: UpdateAsync with non-existent ID should return false.</summary>
        [Fact]
        public async Task UpdateAsync_NonExistentId_ReturnsFalse()
        {
            // Arrange
            var context = CreateInMemoryContext();
            var service = new PoliceStationService(context);

            // Act
            var result = await service.UpdateAsync(999, new UpdatePoliceStationRequest
            {
                Name = "Test",
                Latitude = 44.1,
                Longitude = 23.1
            });

            // Assert
            Assert.False(result);
        }

        /// <summary>REQ-3: DeleteAsync should remove station from database.</summary>
        [Fact]
        public async Task DeleteAsync_ExistingStation_ReturnsTrue()
        {
            // Arrange
            var context = CreateInMemoryContext();
            var station = new PoliceStation { Name = "Sectia Test", Address = "Addr", Latitude = 44.1, Longitude = 23.1 };
            context.PoliceStations.Add(station);
            await context.SaveChangesAsync();
            var service = new PoliceStationService(context);

            // Act
            var result = await service.DeleteAsync(station.Id);

            // Assert
            Assert.True(result);
            Assert.Equal(0, context.PoliceStations.Count());
        }

        /// <summary>GetByIdAsync with non-existent ID should return null.</summary>
        [Fact]
        public async Task GetByIdAsync_NonExistentId_ReturnsNull()
        {
            // Arrange
            var context = CreateInMemoryContext();
            var service = new PoliceStationService(context);

            // Act
            var result = await service.GetByIdAsync(999);

            // Assert
            Assert.Null(result);
        }
    }
}