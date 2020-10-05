using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
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
            var builder = new StringBuilder();

            foreach (var test in Tests)
            {
                builder.AppendLine($"filename|{test.Filename}");

                foreach (var child in test.Cases)
                {
                    try
                    {
                        var sql = child.GetSql(test.Document);
                        builder.AppendLine($"sql|{sql.Command}");

                        foreach (var dbConnection in DatabaseConnections)
                        {
                            try
                            {
                                var result = sql.ExecuteCommand(dbConnection);
                                builder.AppendLine($"result|{result}");
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

            var fileInfo = new FileInfo($"./output.csv");

            if (fileInfo.Exists)
            {
                fileInfo.Delete();
            }

            using (var write = fileInfo.CreateText())
            {
                write.Write(builder.ToString());
            }
        }
    }
}