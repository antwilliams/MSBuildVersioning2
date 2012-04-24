using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;

namespace MSBuildVersioning.Test
{
    [TestFixture]
    public class SvnInfoProviderTest
    {
        public SvnInfoProvider SvnWC1
        {
            get { return new SvnInfoProvider() { Path = @"C:\Temp\TestRepositories\SvnWC1" }; }
        }

        public SvnInfoProvider SvnWC2
        {
            get { return new SvnInfoProvider() { Path = @"C:\Temp\TestRepositories\SvnWC2" }; }
        }

        [Test]
        public void GetRevisionNumberTest()
        {
            Assert.AreEqual(6, SvnWC1.GetRevisionNumber());
            Assert.AreEqual(5, SvnWC2.GetRevisionNumber());
        }

        [Test]
        public void IsMixedRevisionsTest()
        {
            Assert.IsTrue(SvnWC1.IsMixedRevisions());
            Assert.IsFalse(SvnWC2.IsMixedRevisions());
        }

        [Test]
        public void IsWorkingCopyDirtyTest()
        {
            Assert.IsFalse(SvnWC1.IsWorkingCopyDirty());
            Assert.IsTrue(SvnWC2.IsWorkingCopyDirty());
        }

        [Test]
        public void GetRepositoryUrlTest()
        {
            Assert.AreEqual("file:///C:/Temp/TestRepositories/SvnRepo", SvnWC1.GetRepositoryUrl());
            Assert.AreEqual("file:///C:/Temp/TestRepositories/SvnRepo/branches/beef", SvnWC2.GetRepositoryUrl());
        }

        [Test]
        public void GetRepositoryRootTest()
        {
            Assert.AreEqual("file:///C:/Temp/TestRepositories/SvnRepo", SvnWC1.GetRepositoryRoot());
            Assert.AreEqual("file:///C:/Temp/TestRepositories/SvnRepo", SvnWC2.GetRepositoryRoot());
        }

        [Test]
        public void GetRepositoryPathTest()
        {
            Assert.AreEqual("/", SvnWC1.GetRepositoryPath());
            Assert.AreEqual("/branches/beef", SvnWC2.GetRepositoryPath());
        }

        [Test]
        public void GetBranchTest()
        {
            Assert.AreEqual("", SvnWC1.GetBranch());
            Assert.AreEqual("beef", SvnWC2.GetBranch());
        }

        [Test]
        public void GetTagTest()
        {
            Assert.AreEqual("", SvnWC1.GetTag());
            Assert.AreEqual("", SvnWC2.GetTag());
        }
    }
}
