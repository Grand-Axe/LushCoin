using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using LushCoin.OpenAsset;
using LushCoin.Protocol;

namespace LushCoin.Community.Extensions
{
    /// <summary>
    /// Handles trades, colored coin issuance and broadcasts to the network.
    /// </summary>
    public class MarketService
    {
        /// <summary>
        /// Broadcasts a transaction to the network.
        /// </summary>
        /// <param name="transaction"></param>
        /// <param name="network"></param>
        public static void Broadcast(Transaction transaction, Network network)
        {
            //broadcast to network
            using (Node node = Node.ConnectToLocal(network))
            {
                node.VersionHandshake();
                //Advertise transaction with its hash
                node.SendMessage(new InvPayload(InventoryType.MSG_TX, transaction.GetHash()));
                //broadcast
                node.SendMessage(new TxPayload(transaction));
                Thread.Sleep(500);
            }
        }

        /// <summary>
        /// Builds a colored coin transaction.
        /// </summary>
        /// <param name="issuanceCoin"></param>
        /// <param name="destinitionPubKey"></param>
        /// <param name="bitcoinSecret"></param>
        /// <param name="amount"></param>
        /// <param name="quantity"></param>
        /// <returns></returns>
        public static Transaction BuildCommunityTransaction
            (IssuanceCoin issuanceCoin, BitcoinColoredAddress destinitionPubKey, BitcoinSecret bitcoinSecret, decimal amount, int quantity)
        {
            TransactionBuilder transactionBuilder = new TransactionBuilder();

            Transaction transaction = transactionBuilder
                .AddKeys(bitcoinSecret)
                .AddCoins(issuanceCoin)
                .IssueAsset(destinitionPubKey, new AssetMoney(issuanceCoin.AssetId, quantity: quantity))
                .SendFees(Money.Coins(amount))
                .SetChange(bitcoinSecret.GetAddress())
                .BuildTransaction(true);

            if (transactionBuilder.Verify(transaction))
            {
                return transaction;
            }

            return null;
        }

        /// <summary>
        /// Issues a new colored coin.
        /// </summary>
        /// <param name="transactionId"></param>
        /// <param name="scriptPubKey"></param>
        /// <param name="url"></param>
        /// <returns></returns>
        public static IssuanceCoin IssueCoin(uint256 transactionId, Script scriptPubKey, string url)
        {
            uint256 fromTransactionHash = transactionId;
            uint fromOutputIndex = 0;
            Money amount = Money.Satoshis(6000);//an abitrary sum that is greater than the minimum 5460 satoshis accepted by Bitcoin miners.
            //Script scriptPubKey = new Script(Encoders.Hex.DecodeData(scriptPubKeyString));

            Coin coin = new Coin(fromTransactionHash, fromOutputIndex, amount, scriptPubKey);

            IssuanceCoin issuanceCoin = new IssuanceCoin(coin);
            issuanceCoin.DefinitionUrl = new Uri(url);

            return issuanceCoin;
        }
    }
}
