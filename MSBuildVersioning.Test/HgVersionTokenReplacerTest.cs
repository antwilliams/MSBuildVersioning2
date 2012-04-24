using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using Moq;

namespace MSBuildVersioning.Test
{
    [TestFixture]
    public class HgVersionTokenReplacerTest
    {
        [Test]
        public void ReplaceTest()
        {
            var infoProviderMock = new Mock<HgInfoProvider>();
            infoProviderMock.Setup(x => x.GetRevisionNumber()).Returns(1437);

            string content = "Revision 1.0.$REVNUM_DIV(100)$.$REVNUM_MOD(100)$";
            content = new HgVersionTokenReplacer(infoProviderMock.Object).Replace(content);

            Assert.AreEqual("Revision 1.0.14.37", content);
        }
    }
}
