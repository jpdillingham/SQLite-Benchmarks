using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Microsoft.Data.Sqlite;

namespace Benchmarks
{
    public static class Extensions
    {
        public static int ExecuteNonQuery(this SqliteConnection conn, string query, Action<SqliteCommand>? action = null)
        {
            using var cmd = new SqliteCommand(query, conn);
            action?.Invoke(cmd);
            return cmd.ExecuteNonQuery();
        }
    }
}
