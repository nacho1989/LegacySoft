using System;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NUnit.Framework;
using TextDiffEngine;
using TextDiffEngine.Models;

namespace TextDiff.Tests
{
    [TestFixture]
    public class TextDiffTest
    {
        [TestCase]
        public void TestMethod1()
        {
            var sourceFile = new TextFile(File.ReadAllText(@"C:\Users\newton.acho\Documents\Visual Studio 2017\Projects\TextDiffEngine\TextDiff.Tests\DestFile.txt"));
            var destFile = new TextFile(File.ReadAllText(@"C:\Users\newton.acho\Documents\Visual Studio 2017\Projects\TextDiffEngine\TextDiff.Tests\SourceFile.txt"));
            var engine = new Engine();
            engine.ProcessDiff(sourceFile, destFile);
        }
    }
}
