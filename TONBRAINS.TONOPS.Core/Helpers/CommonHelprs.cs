using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using TONBRAINS.TONOPS.Core.SSH;

namespace TONBRAINS.TONOPS.Core.Handlers
{
    public class CommonHelprs
    {
        public string GetEnvProjectDirecotry()
        {
#if DEBUG 
            return Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
#endif
#if !DEBUG
            return Environment.CurrentDirectory;
#endif
        }

        public string GetBashContentFromFile(string fileName)
        {
            var filename = Path.Combine(GetEnvProjectDirecotry(), "Bashes", $"{fileName}.sh");
            //Dos2Unix(filename);
            var content = File.ReadAllText(filename);
            Debug.WriteLine(content);
            return content;
        }

        public byte[] GetBashByteaContentFromFile(string fileName)
        {
            var filePath = Path.Combine(GetEnvProjectDirecotry(), "Bashes", $"{fileName}.sh");
            var bytea = File.ReadAllBytes(filePath);
            return bytea;
        }

        private void Dos2Unix(string fileName)
        {
            const byte CR = 0x0D;
            const byte LF = 0x0A;
            byte[] data = File.ReadAllBytes(fileName);
            using (FileStream fileStream = File.OpenWrite(fileName))
            {
                BinaryWriter bw = new BinaryWriter(fileStream);
                int position = 0;
                int index = 0;
                do
                {
                    index = Array.IndexOf<byte>(data, CR, position);
                    if ((index >= 0) && (data[index + 1] == LF))
                    {
                        // Write before the CR
                        bw.Write(data, position, index - position);
                        // from LF
                        position = index + 1;
                    }
                }
                while (index >= 0);
                bw.Write(data, position, data.Length - position);
                fileStream.SetLength(fileStream.Position);
            }
        }


        public string GetConfigContentFromFile(string fileName)
        {
            return File.ReadAllText($"{GetEnvProjectDirecotry()}/configs/{fileName}"); ;
        }

        public Stream GenerateStreamFromString(string content)
        {
            var stream = new MemoryStream();
            var writer = new StreamWriter(stream);
            writer.Write(content);
            writer.Flush();
            stream.Position = 0;
            return stream;
        }

        public byte[] GenerateByteaFromString(string content)
        {
            return Encoding.ASCII.GetBytes(content);
        }

        public Stream GenerateStreamFromBash(string fileName)
        {
            var content = GetBashContentFromFile(fileName);
            return GenerateStreamFromString(content);
        }

        public byte[] GetFiletoByteForTonConfig(string fileName)
        {
            var filePath = Path.Combine(GetEnvProjectDirecotry(), "TonConfigs", $"{fileName}");
            var bytea = File.ReadAllBytes(filePath);
            return bytea;
        }

        public string GetFiletoStringForTonConfig(string fileName)
        {
            var filePath = Path.Combine(GetEnvProjectDirecotry(), "TonConfigs", $"{fileName}");
            var bytea = File.ReadAllBytes(filePath);
            var str = Encoding.Default.GetString(bytea);
            return str;
        }

        public string GetStringFromNBytea(byte[] content)
        {
            var str = Encoding.Default.GetString(content);
            return str;
        }

        public byte[] GetFiletoByteForFiftConfig(string fileName)
        {
            var filePath = Path.Combine(GetEnvProjectDirecotry(), "Fift", $"{fileName}.fif");
            var bytea = File.ReadAllBytes(filePath);
            return bytea;
        }

        public string GetFiletoStringForFift(string fileName)
        {
            var filePath = Path.Combine(GetEnvProjectDirecotry(), "Fift", $"{fileName}.fif");
            var bytea = File.ReadAllBytes(filePath);
            var str = Encoding.Default.GetString(bytea);
            return str;
        }

        public byte[] GetFiletoByteForSmartContract(string fileName)
        {
            var filePath = Path.Combine(GetEnvProjectDirecotry(), "SmartContracts", $"{fileName}");
            var bytea = File.ReadAllBytes(filePath);
            return bytea;
        }

        public string GetFiletoStringForSmartContract(string fileName)
        {
            var filePath = Path.Combine(GetEnvProjectDirecotry(), "SmartContracts", $"{fileName}");
            var bytea = File.ReadAllBytes(filePath);
            var str = Encoding.Default.GetString(bytea);
            return str;
        }

        public List<SSHAuthMdl> ToAuthSSHAuth(IEnumerable<string> hosts)
        {
            var rs = new List<SSHAuthMdl>();

            foreach (var h in hosts)
            {
                rs.Add(new SSHAuthMdl(h));
            }
            return rs;
        }

        public List<SSHAuthMdl> ToAuthSSHAuth(IEnumerable<string> hosts, string user, string pass)
        {
            var rs = new List<SSHAuthMdl>();

            foreach (var h in hosts)
            {
                rs.Add(new SSHAuthMdl(h, user, pass));
            }
            return rs;
        }

        public List<SSHAuthMdl> ToAuthSSHAuth(params string[] hosts)
        {
            var hs = hosts.ToList();

            return ToAuthSSHAuth(hs);
        }

        public List<SSHAuthMdl> ToAuthSSHAuth(string user, string pass, params string[] hosts)
        {
            var hs = hosts.ToList();

            return ToAuthSSHAuth(hs, user, pass);
        }

        public string ProcessResult(string input)
        {

            return input.TrimEnd('\r', '\n');
        }

        public byte[] CombineArray(IEnumerable<byte[]> arrays)
        {
            byte[] rv = new byte[arrays.Sum(a => a.Length)];
            int offset = 0;
            foreach (byte[] array in arrays)
            {
                System.Buffer.BlockCopy(array, 0, rv, offset, array.Length);
                offset += array.Length;
            }
            return rv;
        }

        public byte[] ReadToEnd(System.IO.Stream stream)
        {
            long originalPosition = 0;

            if (stream.CanSeek)
            {
                originalPosition = stream.Position;
                stream.Position = 0;
            }

            try
            {
                byte[] readBuffer = new byte[32];

                int totalBytesRead = 0;
                int bytesRead;

                while ((bytesRead = stream.Read(readBuffer, totalBytesRead, readBuffer.Length - totalBytesRead)) > 0)
                {
                    totalBytesRead += bytesRead;

                    if (totalBytesRead == readBuffer.Length)
                    {
                        int nextByte = stream.ReadByte();
                        if (nextByte != -1)
                        {
                            byte[] temp = new byte[readBuffer.Length * 2];
                            Buffer.BlockCopy(readBuffer, 0, temp, 0, readBuffer.Length);
                            Buffer.SetByte(temp, totalBytesRead, (byte)nextByte);
                            readBuffer = temp;
                            totalBytesRead++;
                        }
                    }
                }

                byte[] buffer = readBuffer;
                if (readBuffer.Length != totalBytesRead)
                {
                    buffer = new byte[totalBytesRead];
                    Buffer.BlockCopy(readBuffer, 0, buffer, 0, totalBytesRead);
                }
                return buffer;
            }
            finally
            {
                if (stream.CanSeek)
                {
                    stream.Position = originalPosition;
                }
            }
        }



    }
}
