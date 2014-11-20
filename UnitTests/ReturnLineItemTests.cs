using System;
using ApplicationSource.Models;
using NUnit.Framework;

namespace UnitTests
{
    [TestFixture]
    public class ReturnLineItemTests
    {
        [Test]
        public void MacId_SmartMac_GreaterThan_29_ReturnsCorrectLenght()
        {
            //XXXXXXXXXXXXMAC00000000000000000
            var smartCode = "MAC".PadLeft(15,'X').PadRight(32,'0');
            var actual = new ReturnLineItem {SmartCode = smartCode};
            Assert.AreEqual(smartCode.Length, 32);
            Assert.AreEqual(actual.MacId, "MAC".PadLeft(15, 'X'));
        }        [Test]
        public void MacId_SmartMac_LessThan_29_ReturnsmartMac()
        {
            //XXXXXXXXXXXXMAC00000000000000000
            var smartCode = "MAC".PadLeft(15,'X').PadRight(28,'0');
            var actual = new ReturnLineItem {SmartCode = smartCode};
            Assert.AreEqual(smartCode.Length, 28);
            Assert.AreEqual(actual.MacId, smartCode);
        }

        [Test]
        public void HasErrors()
        {
            var actual = new ReturnLineItem {ErrorMessage = "ERROR"};
            Assert.IsTrue(actual.HasErrors);
        }
        
        [Test]
        public void HasErrors_NoMessage()
        {
            var actual = new ReturnLineItem {};
            Assert.IsFalse(actual.HasErrors);
        }
    }
}