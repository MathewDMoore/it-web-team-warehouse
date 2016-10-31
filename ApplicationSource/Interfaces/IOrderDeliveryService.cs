using System.ServiceModel;
using System.ServiceModel.Web;
using ApplicationSource.Models;

namespace ApplicationSource.Interfaces
{
    [ServiceContract(Namespace = "http://Control4", SessionMode = SessionMode.Allowed)]
    public interface IOrderDeliveryService
    {
        [OperationContract]
        [WebInvoke(Method = "POST", BodyStyle = WebMessageBodyStyle.Bare, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        OrderDeliveryModel OrderLookUp(MacDeliveryModel lookup);

        [OperationContract]
        [WebInvoke(Method = "POST", BodyStyle = WebMessageBodyStyle.Bare, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        MacDeliveryModel LocateMacIds(string macId);

        [OperationContract]
        [WebInvoke(Method = "POST", BodyStyle = WebMessageBodyStyle.Bare, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        bool ClearDelivery(ClearDeliveryModel delivery);

        [OperationContract]
        [WebInvoke(Method = "POST", BodyStyle = WebMessageBodyStyle.Bare, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        bool ReturnDelivery(ClearDeliveryModel delivery);

        [OperationContract]
        [WebInvoke(Method = "POST", BodyStyle = WebMessageBodyStyle.Bare, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        bool ReturnDeliveryLineItem(ReturnModel returns);

        [OperationContract]
        [WebInvoke(Method = "POST", BodyStyle = WebMessageBodyStyle.Bare, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        bool VerifyDelivery(int deliveryNumber);

        [OperationContract]
        [WebInvoke(Method = "POST", BodyStyle = WebMessageBodyStyle.Bare, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        SaveScanModel MatchAndSave(MatchModel scanModel);


        [OperationContract]
        [WebInvoke(Method = "POST", BodyStyle = WebMessageBodyStyle.Bare, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        bool UpdateScanByUser(UpdateUserNameModel updateModel);
    }
}
