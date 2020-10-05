using System;
using System.Collections.Generic;
using System.IO;
using SharpYaml.Serialization;

namespace DBTester
{
    public class Settings
    {
        [YamlMember("databaseConnections")]
        public IEnumerable<DatabaseConnection> DatabaseConnections { get; set; }

        public IEnumerable<Test> Tests { get; set; }

        public static Settings FromFile(string filename)
        {
            var file = new FileInfo(filename);
            using (var stream = file.OpenRead())
            {
                var serializer = new Serializer(new SerializerSettings()
                {
                    NamingConvention = new FlatNamingConvention(),
                });

                var result = serializer.Deserialize<Settings>(stream);

                return result;
            }
        }

        public void RunAll()
        {
            foreach (var item in Tests)
            {
                foreach (var child in item.Cases)
                {
                    try
                    {
                        var sql = child.GetSql(item.Document);

                        foreach (var dbConnection in DatabaseConnections)
                        {
                            try
                            {
                                var result = sql.ExecuteCommand(dbConnection);
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine(ex);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex);
                    }
                }
            }
        }
    }
}