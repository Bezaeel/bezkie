using bezkie.application.Features.RSVPs.Commands;
using bezkie.infrastructure.Persistence;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bezkie.tests
{
    public class RSVPTests
    {
        private readonly Mock<ILogger<CreateRSVPHandler>> _loggerMock = new();
        private CancellationTokenSource _cts = new CancellationTokenSource();

        [Fact]
        public async Task AddBookToCatalogueHandler_NewBook_ReturnsSuccess()
        {
            using var _dbContext = new SetUp().dbContext;
            new SetUp().Before();
            var book = new CreateRSVPRequest
            {
                CustomerId = 1,
                BookId = 1,
                Status = core.Enums.RSVPStatus.RESERVED
            };
            var _addToCatalogue = new CreateRSVPHandler(_dbContext, _loggerMock.Object);

            var expected = await _addToCatalogue.Handle(book, _cts.Token);
            Assert.True(expected.Status);
        }

        [Fact]
        public async Task CreateRSVPHandler_ReservedBook_ReturnsError()
        {
            using var _dbContext = new SetUp().dbContext;
            new SetUp().Before();
            new SetUp().SeedRSVP();
            var book = new CreateRSVPRequest
            {
                CustomerId = 1,
                BookId = 1,
                Status = core.Enums.RSVPStatus.RESERVED
            };
            var _addToCatalogue = new CreateRSVPHandler(_dbContext, _loggerMock.Object);

            var expected = await _addToCatalogue.Handle(book, _cts.Token);
            Assert.False(expected.Status);
        }
    }
}
