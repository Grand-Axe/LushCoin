using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LushCoin.Community.Extensions
{
    public class Ratification
    {
        /// <summary>
        /// Check if community has ratified idea.
        /// </summary>
        public static bool IsRatified(ChainedBlock chainedBlock)
        {
            bool isOriginal = EVec.IsOriginal(chainedBlock);
            bool isNovelEnough = EVec.IsNovelEnough(chainedBlock);

            //check if idea is ratified only if it is original and novel enough
            if (isOriginal && isNovelEnough)
            {
                //to do
            }
            throw new NotImplementedException("Ratification has not been implemented");
            //return false;
        }
    }
}
