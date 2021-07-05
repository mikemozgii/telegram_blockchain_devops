using Microsoft.EntityFrameworkCore;
using Npgsql;
using NpgsqlTypes;
using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TONBRAINS.TONOPS.Core.DAL;
using TONBRAINS.TONOPS.Core.Handlers;
using TONBRAINS.TONOPS.Core.Helpers;
using TONBRAINS.TONOPS.WebApp.WebApp.Helpers;

namespace TONBRAINS.TONOPS.Core.Services
{
    public class FileSvc
    {
        private TonOpsDbContext _context { get; set; }

        public FileSvc()
        {
            _context = new TonOpsDbContext(GlobalAppConfHandler.GetTonOpsDbContextOption());
        }

        /// <summary>
        /// Get File
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<byte[]> GetFileAsync(string id)
        {
            var result = await _context.Files.FirstOrDefaultAsync(q => q.Id == id);
            if (result == null) throw new ArgumentException($"File with id:{id} doesn't exists!");

            using (var connection = (NpgsqlConnection)new NpgSQLHlp().OpenDbConnection())
            {
                var fileManager = new NpgsqlLargeObjectManager(connection);
                using (connection.BeginTransaction())
                using (var memoryStream = new MemoryStream())
                using (var dbStream = await fileManager.OpenReadAsync(result.Oid, default(CancellationToken)))
                {
                    var buffer = new byte[262144];

                    while (memoryStream.Position < dbStream.Length)
                    {
                        var count = await dbStream.ReadAsync(buffer, 0, buffer.Length, default(CancellationToken));
                        await memoryStream.WriteAsync(buffer, 0, count, default(CancellationToken));
                    }

                    memoryStream.Position = 0;
                    return memoryStream.ToArray();
                }
            }
        }


        public byte[] GetFile(string id)
        {
            var result = _context.Files.FirstOrDefault(q => q.Id == id);
            if (result == null) throw new ArgumentException($"File with id:{id} doesn't exists!");

            using (var connection = (NpgsqlConnection)new NpgSQLHlp().OpenDbConnection())
            {
                var fileManager = new NpgsqlLargeObjectManager(connection);
                using (connection.BeginTransaction())
                using (var memoryStream = new MemoryStream())
                using (var dbStream = fileManager.OpenRead(result.Oid))
                {
                    var buffer = new byte[262144];

                    while (memoryStream.Position < dbStream.Length)
                    {
                        var count = dbStream.Read(buffer, 0, buffer.Length);
                        memoryStream.Write(buffer, 0, count);
                    }

                    memoryStream.Position = 0;
                    return memoryStream.ToArray();
                }
            }
        }

        public async Task<string> CreateAndUploadFileAsync(Stream stream, string fileName)
        {
            var item = new FileEntity
            {
                Id = IdGenerator.Generate(),
                Name = Path.GetFileNameWithoutExtension(fileName)
            };

            await _context.Files.AddAsync(item);
            await _context.SaveChangesAsync();

            if (stream.Position > 0) stream.Position = 0;

            await UploadFile(item.Id, stream);

            return item.Id;
        }

        private static async Task UploadFile(string id, Stream stream)
        {
            using (var connection = (NpgsqlConnection)new NpgSQLHlp().OpenDbConnection())
            using (var transaction = connection.BeginTransaction())
            {
                var loManager = new NpgsqlLargeObjectManager(connection);
                var oid = loManager.Create();
                using (var dbStream = await loManager.OpenReadWriteAsync(oid, default(CancellationToken)))
                {
                    var buffer = new byte[262144];
                    while (dbStream.Position < stream.Length)
                    {
                        var count = await stream.ReadAsync(buffer, 0, buffer.Length);
                        await dbStream.WriteAsync(buffer, 0, count);
                    }
                }

                using (var command = connection.CreateCommand())
                {
                    command.CommandText = "update \"public\".\"files\" set \"oid\" = @oid where \"id\" = @id";
                    command.Parameters.AddWithValue("@oid", NpgsqlDbType.Oid, oid);
                    command.Parameters.AddWithValue("@id", id);
                    command.ExecuteNonQuery();
                }

                transaction.Commit();
                connection.Close();
            }
        }

        public async Task<bool> UploadFileAsync(string id, byte[] bytea)
        {
            Stream stream = new MemoryStream(bytea);
            
                using (var connection = (NpgsqlConnection)new NpgSQLHlp().OpenDbConnection())
                using (var transaction = connection.BeginTransaction())
                {
                    var loManager = new NpgsqlLargeObjectManager(connection);
                    var oid = loManager.Create();
                    using (var dbStream = await loManager.OpenReadWriteAsync(oid, default(CancellationToken)))
                    {
                        var buffer = new byte[262144];
                        while (dbStream.Position < stream.Length)
                        {
                            var count = await stream.ReadAsync(buffer, 0, buffer.Length);
                            await dbStream.WriteAsync(buffer, 0, count);
                        }
                    }

                    using (var command = connection.CreateCommand())
                    {
                        command.CommandText = "update \"public\".\"files\" set \"oid\" = @oid where \"id\" = @id";
                        command.Parameters.AddWithValue("@oid", NpgsqlDbType.Oid, oid);
                        command.Parameters.AddWithValue("@id", id);
                        command.ExecuteNonQuery();
                    }

                    transaction.Commit();
                    connection.Close();
                }
            

            return true;

        }

        public bool UploadFile(string id, byte[] bytea, string fileName)
        {

                var item = new FileEntity
                {
                    Id = id,
                    Name = fileName
                };
                _context.Files.AddAsync(item);
                _context.SaveChanges();
            


            using (MemoryStream stream = new MemoryStream(bytea))
            {
                using (var connection = (NpgsqlConnection)new NpgSQLHlp().OpenDbConnection())
                {
                    using (var transaction = connection.BeginTransaction())
                    {
                        var loManager = new NpgsqlLargeObjectManager(connection);
                        var oid = loManager.Create();


                        using (var dbStream = loManager.OpenReadWrite(oid))
                        {


                            var buffer = new byte[262144];
                            while (dbStream.Position < stream.Length)
                            {
                                var count = stream.Read(buffer, 0, buffer.Length);
                                dbStream.Write(buffer, 0, count);
                            }
                        }

                        using (var command = connection.CreateCommand())
                        {
                            command.CommandText = "update \"public\".\"files\" set \"oid\" = @oid where \"id\" = @id";
                            command.Parameters.AddWithValue("@oid", NpgsqlDbType.Oid, oid);
                            command.Parameters.AddWithValue("@id", id);
                            command.ExecuteNonQuery();
                        }

                        transaction.Commit();

                    }

                    connection.Close();
                }
            }





            return true;
        }
    }
}
