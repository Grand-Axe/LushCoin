using LushCoin.DataEncoders;
using LushCoin.Protocol;
using LushCoin.Stealth;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LushCoin
{
    public class Consensus
    {
        public static Consensus Main
        {
            get
            {
                return Network.Main.Consensus;
            }
        }
        public static Consensus TestNet
        {
            get
            {
                return Network.TestNet.Consensus;
            }
        }
        public static Consensus RegTest
        {
            get
            {
                return Network.RegTest.Consensus;
            }
        }
        public class BuriedDeploymentsArray
        {
            Consensus _Parent;
            int[] _Heights;
            public BuriedDeploymentsArray(Consensus parent)
            {
                _Parent = parent;
                _Heights = new int[Enum.GetValues(typeof(BuriedDeployments)).Length];
            }
            public int this[BuriedDeployments index]
            {
                get
                {
                    return _Heights[(int)index];
                }
                set
                {
                    _Parent.EnsureNotFrozen();
                    _Heights[(int)index] = value;
                }
            }
        }
        public class BIP9DeploymentsArray
        {
            Consensus _Parent;
            BIP9DeploymentsParameters[] _Parameters;
            public BIP9DeploymentsArray(Consensus parent)
            {
                _Parent = parent;
                _Parameters = new BIP9DeploymentsParameters[Enum.GetValues(typeof(BIP9Deployments)).Length];
            }

            public BIP9DeploymentsParameters this[BIP9Deployments index]
            {
                get
                {
                    return _Parameters[(int)index];
                }
                set
                {
                    _Parent.EnsureNotFrozen();
                    _Parameters[(int)index] = value;
                }
            }
        }

        public Consensus()
        {
            _BuriedDeployments = new BuriedDeploymentsArray(this);
            _BIP9Deployments = new BIP9DeploymentsArray(this);
        }
        private readonly BuriedDeploymentsArray _BuriedDeployments;
        public BuriedDeploymentsArray BuriedDeployments
        {
            get
            {
                return _BuriedDeployments;
            }
        }


        private readonly BIP9DeploymentsArray _BIP9Deployments;
        public BIP9DeploymentsArray BIP9Deployments
        {
            get
            {
                return _BIP9Deployments;
            }
        }

        int _SubsidyHalvingInterval;
        public int SubsidyHalvingInterval
        {
            get
            {
                return _SubsidyHalvingInterval;
            }
            set
            {
                EnsureNotFrozen();
                _SubsidyHalvingInterval = value;
            }
        }


        int _MajorityEnforceBlockUpgrade;
        public int MajorityEnforceBlockUpgrade
        {
            get
            {
                return _MajorityEnforceBlockUpgrade;
            }
            set
            {
                EnsureNotFrozen();
                _MajorityEnforceBlockUpgrade = value;
            }
        }

        int _MajorityRejectBlockOutdated;
        public int MajorityRejectBlockOutdated
        {
            get
            {
                return _MajorityRejectBlockOutdated;
            }
            set
            {
                EnsureNotFrozen();
                _MajorityRejectBlockOutdated = value;
            }
        }

        int _MajorityWindow;
        public int MajorityWindow
        {
            get
            {
                return _MajorityWindow;
            }
            set
            {
                EnsureNotFrozen();
                _MajorityWindow = value;
            }
        }

        uint256 _BIP34Hash;
        public uint256 BIP34Hash
        {
            get
            {
                return _BIP34Hash;
            }
            set
            {
                EnsureNotFrozen();
                _BIP34Hash = value;
            }
        }


        //Target _PowLimit;
        //public Target PowLimit
        //{
        //    get
        //    {
        //        return _PowLimit;
        //    }
        //    set
        //    {
        //        EnsureNotFrozen();
        //        _PowLimit = value;
        //    }
        //}


        //TimeSpan _PowTargetTimespan;
        //public TimeSpan PowTargetTimespan
        //{
        //    get
        //    {
        //        return _PowTargetTimespan;
        //    }
        //    set
        //    {
        //        EnsureNotFrozen();
        //        _PowTargetTimespan = value;
        //    }
        //}


        //TimeSpan _PowTargetSpacing;
        //public TimeSpan PowTargetSpacing
        //{
        //    get
        //    {
        //        return _PowTargetSpacing;
        //    }
        //    set
        //    {
        //        EnsureNotFrozen();
        //        _PowTargetSpacing = value;
        //    }
        //}


        //bool _PowAllowMinDifficultyBlocks;
        //public bool PowAllowMinDifficultyBlocks
        //{
        //    get
        //    {
        //        return _PowAllowMinDifficultyBlocks;
        //    }
        //    set
        //    {
        //        EnsureNotFrozen();
        //        _PowAllowMinDifficultyBlocks = value;
        //    }
        //}


        //bool _PowNoRetargeting;
        //public bool PowNoRetargeting
        //{
        //    get
        //    {
        //        return _PowNoRetargeting;
        //    }
        //    set
        //    {
        //        EnsureNotFrozen();
        //        _PowNoRetargeting = value;
        //    }
        //}


        uint256 _HashGenesisBlock;
        public uint256 HashGenesisBlock
        {
            get
            {
                return _HashGenesisBlock;
            }
            set
            {
                EnsureNotFrozen();
                _HashGenesisBlock = value;
            }
        }

        //public long DifficultyAdjustmentInterval
        //{
        //    get
        //    {
        //        return ((long)PowTargetTimespan.TotalSeconds / (long)PowTargetSpacing.TotalSeconds);
        //    }
        //}

        int _MinerConfirmationWindow;
        public int MinerConfirmationWindow
        {
            get
            {
                return _MinerConfirmationWindow;
            }
            set
            {
                EnsureNotFrozen();
                _MinerConfirmationWindow = value;
            }
        }

        int _RuleChangeActivationThreshold;
        public int RuleChangeActivationThreshold
        {
            get
            {
                return _RuleChangeActivationThreshold;
            }
            set
            {
                EnsureNotFrozen();
                _RuleChangeActivationThreshold = value;
            }
        }


        int _CoinbaseMaturity = 100;
        public int CoinbaseMaturity
        {
            get
            {
                return _CoinbaseMaturity;
            }
            set
            {
                EnsureNotFrozen();
                _CoinbaseMaturity = value;
            }
        }

        bool frozen = false;
        public void Freeze()
        {
            frozen = true;
        }
        private void EnsureNotFrozen()
        {
            if (frozen)
                throw new InvalidOperationException("This instance can't be modified");
        }

        public Consensus Clone()
        {
            return new Consensus()
            {
                _BIP34Hash = _BIP34Hash,
                _HashGenesisBlock = _HashGenesisBlock,
                _MajorityEnforceBlockUpgrade = _MajorityEnforceBlockUpgrade,
                _MajorityRejectBlockOutdated = _MajorityRejectBlockOutdated,
                _MajorityWindow = _MajorityWindow,
                _MinerConfirmationWindow = _MinerConfirmationWindow,
                //_PowAllowMinDifficultyBlocks = _PowAllowMinDifficultyBlocks,
                //_PowLimit = _PowLimit,
                //_PowNoRetargeting = _PowNoRetargeting,
                //_PowTargetSpacing = _PowTargetSpacing,
                //_PowTargetTimespan = _PowTargetTimespan,
                _RuleChangeActivationThreshold = _RuleChangeActivationThreshold,
                _SubsidyHalvingInterval = _SubsidyHalvingInterval,
                _CoinbaseMaturity = _CoinbaseMaturity
            };
        }
    }
}
