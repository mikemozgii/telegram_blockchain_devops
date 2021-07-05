using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TONBRAINS.TONOPS.Core.Handlers;

namespace TONBRAINS.TONOPS.Core.Services
{
    public class LocalTempFileSvc
    {

        public string GetTempFileName(string suffix = null)
        {
            var r = $"{Guid.NewGuid()}";
            if (!string.IsNullOrWhiteSpace(suffix))
            {
                r = $"{r}{suffix}";
            }

           return r;
        }

        //public string GetRandomOutputPath(string suffix = null)
        //{
        //    return $"{GetFilePath(GetTempFileName())}{suffix}";
        //}

        public string GetRandomOutputPath(string fileName)
        {
            return $"{GetFilePath(fileName)}";
        }

        public string GetFilePath(string fileName)
        {
            return $"{GlobalVarHandler.LOCALHOST_TEMP_DIR}\\{fileName}";
        }

        public string AddTempFile(byte[] bytea, string fileName)
        {
            CreateLocalTempDir();
            var fullPath = GetFilePath(fileName);
            File.WriteAllBytes(fullPath, bytea);
            return fullPath;
        }

        public string AddTempFile(string content, string fileName)
        {
            var fullPath = GetFilePath(fileName);
            CreateLocalTempDir();
            File.WriteAllText(fullPath, content);
            return fullPath;
        }

        public void DeleteTempFileByName(string fileName)
        {
            DeleteTempFileByPath(GetFilePath(fileName));
        }

        public void DeleteTempFileByPath(string fullPath)
        {
            File.Delete(fullPath);
        }


        public byte[] GetFiletoByteyPath(string fullPath)
        {
            var bytea = File.ReadAllBytes(fullPath);
            return bytea;
        }
        public byte[] GetFiletoByteByName(string fileName)
        {
            return GetFiletoByteyPath(GetFilePath(fileName));
        }


        //public string CreateTempKeyFile(string publickey, string secretkey)
        //{
        //    var fileName = GenerateKeysFile();

        //    
        //    return fileName;
        //}

        public void DeleteLocalTempDir()
        {
            File.Delete(GlobalVarHandler.LOCALHOST_TEMP_DIR);
        }

        public void CreateLocalTempDir()
        {
            if (!Directory.Exists(GlobalVarHandler.LOCALHOST_TEMP_DIR))
            {
                System.IO.Directory.CreateDirectory(GlobalVarHandler.LOCALHOST_TEMP_DIR);
            }
        }
    }
}
