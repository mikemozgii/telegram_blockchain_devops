using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using TONBRAINS.TONOPS.Core.DAL;
using TONBRAINS.TONOPS.Core.Extensions;
using TONBRAINS.TONOPS.Core.Handlers;
using TONBRAINS.TONOPS.Core.Helpers;
using TONBRAINS.TONOPS.Core.Models;
using TONBRAINS.TONOPS.Core.SSH;
using TONBRAINS.TONOPS.Ffi;
using TONBRAINS.TONOPS.WebApp.WebApp.Helpers;

namespace TONBRAINS.TONOPS.Core.Services
{
    public class TONNodeSetupSvc
    {
        //public IEnumerable<Node> _nodes;

        //public TONNodeSetupSvc(string[] nodeIds)
        //{
        //    _nodes = new NodeDbSvc().GetbyIds(nodeIds);
        //    if (nodeIds.Length != _nodes.Count())
        //    {
        //        throw new System.ArgumentException("Nodes Count does not match");
        //    }
        //}

        //public TONNodeSetupSvc(Node[] nodes)
        //{
        //    _nodes = nodes;
        //}

        //public bool Clone_Build_Ton()
        //{
        //    InstallDependcy();
        //    CloneAndBuildTonRepo();
        //    return true;
        //}

        //public bool Init_Validator()
        //{
        //    PreInit_Ext();
        //    //CreateStaticDir();
        //    //SetZeroState(nodes);

        //    //RunValidatorEngine(nodes);
        //    return true;
        //}

        public bool StartTonNetowrk(string tonNetworkId)
        {

            var TonNetworkDbSvc = new TonNetworkDbSvc();
            var tonnetwork = TonNetworkDbSvc.GetById(tonNetworkId);
            var NodeDbSvc = new NodeDbSvc();
            var nodes = NodeDbSvc.GetByTonNeworkId(tonNetworkId);
            var nodeIds = nodes.Select(q => q.Id).ToList();
            var validatorSmartContract = tonnetwork.ValidatorId;
            var mainSmartContract = tonnetwork.ContractId;
            ClearNodeDbData(nodeIds, tonnetwork);
            StopValidatorEngine(nodeIds);
            PreInit_Ext(nodeIds);
            SetupZeroState(nodeIds, tonnetwork);
            RunValidatorEngine(nodeIds);

            foreach (var n in nodes)
            {
                n.Type = NodeTypes.QUANTONActive;
            }

            tonnetwork.Active = true;
            tonnetwork.DateStarted = DateTime.Now;

            NodeDbSvc.Update(nodes.ToArray());
            TonNetworkDbSvc.Update(tonnetwork);
            return true;
        }


        public Dictionary<string, bool> InstallDependcy(IEnumerable<string> nodeIds)
        {
            return new SSHCommandHlp(nodeIds).ExecuteCommandsParallelBash("ton_install_dependency");
        }

        public Dictionary<string, bool> CloneAndBuildTonRepo(IEnumerable<string> nodeIds)
        {
            var content = new CommonHelprs().GetBashContentFromFile("ton_clone_repo").ReplaceByConfigValues();
            return new SSHCommandHlp(nodeIds).ExecuteCommandsParallelBashByContent(content);
        }


        public bool PreInit_Ext1(IEnumerable<string> nodeIds)
        {
            var nodes = new NodeDbSvc().GetByIds(nodeIds.ToArray());
            foreach (var n in nodes)
            {
                var NODE_PREINIT_CONFIGS_DIR = $"{GlobalVarHandler.CONFIGS_DIR}/preinit_{n.Ip}"; ///root/ton/configs
                var content = new CommonHelprs().GetBashContentFromFile("preinit").ReplaceByConfigValues().
                Replace("${IP_ADDRESS}", n.Ip).Replace("${NODE_PREINIT_CONFIGS_DIR}", NODE_PREINIT_CONFIGS_DIR);
                new SSHCommandHlp(n).ExecuteCommandsParallelBashByContent(content);
            }


            return true;
        }

        public bool PreInit_Ext(IEnumerable<string> nodeIds)
        {
            var CommonHelprs = new CommonHelprs();
            var nodes = new NodeDbSvc().GetByIds(nodeIds.ToArray());
            var fakeConfig = CommonHelprs.GetFiletoByteForTonConfig("ton-globalfake.config.json");
            var tasks = new List<Task>();
            foreach (var n in nodes)
            {
                var task = Task.Run(() =>
                {
                    var NODE_ADDRESS = $"{n.Ip}:{GlobalVarHandler.ADNL_PORT}";
                    var SSHCommandHlp = new SSHCommandHlp(n);

                    //reabuild ton_work directory
                    RebuildTonWorkDir(n.Id.ToEnumerable());
                    var NODE_PREINIT_CONFIGS_DIR = $"{GlobalVarHandler.CONFIGS_DIR}/preinit_{n.Ip}";
                    SSHCommandHlp.DeleteCreateDirectory(GlobalVarHandler.CONFIGS_DIR);
                    SSHCommandHlp.CreateDirectory(NODE_PREINIT_CONFIGS_DIR);

                    //Upload Fake Ton config
                    SSHCommandHlp.UploadFileToHost(fakeConfig, GlobalVarHandler.TON_GLOBAL_FAKECONFIG);

                    //init validator Ton fake db config
                    ExcuteValidatorEngineIComands($"--global-config {GlobalVarHandler.TON_GLOBAL_FAKECONFIG.ToQQ()} --db {GlobalVarHandler.TON_WORK_DB.ToQQ()} --ip {NODE_ADDRESS}", n.Id.ToEnumerable());
                    //// rebuild /root/ton-keys dir
                    RebuildTonKeysDir(n.Id.ToEnumerable());

                    ////generate keys
                    var serverKey = ExcuteGenerateRadomIdComandsWithResult("-m keys -n server", n.Id.ToEnumerable()).First().Value.First().Split(new char[0], StringSplitOptions.RemoveEmptyEntries);
                    SSHCommandHlp.UploadFileToHost(CommonHelprs.GenerateByteaFromString($"{serverKey[0]} {serverKey[1]}"), GlobalVarHandler.KEYS_DIR_KEYS_S);
                    var serverKeyringPath = $"{GlobalVarHandler.TON_WORK_DB_KEYRING}/{serverKey[0]}";
                    SSHCommandHlp.ExecuteCommands(
                        $"mv {GlobalVarHandler.ROOT_DIR}/server {GlobalVarHandler.TON_WORK_DB_KEYRING}/{serverKey[0]}",
                        $"mv {GlobalVarHandler.ROOT_DIR}/server.pub {GlobalVarHandler.KEYS_DIR}/server.pub"
                        );


                    var validatorKey = ExcuteGenerateRadomIdComandsWithResult("-m keys -n validator", n.Id.ToEnumerable()).First().Value.First().Split(new char[0], StringSplitOptions.RemoveEmptyEntries);
                    var validatorPath = $"{GlobalVarHandler.KEYS_DIR}/keys_v";
                    SSHCommandHlp.UploadFileToHost(CommonHelprs.GenerateByteaFromString($"{validatorKey[0]} {validatorKey[1]}"), validatorPath);
                    SSHCommandHlp.ExecuteCommands(
                        $"mv {GlobalVarHandler.ROOT_DIR}/validator {GlobalVarHandler.TON_WORK_DB_KEYRING}/{validatorKey[0]}",
                        $"mv {GlobalVarHandler.ROOT_DIR}/validator.pub {GlobalVarHandler.KEYS_DIR}/validator.pub"
                        );

                    var validatorubPath = $"{GlobalVarHandler.KEYS_DIR}/validator.pub";
                    var keypubPath = $"{NODE_PREINIT_CONFIGS_DIR}/{n.Ip}-key.pub";
                    SSHCommandHlp.ExecuteCommands($"dd skip=4 count=32 if={validatorubPath.ToQQ()} of={keypubPath.ToQQ()} bs=1 status=none");

                    var adnlKey = ExcuteGenerateRadomIdComandsWithResult("-m keys -n adnl", n.Id.ToEnumerable()).First().Value.First().Split(new char[0], StringSplitOptions.RemoveEmptyEntries);
                    var adnlKeyPath = $"{GlobalVarHandler.KEYS_DIR}/keys_a";
                    SSHCommandHlp.UploadFileToHost(CommonHelprs.GenerateByteaFromString($"{adnlKey[0]} {adnlKey[1]}"), adnlKeyPath);
                    SSHCommandHlp.ExecuteCommands(
            $"mv {GlobalVarHandler.ROOT_DIR}/adnl {GlobalVarHandler.TON_WORK_DB_KEYRING}/{adnlKey[0]}",
            $"mv {GlobalVarHandler.ROOT_DIR}/adnl.pub {GlobalVarHandler.KEYS_DIR}/adnl.pub"
            );

                    var liteserverKey = ExcuteGenerateRadomIdComandsWithResult("-m keys -n liteserver", n.Id.ToEnumerable()).First().Value.First().Split(new char[0], StringSplitOptions.RemoveEmptyEntries);
                    var liteserverKeyPath = $"{GlobalVarHandler.KEYS_DIR}/keys_l";
                    SSHCommandHlp.UploadFileToHost(CommonHelprs.GenerateByteaFromString($"{liteserverKey[0]} {liteserverKey[1]}"), liteserverKeyPath);
                    SSHCommandHlp.ExecuteCommands(
$"mv {GlobalVarHandler.ROOT_DIR}/liteserver {GlobalVarHandler.TON_WORK_DB_KEYRING}/{liteserverKey[0]}",
$"mv {GlobalVarHandler.ROOT_DIR}/liteserver.pub {GlobalVarHandler.KEYS_DIR}/liteserver.pub"
);
                    var clientKey = ExcuteGenerateRadomIdComandsWithResult("-m keys -n client", n.Id.ToEnumerable()).First().Value.First().Split(new char[0], StringSplitOptions.RemoveEmptyEntries);
                    var clientPath = $"{GlobalVarHandler.KEYS_DIR}/keys_c";
                    SSHCommandHlp.UploadFileToHost(CommonHelprs.GenerateByteaFromString($"{clientKey[0]} {clientKey[1]}"), clientPath);
                    SSHCommandHlp.ExecuteCommands(
$"mv {GlobalVarHandler.ROOT_DIR}/client.pub {GlobalVarHandler.KEYS_DIR}/client.pub",
$"mv {GlobalVarHandler.ROOT_DIR}/client {GlobalVarHandler.KEYS_DIR}/client"
);


                 
                    var initConfigBytea = SSHCommandHlp.DownloadFileToByteaHost(GlobalVarHandler.TON_WORK_DB_CONFIG);
                    string initConfigString = Encoding.Default.GetString(initConfigBytea.First().Value);
                    var initConfig = JsonConvert.DeserializeObject<InitTonDbConfigMdl>(initConfigString);

                    var udp_Ip = initConfig.addrs.First().ip;
                    var engine_adnl_key = initConfig.adnl.First(q => q.category == 1).id;
                    var engine_dht_key = initConfig.adnl.First(q => q.category == 0).id;
                    var dateTimeNow_unix = UnixTimeHelper.UnixTimeUTCNow()  + 100000;
                    //setup full Ton Db Config
                    var fullConfig = new CommonHelprs().GetFiletoStringForTonConfig("ton-db.config.json");
                    var fullConfigString = fullConfig.Replace("${validator_id}", validatorKey[1])
                        .Replace("${validator_id}", validatorKey[1])
                        .Replace("${adnl_id}", adnlKey[1])
                        .Replace("${adnl_id}", adnlKey[1])
                        .Replace("${server_id}", serverKey[1])
                        .Replace("${liteserver_id}", liteserverKey[1])
                        .Replace("$udp_Ip", udp_Ip.ToString())
                        .Replace("${client_id}", clientKey[1])
                        .Replace("${engine_adnl}", engine_adnl_key)
                        .Replace("${engine_dht}", engine_dht_key).Replace("$expire_at", dateTimeNow_unix.ToString());

                    SSHCommandHlp.UploadFileToHost(CommonHelprs.GenerateByteaFromString(fullConfigString), GlobalVarHandler.TON_WORK_DB_CONFIG);
                    var engine_dht_key_hex = engine_dht_key.FromBase64ToTonHex();

                    var dhtPath = $"{NODE_PREINIT_CONFIGS_DIR}/{n.Ip}-dht";
                    var c1 = $"-k {GlobalVarHandler.TON_WORK_DB_KEYRING}/{engine_dht_key_hex}";
                    var c0 = "{\\\"@type\\\":\\\"adnl.addressList\\\",\\\"addrs\\\":[{\\\"@type\\\":\\\"adnl.address.udp\\\",\\\"ip\\\" : " + udp_Ip + ",\\\"port\\\":30310 }]}";
                    var dhtSignature = ExcuteGenerateRadomIdComandsWithResult($"-m dht -a {c0.ToQQ()} {c1}", n.Id.ToEnumerable()).First().Value.First();
                    SSHCommandHlp.UploadFileToHost(CommonHelprs.GenerateByteaFromString(dhtSignature), dhtPath);

                    SSHCommandHlp.DeleteFile(GlobalVarHandler.TON_GLOBAL_FAKECONFIG);
                });

                tasks.Add(task);
            }

            Task.WaitAll(tasks.ToArray());
            return true;
        }

        public void SetupTonDbNodeConfig(InitTonDbConfigMdl initDbConfig)
        {


        }

        public Dictionary<string, bool> ExcuteValidatorEngineIComands(string cmd, IEnumerable<string> nodeIds)
        {
            var validatorBuildPath = GlobalVarHandler.TON_BUILD_DIR + "/validator-engine/validator-engine";
            return new SSHCommandHlp(nodeIds).ExecuteCommandsParallel($"{validatorBuildPath.ToQQ()} {cmd}");
        }


        public Dictionary<string, IEnumerable<string>> ExcuteGenerateRadomIdComandsWithResult(string cmd, IEnumerable<string> nodeIds)
        {
            var validatorBuildPath = GlobalVarHandler.UTILS_DIR + "/generate-random-id";
            return new SSHCommandHlp(nodeIds).ExecuteCommandsWithResultParallel($"{validatorBuildPath.ToQQ()} {cmd}");
        }


   
        public Dictionary<string, bool> CreateTonWorkDbStaticDir(IEnumerable<Node> nodes)
        {
            var cmds = new List<string>
            {
                $"rm -rf {GlobalVarHandler.TON_WORK_STATIC_DIR.ToQQ()}",
                $"mkdir -p {GlobalVarHandler.TON_WORK_STATIC_DIR.ToQQ()}",
            };

            return new SSHCommandHlp(nodes).ExecuteCommandsParallel(cmds);
        }

        public Dictionary<string, bool> RebuildTonKeysDir(IEnumerable<string> nodeIds)
        {
            var cmds = new List<string>
            {
                $"rm -rf {GlobalVarHandler.KEYS_DIR.ToQQ()}",
                $"mkdir -p {GlobalVarHandler.KEYS_DIR.ToQQ()}",
                $"mkdir  chown {"0:0".ToQQ()} {GlobalVarHandler.KEYS_DIR.ToQQ()}", // assign owner
                $"chmod 700 {GlobalVarHandler.KEYS_DIR.ToQQ()}",
            };
            return new SSHCommandHlp(nodeIds).ExecuteCommandsParallel(cmds);
        }

        public Dictionary<string, bool> RebuildTonWorkDir(IEnumerable<string> nodeIds)
        {
            var db = $"{GlobalVarHandler.TON_WORK_DIR}/db";
            var etc = $"{GlobalVarHandler.TON_WORK_DIR}/etc";
            var cmds = new List<string>
            {
                $"rm -rf {GlobalVarHandler.TON_WORK_DIR.ToQQ()}",
                $"mkdir -p {GlobalVarHandler.TON_WORK_DIR.ToQQ()}",
                $"mkdir  chown {"0:0".ToQQ()} {GlobalVarHandler.TON_WORK_DIR.ToQQ()}", // assign owner
                $"mkdir -p {db.ToQQ()}",
                $"mkdir -p {etc.ToQQ()}",
            };

            return new SSHCommandHlp(nodeIds).ExecuteCommandsParallel(cmds);
        }

        public Dictionary<string, string> GetPreInitDht(IEnumerable<Node> nodes)
        {

            var rs = DictionaryExt.GetConcurrentDefaulStringDic();
            var tasks = nodes.Select(q => Task.Run(() => {

                var cmd = $"cat {GlobalVarHandler.CONFIGS_DIR}/preinit_{q.Ip}/{q.Ip}-dht";
                var d = new SSHCommandHlp(q).ExecuteCommandsWithResult(cmd).First();
                rs.TryAdd(q.Id, d.Value.First()); 
            }));

            Task.WaitAll(tasks.ToArray());

            return rs.ConvertConcurrentToDic();
        }


        public bool StopValidatorEngine(IEnumerable<string> nodeIds)
        {
            var r = new SSHCommandHlp(nodeIds).ExecuteCommandsAsBashParallel(new string[] { "pkill -f validator-engine" });
            return true;
        }

        public bool RunValidatorEngine(IEnumerable<string> nodeIds)
        {
           // var cmd = $"{GlobalVarHandler.VALIDATOR_ENGINE.ToQQ()} -C {GlobalVarHandler.TON_WORK_GLOBAL_CONFIG_PATH.ToQQ()} --db {GlobalVarHandler.TON_WORK_DB.ToQQ()} > {GlobalVarHandler.TON_WORK_NODE_LOG.ToQQ()} 2>&1 &";
            var cmd = $"{GlobalVarHandler.VALIDATOR_ENGINE.ToQQ()} -C {GlobalVarHandler.TON_WORK_GLOBAL_CONFIG_PATH.ToQQ()} --db {GlobalVarHandler.TON_WORK_DB.ToQQ()} --logname {GlobalVarHandler.TON_WORK_PROCNODE_LOG.ToQQ()} --daemonize | head -c 1G > {GlobalVarHandler.TON_WORK_NODE_LOG.ToQQ()} 2>&1 &";
            var r = new SSHCommandHlp(nodeIds).ExecuteCommandsAsBashParallel(new string[] { "pkill -f validator-engine", cmd });
            return true;
        }

        //public bool GenerateGlobalConfig(IEnumerable<string> nodeIds)
        //{
        //    var cmd = $"{GlobalVarHandler.TON_BUILD_DIR}/validator-engine/validator-engine - C {GlobalVarHandler.TON_WORK_DIR}/etc/ton-global1.config.json --db {GlobalVarHandler.TON_WORK_DIR}/db --ip 192.168.0.31";
        //    var r = new SSHCommandHlp(nodeIds).ExecuteCommandsAsBashParallel(new string[] { cmd });

        //    return true;
        //}

        //public bool BuildConvertAddress()
        //{
        //    var cmds = new List<string>
        //    {
        //        $"cd \"{GlobalVarHandler.NET_TON_DEV_SRC_TOP_DIR}/utils/convert_address\"",
        //        $"cargo update",
        //        $"cargo build --release",
        //        $"cp \"${GlobalVarHandler.NET_TON_DEV_SRC_TOP_DIR}/utils /convert_address/target/release/convert_address\" \"${GlobalVarHandler.UTILS_DIR}/\""
        //    };

        //    new SSHCommandHlp().ExecuteCommandsAsBashParallel(cmds);
        //    return true;
        //}

        public string GetTonGlobalConfig(IEnumerable<Node> nodes, string zerostate0_roothash, string zerostate0_filehash)
        {

            var confParts = GetPreInitDht(nodes).Select(q => q.Value);

            var ton_global_config = new CommonHelprs().GetFiletoStringForTonConfig("ton-global.config.json");
            ton_global_config = ton_global_config.Replace("\"${nodes}\"", string.Join(",", confParts)).Replace("${root_hash}", zerostate0_roothash).Replace("${file_hash}", zerostate0_filehash);

            return ton_global_config;
        }

        public byte[] GetValidatorPubKey(IEnumerable<Node> nodes)
        {
            var rs = DictionaryExt.GetConcurrentDefaultByteaDic();

            var tasks = nodes.Select(q => Task.Run(() => {

                var p = $"{GlobalVarHandler.TON_SRC_DIR}/configs/preinit_{q.Ip}/{q.Ip}-key.pub";
                var d = new SSHCommandHlp(q).DownloadFileToByteaHost(p).First();
                rs.TryAdd(q.Id, d.Value);
            }));

            Task.WaitAll(tasks.ToArray());
            return new CommonHelprs().CombineArray(rs.ConvertConcurrentToDic().Select(q=>q.Value).ToList());
        }

        public byte[] ConfigureZeroState(string tonConfigId, string mainConfigName)
        {
            var TonConfigurationDbSvc = new TonConfigurationDbSvc();
            var config = TonConfigurationDbSvc.GetById(tonConfigId);
            var CommonHelprs = new CommonHelprs();
            var genzerostateConfig = CommonHelprs.GetFiletoStringForFift("gen-zerostate-01")
                .Replace("${GlobalId}", config.GlobalId.ToString())
                .Replace("${DefaultSubwalletId}", config.DefaultSubwalletId)
                .Replace("${StartAt}", config.StartAt)
                .Replace("${ActualMinSplit}", config.ActualMinSplit.ToString())
                .Replace("${MinSplitDepth}", config.MinSplitDepth.ToString())
                .Replace("${MaxSplitDepth}", config.MaxSplitDepth.ToString())
                .Replace("${WorkChainId}", config.WorkChainId.ToString())
                .Replace("${RwalletIinitPubkey}", config.RwalletIinitPubkey)
                .Replace("${StA}", config.StA)
                .Replace("${StB}", config.StB)
                .Replace("${AdvancedWalletAllocatedBalance}", config.AdvancedWalletAllocatedBalance.ToString())
                .Replace("${AdvancedWalletSplitDepth}", config.AdvancedWalletSplitDepth.ToString())
                .Replace("${AdvancedWalletTickTock}", config.AdvancedWalletTickTock.ToString())
                .Replace("${AdvancedWalletAddress}", config.AdvancedWalletAddress)
                .Replace("${AdvancedWalletCreateSetaddr}", config.AdvancedWalletCreateSetaddr.ToString())
                .Replace("${ElectorAllocatedBalance}", config.ElectorAllocatedBalance.ToString())
                .Replace("${ElectorSplitDepth}", config.ElectorSplitDepth.ToString())
                .Replace("${ElectorTickTock}", config.ElectorTickTock.ToString())
                .Replace("${ElectorAddress}", config.ElectorAddress)
                .Replace("${ElectorCreateSetaddr}", config.ElectorCreateSetaddr.ToString())
                .Replace("${MaxValidators}", config.MaxValidators.ToString())
                .Replace("${MaxMainValidators}", config.MaxMainValidators.ToString())
                .Replace("${MinValidators}", config.MinValidators.ToString())
                .Replace("${MinStake}", config.MinStake.ToString())
                .Replace("${MaxStake}", config.MaxStake.ToString())
                .Replace("${MinTotalStake}", config.MinTotalStake.ToString())
                .Replace("${MaxFactor}", config.MaxFactor.ToString())
                .Replace("${ElectedFor}", config.ElectedFor.ToString())
                .Replace("${ElectStartBefore}", config.ElectStartBefore.ToString())
                .Replace("${ElectEndBefore}", config.ElectEndBefore.ToString())
                .Replace("${StakesFrozenFor}", config.StakesFrozenFor.ToString())
                .Replace("${ElectorConfigAddress}", config.ElectorConfigAddress)
                .Replace("${MainWalletTvc_Name}", mainConfigName)
                //
                ;
            return CommonHelprs.GenerateByteaFromString(genzerostateConfig);
        }

        public bool ClearNodeDbData(IEnumerable<string> nodeIds, TonNetwork tonNetwork)
        {

            //var smas = new SmartAccountDbSvc().GetByNodeIdsORTonNetwork(nodeIds.ToArray(), tonNetwork.Id);
            var mainConfigSMa = new SmartAccountDbSvc().GetById(tonNetwork.MainConfigSmartAccountId);
            if (mainConfigSMa != null)
            {

                //var smartAccountIds = smas.Select(q => q.Id).ToArray();
                var smartKeyView = new SmartAccountDbSvc().GetAccountSmartKeys(mainConfigSMa.Id);
                var smartKeysIds = smartKeyView.Select(q => q.SmartKeyId).ToArray();
                new SmartAccountKeyDbSvc().DeleteByIds(smartKeyView.Select(q => q.Id).ToArray());
                new SmartKeyDBSvc().DeleteByIds(smartKeysIds);
            }


            var smartAccountNetworks = new SmartAccountNetworkDbSvc().GetByTonNetworkIds(tonNetwork.Id);
            var nodeMetrics = new NodeMetricDbSvc().GetByNodeIds(nodeIds.ToArray());
            new SmartAccountNetworkDbSvc().DeleteByIds(smartAccountNetworks.Select(q=>q.Id).ToArray());
            new SmartAccountDbSvc().DeleteByIds(tonNetwork.MainConfigSmartAccountId);
            new NodeMetricDbSvc().DeleteByIds(nodeMetrics.Select(q=>q.Id).ToArray());

            return true;
        }

        public bool SetupZeroState(IEnumerable<string> nodeIds, TonNetwork tonNetwork)
        {
            try
            {
                var TonConfigurationDbSvc = new TonConfigurationDbSvc();
                var tonConfig = TonConfigurationDbSvc.GetById(tonNetwork.ConfigurationId);
                var CommonHelprs = new CommonHelprs();
                var SmartContractDbSvc = new SmartContractDbSvc();
                var SmartAccountDbSvc = new SmartAccountDbSvc();
                var NpgSQLHlp = new NpgSQLHlp();
                var FileEntityDbSvc = new FileEntityDbSvc();
                var nodes = new NodeDbSvc().GetByIds(nodeIds.ToArray()).OrderBy(q => q.Ip).ToList();
                var SSHCommandHlpFirst = new SSHCommandHlp(nodes.First());
                var CryptoSvc = new CryptoSvc();

                //var validatorSmartContract = SmartContractDbSvc.GetById(tonNetwork.ValidatorId);
                //var validatorAbiFile = NpgSQLHlp.GetFile(FileEntityDbSvc.GetById(validatorSmartContract.AbiFileId).Oid);
                //var validatorTvcFile = NpgSQLHlp.GetFile(FileEntityDbSvc.GetById(validatorSmartContract.TvcFileId).Oid);
                //var validatorContractName = $"{validatorSmartContract.Name}ValidatorWallet";
                //var validorContractPath = $"{GlobalVarHandler.SMARTCONT_DIR}/{validatorContractName}.tvc";

                var mainwalletSmartAccount = SmartAccountDbSvc.GetById(tonNetwork.MainWalletSmartAccountId);
                var mainwalletSmartContract = SmartContractDbSvc.GetById(mainwalletSmartAccount.SmartContractId);
                var mainwalletAbiFile = NpgSQLHlp.GetFile(FileEntityDbSvc.GetById(mainwalletSmartContract.AbiFileId).Oid);
                var mainwalletTvcFile = NpgSQLHlp.GetFile(FileEntityDbSvc.GetById(mainwalletSmartContract.TvcFileId).Oid);
                var manWalletContractName = $"{mainwalletSmartContract.Name}MainWallet";
                var mainWalletContractPath = $"{GlobalVarHandler.SMARTCONT_DIR}/{manWalletContractName}.tvc";

                //var mainwalletMnemonicPhrase = CryptoSvc.GetMnemonicPhrase();
                //var mainwalletKeyPair = CryptoSvc.GetKeyPair(mainwalletMnemonicPhrase);
                var smartKeyView = new SmartAccountDbSvc().GetAccountSmartKeys(mainwalletSmartAccount.Id).First();
                var mainwalletFinalTvc = new SmartAccountSvc().GenerateSmartAccountSaveAndGetTvc(mainwalletAbiFile, mainwalletTvcFile, smartKeyView.PublicKey, smartKeyView.SecretKey, -1);


                SSHCommandHlpFirst.ExecuteCommandsParallel($"rm -rf {mainWalletContractPath}");
                // SSHCommandHlpFirst.ExecuteCommandsParallel($"rm -rf {validorContractPath}", $"rm -rf {mainWalletContractPath}");
                // SSHCommandHlpFirst.UploadFileToHostParallel(validatorTvcFile, $"{validorContractPath}");
                SSHCommandHlpFirst.UploadFileToHostParallel(mainwalletFinalTvc, $"{mainWalletContractPath}");


                var mainwalletSmartAccountNetwork = new SmartAccountNetwork()
                {
                    Id = Guid.NewGuid().ToString(),
                    SmartAccountId = mainwalletSmartAccount.Id,
                    NetworkId = tonNetwork.Id,
                    Balance = new NanoHlp().ConvertToNanoGram(tonConfig.AdvancedWalletAllocatedBalance),
                    IsDeployed = true,
                    StatusId = "active"
                };

                new SmartAccountNetworkDbSvc().Add(mainwalletSmartAccountNetwork);


                SSHCommandHlpFirst.ExecuteCommandsParallel($"rm -rf {GlobalVarHandler.SMARTCONT_DIR_VALIDATOR_KEYS_PUB}", $"rm -rf {GlobalVarHandler.SMARTCONT_DIR_GEN_ZEROSTATE_FIF}");
                SSHCommandHlpFirst.UploadFileToHostParallel(GetValidatorPubKey(nodes), $"{GlobalVarHandler.SMARTCONT_DIR_VALIDATOR_KEYS_PUB}");
                SSHCommandHlpFirst.UploadFileToHostParallel(ConfigureZeroState(tonNetwork.ConfigurationId, manWalletContractName), $"{GlobalVarHandler.SMARTCONT_DIR_GEN_ZEROSTATE_FIF}");


                //var configContractPath = $"{GlobalVarHandler.SMARTCONT_DIR}/auto";
                var configContractPathSMC = $"{GlobalVarHandler.SMARTCONT_DIR}/auto/config-code.fif";
                var configcodeConfigFif = CommonHelprs.GetFiletoStringForFift("config-code");
                SSHCommandHlpFirst.UploadFileToHostParallel(CommonHelprs.GenerateByteaFromString(configcodeConfigFif), $"{configContractPathSMC}");


                var valodatorsKeys = new List<string>();
                var validators_msig = new List<string>();
                var validators_addrs = new List<string>();



                var tasks = new List<Task>();
                foreach (var n in nodes)
                {

                    var task = Task.Run(() =>
                    {

                    //var index = nodes.IndexOf(n) + 1;
                    //var mnemonicPhrase = CryptoSvc.GetMnemonicPhrase();
                    //var keyPair = CryptoSvc.GetKeyPair(mnemonicPhrase);
                    //var validatorSmartKey = new SmartKey
                    //{
                    //    Id = Guid.NewGuid().ToString(),
                    //    Name = $"validator_key_{n.Ip}",
                    //    MnemonicPhrase = mnemonicPhrase,
                    //    PublicKey = keyPair.PublicKey,
                    //    SecretKey = keyPair.SecretKey,
                    //    TonSafePublicKey = keyPair.TonSafePublicKey,
                    //    TypeId = "validator"
                    //};
                    //new SmartKeyDBSvc().Add(validatorSmartKey);

                    //var validatorSmartAccount = new SmartAccount()
                    //{
                    //    Id = Guid.NewGuid().ToString(),
                    //    Name = $"validator_{n.Name}",
                    //    Description = "-",
                    //    Wc = -1,
                    //    Address = new SmartAccountSvc().GenerateSmartAccountAddress(validatorAbiFile, validatorTvcFile, validatorSmartKey.PublicKey, validatorSmartKey.SecretKey, -1),
                    //    SmartContractId = validatorSmartContract.Id,
                    //    NodeId = n.Id,
                    //    TypeId = "validator",
                    //    CreationDate = DateTime.UtcNow,
                    //    TonNetworkId = tonNetwork.Id
                    //};
                    //    new SmartAccountDbSvc().Add(validatorSmartAccount);

                    //    var SmartAccountNetwork = new SmartAccountNetwork()
                    //    {
                    //        Id = Guid.NewGuid().ToString(),
                    //        SmartAccountId = validatorSmartAccount.Id,
                    //        NetworkId = tonNetwork.Id,
                    //        Balance = 300000000000000,
                    //        StatusId = "active"
                    //    };

                    //    new SmartAccountNetworkDbSvc().Add(SmartAccountNetwork);

                    //    var SmartAccountKey = new SmartAccountKey()
                    //    {
                    //        Id = Guid.NewGuid().ToString(),
                    //        SmartAccountId = validatorSmartAccount.Id,
                    //        SmartKeyId = validatorSmartKey.Id,
                    //        SmartContractId = tonNetwork.ValidatorId
                    //    };

                    //    new SmartAccountKeyDbSvc().Add(SmartAccountKey);


                      

                    //valodatorsKeys.Add(keyPair.PublicKey);
                    //validators_msig.Add($"entry \"validator{index}\" GR${new NanoHlp().ConvertFromNanoGram(SmartAccountNetwork.Balance)} \"{keyPair.PublicKey}\" create-msig");



                    //var tvc = new SmartAccountSvc().GenerateSmartAccountAddressSaveAndGetTvc(validatorAbiFile, validatorTvcFile, keyPair.PublicKey, keyPair.SecretKey, -1, out var addr);
                    //validators_addrs.Add(addr);
                    //var homeAddrPath = $"{GlobalVarHandler.KEYS_DIR}/{n.HostName}.addr";
                    var SSHCommandHlp = new SSHCommandHlp(n);
                    //SSHCommandHlp.ExecuteCommandsParallel($"rm -rf {homeAddrPath}", $"rm -rf {GlobalVarHandler.TON_WORK_NODE_LOG}");
                    //SSHCommandHlp.UploadFileToHostParallel(CommonHelprs.GenerateByteaFromString(addr), $"{homeAddrPath}");



                    //var validorzeroStateTvcPath = $"{GlobalVarHandler.SMARTCONT_DIR}/validator{index}{validatorContractName}.tvc";
                    //SSHCommandHlpFirst.ExecuteCommandsParallel($"rm -rf {validorzeroStateTvcPath}");
                    //SSHCommandHlpFirst.UploadFileToHostParallel(tvc, $"{validorzeroStateTvcPath}");

                    
                    SSHCommandHlp.DeleteCreateDirectory(GlobalVarHandler.TON_WORK_DB_IMPORT);


                    //var/ton-work/db/import

                    });

                    tasks.Add(task);
                    //tvc

                }

                Task.WaitAll(tasks.ToArray());

               // var ValidatorWalletsFif = CommonHelprs.GetFiletoStringForFift("ValidatorWallets");
                //var fullValidators_msig = string.Join("\n", validators_msig);
                //ValidatorWalletsFif = ValidatorWalletsFif.Replace("${validators_msig}", fullValidators_msig).Replace("${validatorContractName_msig}", validatorContractName);
               // var validatorFift = $"{GlobalVarHandler.SMARTCONT_DIR}/ValidatorWallets.fif";

               // SSHCommandHlpFirst.ExecuteCommandsParallel($"rm -rf {validatorFift}");
               // SSHCommandHlpFirst.UploadFileToHostParallel(CommonHelprs.GenerateByteaFromString(ValidatorWalletsFif), $"{validatorFift}");



                //  //-MPEfLplAm9Hg1l-6fjJ

                var zerostate_filehash_data = new string[] { };
                var basestate0_filehash_data = new string[] { };
                var zerostate_roothash_data = new string[] { };

                Gneratezerostate(SSHCommandHlpFirst, nodes.First(), out zerostate_filehash_data, out basestate0_filehash_data, out zerostate_roothash_data, out string configPrivateKey, out string configPublicKey);




                var tonGlobalConfig = GetTonGlobalConfig(nodes, zerostate_roothash_data[5].FixBase64(), zerostate_filehash_data[5].FixBase64());

                SSHCommandHlpFirst.ExecuteCommandsParallel($"rm -rf {GlobalVarHandler.TON_WORK_GLOBAL_CONFIG_PATH}");
                new SSHCommandHlp(nodes).UploadFileToHostParallel(CommonHelprs.GenerateByteaFromString(tonGlobalConfig), GlobalVarHandler.TON_WORK_GLOBAL_CONFIG_PATH);

                var zerostate_hash_file = SSHCommandHlpFirst.DownloadFileToByteaHostParallel($"{GlobalVarHandler.SMARTCONT_DIR}/zerostate.boc").First().Value;
                var base0_hash_file = SSHCommandHlpFirst.DownloadFileToByteaHostParallel($"{GlobalVarHandler.SMARTCONT_DIR}/basestate0.boc").First().Value;




                //upload global config to host




                CreateTonWorkDbStaticDir(nodes);
                new SSHCommandHlp(nodes).UploadFileToHostParallel(zerostate_hash_file, $"{GlobalVarHandler.TON_WORK_STATIC_DIR}/{zerostate_filehash_data[3]}");
                new SSHCommandHlp(nodes).UploadFileToHostParallel(base0_hash_file, $"{GlobalVarHandler.TON_WORK_STATIC_DIR}/{basestate0_filehash_data[3]}");




                var mainConfigTypeId = "system";
                var mainConfigSmartKey = new SmartKey
                {
                    Id = Guid.NewGuid().ToString(),
                    Name = $"MainConfigSK",
                    PublicKey = configPublicKey,
                    SecretKey = configPrivateKey,
                    TonSafePublicKey = CryptoSvc.GetTonSaFeFormat(configPublicKey),
                    TypeId = mainConfigTypeId
                };
                new SmartKeyDBSvc().Add(mainConfigSmartKey);

                //var mainConfigSmartContract = SmartContractDbSvc.GetById(GlobalAppConfHandler.MainConfigFakeMainConfigSCId);
                var mainConfigSmartAccount = new SmartAccount()
                {
                    Id = Guid.NewGuid().ToString(),
                    Name = $"MainConfigSA",
                    Description = "-",
                    Wc = -1,
                    Address = "-1:5555555555555555555555555555555555555555555555555555555555555555",
                    SmartContractId = GlobalAppConfHandler.MainConfigFakeMainConfigSCId,
                    TypeId = mainConfigTypeId,
                    CreationDate = DateTime.UtcNow,
                    TonNetworkId = tonNetwork.Id

                };
                new SmartAccountDbSvc().Add(mainConfigSmartAccount);

                var mainConfigSmartAccountNetwork = new SmartAccountNetwork()
                {
                    Id = Guid.NewGuid().ToString(),
                    SmartAccountId = mainConfigSmartAccount.Id,
                    NetworkId = tonNetwork.Id,
                    Balance = 500000000000,
                    StatusId = "active"
                };

                new SmartAccountNetworkDbSvc().Add(mainConfigSmartAccountNetwork);

                var mainConfigSmartAccountKey = new SmartAccountKey()
                {
                    Id = Guid.NewGuid().ToString(),
                    SmartAccountId = mainConfigSmartAccount.Id,
                    SmartKeyId = mainConfigSmartKey.Id,
                    SmartContractId = GlobalAppConfHandler.MainConfigFakeMainConfigSCId
                };

                new SmartAccountKeyDbSvc().Add(mainConfigSmartAccountKey);

                var TonNetworkDbSvc = new TonNetworkDbSvc();
                tonNetwork.MainConfigSmartAccountId = mainConfigSmartAccount.Id;
                tonNetwork.MainWalletSmartAccountId = mainwalletSmartAccount.Id;
                TonNetworkDbSvc.Update(tonNetwork);

                return true;
            }
            catch (Exception ex)
            { 
            
            }

            return true;

        }

        public bool ProccessMainConfigKeys(SSHCommandHlp SSHCommandHlpFirst, TonNetwork tonNetwork)
        {
            var keyFile = SSHCommandHlpFirst.DownloadFileToByteaHostParallel(GlobalVarHandler.SMARTCONT_DIR_MAIN_WALLET_PK);
            var privateKey = BitConverter.ToString(keyFile.First().Value).Replace("-", "").ToLower();
           // var tonSafePublic = new CryptoSvc().GetTonSaFeFormat("publicKey");
            var mainConfigSmartKey = new SmartKey
            {
                Id = Guid.NewGuid().ToString(),
                Name = $"mainConfig_key_{tonNetwork.Id}",
                MnemonicPhrase = null,
                PublicKey = null,
                SecretKey = privateKey,
                TonSafePublicKey = "",
                TypeId = "system"
            };
            new SmartKeyDBSvc().Add(mainConfigSmartKey);

            var mainConfigSmartAccount = new SmartAccount()
            {
                Id = Guid.NewGuid().ToString(),
                Name = $"mainConfig_{tonNetwork.Id}",
                Description = "-",
                Wc = -1,
                Address = "-1:1111111111111111111111111111111111111111111111111111111111111111",
                SmartContractId = tonNetwork.ValidatorId,
                TypeId = "system",
                CreationDate = DateTime.UtcNow
            };
            new SmartAccountDbSvc().Add(mainConfigSmartAccount);

            var mainConfigSmartAccountNetwork = new SmartAccountNetwork()
            {
                Id = Guid.NewGuid().ToString(),
                SmartAccountId = mainConfigSmartAccount.Id,
                NetworkId = tonNetwork.Id,
                Balance = 0,
                StatusId = "active"
            };

            new SmartAccountNetworkDbSvc().Add(mainConfigSmartAccountNetwork);

            var SmartAccountKey = new SmartAccountKey()
            {
                Id = Guid.NewGuid().ToString(),
                SmartAccountId = mainConfigSmartAccount.Id,
                SmartKeyId = mainConfigSmartKey.Id,
                SmartContractId = tonNetwork.ContractId
            };

            new SmartAccountKeyDbSvc().Add(SmartAccountKey);


            return true;
        }

        public bool ProccessMainWalletKeys(SSHCommandHlp SSHCommandHlpFirst, TonNetwork tonNetwork)
        {
            var keyFile = SSHCommandHlpFirst.DownloadFileToByteaHostParallel(GlobalVarHandler.SMARTCONT_DIR_MAIN_WALLET_PK);
            var privateKey = BitConverter.ToString(keyFile.First().Value).Replace("-","").ToLower();
            // var tonSafePublic = new CryptoSvc().GetTonSaFeFormat("publicKey");
            var mainWalletSmartKey = new SmartKey
            {
                Id = Guid.NewGuid().ToString(),
                Name = $"mainWallet_key_{tonNetwork.Id}",
                MnemonicPhrase = null,
                PublicKey = null,
                SecretKey = privateKey,
                TonSafePublicKey = "",
                TypeId = "system"
            };
            new SmartKeyDBSvc().Add(mainWalletSmartKey);

            var mainWalletSmartAccount = new SmartAccount()
            {
                Id = Guid.NewGuid().ToString(),
                Name = $"mainWallet_{tonNetwork.Id}",
                Description = "-",
                Wc = -1,
                Address = "-1:1111111111111111111111111111111111111111111111111111111111111111",
                SmartContractId = tonNetwork.ValidatorId,
                TypeId = "system",
                CreationDate = DateTime.UtcNow
            };
            new SmartAccountDbSvc().Add(mainWalletSmartAccount);

            var mainWalletSmartAccountNetwork = new SmartAccountNetwork()
            {
                Id = Guid.NewGuid().ToString(),
                SmartAccountId = mainWalletSmartAccount.Id,
                NetworkId = tonNetwork.Id,
                Balance = 0,
                StatusId = "active"
            };

            new SmartAccountNetworkDbSvc().Add(mainWalletSmartAccountNetwork);

            var SmartAccountKey = new SmartAccountKey()
            {
                Id = Guid.NewGuid().ToString(),
                SmartAccountId = mainWalletSmartAccount.Id,
                SmartKeyId = mainWalletSmartKey.Id,
                SmartContractId = tonNetwork.ContractId
            };

            new SmartAccountKeyDbSvc().Add(SmartAccountKey);


            return true; 
        }

        public void Gneratezerostate(SSHCommandHlp SSHCommandHlpFirst, Node n, out string[] zerostate_filehash_data, out string[] basestate0_filehash_data, out string[] zerostate_roothash_data, out string configPrivateKey, out string configPublicKey)
        {

            var dumplogPath = $"{GlobalVarHandler.SMARTCONT_DIR}/dump.log";
            var s = $"{GlobalVarHandler.SMARTCONT_DIR}/gen-zerostate.fif";
            new FiftSvc().ExecuteCreateZeroState($"{s.ToQQ()} > {dumplogPath}", new string[] { n.Id });

            var initConfigBytea = SSHCommandHlpFirst.DownloadFileToByteaHostParallel(dumplogPath);
            string dump_log = Encoding.Default.GetString(initConfigBytea.First().Value);
            var dump_log_split = dump_log.Split("\n");

            zerostate_filehash_data = dump_log_split.Where(q => q.Contains("Zerostate file hash=")).First().Split(" ");
            basestate0_filehash_data = dump_log_split.Where(q => q.Contains("Basestate0 file hash=")).First().Split(" ");
            zerostate_roothash_data = dump_log_split.Where(q => q.Contains("Zerostate root hash=")).First().Split(" ");
            configPublicKey = dump_log_split.Where(q => q.Contains("config public key =")).First().Split(" ")[4].ToLower();
            configPrivateKey = dump_log_split.Where(q => q.Contains("config private key =")).First().Split(" ")[4].ToLower();
        }

        //public void DonwloadFileFromHostToTempDir(SSHAuthMdl auth, string filePath)
        //{
        //    CreateLocalTempDir();
        //    var fileName = Path.GetFileName(filePath);
        //    using (var sftp = new SftpClient(GetConnectionInfo(auth)))
        //    {
        //        sftp.Connect();
        //        using (Stream fileStream = File.Create($"{GlobalVarHandler.LOCAL_TEMP_DIR}\\{fileName}"))
        //        {
        //            sftp.DownloadFile(filePath, fileStream);
        //        }
        //        sftp.Disconnect();
        //    }
        //}

    }
}
