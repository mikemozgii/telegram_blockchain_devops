using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TONBRAINS.TONOPS.Core.Models
{
    public class ZaroStateConfMdl
    {
        // 13 setglobalid   //negative value means a test instance of the blockchain
        public int GlobalId { get; set; } = 13;
        //0x11EF55AA default-subwallet-id!  // use this subwallet id in user wallets by default
        public string DefaultSubwalletId { get; set; } = "0x11EF55AA";

        // Initial state of Workchain 0 (Basic workchain)
        // root-hash file-hash start-at actual-min-split min-split-depth max-split-depth wc-id
        //original basestate0_rhash basestate0_fhash now 0 4 8 0 add-std-workchain
        public string StartAt { get; set; } = "now";
        public int ActualMinSplit { get; set; } = 0;
        public int MinSplitDepth { get; set; } = 4;
        public int MaxSplitDepth { get; set; } = 8;
        public int WorkChainId { get; set; } = 0;

        // Stage 1 wallets
        //PK'Puasxr0QfFZZnYISRphVse7XHKfW7pZU5SJarVHXvQ+rpzkD rwallet-init-pubkey !
        public string RwalletIinitPubkey { get; set; } = "Puasxr0QfFZZnYISRphVse7XHKfW7pZU5SJarVHXvQ+rpzkD";
        // test
        //StA 1001 PuZ8WoEOTgSR8-HopmCIVlOVSL94tNn9zgraiJqMk1SnioEQ
        public string StA { get; set; } = "PuZ8WoEOTgSR8-HopmCIVlOVSL94tNn9zgraiJqMk1SnioEQ";

        // Stage 2 wallets
        // test
        //StB 999. PubMMGvqM08jx_6BibYldMclwjl-D88r7-u0_IEcDXHA30-G
        public string StB { get; set; } = "PubMMGvqM08jx_6BibYldMclwjl-D88r7-u0_IEcDXHA30-G";

        // Lockdowns

        // SmartContract #1 (Advanced wallet)
        // Create new advanced wallet; code adapted from `auto/wallet3-code.fif`
        // balance
        //GR$4999999000 allocated-balance - // balance
        public long AdvancedWallet_AllocatedBalance { get; set; } = 4999999000;
        // split_depth //0
        public int AdvancedWallet_SplitDepth { get; set; } = 0;
        // ticktock //0
        public int AdvancedWallet_TickTock { get; set; } = 0;
        // address //1 *  what is AllOnes ?
        public string AdvancedWallet_Address { get; set; } = "1 *";
        // mode: create+setaddr //6
        public int AdvancedWallet_CreateSetaddr { get; set; } = 6;


        //SmartContract #4 (elector)
        // balance: 500 grams
        public long Elector_AllocatedBalance { get; set; } = 500;
        // split_depth //0
        public int Elector_SplitDepth { get; set; } = 0;
        // ticktock: tick //2
        public int Elector_TickTock { get; set; } = 2;
        // address: -1:333...333
        public string Elector_Address { get; set; } = "3 *";
        // mode: create+setaddr //6
        public int Elector_CreateSetaddr { get; set; } = 6;


        //Configuration Parameters
        //1000 100 13 config.validator_num!
        public int MaxValidators { get; set; } = 1000;
        public int MaxMainValidators { get; set; } = 100;
        public int MinValidators { get; set; } = 13;

        //GR$10000 GR$10000000 GR$500000 sg~3 config.validator_stake_limits!
        public int MinStake { get; set; } = 10000;
        public int MaxStake { get; set; } = 10000000;
        public int MinTotalStake { get; set; } = 500000;
        // what is max factor
        public int MaxFactor { get; set; } = 3;

        // elected-for elect-start-before elect-end-before stakes-frozen-for
        // 400000 200000 4000 400000 config.election_params!
        //65536 32768 8192 32768 config.election_params! tl
        // will validate for 400k min
        public int ElectedFor { get; set; } = 400000;
        //election start in 200k min
        public int ElectStartBefore { get; set; } = 200000;
        //election times slot 4k 200k + 4k = end election in 204k
        public int ElectEndBefore { get; set; } = 4000;
        //can't take stake back
        public int StakesFrozenFor { get; set; } = 400000;

        //can't take stake back
        public string ElectorConfig_Address { get; set; } = "5 *";
    }
}
