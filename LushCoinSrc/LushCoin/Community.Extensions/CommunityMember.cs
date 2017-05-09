using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LushCoin.Community.Extensions
{
    public class CommunityMember : LushCoin.Community.Extensions.ICommunityNode
    {
        public string Key { get; set; }
        public bool CanVote { get; set; }
        public HashSet<string> ParentId { get; set; }
        public HashSet<string> Children { get; set; }
    }
}
