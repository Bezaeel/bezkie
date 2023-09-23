using bezkie.application.Features.Profile.Command;
using bezkie.core.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Moq;

namespace bezkie.tests
{
    public class AccountTests
    {
        private readonly Mock<ILogger<CreateProfileHandler>> _createProfileLoggerMock = new();
        private CancellationTokenSource _cts = new CancellationTokenSource();
        public AccountTests()
        {
        }

        [Fact]
        public async Task CreateProfile_NewUser_ReturnsSuccess()
        {
            using var _dbContext = new SetUp().dbContext;
            new SetUp().Before();
            var user = new CreateProfileRequest
            {
                Email = "talabi@mail.com",
                Password = "password",
            };

            var validator = new UserValidator<User>();
            var passwordValidator = new PasswordValidator<User>();

            var userStoreMock = new Mock<IUserStore<User>>();

            var mgr = new Mock<UserManager<User>>(userStoreMock.Object, null, null, null, null, null, null, null, null);

            mgr.Object.UserValidators.Add(validator);
            mgr.Object.PasswordValidators.Add(passwordValidator);
            mgr.Object.PasswordHasher = new PasswordHasher<User>();

            mgr.Setup(x => x.CreateAsync(It.IsAny<User>(), It.IsAny<string>())).ReturnsAsync(IdentityResult.Success);

            var createProfileHandler = new CreateProfileHandler(
                mgr.Object, _createProfileLoggerMock.Object);


            var expected = await createProfileHandler.Handle(user, _cts.Token);
            Assert.True(expected.Status);
        }
    }
}
