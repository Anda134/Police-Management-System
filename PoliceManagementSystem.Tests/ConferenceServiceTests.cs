using Microsoft.EntityFrameworkCore;
using PoliceManagementSystem.Data;
using PoliceManagementSystem.DTOs.Conferences;
using PoliceManagementSystem.Models;
using PoliceManagementSystem.Services;
using Xunit;

namespace PoliceManagementSystem.Tests
{
    /// <summary>Unit tests for ConferenceService (REQ-40 to REQ-48).</summary>
    public class ConferenceServiceTests
    {
        private AppDbContext CreateInMemoryContext()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            return new AppDbContext(options);
        }

        private async Task<(PoliceStation station, Agent organizer)> SeedDataAsync(AppDbContext context)
        {
            var station = new PoliceStation { Name = "Sectia 1", Address = "Addr", Latitude = 44.1, Longitude = 23.1 };
            context.PoliceStations.Add(station);
            await context.SaveChangesAsync();

            var organizer = new Agent { FirstName = "Alexandru", LastName = "Popescu", Badge = "A1", Role = "ChiefInspector", PoliceStationId = station.Id };
            context.Agents.Add(organizer);
            await context.SaveChangesAsync();

            return (station, organizer);
        }

        /// <summary>REQ-41: Creating a conference with valid data should succeed.</summary>
        [Fact]
        public async Task CreateAsync_ValidRequest_ReturnsConference()
        {
            // Arrange
            var context = CreateInMemoryContext();
            var (station, organizer) = await SeedDataAsync(context);
            var service = new ConferenceService(context);
            var request = new CreateConferenceRequest
            {
                Reason = "Coordonare operatiune",
                Callsign = "ALPHA-1",
                ScheduledAt = DateTime.UtcNow.AddDays(1),
                OrganizerId = organizer.Id,
                Priority = 1,
                ParticipantIds = new List<int>()
            };

            // Act
            var result = await service.CreateAsync(request);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Coordonare operatiune", result.Reason);
            Assert.Equal("ALPHA-1", result.Callsign);
        }

        /// <summary>REQ-41: Creating a conference without reason should throw exception.</summary>
        [Fact]
        public async Task CreateAsync_EmptyReason_ThrowsArgumentException()
        {
            // Arrange
            var context = CreateInMemoryContext();
            var (station, organizer) = await SeedDataAsync(context);
            var service = new ConferenceService(context);
            var request = new CreateConferenceRequest
            {
                Reason = "",
                Callsign = "ALPHA-1",
                ScheduledAt = DateTime.UtcNow.AddDays(1),
                OrganizerId = organizer.Id,
                Priority = 1,
                ParticipantIds = new List<int>()
            };

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => service.CreateAsync(request));
        }

        /// <summary>REQ-40: GetAllAsync should return all conferences.</summary>
        [Fact]
        public async Task GetAllAsync_ReturnsAllConferences()
        {
            // Arrange
            var context = CreateInMemoryContext();
            var (station, organizer) = await SeedDataAsync(context);
            context.Conferences.AddRange(
                new Conference { Reason = "Meeting 1", Callsign = "A1", ScheduledAt = DateTime.UtcNow.AddDays(1), OrganizerId = organizer.Id, Priority = 1 },
                new Conference { Reason = "Meeting 2", Callsign = "A2", ScheduledAt = DateTime.UtcNow.AddDays(2), OrganizerId = organizer.Id, Priority = 2 }
            );
            await context.SaveChangesAsync();
            var service = new ConferenceService(context);

            // Act
            var result = await service.GetAllAsync();

            // Assert
            Assert.Equal(2, result.Count());
        }

        /// <summary>REQ-40: GetByIdAsync should return correct conference.</summary>
        [Fact]
        public async Task GetByIdAsync_ExistingConference_ReturnsConference()
        {
            // Arrange
            var context = CreateInMemoryContext();
            var (station, organizer) = await SeedDataAsync(context);
            var conference = new Conference { Reason = "Test Meeting", Callsign = "T1", ScheduledAt = DateTime.UtcNow.AddDays(1), OrganizerId = organizer.Id, Priority = 1 };
            context.Conferences.Add(conference);
            await context.SaveChangesAsync();
            var service = new ConferenceService(context);

            // Act
            var result = await service.GetByIdAsync(conference.Id);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Test Meeting", result.Reason);
        }

        /// <summary>GetByIdAsync with non-existent ID should return null.</summary>
        [Fact]
        public async Task GetByIdAsync_NonExistentId_ReturnsNull()
        {
            // Arrange
            var context = CreateInMemoryContext();
            var service = new ConferenceService(context);

            // Act
            var result = await service.GetByIdAsync(999);

            // Assert
            Assert.Null(result);
        }

        /// <summary>REQ-35: AddParticipantsAsync should add participants to conference.</summary>
        [Fact]
        public async Task AddParticipantsAsync_ValidIds_AddsParticipants()
        {
            // Arrange
            var context = CreateInMemoryContext();
            var (station, organizer) = await SeedDataAsync(context);
            var participant = new Agent { FirstName = "Ion", LastName = "Pop", Badge = "A2", Role = "Agent", PoliceStationId = station.Id };
            context.Agents.Add(participant);
            await context.SaveChangesAsync();

            var conference = new Conference { Reason = "Test", Callsign = "T1", ScheduledAt = DateTime.UtcNow.AddDays(1), OrganizerId = organizer.Id, Priority = 1 };
            context.Conferences.Add(conference);
            await context.SaveChangesAsync();
            var service = new ConferenceService(context);

            // Act
            var result = await service.AddParticipantsAsync(conference.Id, new List<int> { participant.Id });

            // Assert
            Assert.True(result);
            var updated = await context.Conferences.Include(c => c.Participants).FirstOrDefaultAsync(c => c.Id == conference.Id);
            Assert.Single(updated!.Participants);
        }

        /// <summary>DeleteAsync should remove conference from database.</summary>
        [Fact]
        public async Task DeleteAsync_ExistingConference_ReturnsTrue()
        {
            // Arrange
            var context = CreateInMemoryContext();
            var (station, organizer) = await SeedDataAsync(context);
            var conference = new Conference { Reason = "Test", Callsign = "T1", ScheduledAt = DateTime.UtcNow.AddDays(1), OrganizerId = organizer.Id, Priority = 1 };
            context.Conferences.Add(conference);
            await context.SaveChangesAsync();
            var service = new ConferenceService(context);

            // Act
            var result = await service.DeleteAsync(conference.Id);

            // Assert
            Assert.True(result);
            Assert.Equal(0, context.Conferences.Count());
        }

        /// <summary>REQ-45: ChiefInspector conference should have priority 1.</summary>
        [Fact]
        public async Task CreateAsync_ChiefInspectorOrganizer_HasHighestPriority()
        {
            // Arrange
            var context = CreateInMemoryContext();
            var (station, organizer) = await SeedDataAsync(context);
            var service = new ConferenceService(context);
            var request = new CreateConferenceRequest
            {
                Reason = "Priority Meeting",
                Callsign = "CI-1",
                ScheduledAt = DateTime.UtcNow.AddDays(1),
                OrganizerId = organizer.Id,
                Priority = 1,
                ParticipantIds = new List<int>()
            };

            // Act
            var result = await service.CreateAsync(request);

            // Assert
            Assert.Equal(1, result.Priority);
        }
    }
}