using System.ServiceModel;
using System.ServiceModel.Web;
using ApplicationSource.Models;

namespace ApplicationSource.Interfaces
{
    [ServiceContract(Namespace = "http://Control4", SessionMode = SessionMode.Allowed)]
    public interface IUserAuthenticationService
    {
        [OperationContract]
        [WebInvoke(Method = "POST", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        UserAuthenticationModel UserAuthenticationLogin(string userName, string password, string contractorName);
    }
}