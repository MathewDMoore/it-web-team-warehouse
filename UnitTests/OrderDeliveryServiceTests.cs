using System.Collections.Generic;
using System.Security.Principal;
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
        private const string RSK_PRIMARY = "000FFF00005483D6161438116200776BL";
        private const int DOCNUM=1234;

        [SetUp]
        public void SetUp()
        {
            _repo = MockRepository.GenerateMock<IInventoryRepository>();
            _settings = MockRepository.GenerateStub<ISettings>();
            _identity = MockRepository.GenerateStub<IIdentity>();
            DomainModelMapper.Initialize();
        }

        [Test]
        public void MatchAndSave_LoopsUntil_SuccessfulSave()
        {
            var expected = new List<SerialNumberItem> { new SerialNumberItem { ProductId = "00776", Color = "BL", SerialNum = 1 }, new SerialNumberItem { ProductId = "00776", Color = "BL", SerialNum = 2 } };
            _repo.Stub(r => r.UpdateSerialNumberItem(Arg<SerialNumberItem>.Matches(a => a.SerialNum.Equals(1)), Arg<bool>.Is.Equal(false))).Return(false);
            _repo.Stub(r => r.UpdateSerialNumberItem(Arg<SerialNumberItem>.Matches(a => a.SerialNum.Equals(2)), Arg<bool>.Is.Equal(false))).Return(true);
            _repo.Stub(r => r.FindUnScannedMatch(Arg<SerialNumberItemQuery>.Is.Anything)).Return(expected);
            var model = new MatchModel { SerialCode = RSK_PRIMARY, IsInternal = false, DocNumber = DOCNUM };
        
            var actual = new OrderDeliveryService(_repo, _settings, _identity).MatchAndSave(model);
            _repo.AssertWasCalled(r => r.FindUnScannedMatch(Arg<SerialNumberItemQuery>.Matches(a => a.ProductId.Equals("00776") && a.Color.Equals("BL"))));
        
            Assert.AreEqual(actual.UpdatedItem.SerialNum, 2);
        }
        
        [Test]
        public void MatchAndSave_LoopsUntil_SuccessfulSave_IfNoMoreResults_ReturnError()
        {
            var expected = new List<SerialNumberItem> { new SerialNumberItem { ProductId = "00776", Color = "BL", SerialNum = 1 }, new SerialNumberItem { ProductId = "00776", Color = "BL", SerialNum = 2 } };
            _repo.Stub(r => r.UpdateSerialNumberItem(Arg<SerialNumberItem>.Matches(a => a.SerialNum.Equals(1)), Arg<bool>.Is.Equal(false))).Return(false);
            _repo.Stub(r => r.UpdateSerialNumberItem(Arg<SerialNumberItem>.Matches(a => a.SerialNum.Equals(2)), Arg<bool>.Is.Equal(false))).Return(false);
            _repo.Stub(r => r.FindUnScannedMatch(Arg<SerialNumberItemQuery>.Is.Anything)).Return(expected);
            var model = new MatchModel { SerialCode = RSK_PRIMARY, IsInternal = false, DocNumber = DOCNUM };
        
            var actual = new OrderDeliveryService(_repo, _settings, _identity).MatchAndSave(model);
            _repo.AssertWasCalled(r => r.FindUnScannedMatch(Arg<SerialNumberItemQuery>.Matches(a => a.ProductId.Equals("00776") && a.Color.Equals("BL"))));
        
            Assert.AreEqual(actual.ErrorMessage, "All matches already scanned");
        }

        [Test]
        public void MatchAndSave_VerifyProductSku()
        {
            _repo.Stub(r => r.FindUnScannedMatch(Arg<SerialNumberItemQuery>.Is.Anything)).Return(new List<SerialNumberItem>());
            var model = new MatchModel { SerialCode = RSK_PRIMARY, IsInternal = false, DocNumber = DOCNUM };
            new OrderDeliveryService(_repo, _settings, _identity).MatchAndSave(model);
            _repo.AssertWasCalled(r => r.FindUnScannedMatch(Arg<SerialNumberItemQuery>.Matches(a => a.ProductId.Equals("00776") && a.Color.Equals("BL"))));
        }

        [Test]
        public void MatchAndSave_NoMatches_ReturnsErrorMessage()
        {
            _repo.Stub(r => r.FindUnScannedMatch(Arg<SerialNumberItemQuery>.Is.Anything)).Return(new List<SerialNumberItem>());
            var model = new MatchModel { SerialCode = RSK_PRIMARY, IsInternal = false, DocNumber = DOCNUM };

            var actual = new OrderDeliveryService(_repo, _settings, _identity).MatchAndSave(model);
            Assert.AreEqual(actual.ErrorMessage, "No items found that match that MacId");

        }

        [Test]
        public void MatchAndSave_KitsAndSingles_ReturnsErrorMessage()
        {
            var expected = new List<SerialNumberItem> { new SerialNumberItem(), new SerialNumberItem { KitId = 10 } };
            _repo.Stub(r => r.FindUnScannedMatch(Arg<SerialNumberItemQuery>.Is.Anything)).Return(expected);
            var model = new MatchModel {SerialCode = RSK_PRIMARY, IsInternal = false, DocNumber = DOCNUM};
            var actual = new OrderDeliveryService(_repo, _settings, _identity).MatchAndSave(model);
            Assert.AreEqual(actual.ErrorMessage, "Please scan single items first. Then scan kits");

        }

        [Test]
        public void OrderLookUp_HappyPath()
        {
            var lookup = new MacDeliveryModel { DeliveryNumber = 1234, IsInternal = false };
            var expectedDelivery = new Delivery { Address = "ADDRESS" };
            var expectedItems = new List<SerialNumberItem> { new SerialNumberItem { DocEntry = 1 } };
            _identity.Stub(i => i.Name).Return("USERNAME");
            _settings.Stub(s => s.GetServerLocation).Return("SERVER");
            _repo.Expect(r => r.GetDelivery(Arg<DeliveryOrderQuery>.Matches(a => a.IsInternal.Equals(false) && a.DocNum.Equals(lookup.DeliveryNumber) && a.ServerLocation.Equals("SERVER")))).Return(expectedDelivery);
            _repo.Expect(r => r.GetDeliveryItems(Arg<DeliveryOrderItemsQuery>.Matches(a => a.DocNum.Equals(lookup.DeliveryNumber) && a.ServerLocation.Equals("SERVER") && a.Username.Equals("USERNAME") && a.IsInternal.Equals(false)))).Return(expectedItems);

            var actual = new OrderDeliveryService(_repo, _settings, _identity).OrderLookUp(lookup);

            Assert.AreEqual("ADDRESS", actual.Address);
            Assert.AreEqual(lookup.DeliveryNumber, actual.DeliveryNumber);

        }
    }
}
