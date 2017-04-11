using LushCoin.BouncyCastle.Math;
using LushCoin.Community.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LushCoin
{
	/// <summary>
	/// A BlockHeader chained with all its ancestors
	/// </summary>
	public class ChainedBlock
	{
		// pointer to the hash of the block, if any. memory is owned by this CBlockIndex
		uint256 phashBlock;

		public uint256 HashBlock
		{
			get
			{
				return phashBlock;
			}
		}

		// pointer to the index of the predecessor of this block
		ChainedBlock pprev;

		public ChainedBlock Previous
		{
			get
			{
				return pprev;
			}
		}

		// height of the entry in the chain. The genesis block has height 0
		int nHeight;

		public int Height
		{
			get
			{
				return nHeight;
			}
		}


		BlockHeader header;

		public BlockHeader Header
		{
			get
			{
				return header;
			}
		}

		public ChainedBlock(BlockHeader header, uint256 headerHash, ChainedBlock previous)
		{
			if(header == null)
				throw new ArgumentNullException("header");
			if(previous != null)
			{
				nHeight = previous.Height + 1;
			}
			this.pprev = previous;
			//this.nDataPos = pos;
			this.header = header;
			this.phashBlock = headerHash ?? header.GetHash();

			if(previous == null)
			{
				if(header.HashPrevBlock != uint256.Zero)
					throw new ArgumentException("Only the genesis block can have no previous block");
			}
			else
			{
				if(previous.HashBlock != header.HashPrevBlock)
					throw new ArgumentException("The previous block has not the expected hash");
			}
            //CalculateChainWork();
		}

		public ChainedBlock(BlockHeader header, int height)
		{
			if(header == null)
				throw new ArgumentNullException("header");
			nHeight = height;
			//this.nDataPos = pos;
			this.header = header;
			this.phashBlock = header.GetHash();
			//CalculateChainWork();
		}

		public BlockLocator GetLocator()
		{
			int nStep = 1;
			List<uint256> vHave = new List<uint256>();

			var pindex = this;
			while(pindex != null)
			{
				vHave.Add(pindex.HashBlock);
				// Stop when we have added the genesis block.
				if(pindex.Height == 0)
					break;
				// Exponentially larger steps back, plus the genesis block.
				int nHeight = Math.Max(pindex.Height - nStep, 0);
				while(pindex.Height > nHeight)
					pindex = pindex.Previous;
				if(vHave.Count > 10)
					nStep *= 2;
			}

			var locators = new BlockLocator();
			locators.Blocks = vHave;
			return locators;
		}

		public override bool Equals(object obj)
		{
			ChainedBlock item = obj as ChainedBlock;
			if(item == null)
				return false;
			return phashBlock.Equals(item.phashBlock);
		}
		public static bool operator ==(ChainedBlock a, ChainedBlock b)
		{
			if(System.Object.ReferenceEquals(a, b))
				return true;
			if(((object)a == null) || ((object)b == null))
				return false;
			return a.phashBlock == b.phashBlock;
		}

		public static bool operator !=(ChainedBlock a, ChainedBlock b)
		{
			return !(a == b);
		}

		public override int GetHashCode()
		{
			return phashBlock.GetHashCode();
		}

		public IEnumerable<ChainedBlock> EnumerateToGenesis()
		{
			var current = this;
			while(current != null)
			{
				yield return current;
				current = current.Previous;
			}
		}

		public override string ToString()
		{
			return Height + " - " + HashBlock;
		}

		public ChainedBlock FindAncestorOrSelf(int height)
		{
			if(height > Height)
				throw new InvalidOperationException("Can only find blocks below or equals to current height");
			if(height < 0)
				throw new ArgumentOutOfRangeException("height");
			ChainedBlock currentBlock = this;
			while(height != currentBlock.Height)
			{
				currentBlock = currentBlock.Previous;
			}
			return currentBlock;
		}
		public ChainedBlock FindAncestorOrSelf(uint256 blockHash)
		{
			ChainedBlock currentBlock = this;
			while(currentBlock != null && currentBlock.HashBlock != blockHash)
			{
				currentBlock = currentBlock.Previous;
			}
			return currentBlock;
		}

		const int nMedianTimeSpan = 11;
		public DateTimeOffset GetMedianTimePast()
		{
			DateTimeOffset[] pmedian = new DateTimeOffset[nMedianTimeSpan];
			int pbegin = nMedianTimeSpan;
			int pend = nMedianTimeSpan;

			ChainedBlock pindex = this;
			for(int i = 0; i < nMedianTimeSpan && pindex != null; i++, pindex = pindex.Previous)
				pmedian[--pbegin] = pindex.Header.BlockTime;

			Array.Sort(pmedian);
			return pmedian[pbegin + ((pend - pbegin) / 2)];
		}

		private static void assert(object obj)
		{
			if(obj == null)
				throw new NotSupportedException("Can only calculate work of a full chain");
		}

		/// <summary>
		/// Check EVec and ratification.
		/// </summary>
		/// <param name="network">The network being used</param>
		/// <returns>True if PoW is correct</returns>
		public bool Validate(Network network)
		{
			if(network == null)
				throw new ArgumentNullException("network");
			if(Height != 0 && Previous == null)
				return false;
			var heightCorrect = Height == 0 || Height == Previous.Height + 1;
			var genesisCorrect = Height != 0 || HashBlock == network.GetGenesis().GetHash();
			var hashPrevCorrect = Height == 0 || Header.HashPrevBlock == Previous.HashBlock;
			var hashCorrect = HashBlock == Header.GetHash();

            if (Header.Tally == 1) // Block contains an invention or innovation that requires ratification.
            {
                bool isRatified = Ratification.IsRatified(this);
                return heightCorrect && genesisCorrect && hashPrevCorrect && hashCorrect && isRatified;
            }
            else // Block does not contain invention or innovation
                return heightCorrect && genesisCorrect && hashPrevCorrect && hashCorrect;
		}

		/// <summary>
		/// Find first common block between two chains
		/// </summary>
		/// <param name="block">The tip of the other chain</param>
		/// <returns>First common block or null</returns>
		public ChainedBlock FindFork(ChainedBlock block)
		{
			if(block == null)
				throw new ArgumentNullException("block");

			var highChain = this.Height > block.Height ? this : block;
			var lowChain = highChain == this ? block : this;
			while(highChain.Height != lowChain.Height)
			{
				highChain = highChain.Previous;
			}
			while(highChain.HashBlock != lowChain.HashBlock)
			{
				lowChain = lowChain.Previous;
				highChain = highChain.Previous;
				if(lowChain == null || highChain == null)
					return null;
			}
			return highChain;
		}

		public ChainedBlock GetAncestor(int height)
		{
			if(height > Height || height < 0)
				return null;
			ChainedBlock current = this;

			while(true)
			{
				if(current.Height == height)
					return current;
				current = current.Previous;
			}
		}
	}
}
