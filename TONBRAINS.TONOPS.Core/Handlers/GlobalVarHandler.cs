using System;
using System.Collections.Generic;
using System.Text;

namespace TONBRAINS.TONOPS.Core.Handlers
{
    public static class GlobalVarHandler
    {
        public static string LOCALHOST_TEMP_DIR { get; set; }
        public static string SCRIPT_DIR { get; set; }
        public static string ROOT_DIR { get; set; }
        public static string NET_TON_DEV_SRC_TOP_DIR { get; set; }
        public static string TON_GITHUB_REPO { get; set; } = "https://github.com/tonlabs/ton-1.git";
        public static string TON_STABLE_GITHUB_COMMIT_ID { get; set; } = "37d1935f504c1d2939155325f0cbf2c1f595dbd4";
        public static string TON_SRC_DIR { get; set; }
        public static string TON_BUILD_DIR { get; set; }
        public static string TONOS_CLI_SRC_DIR { get; set; }
        public static string TON_WORK_DIR { get; set; }
        public static string UTILS_DIR { get; set; }
        public static string KEYS_DIR { get; set; }
        public static string KEYS_DIR_KEYS_S { get; set; }
        public static string KEYS_DIR_KEYS_L { get; set; }
        public static string KEYS_DIR_KEYS_A { get; set; }
        public static string KEYS_DIR_KEYS_C { get; set; }
        public static string KEYS_DIR_KEYS_V { get; set; }




        public static string CONFIGS_DIR { get; set; }
        public static string ADNL_PORT { get; set; }
        public static string PATH { get; set; }
        public static string LITESERVER_IP { get; set; }
        public static string LITESERVER_PORT { get; set; }
        public static string ENGINESERVER_IP { get; set; }
        public static string ENGINESERVER_PORT { get; set; }
        public static string SETUP_USER { get; set; } = "0";
        public static string SETUP_GROUP { get; set; } = "0";

        // public static string NODE_PREINIT_CONFIGS_DIR { get; set; } = getting from actual host "/root/configs/preinit_{auth.Host}";
        public static string TON_WORK_STATIC_DIR { get; set; }
        public static string TON_WORK_GLOBAL_CONFIG_PATH { get; set; }

        public static string TON_WORK_DB { get; set; }
        public static string TON_WORK_DB_CONFIG { get; set; }
        
        public static string TON_WORK_ETC { get; set; }

        public static string LOCAL_TEMP_DIR { get; set; } = "C:\\ton_tb_tmp";
        public static string TMP_DIR { get; set; } = "/tmp/ton_temp"; //getting from actual host TMP_DIR="/tmp/$(basename "$0" .sh)_$$"


        public static string TON_CRYPTO_DIR { get; set; }
        public static string FIFT_LIB_DIR { get; set; }
        public static string SMARTCONT_DIR { get; set; }

        public static string SMARTCONT_DIR_VALIDATOR_KEYS_PUB { get; set; }
        public static string SMARTCONT_DIR_GEN_ZEROSTATE_FIF { get; set; }
        public static string SMARTCONT_DIR_MAIN_WALLET_PK { get; set; }
        public static string SMARTCONT_DIR_CONFIG_MASTER_PK  { get; set; }

        public static string VALIDATOR_ENGINE { get; set; }
        public static string TON_WORK_NODE_LOG { get; set; }
        public static string TON_WORK_PROCNODE_LOG { get; set; }

        public static string LITE_CLIENT { get; set; }
     
        public static string LITE_SERVER_PUB { get; set; }
        public static string VALIDATOR_ENGINE_PUB { get; set; }
        public static string VALIDATOR_ENGINE_CONSOLE { get; set; }
        public static string BLOCKCHAIN_EXPLORER { get; set; }
        
        public static string CLIENT_KEY { get; set; }
        public static string FIFT { get; set; }
        public static string CREATE_STATE { get; set; }
        public static string TON_WORK_DB_IMPORT { get; set; }
        public static string TON_WORK_DB_KEYRING { get; set; }
        public static string TON_GLOBAL_FAKECONFIG { get; set; }


        static GlobalVarHandler()
        {
            LOCALHOST_TEMP_DIR = "C:\\temp_local_host";
            ROOT_DIR = "/root";
            SCRIPT_DIR = $"{ROOT_DIR}";
      
            NET_TON_DEV_SRC_TOP_DIR = $"{ROOT_DIR}/ton";
            TON_SRC_DIR = $"{ROOT_DIR}/ton";        //build//ton
            TON_BUILD_DIR = $"{TON_SRC_DIR}/build"; //root//ton/build
            TON_WORK_DIR = "/var/ton-work";
            TONOS_CLI_SRC_DIR = $"{ROOT_DIR}/tonos-cli";
            UTILS_DIR = $"{TON_BUILD_DIR}/utils";
            KEYS_DIR = $"{ROOT_DIR}/ton-keys";
            KEYS_DIR_KEYS_S = $"{KEYS_DIR}/keys_s";
            KEYS_DIR_KEYS_L = $"{KEYS_DIR}/keys_l";
            KEYS_DIR_KEYS_A = $"{KEYS_DIR}/keys_a";
            KEYS_DIR_KEYS_C = $"{KEYS_DIR}/keys_c";
            KEYS_DIR_KEYS_V = $"{KEYS_DIR}/keys_v";



            CONFIGS_DIR = $"{TON_SRC_DIR}/configs";
            PATH = $"{UTILS_DIR}:$PATH";
            ADNL_PORT = $"30310";
            LITESERVER_IP = "127.0.0.1";
            LITESERVER_PORT = "3031";
            ENGINESERVER_IP = "127.0.0.1";
            ENGINESERVER_PORT = "3030";
            VALIDATOR_ENGINE = $"{TON_BUILD_DIR}/validator-engine/validator-engine";
            SETUP_USER = "0";
            SETUP_GROUP = "0";
            TON_WORK_DB = $"{TON_WORK_DIR}/db";
            TON_WORK_DB_CONFIG = $"{TON_WORK_DB }/config.json";
            TON_WORK_DB_KEYRING = $"{TON_WORK_DB}/keyring";
            TON_WORK_ETC = $"{TON_WORK_DIR}/etc";
            TON_WORK_STATIC_DIR = $"{TON_WORK_DB}/static";
            TON_WORK_GLOBAL_CONFIG_PATH = $"{TON_WORK_DIR}/etc/ton-global.config.json";
            TON_CRYPTO_DIR = $"{TON_SRC_DIR}/crypto";
            FIFT_LIB_DIR = $"{TON_CRYPTO_DIR}/fift/lib";
            TON_WORK_DB_IMPORT = $"{TON_WORK_DB}/import";
            SMARTCONT_DIR = $"{TON_CRYPTO_DIR}/smartcont";
            SMARTCONT_DIR_VALIDATOR_KEYS_PUB = $"{SMARTCONT_DIR}/validator-keys.pub";
            SMARTCONT_DIR_GEN_ZEROSTATE_FIF = $"{SMARTCONT_DIR}/gen-zerostate.fif";
            SMARTCONT_DIR_MAIN_WALLET_PK = $"{SMARTCONT_DIR}/main-wallet.pk";
            SMARTCONT_DIR_CONFIG_MASTER_PK = $"{SMARTCONT_DIR}/config-master.pk";

            //

            TON_WORK_NODE_LOG = $"{TON_WORK_DIR}/node.log";
            TON_WORK_PROCNODE_LOG = $"{TON_WORK_DIR}/proc_node.log";
            TON_GLOBAL_FAKECONFIG = $"{TON_WORK_ETC}/ton-global.fakeconfig.json";

            LITE_CLIENT = $"{TON_BUILD_DIR}/lite-client/lite-client";

            VALIDATOR_ENGINE_CONSOLE = $"{TON_BUILD_DIR}/validator-engine-console/validator-engine-console";
            BLOCKCHAIN_EXPLORER = $"{TON_BUILD_DIR}/blockchain-explorer/blockchain-explorer";


            //
            LITE_SERVER_PUB = $"{KEYS_DIR}/liteserver.pub";
            VALIDATOR_ENGINE_PUB = $"{KEYS_DIR}/server.pub";
            CLIENT_KEY = $"{KEYS_DIR}/client";
            FIFT = $"{TON_BUILD_DIR}/crypto/fift";
            CREATE_STATE = $"{TON_BUILD_DIR}/crypto/create-state";

        }


    }
}