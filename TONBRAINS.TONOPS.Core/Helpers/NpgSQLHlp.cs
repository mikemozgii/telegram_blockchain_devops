using Npgsql;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TONBRAINS.TONOPS.Core.Handlers;

namespace TONBRAINS.TONOPS.Core.Helpers
{
    public class NpgSQLHlp
    {
        public byte[] GetFile(uint oid)
        {
            using (var connection = (NpgsqlConnection)OpenDbConnection())
            {
                var fileManager = new NpgsqlLargeObjectManager(connection);
                using (connection.BeginTransaction())
                using (var memoryStream = new MemoryStream())
                using (var dbStream = fileManager.OpenRead(oid))
                {
                    var buffer = new byte[262144];

                    while (memoryStream.Position < dbStream.Length)
                    {
                        var count =  dbStream.Read(buffer, 0, buffer.Length);
                        memoryStream.Write(buffer, 0, count);
                    }

                    memoryStream.Position = 0;
                    return memoryStream.ToArray();
                }
            }
        }


        public IDbConnection OpenNpgsqlConnection(string connStr)
        {
            var conn = new NpgsqlConnection(connStr);
            conn.Open();
            return conn;
        }

        public IDbConnection OpenDbConnection()
        {
            var connectionString = GlobalAppConfHandler.TonOpsDbConnectionString;
            return OpenNpgsqlConnection(connectionString);
        }
    }
}
