using Microsoft.EntityFrameworkCore;
using PoliceManagementSystem.Data;
using PoliceManagementSystem.Models;
using PoliceManagementSystem.Services;
using Xunit;

namespace PoliceManagementSystem.Tests
{
    /// <summary>Unit tests for AuditLoggingService (REQ-79, REQ-80).</summary>
    public class AuditLoggingServiceTests
    {
        private AppDbContext CreateInMemoryContext()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            return new AppDbContext(options);
        }

        /// <summary>REQ-79: LogAsync should create an audit log entry.</summary>
        [Fact]
        public async Task LogAsync_ValidData_CreatesAuditLog()
        {
            // Arrange
            var context = CreateInMemoryContext();
            var service = new AuditLoggingService(context);

            // Act
            await service.LogAsync(
                action: "LOGIN",
                entityType: "User",
                entityId: "1",
                userId: 1,
                success: true,
                ipAddress: "127.0.0.1"
            );

            // Assert
            Assert.Equal(1, context.AuditLogs.Count());
            var log = context.AuditLogs.First();
            Assert.Equal("LOGIN", log.Action);
            Assert.Equal("User", log.EntityType);
            Assert.True(log.Success);
        }

        /// <summary>REQ-79: LogAsync should record failed login attempts.</summary>
        [Fact]
        public async Task LogAsync_FailedLogin_RecordsFailure()
        {
            // Arrange
            var context = CreateInMemoryContext();
            var service = new AuditLoggingService(context);

            // Act
            await service.LogAsync(
                action: "LOGIN",
                entityType: "User",
                entityId: null,
                success: false,
                ipAddress: "192.168.1.1"
            );

            // Assert
            Assert.Equal(1, context.AuditLogs.Count());
            var log = context.AuditLogs.First();
            Assert.False(log.Success);
            Assert.Equal("192.168.1.1", log.IpAddress);
        }

        /// <summary>REQ-80: LogAsync should record file operations.</summary>
        [Fact]
        public async Task LogAsync_FileCreate_RecordsAction()
        {
            // Arrange
            var context = CreateInMemoryContext();
            var service = new AuditLoggingService(context);

            // Act
            await service.LogAsync(
                action: "FILE_CREATE",
                entityType: "CriminalFile",
                entityId: "5",
                userId: 2
            );

            // Assert
            Assert.Equal(1, context.AuditLogs.Count());
            var log = context.AuditLogs.First();
            Assert.Equal("FILE_CREATE", log.Action);
            Assert.Equal("CriminalFile", log.EntityType);
            Assert.Equal("5", log.EntityId);
            Assert.Equal(2, log.UserId);
        }

        /// <summary>REQ-80: LogAsync should record old and new values for updates.</summary>
        [Fact]
        public async Task LogAsync_WithOldAndNewValues_RecordsBothValues()
        {
            // Arrange
            var context = CreateInMemoryContext();
            var service = new AuditLoggingService(context);

            // Act
            await service.LogAsync(
                action: "FILE_UPDATE",
                entityType: "CriminalFile",
                entityId: "3",
                oldValue: "Status=Open",
                newValue: "Status=Closed"
            );

            // Assert
            var log = context.AuditLogs.First();
            Assert.Equal("Status=Open", log.OldValue);
            Assert.Equal("Status=Closed", log.NewValue);
        }

        /// <summary>REQ-79: Multiple log entries should all be saved.</summary>
        [Fact]
        public async Task LogAsync_MultipleEntries_AllSaved()
        {
            // Arrange
            var context = CreateInMemoryContext();
            var service = new AuditLoggingService(context);

            // Act
            await service.LogAsync(action: "LOGIN", entityType: "User", entityId: "1", success: true);
            await service.LogAsync(action: "FILE_CREATE", entityType: "CriminalFile", entityId: "1");
            await service.LogAsync(action: "FILE_DELETE", entityType: "CriminalFile", entityId: "1");

            // Assert
            Assert.Equal(3, context.AuditLogs.Count());
        }
    }
}