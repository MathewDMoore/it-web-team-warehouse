using System.Collections.Generic;
using ApplicationSource.Models;
using NUnit.Framework;

namespace UnitTests
{
    [TestFixture]
    public class OrderDeliveryModelTests
    {
        [Test]
        public void IsVerified_NotScanned_HasVerified_Scanned_HasNone()
        {
            var actual = new OrderDeliveryModel();
            actual.NotScannedItems = new List<DeliveryOrderItemModel> {new DeliveryOrderItemModel{Verified = true}};
            actual.ScannedItems = new List<DeliveryOrderItemModel> {new DeliveryOrderItemModel{}};
            Assert.IsTrue(actual.IsVerified);
        }
        
        [Test]
        public void IsVerified_NotScanned_HasNoVerified_Scanned_HasVerified()
        {
            var actual = new OrderDeliveryModel();
            actual.ScannedItems = new List<DeliveryOrderItemModel> {new DeliveryOrderItemModel{Verified = true}};
            actual.NotScannedItems = new List<DeliveryOrderItemModel> {new DeliveryOrderItemModel{}};
            Assert.IsTrue(actual.IsVerified);
        }
        [Test]
        public void IsVerified_NotScanned_HasNoVerified_Scanned_HasNone()
        {
            var actual = new OrderDeliveryModel();
            actual.ScannedItems = new List<DeliveryOrderItemModel> {new DeliveryOrderItemModel{}};
            actual.NotScannedItems = new List<DeliveryOrderItemModel> {new DeliveryOrderItemModel{}};
            Assert.IsFalse(actual.IsVerified);
        }
    }
}