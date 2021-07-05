using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TONBRAINS.TONOPS.Core.DAL;
using TONBRAINS.TONOPS.Core.Models;

namespace TONBRAINS.TONOPS.Core.Services
{
    public class TonNetworkSvc
    {
        public IEnumerable<Node> GetNodesByTonNetworkId(string tonNetworkId)
        {
            return new NodeDbSvc().GetByTonNeworkId(tonNetworkId);

        }
        public string GetActualTonConfig(string tonNetworkId)
        {
            var nodes = new NodeDbSvc().GetByTonNeworkId(tonNetworkId);
            var config = new TONClientSvc().GetConfig(nodes.Select(q => q.Id).First());
            var rs = config.First().Value.First();
            return rs;
        }


        public Dictionary<string, string> GetActualTonConfigForNodes(params string[] nodeIds)
        {       
            var configs = new TONClientSvc().GetConfig(nodeIds);
            var rs = new Dictionary<string, string>();
            foreach (var c in configs)
            {
                if(c.Value.Any()) rs.Add(c.Key, c.Value.First());
            }
            return rs;
        }

        public TonConfigActulMdl ParseActualTonConfig(string rs)
        {
            var d = GetBasicValueWithRegex(rs, "config_addr");
            var m = new TonConfigActulMdl();
            m.ConfigAddr0 = GetBasicValueWithRegex(rs, "config_addr");
            m.ЕlectorAddr1 = GetBasicValueWithRegex(rs, "elector_addr");
            m.МinterAddr2 = GetBasicValueWithRegex(rs, "minter_addr");
            m.CapabilitiesVersion8 = GetBasicValueWithRegex(rs, "capabilities version", "capabilities");
            m.Capabilities8 = GetBasicValueWithRegex(rs, "capabilities");

            var normal_params_11 = GetBasicValueWithRegexExt(rs, "normal_params");
            var normal_params_11_tmps = normal_params_11.Split(" ");
            m.NormalParamsMinTotRounds11 = normal_params_11_tmps[1].Replace("min_tot_rounds:", "").Trim();
            m.NormalParamsMaxTotRounds11 = normal_params_11_tmps[2].Replace("max_tot_rounds:", "").Trim();
            m.NormalParamsMinWins11 = normal_params_11_tmps[3].Replace("min_wins:", "").Trim();
            m.NormalParamsMaxLosses11 = normal_params_11_tmps[4].Replace("max_losses:", "").Trim();
            m.NormalParamsMinStoreSec11 = normal_params_11_tmps[5].Replace("min_store_sec:", "").Trim();
            m.NormalParamsMaxStoreSec11 = normal_params_11_tmps[6].Replace("max_store_sec:", "").Trim();
            m.NormalParamsBitPrice11 = normal_params_11_tmps[7].Replace("bit_price:", "").Trim();
            m.NormalParamsCellPrice11 = normal_params_11_tmps[8].Replace("cell_price:", "").Trim();
            var critical_params_11 = GetBasicValueWithRegexExt(rs, "critical_params");
            var critical_params_tmps = normal_params_11.Split(" ");
            m.CriticalParamsMinTotRounds11 = critical_params_tmps[1].Replace("min_tot_rounds:", "").Trim();
            m.CriticalParamsMaxTotRounds11 = critical_params_tmps[2].Replace("max_tot_rounds:", "").Trim();
            m.CriticalParamsMinWins11 = critical_params_tmps[3].Replace("min_wins:", "").Trim();
            m.CriticalParamsMaxLosses11 = critical_params_tmps[4].Replace("max_losses:", "").Trim();
            m.CriticalParamsMinStoreSec11 = critical_params_tmps[5].Replace("min_store_sec:", "").Trim();
            m.CriticalParamsMaxStoreSec11 = critical_params_tmps[6].Replace("max_store_sec:", "").Trim();
            m.CriticalParamsBitPrice11 = critical_params_tmps[7].Replace("bit_price:", "").Trim();
            m.CriticalParamsCellPrice11 = critical_params_tmps[8].Replace("cell_price:", "").Trim();

            var workchain_12 = GetBasicValueWithRegexValue(rs, "workchain");
            var workchain_tmps = workchain_12.Split(" ");
            m.WorkchainEnabledSince12 = workchain_tmps[0].Replace("enabled_since:", "").Trim();
            m.WorkchainActualНinSplit12 = workchain_tmps[1].Replace("actual_min_split:", "").Trim();
            m.WorkchainMinSplit12 = workchain_tmps[2].Replace("min_split:", "").Trim();
            m.WorkchainMaxSplit12 = workchain_tmps[3].Replace("max_split:", "").Trim();
            m.WorkchainBasic12 = workchain_tmps[4].Replace("basic:", "").Trim();
            m.WorkchainActive12 = workchain_tmps[5].Replace("active:", "").Trim();
            m.WorkchainAcceptMsgs12 = workchain_tmps[6].Replace("accept_msgs:", "").Trim();
            m.WorkchainFlags12 = workchain_tmps[7].Replace("flags:", "").Trim();
            m.WorkchainZerostateRootHash12 = workchain_tmps[8].Replace("zerostate_root_hash:", "").Trim();
            m.WorkchainZerostateFileHash12 = workchain_tmps[9].Replace("zerostate_file_hash:", "").Trim();
            m.WorkchainVersion12 = workchain_tmps[10].Replace("version:", "").Trim();


            var complaint_prices_13 = GetBasicValueWithZero(rs, "complaint_prices", "x{");
            var complaint_prices_tmps = complaint_prices_13.Split(" ");
            m.ComplaintPricesDepositAmount13 = complaint_prices_tmps[8].Replace("value:", "").Replace(")", "").Trim();
            m.ComplaintPricesBitPrice13 = complaint_prices_tmps[20].Replace("value:", "").Replace(")", "").Trim();
            m.ComplaintPricesCellPrice13 = complaint_prices_tmps[32].Replace("value:", "").Replace(")", "").Trim();

            var block_grams_created_14 = GetBasicValueWithZero(rs, "block_grams_created", "x{");
            var block_grams_created_tmps = block_grams_created_14.Split(" ");
            m.BlockGramsCreatedMasterchainBlockFee14 = block_grams_created_tmps[8].Replace("value:", "").Replace(")", "").Trim();
            m.BlockGramsCreatedBasechainBlockFee14 = block_grams_created_tmps[20].Replace("value:", "").Replace(")", "").Trim();


            var t_15 = GetBasicValueWithRegex(rs, "validators_elected_for");
            var t_15_tmps = t_15.Split(" ");
            m.ValidatorsElectedFor15 = t_15_tmps[0].Trim();
            m.ElectionsStartBefore15 = t_15_tmps[1].Replace("elections_start_before:", "").Trim();
            m.ElectionsEndBefore15 = t_15_tmps[2].Replace("elections_end_before:", "").Trim();
            m.StakeHeldFor15 = t_15_tmps[3].Replace("stake_held_for:", "").Trim();


            var t_16 = GetBasicValueWithRegex(rs, "max_validators");
            var t_16_tmps = t_16.Split(" ");
            m.MaxValidators16 = t_15_tmps[0].Trim();
            m.MaxMainValidators16 = t_15_tmps[1].Replace("max_main_validators:", "").Trim();
            m.MinValidators16 = t_15_tmps[2].Replace("min_validators:", "").Trim();

            var t_17 = GetBasicValueWithZero(rs, "min_stake", "x{");
            var t_17_tmps = t_17.Split(" "); 
            m.MinStake17 = t_17_tmps[6].Replace("value:", "").Replace(")", "").Trim();
            m.MaxStake17 = t_17_tmps[14].Replace("value:", "").Replace(")", "").Trim();
            m.MinTotalStake17 = t_17_tmps[22].Replace("value:", "").Replace(")", "").Trim();
            m.MaxStakeFacor17 = t_17_tmps[23].Replace("max_stake_factor:", "").Replace(")", "").Trim();



            var t_18 = GetBasicValueWithRegex(rs, "utime_since");
            var t_18_tmps = t_18.Split(" ");
            m.UtimeSince18 = t_18_tmps[0].Trim();
            m.BitPricePs18 = t_18_tmps[1].Replace("bit_price_ps:", "").Trim();
            m.CellPricePs18 = t_18_tmps[2].Replace("cell_price_ps:", "").Trim();
            m.McBitPricePs18 = t_18_tmps[3].Replace("mc_bit_price_ps:", "").Trim();
            m.McCellPricePs18 = t_18_tmps[4].Replace("mc_cell_price_ps:", "").Trim();

            var t_20 = GetBasicValueWithZero(rs, "config_mc_gas_prices", "x{");
            var t_20_tmps = t_20.Split(" ");

            m.ConfigMcGasPricesGasFlatPfxFlatGasLimit20 = t_20_tmps[1].Replace("flat_gas_limit:", "").Trim();
            m.ConfigMcGasPricesGasFlatPfxFlatGasPrice20 = t_20_tmps[2].Replace("flat_gas_price:", "").Trim();

            m.ConfigMcGasPricesGasPricesExtGasPrice20 = t_20_tmps[7].Replace("gas_price:", "").Trim();
            m.ConfigMcGasPricesGasPricesExtGasLimit20 = t_20_tmps[8].Replace("gas_limit:", "").Trim();
            m.ConfigMcGasPricesGasPricesExtSpecialGasLimit20 = t_20_tmps[9].Replace("special_gas_limit:", "").Trim();
            m.ConfigMcGasPricesGasPricesExtGasCredit20 = t_20_tmps[10].Replace("gas_credit:", "").Trim();
            m.ConfigMcGasPricesGasPricesExtBlockGasLimit20 = t_20_tmps[11].Replace("block_gas_limit:", "").Trim();
            m.ConfigMcGasPricesGasPricesExtFreezeDueLimit20 = t_20_tmps[12].Replace("freeze_due_limit:", "").Trim();
            m.ConfigMcGasPricesGasPricesExtDeleteDueLimit20 = t_20_tmps[13].Replace("delete_due_limit:", "").Replace(")", "").Trim();



            var t_21 = GetBasicValueWithZero(rs, "config_gas_prices", "x{");
            var t_21_tmps = t_21.Split(" ");

            m.ConfigGasPricesGasFlatPfxFlatGasLimit21 = t_21_tmps[1].Replace("flat_gas_limit:", "").Trim();
            m.ConfigGasPricesGasFlatPfxFlatGasPrice21 = t_21_tmps[2].Replace("flat_gas_price:", "").Trim();

            m.ConfigGasPricesGasPricesExtGasPrice21 = t_21_tmps[7].Replace("gas_price:", "").Trim();
            m.ConfigGasPricesGasPricesExtGasLimit21 = t_21_tmps[8].Replace("gas_limit:", "").Trim();
            m.ConfigGasPricesGasPricesExtSpecialGasLimit21 = t_21_tmps[9].Replace("special_gas_limit:", "").Trim();
            m.ConfigGasPricesGasPricesExtGasCredit21 = t_21_tmps[10].Replace("gas_credit:", "").Trim();
            m.ConfigGasPricesGasPricesExtBlockGasLimit21 = t_21_tmps[11].Replace("block_gas_limit:", "").Trim();
            m.ConfigGasPricesGasPricesExtFreezeDueLimit21 = t_21_tmps[12].Replace("freeze_due_limit:", "").Trim();
            m.ConfigGasPricesGasPricesExtDeleteDueLimit21 = t_21_tmps[13].Replace("delete_due_limit:", "").Replace(")", "").Trim();


            var t_22 = GetBasicValueWithZero(rs, "config_mc_block_limits", "x{");
            var t_22_tmps = t_22.Split(" ");

            m.ConfigMcBlockLimitsBytesParamLimitsUnderload22 = t_22_tmps[5].Replace("underload:", "").Trim();
            m.ConfigMcBlockLimitsBytesSoftLimitsUnderload22 = t_22_tmps[6].Replace("soft_limit:", "").Trim();
            m.ConfigMcBlockLimitsBytesHardLimitsUnderload22 = t_22_tmps[7].Replace("hard_limit:", "").Replace(")", "").Trim();
            m.ConfigMcBlockLimitsGasParamLimitsUnderload22 = t_22_tmps[12].Replace("underload:", "").Trim();
            m.ConfigMcBlockLimitsGasSoftLimitsUnderload22 = t_22_tmps[13].Replace("soft_limit:", "").Trim();
            m.ConfigMcBlockLimitsGasHardLimitsUnderload22 = t_22_tmps[14].Replace("hard_limit:", "").Replace(")", "").Trim();
            m.ConfigMcBlockLimitsLtDataParamLimitsUnderload22 = t_22_tmps[19].Replace("underload:", "").Trim();
            m.ConfigMcBlockLimitsLtDataSoftLimitsUnderload22 = t_22_tmps[20].Replace("soft_limit:", "").Trim();
            m.ConfigMcBlockLimitsLtDataHardLimitsUnderload22 = t_22_tmps[21].Replace("hard_limit:", "").Replace(")", "").Trim();


            var t_23 = GetBasicValueWithZero(rs, "config_block_limits", "x{");
            var t_23_tmps = t_23.Split(" ");

            m.ConfigBlockLimitsBytesParamLimitsUnderload23 = t_23_tmps[5].Replace("underload:", "").Trim();
            m.ConfigBlockLimitsBytesSoftLimitsUnderload23 = t_23_tmps[6].Replace("soft_limit:", "").Trim();
            m.ConfigBlockLimitsBytesHardLimitsUnderload23 = t_23_tmps[7].Replace("hard_limit:", "").Replace(")", "").Trim();
            m.ConfigBlockLimitsGasParamLimitsUnderload23 = t_23_tmps[12].Replace("underload:", "").Trim();
            m.ConfigBlockLimitsGasSoftLimitsUnderload23 = t_23_tmps[13].Replace("soft_limit:", "").Trim();
            m.ConfigBlockLimitsGasHardLimitsUnderload23 = t_23_tmps[14].Replace("hard_limit:", "").Replace(")", "").Trim();
            m.ConfigBlockLimitsLtDataParamLimitsUnderload23 = t_23_tmps[10].Replace("underload:", "").Trim();
            m.ConfigBlockLimitsLtDataSoftLimitsUnderload23 = t_23_tmps[20].Replace("soft_limit:", "").Trim();
            m.ConfigBlockLimitsLtDataHardLimitsUnderload23 = t_23_tmps[21].Replace("hard_limit:", "").Replace(")", "").Trim();


            var t_24 = GetBasicValueWithZero(rs, "config_mc_fwd_prices", "x{");
            var t_24_tmps = t_24.Split(" ");

            m.ConfigMcFwdPricesmsgForwardPricesLumpPrice24 = t_24_tmps[1].Replace("lump_price:", "").Trim();
            m.ConfigMcFwdPricesmsgForwardPricesBitPrice24 = t_24_tmps[2].Replace("bit_price:", "").Trim();
            m.ConfigMcFwdPricesmsgForwardPricesCellPrice24 = t_24_tmps[3].Replace("cell_price:", "").Replace(")", "").Trim();
            m.ConfigMcFwdPricesmsgForwardPricesIhrPriceFactor24 = t_24_tmps[4].Replace("ihr_price_factor:", "").Trim();
            m.ConfigMcFwdPricesmsgForwardPricesFirstFrac24 = t_24_tmps[5].Replace("first_frac:", "").Trim();
            m.ConfigMcFwdPricesmsgForwardPricesNextFrac24 = t_24_tmps[6].Replace("next_frac:", "").Replace(")", "").Trim();


            var t_25 = GetBasicValueWithZero(rs, "config_fwd_prices", "x{");
            var t_25_tmps = t_25.Split(" ");

            m.ConfigFwdPricesmsgForwardPricesLumpPrice25 = t_25_tmps[1].Replace("lump_price:", "").Trim();
            m.ConfigFwdPricesmsgForwardPricesBitPrice25 = t_25_tmps[2].Replace("bit_price:", "").Trim();
            m.ConfigFwdPricesmsgForwardPricesCellPrice25 = t_25_tmps[3].Replace("cell_price:", "").Replace(")", "").Trim();
            m.ConfigFwdPricesmsgForwardPricesIhrPriceFactor25 = t_25_tmps[4].Replace("ihr_price_factor:", "").Trim();
            m.ConfigFwdPricesmsgForwardPricesFirstFrac25 = t_25_tmps[5].Replace("first_frac:", "").Trim();
            m.ConfigFwdPricesmsgForwardPricesNextFrac25 = t_25_tmps[6].Replace("next_frac:", "").Replace(")", "").Trim();

            var t_28 = GetBasicValueWithRegex(rs, "catchain_config_new flags");
            var t_28_tmps = t_28.Split(" ");

            m.CatchainConfigNewFlags28 = t_28_tmps[0].Replace("flags:", "").Trim();
            m.CatchainConfigNewShuffleMcValidators28 = t_28_tmps[1].Replace("shuffle_mc_validators:", "").Trim();
            m.CatchainConfigNewMcCatchainLifetime28 = t_28_tmps[2].Replace("mc_catchain_lifetime:", "").Replace(")", "").Trim();
            m.CatchainConfigNewShardCatchainLifetime28 = t_28_tmps[3].Replace("shard_catchain_lifetime:", "").Trim();
            m.CatchainConfigNewShardValidatorsLifetime28 = t_28_tmps[4].Replace("shard_validators_lifetime:", "").Trim();
            m.CatchainConfigNewShardValidatorsLifetime28 = t_28_tmps[5].Replace("shard_validators_num:", "").Replace(")", "").Trim();

            var t_29 = GetBasicValueWithRegex(rs, "consensus_config_new flags");
            var t_29_tmps = t_29.Split(" ");

            m.ConsensusConfigNewFlags29 = t_29_tmps[0].Replace("flags:", "").Trim();
            m.ConsensusNewCatchain_ids29 = t_29_tmps[1].Replace("new_catchain_ids:", "").Trim();
            m.ConsensusRoundCandidates29 = t_29_tmps[2].Replace("round_candidates:", "").Replace(")", "").Trim();
            m.ConsensusNextCandidateDelayMs29 = t_29_tmps[3].Replace("next_candidate_delay_ms:", "").Trim();
            m.ConsensusConsensusTimeoutMs29 = t_29_tmps[4].Replace("consensus_timeout_ms:", "").Trim();
            m.ConsensusFastAttempts29 = t_29_tmps[5].Replace("fast_attempts:", "").Trim();
            m.ConsensusAttemptDuration29 = t_29_tmps[6].Replace("attempt_duration:", "").Trim();
            m.ConsensusCatchainMaxDeps29 = t_29_tmps[7].Replace("catchain_max_deps:", "").Trim();
            m.ConsensusMaxBlockBytes29 = t_29_tmps[8].Replace("max_block_bytes:", "").Trim();
            m.ConsensusMaxCollatedBytes29 = t_29_tmps[9].Replace("max_collated_bytes:", "").Replace(")", "").Trim();

            var t_34 = GetBasicValueWithRegexNewLine(rs, "prev_validators:\\(validators_ext utime_since");
            if (!string.IsNullOrWhiteSpace(t_34))
            {
                var t_34_tmps = t_34.Split(" ");

                m.PrevValidatorsExtUtimeSince34 = t_34_tmps[0].Replace("utime_since:", "").Trim();
                m.PrevValidatorsExtUtimeUuntil34 = t_34_tmps[1].Replace("utime_until:", "").Trim();
                m.PrevValidatorsExtTotal34 = t_34_tmps[2].Replace("total:", "").Trim();
                m.PrevValidatorsExtMain34 = t_34_tmps[3].Replace("main:", "").Trim();
                m.PrevValidatorsExtTotalWeight34 = t_34_tmps[4].Replace("total_weight:", "").Replace(")", "").Trim();
            }
 

            var t_35 = GetBasicValueWithRegexNewLine(rs, "cur_validators:\\(validators_ext utime_since");
            var t_35_tmps = t_35.Split(" ");
            m.CurValidatorsExtUtimeSince35 = t_35_tmps[0].Replace("utime_since:", "").Trim();
            m.CurValidatorsExtUtimeUuntil35 = t_35_tmps[1].Replace("utime_until:", "").Trim();
            m.CurValidatorsExtTotal35 = t_35_tmps[2].Replace("total:", "").Trim();
            m.CurValidatorsExtMain35 = t_35_tmps[3].Replace("main:", "").Trim();
            m.CurValidatorsExtTotalWeight35 = t_35_tmps[4].Replace("total_weight:", "").Replace(")", "").Trim();

            

            var currentValidators = GetBasicValueWithZero(rs, "cur_validators", "x{");
            var currentValidators_tmps = currentValidators.Split("\n");
            foreach (var t in currentValidators_tmps)
            {
                if (t.Contains("ed25519_pubkey pubkey:"))
                {

                    var tt = t.Replace("public_key:(ed25519_pubkey pubkey:x", "").Replace(")", "").Replace("weight:17", "").Trim();
                    m.CurrentValidators.Add(tt);


                }
            
            }



            return m;
        }


        public string GetBasicValueWithRegex(string val, string start)
        {
            try
            {
                var r = Regex.Matches(val, $"{start}:(.*?)\\)");
                var rs = r[0].Groups[1].Value.Trim();

                return rs;
            }
            catch
            {
                return null;
            }

 
           
        }

        public string GetBasicValueWithRegexNewLine(string val, string start)
        {

            try
            {
                var r = Regex.Matches(val, $"{start}:(.*?)\n");
                var rs = r[0].Groups[1].Value.Trim();
                return rs;

            }
            catch
            {
                return null;
            }
          

         

        }

        public string GetBasicValueWithZero(string val, string start, string end)
        {
            //"(?<=" & " person—" & “)([\s\S]*?)(?=" & "Not over" & ")"
            var r = Regex.Matches(val, $"(?<={start})[\\S\\s]*?(?={end})");
            var rs = r[0].Groups[0].Value.Trim();

            return rs;

        }

        public string GetBasicValueWithRegexExt(string val, string start)
        {

            var r = Regex.Matches(val, $"{start}:\\((.*?)\\)");
            var rs = r[0].Groups[1].Value.Trim();

            return rs;

        }

        public string GetBasicValueWithRegexValue(string val, string start)
        {

            var r = Regex.Matches(val, $"value:\\({start}(.*?)\n");
            var rs = r[0].Groups[1].Value.Trim();

            return rs;

        }

        public string GetBasicValueWithRegex(string val, string start, string end)
        {

            var r = Regex.Matches(val, $"{start}:(.*?){end}");
            var rs = r[0].Groups[1].Value.Trim();

            return rs;

        }
    }
}
