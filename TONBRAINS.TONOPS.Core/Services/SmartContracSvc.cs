using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TONBRAINS.TONOPS.Core.DAL;
using TONBRAINS.TONOPS.WebApp.WebApp.Helpers;

namespace TONBRAINS.TONOPS.Core.Services
{
    public class SmartContracSvc
    {
        public bool Add(byte[] abi, byte[] sol, byte[] tvc,  SmartContract sc)
        {

            var r = new SmartContractDbSvc().GetByName(sc.Name);
            if (r != null)
                throw new Exception("Smart Contract already exist with such name");

            sc.Id = IdGenerator.Generate();

            var fileSvc = new FileSvc();
            sc.AbiFileId = IdGenerator.Generate();
            sc.SolFileId = IdGenerator.Generate();
            sc.TvcFileId = IdGenerator.Generate();


            var uploadAbi = fileSvc.UploadFile(sc.AbiFileId, abi, $"{sc.Name}.abi.json");
            var uploadSol = fileSvc.UploadFile(sc.SolFileId, sol, $"{sc.Name}.sol");
            var uploadTvc = fileSvc.UploadFile(sc.TvcFileId, tvc, $"{sc.Name}.tvc");
            
            sc.AbiJson = Encoding.UTF8.GetString(abi);
            sc.SolJson = Encoding.UTF8.GetString(sol);

            new SmartContractDbSvc().Add(sc);
            return true;
        }
    }
}
