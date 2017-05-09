using System;
using System.Collections.Generic;

namespace LushCoin.Community.Extensions
{
    public interface ICommunityNode
    {
        string Key { get; set; }
        HashSet<string> Children { get; set; }
        HashSet<string> ParentId { get; set; }
    }
}
