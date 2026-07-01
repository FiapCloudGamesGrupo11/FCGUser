namespace UserAPI.Application.Events;

public record UserCreatedEvent(Guid Id, string Nome, string Email);
