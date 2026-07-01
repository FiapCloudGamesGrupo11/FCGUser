namespace UserAPI.Application.Results
{
    public class Result<T>
    {
        public T Value { get; private set; }
        public bool IsSuccess { get; private set; }

        private Result(T value, bool isSuccess)
        {
            Value = value;
            IsSuccess = isSuccess;
        }

        public static Result<T> Success(T value) => new(value, true);
        public static Result<T> Failure() => new(default!, false);
    }
}
