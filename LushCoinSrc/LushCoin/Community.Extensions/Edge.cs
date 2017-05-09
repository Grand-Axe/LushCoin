using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LushCoin.Community.Extensions
{
    public class Edge
    {
        public string CommunityKey { get; set; }
        public string ForeignKey { get; set; }
        public int CommunityBagCount { get; set; }
        public int ForeignBagCount { get; set; }
        public HashSet<int> CommunityBag { get; set; }
        public HashSet<int> ForeignBag { get; set; }
    }
}
