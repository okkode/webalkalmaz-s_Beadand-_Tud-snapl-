using System;
using System.Threading.Tasks;
using GdeWebDB;
using GdeWebDB.Entities;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace GdeWeb.Tests.DbTests
{
    public class NoteDbTests
    {
        // InMemory DbContext létrehozása
        private GdeDbContext CreateContext()
        {
            var options = new DbContextOptionsBuilder<GdeDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()) // minden teszthez új DB
                .Options;

            return new GdeDbContext(options);
        }

        [Fact]
        public async Task Can_Create_Read_Update_Delete_Note()
        {
            // Arrange
            using var ctx = CreateContext();


            var note = new Note
            {
                USERID = 1, // tetszőleges user ID
                NOTETITLE = "First note",
                NOTECONTENT = "This is a test note.",
                NOTEDATE = DateTime.UtcNow,
                MODIFICATIONDATE = DateTime.UtcNow
            };


            ctx.Notes.Add(note);
            await ctx.SaveChangesAsync();

            // READ
            var loaded = await ctx.Notes.FirstOrDefaultAsync();
            Assert.NotNull(loaded);
            Assert.Equal("First note", loaded!.NOTETITLE);

            // UPDATE
            loaded.NOTETITLE = "Updated note";
            await ctx.SaveChangesAsync();

            var updated = await ctx.Notes.FirstOrDefaultAsync();
            Assert.NotNull(updated);
            Assert.Equal("Updated note", updated!.NOTETITLE);

            // DELETE
            ctx.Notes.Remove(updated);
            await ctx.SaveChangesAsync();

            var anyNotes = await ctx.Notes.AnyAsync();
            Assert.False(anyNotes);
        }
    }
}
