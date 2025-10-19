namespace RetailShop.Dtos;

public class ResultService<T>
{
    public bool IsSuccess { get; set; }
    public string? Message { get; set; }
    public T? Data { get; set; }
    
    // 🟢 Trả về khi thành công
        public static ResultService<T> Success(T data, string? message = null)
        {
            return new ResultService<T>
            {
                IsSuccess = true,
                Message = message,
                Data = data
            };
        }

        // 🔴 Trả về khi thất bại
        public static ResultService<T> Fail(string message)
        {
            return new ResultService<T>
            {
                IsSuccess = false,
                Message = message,
                Data = default
            };
        }
}
