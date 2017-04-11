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
	public class Block : IBitcoinSerializable
	{
		//FIXME: it needs to be changed when Gavin Andresen increase the max block size. 
		public const uint MAX_BLOCK_SIZE = 1000 * 1000;

		BlockHeader header = new BlockHeader();
		// network and disk
		List<Transaction> vtx = new List<Transaction>();

		public List<Transaction> Transactions
		{
			get
			{
				return vtx;
			}
			set
			{
				vtx = value;
			}
		}

		public MerkleNode GetMerkleRoot()
		{
			return MerkleNode.GetRoot(Transactions.Select(t => t.GetHash()));
		}


		public Block()
		{
			SetNull();
		}

		public Block(BlockHeader blockHeader)
		{
			SetNull();
			header = blockHeader;
		}
		public Block(byte[] bytes)
		{
			this.ReadWrite(bytes);
		}


		public void ReadWrite(BitcoinStream stream)
		{
			stream.ReadWrite(ref header);
			stream.ReadWrite(ref vtx);
		}

		public bool HeaderOnly
		{
			get
			{
				return vtx == null || vtx.Count == 0;
			}
		}


		void SetNull()
		{
			header.SetNull();
			vtx.Clear();
		}

		public BlockHeader Header
		{
			get
			{
				return header;
			}
		}
		public uint256 GetHash()
		{
			//Block's hash is his header's hash
			return header.GetHash();
		}

		public void ReadWrite(byte[] array, int startIndex)
		{
			var ms = new MemoryStream(array);
			ms.Position += startIndex;
			BitcoinStream bitStream = new BitcoinStream(ms, false);
			ReadWrite(bitStream);
		}

		public Transaction AddTransaction(Transaction tx)
		{
			Transactions.Add(tx);
			return tx;
		}

		/// <summary>
		/// Create a block with the specified option only. (useful for stripping data from a block)
		/// </summary>
		/// <param name="options">Options to keep</param>
		/// <returns>A new block with only the options wanted</returns>
		public Block WithOptions(TransactionOptions options)
		{
			if(Transactions.Count == 0)
				return this;
			if(options == TransactionOptions.Witness && Transactions[0].HasWitness)
				return this;
			if(options == TransactionOptions.None && !Transactions[0].HasWitness)
				return this;
			var instance = new Block();
			var ms = new MemoryStream();
			var bms = new BitcoinStream(ms, true);
			bms.TransactionOptions = options;
			this.ReadWrite(bms);
			ms.Position = 0;
			bms = new BitcoinStream(ms, false);
			bms.TransactionOptions = options;
			instance.ReadWrite(bms);
			return instance;
		}

		public void UpdateMerkleRoot()
		{
			this.Header.HashMerkleRoot = GetMerkleRoot().Hash;
		}

		public bool CheckMerkleRoot()
		{
			return Header.HashMerkleRoot == GetMerkleRoot().Hash;
		}

        //public Block CreateNextBlockWithCoinbase(BitcoinAddress address, int height)
        //{
        //    return CreateNextBlockWithCoinbase(address, height, DateTimeOffset.UtcNow);
        //}
        //public Block CreateNextBlockWithCoinbase(BitcoinAddress address, int height, DateTimeOffset now)
        //{
        //    if(address == null)
        //        throw new ArgumentNullException("address");
        //    Block block = new Block();
        //    block.Header.Nonce = RandomUtils.GetUInt32();
        //    block.Header.HashPrevBlock = this.GetHash();
        //    block.Header.BlockTime = now;
        //    var tx = block.AddTransaction(new Transaction());

        //    //pay miner
        //    tx.AddInput(new TxIn()
        //    {
        //        ScriptSig = new Script(Op.GetPushOp(RandomUtils.GetBytes(30)))
        //    });
        //    tx.Outputs.Add(new TxOut(address.Network.GetReward(height), address)
        //    {
        //        Value = address.Network.GetReward(height)
        //    });
        //    return block;
        //}

        ///// <summary>
        ///// This code seems to be either orphaned or used externally (rpc client?)
        ///// </summary>
        ///// <param name="pubkey"></param>
        ///// <param name="value"></param>
        ///// <returns></returns>
        //public Block CreateNextBlockWithCoinbase(PubKey pubkey, Money value)
        //{
        //    return CreateNextBlockWithCoinbase(pubkey, value, DateTimeOffset.UtcNow);
        //}
        //public Block CreateNextBlockWithCoinbase(PubKey pubkey, Money value, DateTimeOffset now)
        //{
        //    Block block = new Block();
        //    block.Header.Nonce = RandomUtils.GetUInt32();
        //    block.Header.HashPrevBlock = this.GetHash();
        //    block.Header.BlockTime = now;
        //    var tx = block.AddTransaction(new Transaction());


        //    tx.AddInput(new TxIn()
        //    {
        //        ScriptSig = new Script(Op.GetPushOp(RandomUtils.GetBytes(30)))
        //    });
        //    tx.Outputs.Add(new TxOut()
        //    {
        //        Value = value,
        //        ScriptPubKey = PayToPubkeyHashTemplate.Instance.GenerateScriptPubKey(pubkey)
        //    });
        //    return block;
        //}
#if !NOJSONNET
		public static Block ParseJson(string json)
		{
			var formatter = new BlockExplorerFormatter();
			var block = JObject.Parse(json);
			var txs = (JArray)block["tx"];
			Block blk = new Block();
			//blk.Header.Bits = new Target((uint)block["bits"]);
			blk.Header.BlockTime = Utils.UnixTimeToDateTime((uint)block["time"]);
			blk.Header.Tally = (uint)block["tally"];
			blk.Header.Version = (int)block["ver"];
			blk.Header.HashPrevBlock = uint256.Parse((string)block["prev_block"]);
			blk.Header.HashMerkleRoot = uint256.Parse((string)block["mrkl_root"]);
			foreach(var tx in txs)
			{
				blk.AddTransaction(formatter.Parse((JObject)tx));
			}
			return blk;
		}
#endif
		public static Block Parse(string hex)
		{
			return new Block(Encoders.Hex.DecodeData(hex));
		}

		public MerkleBlock Filter(params uint256[] txIds)
		{
			return new MerkleBlock(this, txIds);
		}

		public MerkleBlock Filter(BloomFilter filter)
		{
			return new MerkleBlock(this, filter);
		}
	}
}
