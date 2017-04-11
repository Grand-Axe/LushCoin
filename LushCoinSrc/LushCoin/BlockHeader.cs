using LushCoin.BouncyCastle.Math;
using LushCoin.Crypto;
using LushCoin.DataEncoders;
using LushCoin.RPC;
#if !NOJSONNET
using Newtonsoft.Json.Linq;
#endif
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace LushCoin
{
    /// <summary>
    /// Nodes collect new transactions into a block, hash them into a hash tree,
    /// and scan through nonce values to make the block's hash satisfy proof-of-work
    /// requirements.  When they solve the proof-of-work, they broadcast the block
    /// to everyone and the block is added to the block chain.  The first transaction
    /// in the block is a special one that creates a new coin owned by the creator
    /// of the block.
    /// </summary>
    public class BlockHeader : IBitcoinSerializable
    {
        internal const int Size = 80;

        public static BlockHeader Parse(string hex)
        {
            return new BlockHeader(Encoders.Hex.DecodeData(hex));
        }

        public BlockHeader(string hex)
            : this(Encoders.Hex.DecodeData(hex))
        {

        }

        public BlockHeader(byte[] bytes)
        {
            this.ReadWrite(bytes);
        }


        // header
        const int CURRENT_VERSION = 3;

        uint256 hashPrevBlock;

        public uint256 HashPrevBlock
        {
            get
            {
                return hashPrevBlock;
            }
            set
            {
                hashPrevBlock = value;
            }
        }
        uint256 hashMerkleRoot;

        uint nTime;
        uint nBits;

        //public Target Bits
        //{
        //    get
        //    {
        //        return nBits;
        //    }
        //    set
        //    {
        //        nBits = value;
        //    }
        //}

        int nVersion;

        public int Version
        {
            get
            {
                return nVersion;
            }
            set
            {
                nVersion = value;
            }
        }

        uint nTally;

        /// <summary>
        /// Number of inventions or innovations in block. Only one or zero allowed.
        /// </summary>
        public uint Tally
        {
            get
            {
                return nTally;
            }
            set
            {
                nTally = value;
            }
        }

        public uint256 HashMerkleRoot
        {
            get
            {
                return hashMerkleRoot;
            }
            set
            {
                hashMerkleRoot = value;
            }
        }

        public BlockHeader()
        {
            SetNull();
        }


        internal void SetNull()
        {
            nVersion = CURRENT_VERSION;
            hashPrevBlock = 0;
            hashMerkleRoot = 0;
            nTime = 0;
            nBits = 0;
            nTally = 0;
        }

        public bool IsNull
        {
            get
            {
                return (nBits == 0);
            }
        }
        #region IBitcoinSerializable Members

        public void ReadWrite(BitcoinStream stream)
        {
            stream.ReadWrite(ref nVersion);
            stream.ReadWrite(ref hashPrevBlock);
            stream.ReadWrite(ref hashMerkleRoot);
            stream.ReadWrite(ref nTime);
            stream.ReadWrite(ref nBits);
            stream.ReadWrite(ref nTally);
        }

        #endregion

        public uint256 GetHash()
        {
            if (_Hashes != null && _Hashes[0] != null)
            {
                return _Hashes[0];
            }
            var h = Hashes.Hash256(this.ToBytes());
            if (_Hashes != null)
            {
                _Hashes[0] = h;
            }
            return h;
        }

        /// <summary>
        /// If called, GetHash becomes cached, only use if you believe the instance will not be modified after calculation. Calling it a second type invalidate the cache.
        /// </summary>
        public void CacheHashes()
        {
            _Hashes = new uint256[1];
        }

        uint256[] _Hashes;

        public DateTimeOffset BlockTime
        {
            get
            {
                return Utils.UnixTimeToDateTime(nTime);
            }
            set
            {
                this.nTime = Utils.DateTimeToUnixTime(value);
            }
        }

        public override string ToString()
        {
            return GetHash().ToString();
        }

        /// <summary>
        /// Set time to consensus acceptable value
        /// </summary>
        /// <param name="network">Network</param>
        /// <param name="prev">previous block</param>
        public void UpdateTime(Network network, ChainedBlock prev)
        {
            UpdateTime(DateTimeOffset.UtcNow, network, prev);
        }

        /// <summary>
        /// Set time to consensus acceptable value
        /// </summary>
        /// <param name="consensus">Consensus</param>
        /// <param name="prev">previous block</param>
        public void UpdateTime(Consensus consensus, ChainedBlock prev)
        {
            UpdateTime(DateTimeOffset.UtcNow, consensus, prev);
        }

        /// <summary>
        /// Set time to consensus acceptable value
        /// </summary>
        /// <param name="now">The expected date</param>
        /// <param name="consensus">Consensus</param>
        /// <param name="prev">previous block</param>		
        public void UpdateTime(DateTimeOffset now, Consensus consensus, ChainedBlock prev)
        {
            var nOldTime = this.BlockTime;
            var mtp = prev.GetMedianTimePast() + TimeSpan.FromSeconds(1);
            var nNewTime = mtp > now ? mtp : now;

            if (nOldTime < nNewTime)
                this.BlockTime = nNewTime;
        }

        /// <summary>
        /// Set time to consensus acceptable value
        /// </summary>
        /// <param name="now">The expected date</param>
        /// <param name="network">Network</param>
        /// <param name="prev">previous block</param>		
        public void UpdateTime(DateTimeOffset now, Network network, ChainedBlock prev)
        {
            UpdateTime(now, network.Consensus, prev);
        }
    }
}
