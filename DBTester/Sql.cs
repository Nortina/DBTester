using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;

namespace DBTester
{
    public class Sql
    {
        public string Command { get; set; }

        public string Type { get; set; }

        public string ExecuteCommand(DatabaseConnection dbConnection)
        {
            var result = new StringBuilder();

            using (var connection = dbConnection.GetConnection())
            {
                connection.Open();

                using (var tran = connection.BeginTransaction())
                {
                    var command = connection.CreateCommand();
                    command.CommandText = Command;

                    switch (Type)
                    {
                        case "SELECT":
                            using (var reader = command.ExecuteReader())
                            {
                                var rows = new List<Dictionary<string, object>>();

                                while (reader.Read())
                                {
                                    var row = new Dictionary<string, object>();

                                    for (var i = 0; i < reader.FieldCount; i++)
                                    {
                                        var name = reader.GetName(i);
                                        row[name] = reader[i];
                                    }

                                    rows.Add(row);
                                }

                                result.Append(JsonSerializer.Serialize(rows));
                            }
                            break;

                        case "INSERT":
                        case "UPDATE":
                        case "DELETE":
                            result.Append(command.ExecuteNonQuery());
                            break;
                    }
                }
            }

            return result.ToString();
        }
    }
}