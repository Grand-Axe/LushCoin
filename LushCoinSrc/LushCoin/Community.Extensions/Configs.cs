using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LushCoin.Community.Extensions
{
    public class Configs
    {
        public string BaseDirectory
        {
            get
            {
                return AppDomain.CurrentDomain.BaseDirectory;
            }
        }

        public static string CommunityRegistrationAddress()
        {
            throw new NotImplementedException("CommunityRegistrationAddress has not yet been implemented");
        }

        public static string InventionApplicationAddress()
        {
            throw new NotImplementedException("InventionApplicationAddress has not yet been implemented");
        }

        public static string InnovationApplicationAddress()
        {
            throw new NotImplementedException("InnovationApplicationAddress has not yet been implemented");
        }

        /// <summary>
        /// Burns bitcoin using nBitcoin.
        /// </summary>
        /// <returns></returns>
        public static string BurnExchangeAddress()
        {
            throw new NotImplementedException("BurnExchangeAddress has not yet been implemented");
        }

        public static HashSet<string> GetSystemAddresses()
        {
            throw new NotImplementedException("GetSystemAddresses has not yet been implemented");
        }

        public static HashSet<string> GetCommunityAddresses()
        {
            throw new NotImplementedException("GetCommunityAddresses has not yet been implemented");
        }

        public static bool IsSystemAddress(string address)
        {
            HashSet<string> systemAddresses = GetSystemAddresses();
            bool containsAddress = systemAddresses.Contains(address);
            return containsAddress;
        }

        public static bool IsCommunityAddress(string address)
        {
            HashSet<string> communityAddresses = GetCommunityAddresses();
            bool containsAddress = communityAddresses.Contains(address);
            return containsAddress;
        }

    }
}
