namespace UserAPI.Application.Events;

public record UserCreatedEvent(Guid UserId, string Name, string Email);
