﻿using System.Security.Cryptography.X509Certificates;
using System.ServiceModel;
using System.ServiceModel.Web;
using ApplicationSource.Models;

namespace ApplicationSource.Interfaces
{
    [ServiceContract(Namespace = "http://Control4", SessionMode = SessionMode.Allowed)]
    public interface IOrderDeliveryService
    {
        [OperationContract]
        [WebInvoke(Method = "POST", BodyStyle = WebMessageBodyStyle.Bare, ResponseFormat = WebMessageFormat.Json,
            RequestFormat = WebMessageFormat.Json)]
        OrderDeliveryModel OrderLookUp(int orderId);

        [OperationContract]
        [WebInvoke(Method = "POST", BodyStyle = WebMessageBodyStyle.Wrapped, ResponseFormat = WebMessageFormat.Json,RequestFormat = WebMessageFormat.Json)]
        VerifyUniqueMacModel SaveDeliveryItem(VerifyUniqueMacModel model);
    }
}