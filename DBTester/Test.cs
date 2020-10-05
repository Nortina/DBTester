using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using SharpYaml.Serialization;
using System.Text;
using DBTester.TestCompiler;
using System.Text.RegularExpressions;

namespace DBTester
{
    public class Test
    {
        [YamlIgnore]
        public bool IsInitialized { get; private set; }

        [YamlIgnore]
        public XmlDocument Document { get; set; } = new XmlDocument();

        public string Filename { get; set; }

        public IEnumerable<Case> Cases { get; set; }

        public void Initialize()
        {
            var fileInfo = new FileInfo(Filename);
            if (fileInfo.Exists)
            {
                using (var stream = fileInfo.OpenRead())
                {
                    Document.Load(stream);
                }

                IsInitialized = true;
            }
        }

        public XmlNode GetMapperNode()
        {
            foreach (XmlNode item in Document.ChildNodes)
            {
                if (item.Name.ToLower() == "mapper")
                {
                    return item;
                }
            }

            return null;
        }

        public XmlNode GetNodeById(string id)
        {
            var mapperNode = GetMapperNode();
            foreach (XmlNode item in mapperNode.ChildNodes)
            {
                if (item.Attributes["id"]?.Value == id)
                {
                    return item;
                }
            }

            return null;
        }

        public string GetSql(XmlNode node, Dictionary<string, object> paramaters)
        {
            var result = new StringBuilder();

            switch (node.NodeType)
            {
                case XmlNodeType.Element:
                    if (node.Name.ToLower() == "if")
                    {
                        var test = node.Attributes["test"]?.Value;
                        if (string.IsNullOrEmpty(test))
                        {
                            break;
                        }

                        var lexer = new Lexer(test);
                        var tokens = lexer.Lex();
                        var tree = new AbstractSyntaxTree(tokens, paramaters);

                        if (tree.Calc() is bool testResult)
                        {
                            if (!testResult)
                            {
                                break;
                            }
                        }
                        else
                        {
                            break;
                        }
                    }

                    if (node.HasChildNodes)
                    {
                        var temp = new List<string>();
                        foreach (XmlNode item in node.ChildNodes)
                        {
                            temp.Add(GetSql(item, paramaters));
                        }

                        result.AppendJoin(" ", temp);
                    }
                    break;

                case XmlNodeType.Text:
                    result.Append(node.InnerText.Trim());
                    break;

                case XmlNodeType.CDATA:
                    result.Append(node.InnerText.Trim());
                    break;
            }

            return result.ToString();
        }

        public void RunAll()
        {
            if (!IsInitialized)
            {
                return;
            }

            foreach (var item in Cases)
            {
                var node = GetNodeById(item.Id);
                if (node != null)
                {
                    var sql = GetSql(node, item.Params);
                    var formattedSql = sql;

                    foreach (var key in item.Params.Keys)
                    {
                        var value = item.Params[key];
                        var regex = new Regex($"#{{{key}}}");

                        formattedSql = regex.Replace(formattedSql, $"'{value}'");
                    }

                    Console.WriteLine("----------");
                    Console.WriteLine(formattedSql);
                }
            }
        }
    }
}