using System;
using System.Threading.Tasks;
using GdeWebDB;
using GdeWebDB.Entities;
using GdeWebDB.Interfaces;
using GdeWebDB.Services;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;

namespace GdeWeb.Tests.ServiceTests
{
    public class SummarizationTests
    {
        private GdeDbContext CreateContext()
        {
            var options = new DbContextOptionsBuilder<GdeDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            return new GdeDbContext(options);
        }

        [Fact]
        public async Task GenerateMonthlySummary_UsesAiAndSavesToDb()
        {
            // Arrange
            using var ctx = CreateContext();

            int userId = 1;
            int year = 2025;
            int month = 11;



            ctx.Notes.Add(new Note
            {
                USERID = userId,
                NOTETITLE = "Jegyzet 1",
                NOTECONTENT = "Ez egy teszt jegyzet a hónapban.",
                NOTEDATE = new DateTime(year, month, 10),
                MODIFICATIONDATE = DateTime.UtcNow
            });

            await ctx.SaveChangesAsync();

            // Mockolt AI kliens – mindig ugyanazt adja vissza
            var aiMock = new Mock<IAiClient>();
            aiMock
                .Setup(x => x.SummarizeAsync(It.IsAny<string>()))
                .ReturnsAsync("FAKE SUMMARY FROM AI");

            // A NoteService most már GdeDbContext + IAiClient-et vár
            var service = new NoteService(ctx, aiMock.Object);

            // Act
            var result = await service.GenerateMonthlySummary(userId, year, month);

            // Assert – sikerült-e a hívás
            Assert.True(result.Success);

            // Megnézzük, hogy a DB-ben létrejött-e a MonthlySummary
            var summary = await ctx.MonthlySummaries.FirstOrDefaultAsync(ms =>
                ms.USERID == userId &&
                ms.YEAR == year &&
                ms.MONTH == month);

            Assert.NotNull(summary);
            Assert.Equal("FAKE SUMMARY FROM AI", summary!.SUMMARYTEXT);

            // Extra: ellenőrizzük, hogy az AI-t tényleg hívtuk
            aiMock.Verify(x => x.SummarizeAsync(It.IsAny<string>()), Times.Once);
        }
    }
}

