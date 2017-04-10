using System.IO;

namespace LushCoin.BouncyCastle.Asn1
{
	internal interface Asn1OctetStringParser
		: IAsn1Convertible
	{
		Stream GetOctetStream();
	}
}
