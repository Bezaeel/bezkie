using bezkie.application.Features.Catalogue.Commands;
using bezkie.application.Features.Profile.Command;
using bezkie.core.Entities;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bezkie.tests
{
    public class CatalogueTests
    {
        private readonly Mock<ILogger<CreateRSVPHandler>> _loggerMock = new();
        private CancellationTokenSource _cts = new CancellationTokenSource();

        [Fact]
        public async Task AddBookToCatalogueHandler_NewBook_ReturnsSuccess()
        {
            using var _dbContext = new SetUp().dbContext;
            new SetUp().Before();
            var book = new AddBookToCatalogueRequest
            {
                Name = "Test",
            };
            var _addToCatalogue = new CreateRSVPHandler(_dbContext, _loggerMock.Object);

            var expected = await _addToCatalogue.Handle(book, _cts.Token);
            Assert.True(expected.Status);
        }

        [Fact]
        public async Task AddBookToCatalogueHandler_ExisitingBook_ReturnsError()
        {
            using var _dbContext = new SetUp().dbContext;
            new SetUp().Before();
            new SetUp().SeedBook();
            var book = new AddBookToCatalogueRequest
            {
                Name = "Test",
            };
            var _addToCatalogue = new CreateRSVPHandler(_dbContext, _loggerMock.Object);

            var expected = await _addToCatalogue.Handle(book, _cts.Token);
            Assert.False(expected.Status);
        }
    }
}
