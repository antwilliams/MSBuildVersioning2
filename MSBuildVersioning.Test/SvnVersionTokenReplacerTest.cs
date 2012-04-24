using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using Moq;

namespace MSBuildVersioning.Test
{
    [TestFixture]
    public class SvnVersionTokenReplacerTest
    {
        [Test]
        public void ReplaceTest()
        {
            var infoProviderMock = new Mock<SvnInfoProvider>();
            infoProviderMock.Setup(x => x.GetRevisionNumber()).Returns(1437);
            infoProviderMock.Setup(x => x.GetRepositorySubDirectory("branches")).Returns("1.0-stable");

            string content =
                "Revision 1.0.$REVNUM_DIV(100)$.$REVNUM_MOD(100)$ of $SUBDIR(\"branches\")$";

            content = new SvnVersionTokenReplacer(infoProviderMock.Object).Replace(content);

            Assert.AreEqual("Revision 1.0.14.37 of 1.0-stable", content);
        }
    }
}
