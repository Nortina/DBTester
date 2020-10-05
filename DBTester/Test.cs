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
        public XmlDocument Document { get => _documentLazy.Value; }

        public string Filename { get; set; }

        public IEnumerable<Case> Cases { get; set; }

        private readonly Lazy<XmlDocument> _documentLazy;

        public Test()
        {
            _documentLazy = new Lazy<XmlDocument>(() =>
                    {
                        var document = new XmlDocument();

                        var fileInfo = new FileInfo(Filename);
                        if (fileInfo.Exists)
                        {
                            using (var stream = fileInfo.OpenRead())
                            {
                                document.Load(stream);
                            }
                        }

                        return document;
                    });
        }
    }
}