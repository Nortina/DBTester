using System.Collections.Generic;
using SharpYaml.Serialization;

namespace DBTester
{
    public class Case
    {
        public string Id { get; set; }

        public Dictionary<string, object> Params { get; set; }
    }
}