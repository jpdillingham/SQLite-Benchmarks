using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.Sqlite;

namespace Benchmarks
{
    public class Database
    {
        public Database()
        {
            CreateTables();
        }

        public SqliteConnection GetConnection(string? connectionString = null)
        {
            var conn = new SqliteConnection(connectionString ?? "Data Source=file:db?mode=memory");
            conn.Open();
            return conn;
        }

        public IEnumerable<string> Search(string query)
        {
            var sql = $"SELECT data.name FROM idx " +
                "INNER JOIN data ON idx.id = data.id " +
                $"WHERE idx MATCH '({query})' ORDER BY name ASC;";

            var results = new List<string>();
            
            using var conn = GetConnection();
            using var cmd = new SqliteCommand(sql, conn);
            var reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                var name = reader.GetString(0);
                results.Add(name);
            }

            return results;
        }

        public void CreateTables()
        {
            using var conn = GetConnection();

            conn.ExecuteNonQuery("DROP TABLE IF EXISTS idx; CREATE VIRTUAL TABLE idx USING fts5(id);");
            conn.ExecuteNonQuery("DROP TABLE IF EXISTS data; CREATE TABLE data (id TEXT PRIMARY KEY, name TEXT NOT NULL);");
        }

        public void Insert()
        {
            using var conn = GetConnection();

            var id = Guid.NewGuid().ToString();
            var name = Guid.NewGuid().ToString();

            conn.ExecuteNonQuery("INSERT INTO data (id, name) VALUES(@id, @name);", cmd =>
            {
                cmd.Parameters.AddWithValue("id", id);
                cmd.Parameters.AddWithValue("name", name);
            });

            conn.ExecuteNonQuery("INSERT INTO idx (id) VALUES(@id);", cmd =>
            {
                cmd.Parameters.AddWithValue("id", id);
            });
        }
    }
}
