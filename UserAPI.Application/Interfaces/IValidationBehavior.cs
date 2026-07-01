namespace UserAPI.Application.Interfaces
{
    public interface IValidationBehavior<T>
    {
        Task ValidateAsync(T request);
    }
}
