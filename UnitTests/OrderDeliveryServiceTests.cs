using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using ApplicationSource;
using ApplicationSource.Interfaces;
using ApplicationSource.Models;
using ApplicationSource.Services;
using Domain;
using NUnit.Framework;
using Persistence.Repositories.Interfaces;
using Rhino.Mocks;

namespace UnitTests
{
    [TestFixture]
    public class OrderDeliveryServiceTests
    {
        private IInventoryRepository _repo;
        private ISettings _settings;
        private IIdentity _identity;

        [SetUp]
        public void SetUp()
        {
            _repo = MockRepository.GenerateMock<IInventoryRepository>();
            _settings = MockRepository.GenerateStub<ISettings>();
            _identity = MockRepository.GenerateStub<IIdentity>();
        DomainModelMapper.Initialize();
        }

        [Test]
        public void OrderLookUp_HappyPath()
        {
            var lookup = new MacDeliveryModel{DeliveryNumber = 1234,IsInternal = false};
            var expectedDelivery = new Delivery{Address = "ADDRESS"};
            var expectedItems = new List<SerialNumberItem>{ new SerialNumberItem{DocEntry = 1}};
            _identity.Stub(i=>i.Name).Return("USERNAME");
            _settings.Stub(s => s.GetServerLocation).Return("SERVER");
            _repo.Expect(r => r.GetDelivery(Arg<DeliveryOrderQuery>.Matches(a=>a.IsInternal.Equals(false) && a.DocNum.Equals(lookup.DeliveryNumber) && a.ServerLocation.Equals("SERVER")))).Return(expectedDelivery);
            _repo.Expect(r =>r.GetDeliveryItems(Arg<DeliveryOrderItemsQuery>.Matches(a =>a.DocNum.Equals(lookup.DeliveryNumber) && a.ServerLocation.Equals("SERVER") &&a.Username.Equals("USERNAME") && a.IsInternal.Equals(false)))).Return(expectedItems);
            
            var actual = new OrderDeliveryService(_repo,_settings,_identity).OrderLookUp(lookup);

            Assert.AreEqual("ADDRESS",actual.Address);
            Assert.AreEqual(lookup.DeliveryNumber,actual.DeliveryNumber);
            
        }
    }
}
