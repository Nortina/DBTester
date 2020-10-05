using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using DBTester.TestCompiler;

namespace DBTester
{
    public static class XmlDocumentExtensions
    {
        public static XmlNode GetMapperNode(this XmlDocument document)
        {
            foreach (XmlNode item in document.ChildNodes)
            {
                if (item.Name.ToLower() == "mapper")
                {
                    return item;
                }
            }

            return null;
        }

        public static XmlNode GetNodeById(this XmlDocument document, string id)
        {
            var mapperNode = document.GetMapperNode();
            foreach (XmlNode item in mapperNode.ChildNodes)
            {
                if (item.Attributes["id"]?.Value == id)
                {
                    return item;
                }
            }

            return null;
        }

        public static string GetSql(this XmlNode node, Dictionary<string, object> parameters)
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
                        var tree = new AbstractSyntaxTree(tokens, parameters);

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
                            temp.Add(item.GetSql(parameters));
                        }

                        result.AppendJoin(" ", temp);
                    }
                    break;

                case XmlNodeType.Text:
                case XmlNodeType.CDATA:
                    var regex = new Regex("^\\s*(.*)\\s*$", RegexOptions.Multiline);
                    var multiLineRegex = new Regex("\\s*\\n\\s*", RegexOptions.Multiline);
                    var text = regex.Replace(node.InnerText, "$1");
                    text = multiLineRegex.Replace(text, " ");
                    result.Append(text);
                    break;
            }

            return result.ToString();
        }
    }
}