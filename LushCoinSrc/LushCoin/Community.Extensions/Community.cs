using LushCoin.DataEncoders;
using LushCoin.OpenAsset;
using LushCoin.Protocol;
using LushCoin.RPC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LushCoin.Community.Extensions
{
    /// <summary>
    /// Individuals that aspire to register a community will be required to create an invitation only
    /// list, such that it forms a graph with a single person as its root node. They would then
    /// create the communities public key, after which they would pay a colored coin, the
    /// community application coin, to an address called registration address.
    /// </summary>
    public class Community : ICommunityNode
    {
        /// <summary>
        /// SHA-256 id key.
        /// </summary>
        public string Key { get; set; }

        /// <summary>
        /// SHA-256 key of this community's parent community.
        /// </summary>
        public HashSet<string> ParentId { get; set; }

        /// <summary>
        /// Set of SHA-256 keys of sub-communities that are immediate children of this community.
        /// </summary>
        public HashSet<string> Children { get; set; }

        /// <summary>
        /// Array of border latitude and longitude values.
        /// </summary>
        public double[] LatLong { get; set; }

        /// <summary>
        /// Set of names of incubators that operate within borders of the community. Each incubator name is assumed to be unique.
        /// </summary>
        public HashSet<string> Incubators { get; set; }

        /// <summary>
        /// Set of SHA-256 keys for CommunityMembers - this list represents the people who are members of the community. 
        /// </summary>
        public HashSet<string> Members { get; set; }

        /// <summary>
        /// Simple method to add a person to a community.
        /// A new issuance coin is used for every invite.
        /// This is so that the colored coin can be traced back to the inviters scriptPubKey.
        /// The payment of the community application coin to the registration address adds the new
        /// community to the community registration address list. This enables the referencing of a
        /// community through its public key.
        /// </summary>
        /// <param name="rpcClient"></param>
        /// <param name="community"></param>
        /// <param name="member"></param>
        /// <param name="invitee"></param>
        /// <param name="definitionUrl"></param>
        /// <returns></returns>
        public static bool Invite(RPCClient rpcClient, Community community, CommunityMember member, CommunityMember invitee, string definitionUrl, Network network)
        {
            decimal amount = 0.0001m;
            Script scriptPubKey = new Script();
            uint256 transactionId = new uint256();
            bool canProceed = false;

            //check is member belongs in community to which they are inviting invitee
            if(IsCommunityMember(community, member))
            {
                //check that the inviter has at least one transaction balance that is big enough for the current transaction.
                UnspentCoin[] transactions = rpcClient.ListUnspent();
                for (int i = 0; i < transactions.Length; i++)
                {
                    if (transactions[i].Amount.ToUnit(MoneyUnit.Satoshi) >= amount)
                    {
                       scriptPubKey = transactions[i].ScriptPubKey;
                       transactionId = transactions[i].OutPoint.Hash;
                       canProceed = true;
                       break;
                    }
                }

                if (!canProceed)
                    return false;

                IssuanceCoin issuanceCoin = MarketService.IssueCoin(transactionId, scriptPubKey, definitionUrl);

                //build and verify the transaction
                BitcoinColoredAddress inviteePubKey = BitcoinAddress.Create(invitee.Key).ToColoredAddress();
                BitcoinSecret bitcoinSecret = new BitcoinSecret(community.Key);

                Transaction transaction = MarketService.BuildCommunityTransaction(issuanceCoin, inviteePubKey, bitcoinSecret, amount, 1);

                if (transaction != null)
                {
                    MarketService.Broadcast(transaction, network);
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Removal of a person from the community will remove the member that invited the
        /// affected person (called parent node) as well as the subgraph that has the parent node as
        /// its root. This is to maximise removal of tainted members, since bad members are more
        /// likely to invite those like themselves.
        /// </summary>
        /// <param name="community"></param>
        /// <param name="member"></param>
        public static bool Expel(Community community, CommunityMember member)
        {
            if (community == null || community.Children.Count == 0 || member == null)
                return false;

            //ensure that only members of the community in the method parameter are tested.
            if (!IsCommunityMember(community, member))
                return false;

            throw new NotImplementedException("Expel has not been implemented");
        }

        /// <summary>
        /// Tests if a person is a member of a particular community.
        /// </summary>
        /// <param name="community"></param>
        /// <param name="member"></param>
        /// <returns></returns>
        public static bool IsCommunityMember(Community community, CommunityMember member)
        {
            if (community == null || community.Members == null)
                return false;

            return community.Members.Contains(member.Key);
        }

        /// <summary>
        /// Tests a community for viability.
        /// A community is a network that consists of its skills base, businesses, infrastructure and
        /// demography (for instance, its population and literacy rate). If a community is viewed as a
        /// subgraph, then for it to be viable for registration, the strength of its connections within
        /// itself must be greater than the strength of its connections to its supergraph. This is
        /// observed with economically viable locations such as; villages, towns, cities and countries.
        /// </summary>
        /// <param name="community"></param>
        public static bool IsViable(Community community, HashSet<Edge> world)
        {
            if (community == null || world == null)
                return false;
            throw new NotImplementedException("IsViable has not been implemented");
        }
    }
}
