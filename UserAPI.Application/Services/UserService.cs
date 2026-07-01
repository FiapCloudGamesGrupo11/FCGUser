using UserAPI.Application.DTOs.Request;
using UserAPI.Application.DTOs.Response;
using UserAPI.Application.Events;
using UserAPI.Application.Interfaces;
using UserAPI.Application.Results;
using UserAPI.Domain.Entities;
using UserAPI.Domain.Enums;
using UserAPI.Domain.Interfaces;

namespace UserAPI.Application.Services
{
    public class UserService : IUserService
    {
        private readonly IValidationBehavior<UserRequestView> _validationCreate;
        private readonly IValidationBehavior<UserRequestUpdateView> _validationUpdate;
        private readonly IUserRepository _userRepository;
        private readonly IAuthHelpers _authHelpers;
        private readonly IEventPublisher _eventPublisher;

        private const string UserCreatedQueue = "user-created";

        public UserService(
            IValidationBehavior<UserRequestView> validationCreate,
            IValidationBehavior<UserRequestUpdateView> validationUpdate,
            IUserRepository userRepository,
            IAuthHelpers authHelpers,
            IEventPublisher eventPublisher)
        {
            _validationCreate = validationCreate;
            _validationUpdate = validationUpdate;
            _userRepository   = userRepository;
            _authHelpers      = authHelpers;
            _eventPublisher   = eventPublisher;
        }

        public async Task<UserCreatedResponseView> CreateUser(UserRequestView request)
        {
            await _validationCreate.ValidateAsync(request);

            var userExist = await _userRepository.GetByEmail(request.Email);
            if (userExist != null) return null;

            request.Password = _authHelpers.ComputeSha256Hash(request.Password);

            var user = new User(request.Name, request.LastName, request.Email, request.Password);
            var response = await _userRepository.Create(user);

            await _eventPublisher.PublishAsync(
                new UserCreatedEvent(response.Id, response.Name, response.Email),
                UserCreatedQueue);

            return Result<UserCreatedResponseView>.Success(
                new UserCreatedResponseView(response.Id, response.Name, response.LastName, response.Email)).Value;
        }

        public async Task<UserCreatedResponseView> CreateAdmin(UserRequestView request)
        {
            await _validationCreate.ValidateAsync(request);

            request.Password = _authHelpers.ComputeSha256Hash(request.Password);

            var user = new User(request.Name, request.LastName, request.Email, request.Password, Role.Admin);
            var response = await _userRepository.Create(user);

            await _eventPublisher.PublishAsync(
                new UserCreatedEvent(response.Id, response.Name, response.Email),
                UserCreatedQueue);

            return Result<UserCreatedResponseView>.Success(
                new UserCreatedResponseView(response.Id, response.Name, response.LastName, response.Email)).Value;
        }

        public async Task<LoginResponseView> Login(LoginRequestView request)
        {
            var passHash = _authHelpers.ComputeSha256Hash(request.Password);
            var userResult = await _userRepository.GetByEmailPass(request.Email, passHash);

            if (userResult == null) return null;

            var token = _authHelpers.GenerateJwtToken(userResult.Email, userResult.Role.ToString());
            return new LoginResponseView(userResult.Email, token);
        }

        public async Task<UserCreatedResponseView> GetUserById(Guid id)
        {
            var response = await _userRepository.GetById(id);
            if (response == null) return null;

            return Result<UserCreatedResponseView>.Success(
                new UserCreatedResponseView(response.Id, response.Name, response.LastName, response.Email)).Value;
        }

        public async Task<IList<UserCreatedResponseView>> GetAll()
        {
            var response = await _userRepository.GetAll();
            if (response == null) return null;

            var userResponse = response
                .Select(r => new UserCreatedResponseView(r.Id, r.Name, r.LastName, r.Email))
                .ToList();

            return Result<IList<UserCreatedResponseView>>.Success(userResponse).Value;
        }

        public async Task<UserCreatedResponseView> UpdateRole(Guid id, Role requestRole)
        {
            var user = await _userRepository.GetById(id);
            if (user == null) return null;

            switch (requestRole)
            {
                case Role.Admin: user.ChangeRoleToAdmin(); break;
                default: user.ChangeRoleToCommon(); break;
            }

            var result = await _userRepository.Update(user);
            return new UserCreatedResponseView(result.Id, result.Name, result.LastName, result.Email);
        }

        public async Task<UserCreatedResponseView> UpdateStatus(Guid id, Status requestStatus)
        {
            var user = await _userRepository.GetById(id);
            if (user == null) return null;

            switch (requestStatus)
            {
                case Status.Active: user.ActiveUser(); break;
                case Status.Blocked: user.BlockUser(); break;
                case Status.Banned: user.BanUser(); break;
                default: user.BlockUser(); break;
            }

            var result = await _userRepository.Update(user);
            return new UserCreatedResponseView(result.Id, result.Name, result.LastName, result.Email);
        }

        public async Task<UserCreatedResponseView> UpdateUser(Guid id, UserRequestUpdateView userRequest)
        {
            await _validationUpdate.ValidateAsync(userRequest);

            var user = await _userRepository.GetById(id);
            if (user == null) return null;

            user.Name = userRequest.Name ?? user.Name;
            user.LastName = userRequest.LastName ?? user.LastName;

            var result = await _userRepository.Update(user);
            return new UserCreatedResponseView(result.Id, result.Name, result.LastName, result.Email);
        }
    }
}
