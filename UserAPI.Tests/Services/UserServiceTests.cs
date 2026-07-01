using Moq;
using UserAPI.Application.DTOs.Request;
using UserAPI.Application.Interfaces;
using UserAPI.Application.Services;
using UserAPI.Domain.Entities;
using UserAPI.Domain.Interfaces;

namespace UserAPI.Tests.Services
{
    public class UserServiceTests
    {
        private readonly UserService _userService;
        private readonly Mock<IValidationBehavior<UserRequestView>> _validationCreateMock;
        private readonly Mock<IValidationBehavior<UserRequestUpdateView>> _validationUpdateMock;
        private readonly Mock<IUserRepository> _userRepositoryMock;
        private readonly Mock<IAuthHelpers> _authHelpersMock;
        private readonly Mock<IEventPublisher> _eventPublisherMock;

        public UserServiceTests()
        {
            _validationCreateMock = new Mock<IValidationBehavior<UserRequestView>>();
            _validationUpdateMock = new Mock<IValidationBehavior<UserRequestUpdateView>>();
            _userRepositoryMock = new Mock<IUserRepository>();
            _authHelpersMock = new Mock<IAuthHelpers>();
            _eventPublisherMock = new Mock<IEventPublisher>();

            _userService = new UserService(
                _validationCreateMock.Object,
                _validationUpdateMock.Object,
                _userRepositoryMock.Object,
                _authHelpersMock.Object,
                _eventPublisherMock.Object
            );
        }

        [Fact]
        public async Task CreateUser_ShouldCreateUser_WhenValidRequest()
        {
            var request = new UserRequestView
            {
                Name = "André",
                LastName = "Costa",
                Email = "andre@test.com",
                Password = "Test@123456"
            };

            _authHelpersMock.Setup(a => a.ComputeSha256Hash(It.IsAny<string>()))
                            .Returns("hashedpassword123");

            var expectedUser = new User("André", "Costa", "andre@test.com", "hashedpassword123");

            _userRepositoryMock.Setup(r => r.Create(It.IsAny<User>()))
                               .ReturnsAsync(expectedUser);

            var result = await _userService.CreateUser(request);

            Assert.NotNull(result);
            Assert.Equal("André", result.Name);
            Assert.Equal("andre@test.com", result.Email);
        }

        [Fact]
        public async Task Login_ShouldReturnToken_WhenCredentialsAreValid()
        {
            var request = new LoginRequestView
            {
                Email = "admin@test.com",
                Password = "Admin@123"
            };

            var user = new User("Admin", "Test", "admin@test.com", "hashedpass", Domain.Enums.Role.Admin);

            _authHelpersMock.Setup(a => a.ComputeSha256Hash(It.IsAny<string>()))
                            .Returns("hashedpass");

            _userRepositoryMock.Setup(r => r.GetByEmailPass(It.IsAny<string>(), It.IsAny<string>()))
                               .ReturnsAsync(user);

            _authHelpersMock.Setup(a => a.GenerateJwtToken(It.IsAny<string>(), It.IsAny<string>()))
                            .Returns("fake.jwt.token");

            var result = await _userService.Login(request);

            Assert.NotNull(result);
            Assert.Equal("admin@test.com", result.Email);
            Assert.NotEmpty(result.Token);
        }

        [Fact]
        public async Task Login_ShouldReturnNull_WhenCredentialsAreInvalid()
        {
            var request = new LoginRequestView
            {
                Email = "invalid@test.com",
                Password = "WrongPassword"
            };

            _authHelpersMock.Setup(a => a.ComputeSha256Hash(It.IsAny<string>()))
                            .Returns("wronghashedpass");

            _userRepositoryMock.Setup(r => r.GetByEmailPass(It.IsAny<string>(), It.IsAny<string>()))
                               .ReturnsAsync((User?)null);

            var result = await _userService.Login(request);

            Assert.Null(result);
        }

        [Fact]
        public async Task GetUserById_ShouldReturnUser_WhenUserExists()
        {
            var userId = Guid.NewGuid();
            var expectedUser = new User("John", "Doe", "john@test.com", "hash");

            _userRepositoryMock.Setup(r => r.GetById(userId))
                               .ReturnsAsync(expectedUser);

            var result = await _userService.GetUserById(userId);

            Assert.NotNull(result);
            Assert.Equal("John", result.Name);
            Assert.Equal("john@test.com", result.Email);
        }

        [Fact]
        public async Task GetUserById_ShouldReturnNull_WhenUserDoesNotExist()
        {
            var userId = Guid.NewGuid();
            _userRepositoryMock.Setup(r => r.GetById(userId))
                               .ReturnsAsync((User?)null);

            var result = await _userService.GetUserById(userId);

            Assert.Null(result);
        }

        [Fact]
        public async Task GetAll_ShouldReturnListOfUsers_WhenUsersExist()
        {
            var users = new List<User>
            {
                new User("Alice", "Smith", "alice@test.com", "hash"),
                new User("Bob", "Jones", "bob@test.com", "hash")
            };

            _userRepositoryMock.Setup(r => r.GetAll())
                               .ReturnsAsync(users);

            var result = await _userService.GetAll();

            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
            Assert.Equal("Alice", result[0].Name);
            Assert.Equal("Bob", result[1].Name);
        }

        [Fact]
        public async Task GetAll_ShouldReturnNull_WhenNoUsersExist()
        {
            _userRepositoryMock.Setup(r => r.GetAll())
                               .ReturnsAsync((IList<User>?)null);

            var result = await _userService.GetAll();

            Assert.Null(result);
        }
    }
}
