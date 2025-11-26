using System;
using System.Linq;
using System.Net.Http.Json;
using System.Threading.Tasks;
using GdeWebDB;
using GdeWebModels;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Xunit;

// Ezzel egyértelműen a GdeWebAPI.Program típusra hivatkozunk
using Program = GdeWebAPI.Program;


namespace GdeWeb.Tests.ApiTests
{
    public class NotesApiTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly WebApplicationFactory<Program> _factory;

        public NotesApiTests(WebApplicationFactory<Program> factory)
        {
            _factory = factory.WithWebHostBuilder(builder =>
            {
                // Teszteléshez használjuk a Development környezetet
                builder.UseSetting("Environment", "Development");

                builder.ConfigureServices(services =>
                {
                    // Itt csak annyit csinálunk, hogy a DbContext-hez tartozó adatbázist
                    // létrehozzuk, ha még nem létezik (NOTE tábla stb.)
                    var sp = services.BuildServiceProvider();
                    using (var scope = sp.CreateScope())
                    {
                        var db = scope.ServiceProvider.GetRequiredService<GdeDbContext>();
                        db.Database.EnsureCreated();
                    }
                });
            });
        }



        [Fact]
        public async Task SaveNote_Then_GetNotes_ReturnsSavedNote()
        {
            // Arrange – HTTP kliens a teszt API-hoz
            var client = _factory.CreateClient();

            // ⚠️ AccessToken header – IDE VALAMI OLYAN TOKEN KELL,
            // AMIT A TI AccessTokenFilter-TEK ELFOGAD.
            // Ha nagyon szigorú, később külön tudjuk test-re lazítani.
            client.DefaultRequestHeaders.Add("AccessToken", "TEST_TOKEN");

            var note = new NoteModel
            {
                Title = "Integration test note",
                Content = "Content from integration test.",
                NoteDate = DateTime.UtcNow
            };

            var saveResponse = await client.PostAsJsonAsync("/api/Notes/SaveNote", note);

            // Ha nem sikerült, olvassuk ki a body-t és dobjunk beszédes hibát
            if (!saveResponse.IsSuccessStatusCode)
            {
                var body = await saveResponse.Content.ReadAsStringAsync();
                throw new Exception($"SaveNote failed: {(int)saveResponse.StatusCode} {saveResponse.StatusCode}\nBODY:\n{body}");
            }


            // Act 2 – jegyzetek lekérése
            var getResponse = await client.PostAsync("/api/Notes/GetNotes", content: null);
            getResponse.EnsureSuccessStatusCode();

            var list = await getResponse.Content.ReadFromJsonAsync<NoteListModel>();

            // Assert
            Assert.NotNull(list);
            Assert.Contains(list!.Notes, n => n.Title == "Integration test note");
        }
    }
}

