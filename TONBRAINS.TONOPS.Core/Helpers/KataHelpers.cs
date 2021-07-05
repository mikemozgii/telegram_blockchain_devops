using Dapper;
using Npgsql;
using SqlKata;
using SqlKata.Compilers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace TONBRAINS.TONOPS.Core.Handlers
{
    public class KataHelpers
    {
        public static string QueryToSql(Query query) => new PostgresCompiler().Compile(query).ToString();

        public static string ConnectionString = "";
        public static async Task ExecuteSql(string sql, string applicationName = "", string connectionString = "")
        {
            var usedConnectionString = string.IsNullOrEmpty(connectionString) ? ConnectionString : connectionString;
            var connection = new NpgsqlConnection(usedConnectionString + (!string.IsNullOrEmpty(applicationName) ? $";ApplicationName={applicationName}" : ""));
            await connection.OpenAsync();

            await connection.QueryAsync(sql);

            await connection.CloseAsync();
            connection.Dispose();
        }

        public static async Task<IEnumerable<T>> ExecuteSql<T>(string sql, string ApplicationName = "") where T : new()
        {
            var connection = new NpgsqlConnection(ConnectionString + (!string.IsNullOrEmpty(ApplicationName) ? $";ApplicationName={ApplicationName}" : ""));

            await connection.OpenAsync();

            var result = await connection.QueryAsync<T>(sql);
            await connection.CloseAsync();
            connection.Dispose();

            return result;
        }

        public static async Task<T> ExecuteSqlFirst<T>(string sql, string ApplicationName = "") where T : new() => (await ExecuteSql<T>(sql, ApplicationName)).FirstOrDefault();

        public static async Task<IEnumerable<T>> ExecuteJoinSql<T, U>(string sql, string property, string ApplicationName = "") where T : new() where U : new()
        {
            var connection = new NpgsqlConnection(ConnectionString + (!string.IsNullOrEmpty(ApplicationName) ? $";ApplicationName={ApplicationName}" : ""));

            await connection.OpenAsync();
            var result = await connection.QueryAsync<T, U, T>(sql,
                (t, u) =>
                {
                    if (u == null || u.ToString() != "{ }")
                    {
                        var prop = t.GetType().GetProperty(property);
                        prop.SetValue(t, u);
                    }
                    return t;
                });
            await connection.CloseAsync();
            connection.Dispose();

            return result;
        }


        public static async Task<IEnumerable<T>> ExecuteQueryJoin<T, U>(Query query, string property, string ApplicationName = "")
            where T : new() where U : new()
            => await ExecuteJoinSql<T, U>(QueryToSql(query), property, ApplicationName);


        public static async Task<IEnumerable<T>> ExecuteQuery<T>(Query query, string ApplicationName = "") where T : new() => await ExecuteSql<T>(QueryToSql(query), ApplicationName);

        public static async Task<T> ExecuteQueryFirst<T>(Query query, string ApplicationName = "") where T : new() => (await ExecuteSql<T>(QueryToSql(query), ApplicationName)).FirstOrDefault();

        public static async Task ExecuteQuery(Query query, string ApplicationName = "")
            => await ExecuteSql(QueryToSql(query), ApplicationName);

        public static string ToSnakeCase(string name)
        {
            var words = new List<string>();
            var currentWord = "";
            var iterator = 0;
            foreach (var character in name)
            {
                if (character >= 'A' && character <= 'Z' && iterator > 0)
                {
                    words.Add(currentWord);
                    currentWord = "";
                }
                currentWord += character;
                iterator++;
            }
            words.Add(currentWord);

            return string.Join('_', words.Select(a => a.ToLowerInvariant()));
        }

        /// <summary>
        /// Execute insert or update
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="model">Model</param>
        /// <param name="query">Query</param>
        /// <param name="insert">Insert flag</param>
        /// <param name="touchedFields">Touched fields(optional)</param>
        /// <returns></returns>
        public static async Task<T> ExecuteInsertOrUpdate<T>(T model, Query query, bool insert = true, IEnumerable<string> touchedFields = null) where T : new()
        {
            var names = new List<string>();
            var values = new List<object>();
            string id = "";
            var properties = model.GetType().GetProperties()
                .Where(a => a.Name != "Parser" && a.Name != "Descriptor")
                .ToArray();
            foreach (var property in properties)
            {
                if (property.Name == "Id") id = property.GetGetMethod().Invoke(model, null).ToString();

                if (!insert && property.Name == "Id") continue;

                if (touchedFields == null || (touchedFields != null && touchedFields.Contains(property.Name)))
                {
                    names.Add(ToSnakeCase(property.Name));
                    var value = property.GetGetMethod().Invoke(model, null);
                    if (property.PropertyType.IsEnum) values.Add((int)value);
                    else values.Add(value);
                }
            }
            var sql = QueryToSql(insert ? query.AsInsert(names, values) : query.Where("id", id).AsUpdate(names, values));
            sql += " RETURNING *";
            return (await ExecuteSql<T>(sql)).FirstOrDefault();
        }

        /// <summary>
        /// Execute insert or update with untouched fields
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="model">Model</param>
        /// <param name="query">Query</param>
        /// <param name="untouchedFields">Exclude fields</param>
        /// <param name="insert">Insert flag</param>
        /// <returns></returns>
        public static async Task<T> ExecuteInsertOrUpdate<T>(T model, Query query, IEnumerable<string> untouchedFields, bool insert = true) where T : new()
        {
            var touchedFields = model.GetType().GetProperties()
                .Where(a => a.Name != "Parser" && a.Name != "Descriptor")
                .Select(x => x.Name)
                .Except(untouchedFields)
                .ToArray();
            return await ExecuteInsertOrUpdate<T>(model, query, insert, touchedFields);
        }

        public static IEnumerable<PropertyInfo> FilterFields(object model, IEnumerable<string> onlyFields = null, IEnumerable<string> skipFields = null)
        {
            var props = model.GetType().GetProperties()
                .Where(a => a.Name != "Parser" && a.Name != "Descriptor" && a.Name != "Id");

            //var idProeprty = props.FirstOrDefault(q => q.Name == "Id");

            if (onlyFields != null)
            {
                props = props.Where(q => onlyFields.Contains(q.Name));
            }

            if (skipFields != null)
            {
                props = props.Where(q => onlyFields.Contains(q.Name));
            }

            //if (idProeprty != null && model.GetType().GetProperties().FirstOrDefault(a => a.Name != "Id") != null)
            //{
            //    props = props.Concat(new List<PropertyInfo> { idProeprty });
            //}

            return props;
        }

        public static async Task<T> ExecuteInsertSingle<T>(T model, Query query, IEnumerable<string> onlyFields = null, IEnumerable<string> skipFields = null) where T : new()
        {
            var properties = FilterFields(model, onlyFields, skipFields);

            if (!properties.Any())
            {
                throw new System.ArgumentException("No properties to Insert");
            }

            var idProeprty = model.GetType().GetProperties().First(q => q.Name == "Id");
            var id = idProeprty.GetGetMethod().Invoke(model, null).ToString();
            if (string.IsNullOrWhiteSpace(id))
            {
                throw new System.ArgumentException("Id Property is not Present");
            }

            var names = new List<string>() { "id" };
            var values = new List<object>() { id };

            foreach (var property in properties)
            {
                names.Add(ToSnakeCase(property.Name));
                var value = property.GetGetMethod().Invoke(model, null);
                if (property.PropertyType.IsEnum) values.Add((int)value);
                else values.Add(value);
            }

            var sql = QueryToSql(query.AsInsert(names, values));
            sql += " RETURNING *";
            return (await ExecuteSql<T>(sql)).FirstOrDefault();
        }

        public static async Task<T> ExecuteUpdateSingle<T>(T model, Query query, IEnumerable<string> onlyFields = null, IEnumerable<string> skipFields = null) where T : new()
        {
            var properties = FilterFields(model, onlyFields, skipFields);

            if (!properties.Any())
            {
                throw new System.ArgumentException("No properties to Update");
            }

            var idProeprty = model.GetType().GetProperties().First(q => q.Name == "Id");
            var id = idProeprty.GetGetMethod().Invoke(model, null).ToString();
            if (string.IsNullOrWhiteSpace(id))
            {
                throw new System.ArgumentException("Id Property is not Present");
            }

            var names = new List<string>();
            var values = new List<object>();

            foreach (var property in properties.Where(q => q.Name != "Id"))
            {
                names.Add(ToSnakeCase(property.Name));
                var value = property.GetGetMethod().Invoke(model, null);
                if (property.PropertyType.IsEnum) values.Add((int)value);
                else values.Add(value);
            }

            var sql = QueryToSql(query.Where("id", id).AsUpdate(names, values));
            sql += " RETURNING *";
            return (await ExecuteSql<T>(sql)).FirstOrDefault();
        }

    }

}