using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backgammon.Services.Game.Domain.Models
{
    public class NumsToPlay
    {
        public NumToPlay[] NumbersToPlay { get; set; }
        public NumsToPlay(TwoNums nums)
        {
            if (nums.IsDouble())
            {
                NumbersToPlay = new[] { new NumToPlay(), new NumToPlay(), new NumToPlay(), new NumToPlay() };
                foreach (var num in NumbersToPlay)
                {
                    num.Number = nums.FirstCube;
                    num.IsPlayed = false;
                }
            }
            else
            {
                NumbersToPlay = new[] {new NumToPlay(), new NumToPlay() };
                NumbersToPlay[0].Number = nums.FirstCube;
                NumbersToPlay[1].Number = nums.SecondCube;
                NumbersToPlay[0].IsPlayed = false;
                NumbersToPlay[1].IsPlayed = false;
            }
        }
        public void UseNum(int Usednumber)
        {
            foreach (var num in NumbersToPlay)
            {
                if (num.Number == Usednumber && num.IsPlayed == false)
                {
                    num.IsPlayed = true;
                    return;
                }
            }
        }
        public bool HasMoreNumbers()
        {
            foreach (var number in NumbersToPlay)
            {
                if (number.IsPlayed == false)
                    return true;
            }
            return false;
        }
        public IEnumerable<int> GetAvalableNumbers()
        {
            return NumbersToPlay
                .Where(n => !n.IsPlayed)
                .Select(n => n.Number);
        }
        public bool IsNumAvalble(int number)
        {
            foreach (var num in NumbersToPlay)
            {
                if (num.Number == number && num.IsPlayed == false)
                    return true;
            }
            return false;
        }
    }

    public class NumToPlay
    {
        public int Number { get; set; }
        public bool IsPlayed { get; set; }
    }

}
