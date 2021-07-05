using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TONBRAINS.TONOPS.Core.Extensions;
using TONBRAINS.TONOPS.Core.Helpers;
using TONBRAINS.TONOPS.Ffi;
using TONBRAINS.TONOPS.Core.SSH;
using TONBRAINS.TONOPS.Core.Handlers;
using TONBRAINS.TONOPS.Core.DAL;
using Newtonsoft.Json;
using TONBRAINS.TONOPS.Core.Models.AbiMdls;
using TONBRAINS.TONOPS.Core.Models;
using System.Threading;
using TONBRAINS.TONOPS.WebApp.WebApp.Helpers;
using Quartz;
using TONBRAINS.TONOPS.Core.QuartzJobs;
using TONBRAINS.TONOPS.Core.Enums;

namespace TONBRAINS.TONOPS.Core.Services
{
    public class SmartAccountSvc
    {
        private IScheduler _scheduler { get; set; }
        public SmartAccountSvc()
        {

        }
        public SmartAccountSvc(IScheduler scheduler)
        {
            _scheduler = scheduler;
        }

        //public string GenerateAddress()
        //{

        //    var TonWorkLib = new TonWorkLib();
        //    var c = new CommonHelprs();
        //    var TempFileSvc = new LocalTempFileSvc();
        //    //var abiStream = c.GetStringFromNBytea(TempFileSvc.GetFiletoByteyPath("C:\\macpro_backup\\SmartContractTest\\wallet.abi.json"));
        //    //var tvcStream = TempFileSvc.GetFiletoByteyPath("C:\\macpro_backup\\SmartContractTest\\wallet.tvc");

        //    var abiStream = c.GetStringFromNBytea(TempFileSvc.GetFiletoByteyPath("C:\\macpro_backup\\tonos-cli\\tonos-cli\\tests\\samples\\wallet.abi.json"));
        //    var tvcStream = TempFileSvc.GetFiletoByteyPath("C:\\macpro_backup\\tonos-cli\\tonos-cli\\tests\\samples\\wallet.tvc");

        //    var ValidPublicKey = "16fb1cfd72000a4393d684c3a13662e5ac137a1a04d193814e8c05127b28166b";


        //    var address = TonWorkLib.GenerateAddress(tvcStream, tvcStream.Length, 0, ValidPublicKey, "{}", abiStream);

        //    return address;
        //}


        public string GenerateSmartAccountAddress(string scId, string publicKey, string secretKey, int wc)
        {
            var contract = new SmartContractDbSvc().GetById(scId);
            var fileSvc = new FileSvc();
            var abi = fileSvc.GetFile(contract.AbiFileId);
            var tvc = fileSvc.GetFile(contract.TvcFileId);
            return GenerateSmartAccountAddress(abi, tvc, publicKey, secretKey, wc);
        }

        public string GenerateSmartAccountAddress(byte[] abi, byte[] tvc, string publicKey, string secretKey, int wc)
        {
            //var TempFileSvc = new LocalTempFileSvc();
            //var TLTonCli = new TLTonCli();
            //var fileName = TempFileSvc.GetTempFileName();
            //var keysFileContent = "{" + $"{Environment.NewLine}\"public\": \"{publicKey}\",{Environment.NewLine}\"secret\": \"{secretKey}\"{Environment.NewLine}" + "}";
            //var abiFullPath = TempFileSvc.AddTempFile(abiStream, fileName.ToAbiFileName());
            //var tvcFullPath = TempFileSvc.AddTempFile(tvcStream, fileName.ToTvcFileName());
            //var keysFullPath = TempFileSvc.AddTempFile(keysFileContent, fileName.ToKeysFileName());

            //var address = TLTonCli.GenerateAddress(abiFullPath, tvcFullPath, keysFullPath, wc);

            //TempFileSvc.DeleteTempFileByName(fileName.ToAbiFileName());
            //TempFileSvc.DeleteTempFileByName(fileName.ToTvcFileName());
            //TempFileSvc.DeleteTempFileByName(fileName.ToKeysFileName());
            var address = new TonWorkLib().GenerateAddress(tvc, tvc.Length, wc, publicKey, "{}", new CommonHelprs().GetStringFromNBytea(abi));
            return address;
        }

        public byte[] GenerateSmartAccountAddressSaveAndGetTvc(byte[] abiStream, byte[] tvcStream, string publicKey, string secretKey, int wc, out string address)
        {
            var TempFileSvc = new LocalTempFileSvc();
            var TLTonCli = new TLTonCli();
            var fileName = TempFileSvc.GetTempFileName();
            var keysFileContent = "{" + $"{Environment.NewLine}\"public\": \"{publicKey}\",{Environment.NewLine}\"secret\": \"{secretKey}\"{Environment.NewLine}" + "}";
            var abiFullPath = TempFileSvc.AddTempFile(abiStream, fileName.ToAbiFileName());
            var tvcFullPath = TempFileSvc.AddTempFile(tvcStream, fileName.ToTvcFileName());
            var keysFullPath = TempFileSvc.AddTempFile(keysFileContent, fileName.ToKeysFileName());

            address = TLTonCli.GenerateAddressSave(abiFullPath, tvcFullPath, keysFullPath, wc);

            var getTvcFileBytea = TempFileSvc.GetFiletoByteByName(fileName.ToTvcFileName());

            TempFileSvc.DeleteTempFileByName(fileName.ToAbiFileName());
            TempFileSvc.DeleteTempFileByName(fileName.ToTvcFileName());
            TempFileSvc.DeleteTempFileByName(fileName.ToKeysFileName());


            return getTvcFileBytea;
        }

        public byte[] GenerateSmartAccountSaveAndGetTvc(byte[] abiStream, byte[] tvcStream, string publicKey, string secretKey, int wc)
        {
            var TempFileSvc = new LocalTempFileSvc();
            var TLTonCli = new TLTonCli();
            var fileName = TempFileSvc.GetTempFileName();
            var keysFileContent = "{" + $"{Environment.NewLine}\"public\": \"{publicKey}\",{Environment.NewLine}\"secret\": \"{secretKey}\"{Environment.NewLine}" + "}";
            var abiFullPath = TempFileSvc.AddTempFile(abiStream, fileName.ToAbiFileName());
            var tvcFullPath = TempFileSvc.AddTempFile(tvcStream, fileName.ToTvcFileName());
            var keysFullPath = TempFileSvc.AddTempFile(keysFileContent, fileName.ToKeysFileName());

            var r = TLTonCli.GenerateSave(abiFullPath, tvcFullPath, keysFullPath, wc);

            var getTvcFileBytea = TempFileSvc.GetFiletoByteByName(fileName.ToTvcFileName());

            TempFileSvc.DeleteTempFileByName(fileName.ToAbiFileName());
            TempFileSvc.DeleteTempFileByName(fileName.ToTvcFileName());
            TempFileSvc.DeleteTempFileByName(fileName.ToKeysFileName());


            return getTvcFileBytea;
        }

        //public string GenerateMessage()
        //{

        //    var TonWorkLib = new TonWorkLib();
        //    var c = new CommonHelprs();
        //    var TempFileSvc = new LocalTempFileSvc();
        //    var abiStream = c.GetStringFromNBytea(TempFileSvc.GetFiletoByteyPath("C:\\macpro_backup\\SmartContractTest\\wallet.abi.json"));
        //    var tvcStream = TempFileSvc.GetFiletoByteyPath("C:\\macpro_backup\\SmartContractTest\\wallet.tvc");
        //    var ValidPublicKey = "16fb1cfd72000a4393d684c3a13662e5ac137a1a04d193814e8c05127b28166b";
        //    var ValidSercretKey = "7b03855bc68f1afea36ac477f4c3c6eb52ba8cf90b27683d4dfd6401104cab99";
        //    var keysFileContent = "{" + $"{Environment.NewLine}\"public\": \"{ValidPublicKey}\",{Environment.NewLine}\"secret\": \"{ValidSercretKey}\"{Environment.NewLine}" + "}";
        //    var addr = "-1:56973312e8e0ce7dc5760db964758ca2b30ac078a776871065faa9c86760c510";
        //    var address = TonWorkLib.GenerateMessageBoc(addr, abiStream, "constructor", "{}", keysFileContent, 3600, tvcStream, tvcStream.Length);

        //    return null;
        //}

        //public byte[] GenerateBocMessage(string address, string action, string prams, byte[] abi, string publicKey, string secretKey)
        //{
        //    //var TempFileSvc = new LocalTempFileSvc();
        //    //var TLTonCli = new TLTonCli();
        //    //var fileName = TempFileSvc.GetTempFileName();
        //    var keysFileContent = "{" + $"{Environment.NewLine}\"public\": \"{publicKey}\",{Environment.NewLine}\"secret\": \"{secretKey}\"{Environment.NewLine}" + "}";
        //    //var abiFullPath = TempFileSvc.AddTempFile(abi, fileName.ToAbiFileName());
        //    //var keysFullPath = TempFileSvc.AddTempFile(keysFileContent, fileName.ToKeysFileName());

        //    //var bocFileOutputPath = TempFileSvc.GetRandomOutputPath(fileName.ToBocFileName());
        //    //var message = TLTonCli.GenerateBocMsg(address, action, prams, abiFullPath, keysFullPath, bocFileOutputPath);

        //    //var bocBytea = TempFileSvc.GetFiletoByteyPath(bocFileOutputPath);

        //    //TempFileSvc.DeleteTempFileByName(fileName.ToAbiFileName());
        //    //TempFileSvc.DeleteTempFileByName(fileName.ToKeysFileName());
        //    //TempFileSvc.DeleteTempFileByName(fileName.ToBocFileName());


        //    var bocBytea = new TonWorkLib().GenerateMessageBoc(address, new CommonHelprs().GetStringFromNBytea(abi), action, prams, keysFileContent);

        //    return bocBytea;
        //}




        public SmartKey CreateSmartKey(string mnemonicPhrase)
        {
            var cryptoSvc = new CryptoSvc();
            var keyPair = cryptoSvc.GetKeyPair(mnemonicPhrase);
            var r = new SmartKeyDBSvc().GetBySecretAndPublicKey(keyPair.SecretKey, keyPair.SecretKey);

            if (r != null)
            {
                return r;
            }

            var id = Guid.NewGuid().ToString();
         
            var smartKey = new SmartKey
            {
                Id = id,
                Name = $"{id}SK",
                PublicKey = keyPair.PublicKey,
                SecretKey = keyPair.SecretKey,
                TonSafePublicKey = cryptoSvc.GetTonSaFeFormat(keyPair.PublicKey),
                MnemonicPhrase = mnemonicPhrase,
                TypeId = SmartAssetTypes.common
            };
            new SmartKeyDBSvc().Add(smartKey);

            return new SmartKeyDBSvc().GetById(id);
        }

        public SmartKey CreateSmartKey(string secretKey, string publicKey)
        {
            var cryptoSvc = new CryptoSvc();
            var r = new SmartKeyDBSvc().GetBySecretAndPublicKey(secretKey, publicKey);

            if (r != null)
            {
                return r;
            }

            var id = Guid.NewGuid().ToString();

            var smartKey = new SmartKey
            {
                Id = id,
                Name = $"{id}SK",
                PublicKey = publicKey,
                SecretKey = secretKey,
                TonSafePublicKey = cryptoSvc.GetTonSaFeFormat(publicKey),
                MnemonicPhrase = null,
                TypeId = SmartAssetTypes.common
            };
            new SmartKeyDBSvc().Add(smartKey);

            return new SmartKeyDBSvc().GetById(id);
        }




        public Dictionary<string, SmartAccount> InitSmartAccount(string scId, int ws, string skId = null, string customId = null, params string[] tonNetworkIds)
        {

            var rs = new Dictionary<string, SmartAccount>();
            SmartKey smartKey;
            var id = Guid.NewGuid().ToString();
            var finaId = !string.IsNullOrWhiteSpace(customId) ? customId : id;
            if (skId != null)
            {
                smartKey = new SmartKeyDBSvc().GetById(skId);
                if (smartKey == null) return rs;
            }
            else
            {
                var cryptoSvc = new CryptoSvc();
                var mnemonicPhrase = cryptoSvc.GetMnemonicPhrase();
                var keyPair = cryptoSvc.GetKeyPair(mnemonicPhrase);

      
                var newSmartKey = new SmartKey
                {
                    Id = id,
                    Name = $"{finaId}SK",
                    PublicKey = keyPair.PublicKey,
                    SecretKey = keyPair.SecretKey,
                    TonSafePublicKey = cryptoSvc.GetTonSaFeFormat(keyPair.PublicKey),
                    MnemonicPhrase = mnemonicPhrase,
                    TypeId = SmartAssetTypes.common
                };

                new SmartKeyDBSvc().Add(newSmartKey);
                smartKey = new SmartKeyDBSvc().GetById(newSmartKey.Id);
            }


            var sa = new SmartAccountDbSvc().GetAccounts_BySecretAndPublicKey(smartKey.SecretKey, smartKey.PublicKey);

            if (sa != null)
            {
                //var smartAccount = new SmartAccountDbSvc().GetById(sa.SmartAccountId);
                var nets = new SmartAccountNetworkDbSvc().GetBySmartAccountIds(new string[] { sa.SmartAccountId });
                foreach (var tonNetworkId in tonNetworkIds)
                {

                    var net = nets.FirstOrDefault(q => q.NetworkId == tonNetworkId);

                    if (net == null)
                    {
                        var newSmartAccountNetwork = new SmartAccountNetwork()
                        {
                            Id = id,
                            SmartAccountId = sa.SmartAccountId,
                            NetworkId = tonNetworkId,
                            Balance = 0,
                            StatusId = SmartAccountStatuses.undefined
                        };

                        new SmartAccountNetworkDbSvc().Add(newSmartAccountNetwork);
                        rs.Add(tonNetworkId, new SmartAccountDbSvc().GetById(sa.SmartAccountId));
                    }
                    else
                    {
                        rs.Add(tonNetworkId, new SmartAccountDbSvc().GetById(sa.SmartAccountId));
                    }


                }
            }
            else
            {

                var address = GenerateSmartAccountAddress(scId, smartKey.PublicKey, smartKey.SecretKey, ws);

                if (string.IsNullOrWhiteSpace(address))
                {
                    return rs;
                }


                var newSmartAccount = new SmartAccount()
                {
                    Id = id,
                    Name = $"{finaId}SA",
                    Description = "-",
                    Wc = address.GetWCByAddress(),
                    Address = address,
                    SmartContractId = scId,
                    TypeId = SmartAssetTypes.common,
                    CreationDate = DateTime.UtcNow,

                };

                var newSmartAccountKey = new SmartAccountKey()
                {
                    Id = Guid.NewGuid().ToString(),
                    SmartAccountId = newSmartAccount.Id,
                    SmartKeyId = smartKey.Id,
                    SmartContractId = scId
                };

                new SmartAccountKeyDbSvc().Add(newSmartAccountKey);

                new SmartAccountDbSvc().Add(newSmartAccount);

                foreach (var tonNetworkId in tonNetworkIds)
                {
                   
                    var newSmartAccountNetwork = new SmartAccountNetwork()
                    {
                        Id = id,
                        SmartAccountId = newSmartAccount.Id,
                        NetworkId = tonNetworkId,
                        Balance = 0,
                        StatusId = SmartAccountStatuses.undefined
                    };

                    new SmartAccountNetworkDbSvc().Add(newSmartAccountNetwork);
                    rs.Add(tonNetworkId, new SmartAccountDbSvc().GetById(newSmartAccount.Id));
                }
            }


 
            return rs;
        }








    }
}
