using CodegenCS;
using CodegenCS.DotNet;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CodegenCS.Tests.DotNet
{
    public class DotNetCodegenContextTests
    {
        DotNetCodegenContext _ctx = null;

        [SetUp]
        public void Setup()
        {
            _ctx = new DotNetCodegenContext();
        }


        [Test]
        public void TestMultipleFiles()
        {
            // DotNetCodegenContext will automatically set the FileType (BuildActionType) based on the extension

            var f1 = _ctx["File1.cs"];
            var f2 = _ctx["README.TXT"];
            var f3 = _ctx["Transaltions.RESX"];

            CodegenContext context = (CodegenContext)_ctx;

            Assert.That(_ctx.OutputFiles.Count == 3);
            Assert.That(context.OutputFiles.Count == 3);

            f1.WriteLine("Test");
            Assert.That(context["File1.cs"].GetContents().Length > 0);

            Assert.That(f1.FileType == BuildActionType.Compile);
            Assert.That(f2.FileType == BuildActionType.None);
            Assert.That(f3.FileType == BuildActionType.EmbeddedResource);
        }

    }
}
