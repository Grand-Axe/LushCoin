using System.Threading.Tasks;

namespace LushCoin
{
	public interface IBlockRepository
	{
		Task<Block> GetBlockAsync(uint256 blockId);
	}
}
