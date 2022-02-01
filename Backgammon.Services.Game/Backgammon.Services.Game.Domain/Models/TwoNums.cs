namespace Backgammon.Services.Game.Domain.Models
{
    public class TwoNums
    {
        public int FirstCube { get; set; }
        public int SecondCube { get; set; }

        public bool IsDouble()
        {
            if (FirstCube == SecondCube)
                return true;
            return false;
        }
    }
}
