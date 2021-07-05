using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TONBRAINS.TONOPS.Core.Models
{
    public class TonConfigActulMdl
    {
        public  string ConfigAddr0 {get;set;}
        public  string ЕlectorAddr1 { get; set; }
        public  string МinterAddr2 { get; set; }
        public string CapabilitiesVersion8 { get; set; }
        public string Capabilities8 { get; set; }

        public  string NormalParamsMinTotRounds11 { get; set; }
        public  string NormalParamsMaxTotRounds11 { get; set; }
        public  string NormalParamsMinWins11 { get; set; }
        public  string NormalParamsMaxLosses11 { get; set; }
        public  string NormalParamsMinStoreSec11 { get; set; }
        public  string NormalParamsMaxStoreSec11 { get; set; }
        public  string NormalParamsBitPrice11 { get; set; }
        public  string NormalParamsCellPrice11 { get; set; }


        public  string CriticalParamsMinTotRounds11 { get; set; }
        public  string CriticalParamsMaxTotRounds11 { get; set; }
        public  string CriticalParamsMinWins11 { get; set; }
        public  string CriticalParamsMaxLosses11 { get; set; }
        public  string CriticalParamsMinStoreSec11 { get; set; }
        public  string CriticalParamsMaxStoreSec11 { get; set; }
        public  string CriticalParamsBitPrice11 { get; set; }
        public  string CriticalParamsCellPrice11 { get; set; }




        public string WorkchainEnabledSince12 { get; set; }
        public string WorkchainActualНinSplit12 { get; set; }
        public string WorkchainMinSplit12 { get; set; }
        public string WorkchainMaxSplit12 { get; set; }
        public string WorkchainBasic12 { get; set; }
        public string WorkchainActive12 { get; set; }
        public string WorkchainAcceptMsgs12 { get; set; }
        public string WorkchainFlags12 { get; set; }
        public string WorkchainZerostateRootHash12 { get; set; }
        public string WorkchainZerostateFileHash12 { get; set; }
        public string WorkchainVersion12 { get; set; }

        public string ComplaintPricesDepositAmount13 { get; set; }
        public string ComplaintPricesBitPrice13 { get; set; }
        public string ComplaintPricesCellPrice13 { get; set; }

        public string BlockGramsCreatedMasterchainBlockFee14 { get; set; }
        public string BlockGramsCreatedBasechainBlockFee14 { get; set; }

        public string ValidatorsElectedFor15 { get; set; }
        public string ElectionsStartBefore15 { get; set; }
        public string ElectionsEndBefore15 { get; set; }
        public string StakeHeldFor15 { get; set; }


        public string MaxValidators16 { get; set; }
        public string MaxMainValidators16 { get; set; }
        public string MinValidators16 { get; set; }


        public string MinStake17 { get; set; }
        public string MaxStake17 { get; set; }
        public string MinTotalStake17 { get; set; }
        public string MaxStakeFacor17 { get; set; }


        public string UtimeSince18 { get; set; }
        public string BitPricePs18 { get; set; }
        public string CellPricePs18 { get; set; }
        public string McBitPricePs18 { get; set; }
        public string McCellPricePs18 { get; set; }


        //ConfigMcGasPrices
        public string ConfigMcGasPricesGasFlatPfxFlatGasLimit20 { get; set; }
        public string ConfigMcGasPricesGasFlatPfxFlatGasPrice20 { get; set; }

        public string ConfigMcGasPricesGasPricesExtGasPrice20 { get; set; }
        public string ConfigMcGasPricesGasPricesExtGasLimit20 { get; set; }
        public string ConfigMcGasPricesGasPricesExtSpecialGasLimit20 { get; set; }
        public string ConfigMcGasPricesGasPricesExtGasCredit20 { get; set; }
        public string ConfigMcGasPricesGasPricesExtBlockGasLimit20 { get; set; }
        public string ConfigMcGasPricesGasPricesExtFreezeDueLimit20 { get; set; }
        public string ConfigMcGasPricesGasPricesExtDeleteDueLimit20 { get; set; }



        //ConfigGasPrices
        public string ConfigGasPricesGasFlatPfxFlatGasLimit21 { get; set; }
        public string ConfigGasPricesGasFlatPfxFlatGasPrice21 { get; set; }

        public string ConfigGasPricesGasPricesExtGasPrice21 { get; set; }
        public string ConfigGasPricesGasPricesExtGasLimit21 { get; set; }
        public string ConfigGasPricesGasPricesExtSpecialGasLimit21 { get; set; }
        public string ConfigGasPricesGasPricesExtGasCredit21 { get; set; }
        public string ConfigGasPricesGasPricesExtBlockGasLimit21 { get; set; }
        public string ConfigGasPricesGasPricesExtFreezeDueLimit21 { get; set; }
        public string ConfigGasPricesGasPricesExtDeleteDueLimit21 { get; set; }


        public string ConfigMcBlockLimitsBytesParamLimitsUnderload22 { get; set; }
        public string ConfigMcBlockLimitsBytesSoftLimitsUnderload22 { get; set; }
        public string ConfigMcBlockLimitsBytesHardLimitsUnderload22 { get; set; }
        public string ConfigMcBlockLimitsGasParamLimitsUnderload22 { get; set; }
        public string ConfigMcBlockLimitsGasSoftLimitsUnderload22 { get; set; }
        public string ConfigMcBlockLimitsGasHardLimitsUnderload22 { get; set; }
        public string ConfigMcBlockLimitsLtDataParamLimitsUnderload22 { get; set; }
        public string ConfigMcBlockLimitsLtDataSoftLimitsUnderload22 { get; set; }
        public string ConfigMcBlockLimitsLtDataHardLimitsUnderload22 { get; set; }



        public string ConfigBlockLimitsBytesParamLimitsUnderload23 { get; set; }
        public string ConfigBlockLimitsBytesSoftLimitsUnderload23 { get; set; }
        public string ConfigBlockLimitsBytesHardLimitsUnderload23 { get; set; }
        public string ConfigBlockLimitsGasParamLimitsUnderload23 { get; set; }
        public string ConfigBlockLimitsGasSoftLimitsUnderload23 { get; set; }
        public string ConfigBlockLimitsGasHardLimitsUnderload23 { get; set; }
        public string ConfigBlockLimitsLtDataParamLimitsUnderload23 { get; set; }
        public string ConfigBlockLimitsLtDataSoftLimitsUnderload23 { get; set; }
        public string ConfigBlockLimitsLtDataHardLimitsUnderload23 { get; set; }



        public string ConfigMcFwdPricesmsgForwardPricesLumpPrice24 { get; set; }
        public string ConfigMcFwdPricesmsgForwardPricesBitPrice24 { get; set; }
        public string ConfigMcFwdPricesmsgForwardPricesCellPrice24 { get; set; }
        public string ConfigMcFwdPricesmsgForwardPricesIhrPriceFactor24 { get; set; }
        public string ConfigMcFwdPricesmsgForwardPricesFirstFrac24 { get; set; }
        public string ConfigMcFwdPricesmsgForwardPricesNextFrac24 { get; set; }


        public string ConfigFwdPricesmsgForwardPricesLumpPrice25 { get; set; }
        public string ConfigFwdPricesmsgForwardPricesBitPrice25 { get; set; }
        public string ConfigFwdPricesmsgForwardPricesCellPrice25 { get; set; }
        public string ConfigFwdPricesmsgForwardPricesIhrPriceFactor25 { get; set; }
        public string ConfigFwdPricesmsgForwardPricesFirstFrac25 { get; set; }
        public string ConfigFwdPricesmsgForwardPricesNextFrac25 { get; set; }

        public string CatchainConfigNewFlags28 { get; set; }
        public string CatchainConfigNewShuffleMcValidators28 { get; set; }
        public string CatchainConfigNewMcCatchainLifetime28 { get; set; }
        public string CatchainConfigNewShardCatchainLifetime28 { get; set; }
        public string CatchainConfigNewShardValidatorsLifetime28 { get; set; }
        public string CatchainConfigNewShardValidatorsNum28 { get; set; }

        public string ConsensusConfigNewFlags29 { get; set; }
        public string ConsensusNewCatchain_ids29 { get; set; }
        public string ConsensusRoundCandidates29 { get; set; }
        public string ConsensusNextCandidateDelayMs29 { get; set; }
        public string ConsensusConsensusTimeoutMs29 { get; set; }
        public string ConsensusFastAttempts29 { get; set; }
        public string ConsensusAttemptDuration29 { get; set; }
        public string ConsensusCatchainMaxDeps29 { get; set; }
        public string ConsensusMaxBlockBytes29 { get; set; }
        public string ConsensusMaxCollatedBytes29 { get; set; }

        public string PrevValidatorsExtUtimeSince34 { get; set; }
        public string PrevValidatorsExtUtimeUuntil34 { get; set; }
        public string PrevValidatorsExtTotal34 { get; set; }
        public string PrevValidatorsExtMain34 { get; set; }
        public string PrevValidatorsExtTotalWeight34 { get; set; }

        public string CurValidatorsExtUtimeSince35 { get; set; }
        public string CurValidatorsExtUtimeUuntil35 { get; set; }
        public string CurValidatorsExtTotal35 { get; set; }
        public string CurValidatorsExtMain35 { get; set; }
        public string CurValidatorsExtTotalWeight35 { get; set; }

        public List<string> CurrentValidators { get; set; } = new List<string>();

        //block_grams_created
        //complaint_prices
        //public static string ConfigAddr { get; set; }
        //public static string ConfigAddr { get; set; }
        //public static string ConfigAddr { get; set; }
        //public static string ConfigAddr { get; set; }
        //public static string ConfigAddr { get; set; }
        //public static string ConfigAddr { get; set; }
        //public static string ConfigAddr { get; set; }
        //public static string ConfigAddr { get; set; }
        //public static string ConfigAddr { get; set; }
    }
}
