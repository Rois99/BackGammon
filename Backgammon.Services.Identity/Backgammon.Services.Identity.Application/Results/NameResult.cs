namespace Backgammon.Services.Identity.Application.Results
{
    public class NameResult : Result
    {
        public NameResult() : base(true)
        {
        }

        public string Userame { get; set; }
    }
}
