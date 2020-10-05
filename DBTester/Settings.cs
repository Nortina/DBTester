using System;
using System.Collections.Generic;
using System.IO;
using SharpYaml.Serialization;

namespace DBTester
{
    public class Settings
    {
        public IEnumerable<Database> Databases { get; set; }

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
                Console.WriteLine("==========");
                Console.WriteLine(item.Filename);
                item.Initialize();
                item.RunAll();
            }
        }
    }
}