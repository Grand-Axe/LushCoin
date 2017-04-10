using System.IO;

using LushCoin.BouncyCastle.Utilities.IO;

namespace LushCoin.BouncyCastle.Asn1
{
	internal abstract class LimitedInputStream
		: BaseInputStream
	{
		protected readonly Stream _in;
		private int _limit;

		internal LimitedInputStream(
			Stream inStream,
			int limit)
		{
			this._in = inStream;
			this._limit = limit;
		}

		internal virtual int GetRemaining()
		{
			// TODO: maybe one day this can become more accurate
			return _limit;
		}

		protected virtual void SetParentEofDetect(bool on)
		{
		}
	}
}