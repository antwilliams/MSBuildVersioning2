using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;

namespace MSBuildVersioning.Test
{
    [TestFixture]
    public class HgInfoProviderTest
    {
        public HgInfoProvider Hg1
        {
            get { return new HgInfoProvider() { Path = @"C:\Temp\TestRepositories\Hg1" }; }
        }

        public HgInfoProvider Hg2
        {
            get { return new HgInfoProvider() { Path = @"C:\Temp\TestRepositories\Hg2" }; }
        }

        public HgInfoProvider Hg3
        {
            get { return new HgInfoProvider() { Path = @"C:\Temp\TestRepositories\Hg3" }; }
        }

        [Test]
        public void GetRevisionNumberTest()
        {
            Assert.AreEqual(2, Hg1.GetRevisionNumber());
            Assert.AreEqual(4, Hg2.GetRevisionNumber());
            Assert.AreEqual(2, Hg3.GetRevisionNumber());
        }

        [Test]
        public void GetRevisionIdTest()
        {
            Assert.AreEqual("1024d08c6b37", Hg1.GetRevisionId(false));
            Assert.AreEqual("bf82f571c792", Hg2.GetRevisionId(false));
            Assert.AreEqual("80de7a096ed2", Hg3.GetRevisionId(false));
            Assert.AreEqual("1024d08c6b3733bd3b0a346e485d1ecd64183eeb", Hg1.GetRevisionId(true));
            Assert.AreEqual("bf82f571c7928bc7078b8b8413b601d17fa04cbd", Hg2.GetRevisionId(true));
            Assert.AreEqual("80de7a096ed2d19142a946024165542c043971bf", Hg3.GetRevisionId(true));
        }

        [Test]
        public void IsWorkingCopyDirtyTest()
        {
            Assert.IsFalse(Hg1.IsWorkingCopyDirty());
            Assert.IsTrue(Hg2.IsWorkingCopyDirty());
            Assert.IsTrue(Hg3.IsWorkingCopyDirty());
        }

        [Test]
        public void GetBranchTest()
        {
            Assert.AreEqual("default", Hg1.GetBranch());
            Assert.AreEqual("formal", Hg2.GetBranch());
            Assert.AreEqual("default", Hg3.GetBranch());
        }

        [Test]
        public void GetTagsTest()
        {
            Assert.AreEqual("", Hg1.GetTags());
            Assert.AreEqual("formal-1.0", Hg2.GetTags());
            Assert.AreEqual("tip", Hg3.GetTags());
        }
    }
}
