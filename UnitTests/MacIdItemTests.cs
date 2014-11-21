using ApplicationSource.Models;
using NUnit.Framework;

namespace UnitTests
{
    [TestFixture]
    public class MacIdItemTests
    {
        [Test]
        public void HasErrors()
        {
            Assert.IsTrue(new MacIdItem{ ErrorMessage = "ERROR"}.HasErrors);
        }
        
        [Test]
        public void HasErrors_EmptyErrorMessage()
        {
            Assert.IsFalse(new MacIdItem{ ErrorMessage = ""}.HasErrors);
        }
    }
}