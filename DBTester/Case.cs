using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Xml;
using Npgsql;
using SharpYaml.Serialization;

namespace DBTester
{
    public class Case
    {
        public string Id { get; set; }

        public Dictionary<string, object> Params { get; set; }

        public Sql GetSql(XmlDocument document)
        {
            var node = document.GetNodeById(Id);
            if (node == null)
            {
                return null;
            }

            var command = node.GetSql(Params);

            foreach (var key in Params.Keys)
            {
                var value = Params[key];
                var regex = new Regex($"#{{{key}}}");

                command = regex.Replace(command, $"'{value}'");
            }

            return new Sql()
            {
                Command = command,
                Type = node.Name.ToUpper(),
            };
        }
    }
}