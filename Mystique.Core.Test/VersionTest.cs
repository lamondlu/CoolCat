using Microsoft.VisualStudio.TestTools.UnitTesting;
using Mystique.Core.DomainModel;
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
            var version = new Version("1.1.1");
        }

        [TestMethod]
        public void TestInvalidVersion()
        {
            Assert.ThrowsException<ArgumentException>(() =>
            {
                var version = new Version("abc");
            });
        }
    }
}
