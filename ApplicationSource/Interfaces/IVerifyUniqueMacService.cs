using System.ServiceModel;
using System.ServiceModel.Web;
using ApplicationSource.Models;

namespace ApplicationSource.Interfaces
{
    [ServiceContract(Namespace = "http://Control4", SessionMode = SessionMode.Allowed)]
    public interface IVerifyUniqueMacService
    {
        [OperationContract]
        [WebInvoke(Method = "POST", BodyStyle = WebMessageBodyStyle.Bare, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        VerifyUniqueMacModel VerifyUniqueMac(VerifyUniqueMacModel model);

    }
}
