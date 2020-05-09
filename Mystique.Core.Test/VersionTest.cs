using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Version = Mystique.Core.DomainModel.Version;

namespace Mystique.Core.Test
{
    [TestClass]
    public class VersionTest
    {
        [TestMethod]
        public void TestValidVersion()
        {
            Version version = new Version("1.1.1");
        }

        [TestMethod]
        public void TestInvalidVersion()
        {
            Assert.ThrowsException<ArgumentException>(() =>
            {
                Version version = new Version("abc");
            });
        }
    }
}
