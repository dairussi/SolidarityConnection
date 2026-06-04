namespace SolidarityConnection.Application.Common
{
    public class ResultData<T>
    {
        public bool IsSuccess { get; private set; }
        public string? ErrorMessage { get; private set; }
        public T? Data { get; private set; }

        private ResultData() { }

        public static ResultData<T> Success(T data)
            => new() { IsSuccess = true, Data = data };

        public static ResultData<T> Error(string message)
            => new() { IsSuccess = false, ErrorMessage = message };
    }
}
