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
        public HashSet<int> Bag { get; set; }
    }
}
