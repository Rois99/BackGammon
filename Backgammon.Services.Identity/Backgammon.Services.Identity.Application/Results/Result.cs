namespace Backgammon.Services.Identity.Application.Results
{
    public class Result
    {
        public bool IsSuccess { get; set; }

        public Result(bool isSuccess)
        {
            IsSuccess = isSuccess;
        }
    }
}
