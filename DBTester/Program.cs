using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using DBTester.TestCompiler;
using SharpYaml.Serialization;

namespace DBTester
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var settings = Settings.FromFile("./settings.yaml");
            settings.RunAll();
        }
    }
}