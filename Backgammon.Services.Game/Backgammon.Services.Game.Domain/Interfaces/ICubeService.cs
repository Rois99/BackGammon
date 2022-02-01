using System.Threading.Tasks;

namespace Backgammon.Services.Game.Domain.Interfaces
{
    public interface ICubeService
    {
        public Task<int> RollCube();
    }
}
