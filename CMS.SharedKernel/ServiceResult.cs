namespace CMS.SharedKernel
{
    public record ServiceResult(bool IsSuccess = true, string ErrorMessage = "");
    public record ServiceResult<T>(T Data = default, bool IsSuccess = true, string ErrorMessage = "") :
        ServiceResult(IsSuccess, ErrorMessage);
}
