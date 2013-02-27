using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using System.ServiceModel;
using System.ServiceModel.Web;
using ApplicationSource.Models;
using Domain;

namespace ApplicationSource.Interfaces
{
    [ServiceContract(Namespace = "http://Control4", SessionMode = SessionMode.Allowed)]
    public interface IMacIdSearchService
    {
        [OperationContract]
        [WebInvoke(Method = "POST", BodyStyle = WebMessageBodyStyle.Bare, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        IList<MacIdItem> LocateMacIds(IList<MacIdItem> model);
    }
}
