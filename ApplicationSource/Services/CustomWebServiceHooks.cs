using System;
using System.IO;
using System.Reflection;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;
using System.Text;
using System.Xml;
using ApplicationSource.Helpers;
using StructureMap;

namespace ApplicationSource.Services
{
    // Assuming no unhandled exceptions, here's the call flow:
    //
    // AfterReceiveRequest (it's just a message at this point)
    // GetInstance (overrides how the class gets instantiated)
    // BeforeCall (it's been parsed now and the parameters are available as object[])
    // Call to the service method itself
    // AfterCall (opportunity to affect the result)
    // BeforeSendReply (it's serialized at this point)
    // ReleaseInstance (overrides how the class gets destroyed).  This may still be executing AFTER the client has already received a response.

    // In the event of an exception or faults thrown by the web service method, or a fault thrown in the BeforeCall method (due to precondition violations), this is the order of operations:
    //
    // AfterReceiveRequest
    // GetInstance (interceptors will fire during this call)
    // BeforeCall
    // Call to the service method itself (which throws an exception)
    // ProvideFault
    // BeforeSendReply
    // ReleaseInstance.  This may still be executing AFTER the client has already received a response.
    // HandleError.  This may still be executing AFTER the client has already received a response.

    // Note that as far as I can tell, the same instance of CustomWebServiceHooks will be used for each call to the same web service.
    // Different instances may be used for different services, I'm not sure (todo - verify this).
    public class CustomWebServiceHooks : IDispatchMessageInspector, IErrorHandler, IParameterInspector,
                                         IInstanceProvider
    {
        private static readonly log4net.ILog _log =
            log4net.LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType.Name);

        private static readonly bool _IsRawMessageLoggingEnabled;
        private static readonly bool _IsParameterLoggingEnabled;

        static CustomWebServiceHooks()
        {
            try
            {
                //				string logRawWebserviceXmlsetting = ConfigurationManager.GetSection("rms-settings").Get("LogRawWebserviceXml");
                //				_IsRawMessageLoggingEnabled = (logRawWebserviceXmlsetting != null && logRawWebserviceXmlsetting.ToLower() == "true");
                //
                //				string logParameters = ConfigurationManager.AppSettings.Get("LogWebServiceCallParameters");
                //				_IsParameterLoggingEnabled = (logParameters != null && logParameters.ToLower() == "true");
            }
            catch (Exception exception)
            {
                _log.Error("Error in static CustomWebServiceHooks constructor.  " + exception.Message, exception);
                throw;
            }
        }

        public object AfterReceiveRequest(ref Message request, IClientChannel channel, InstanceContext instanceContext)
        {
            try
            {
                if (_log.IsDebugEnabled && _IsRawMessageLoggingEnabled)
                {
                    // Log this message.  The thing to be aware of here is that a Message can only be read once.  When I first got this logging working
                    // something else starting throwing an exception that the reply had already been read.  The way to get around this is to first create
                    // a MessageBuffer copy.  Then re-create the reply from the buffer and also create the message to be logged from the buffer.  Here are
                    // some references:
                    // http://msdn.microsoft.com/en-us/library/ms734675.aspx
                    // http://weblogs.asp.net/paolopia/archive/2007/08/23/writing-a-wcf-message-inspector.aspx (though his code ends up logging in the wierd binary format).

                    MessageBuffer buffer = request.CreateBufferedCopy(Int32.MaxValue);
                    request = buffer.CreateMessage();
                    Message copyToBeLogged = buffer.CreateMessage();

                    using (MemoryStream memoryStream = new MemoryStream())
                    {
                        using (
                            XmlDictionaryWriter xmlDictionaryWriter = XmlDictionaryWriter.CreateTextWriter(memoryStream)
                            )
                        {
                            copyToBeLogged.WriteMessage(xmlDictionaryWriter);
                            xmlDictionaryWriter.Flush();

                            string rawMessage = Encoding.UTF8.GetString(memoryStream.ToArray());

                            _log.Debug("Raw incoming message: " + rawMessage);
                        }
                    }
                }

                return null;
            }
            catch (Exception exception)
            {
                _log.Error("Error in AfterReceiveRequest.  " + exception.Message, exception);
                throw;
            }
        }

        public object GetInstance(InstanceContext instanceContext, Message message)
        {
            try
            {
                Type serviceType = OperationContext.Current.EndpointDispatcher.DispatchRuntime.Type;

                object serviceInstance = ObjectFactory.GetInstance(serviceType);
                return serviceInstance;
            }
            catch (Exception exception)
            {
                _log.Error("Error in GetInstance call.  " + exception.Message, exception);
                throw;
            }
        }

        // Don't believe this is needed.
        public object GetInstance(InstanceContext instanceContext)
        {
            _log.Error("Non-implemented GetInstance was called.");
            throw new NotImplementedException();
        }

        public object BeforeCall(string operationName, object[] inputs)
        {
			ServiceAuthorization.Authorize();

			return null;
        }

        public void ProvideFault(Exception error, MessageVersion version, ref Message fault)
        {
            try
            {
                //				if (OperationState.State.ContainsKey("Repository"))
                //				{
                //					IRepository repository = (IRepository) OperationState.State["Repository"];
                //
                //					if (repository.IsInTransaction())
                //					{
                //						_log.Info("Fault/Exception was thrown was thrown by the web service operation: "+error.Message+Environment.NewLine+error.ToString());
                //						_log.Info("Rolling back transaction.");
                //						repository.Rollback();
                //					}
                //				}
            }
            catch (Exception exception)
            {
                _log.Error("Error in ProvideFault.  " + exception.Message, exception);
            }
        }

        public void AfterCall(string operationName, object[] outputs, object returnValue, object correlationState)
        {
            try
            {
            }
            catch (Exception exception)
            {
                _log.Error("Error in AfterCall.  " + exception.Message, exception);
                throw;
            }
        }

        public void BeforeSendReply(ref Message reply, object correlationState)
        {
            //            try
            //            {
            //                if (_log.IsDebugEnabled && _IsRawMessageLoggingEnabled)
            //                {
            //                    // Log this message.  The thing to be aware of here is that a Message can only be read once.  When I first got this logging working
            //                    // something else starting throwing an exception that the reply had already been read.  The way to get around this is to first create
            //                    // a MessageBuffer copy.  Then re-create the reply from the buffer and also create the message to be logged from the buffer.  Here are
            //                    // some references:
            //                    // http://msdn.microsoft.com/en-us/library/ms734675.aspx
            //                    // http://weblogs.asp.net/paolopia/archive/2007/08/23/writing-a-wcf-message-inspector.aspx (though his code ends up logging in the wierd binary format).
            //
            //                    MessageBuffer buffer = reply.CreateBufferedCopy(Int32.MaxValue);
            //                    reply = buffer.CreateMessage();
            //                    Message copyToBeLogged = buffer.CreateMessage();
            //
            //                    using (MemoryStream memoryStream = new MemoryStream())
            //                    {
            //                        using (
            //                            XmlDictionaryWriter xmlDictionaryWriter = XmlDictionaryWriter.CreateTextWriter(memoryStream)
            //                            )
            //                        {
            //                            copyToBeLogged.WriteMessage(xmlDictionaryWriter);
            //                            xmlDictionaryWriter.Flush();
            //
            //                            string rawMessage = Encoding.UTF8.GetString(memoryStream.ToArray());
            //
            //                            _log.Debug("Raw outgoing message: " + rawMessage);
            //                        }
            //                    }
            //                }
            //            }
        }

        public void ReleaseInstance(InstanceContext instanceContext, object instance)
        {
            _log.Debug("Call to ReleaseInstance");

            try
            {
                //				IWindsorContainer container = (IWindsorContainer)OperationState.State["Container"];
                //				container.Release(instance);
                //				container.Dispose();
            }
            catch (Exception exception)
            {
                _log.Error("Error in ReleaseInstance.  " + exception.Message, exception);
                throw;
            }
        }

        public bool HandleError(Exception error)
        {
            try
            {
                // At this point we've already returned something to the client.

                //				if (error is FaultException<ParameterValueFault>)
                //				{
                //					FaultException<ParameterValueFault> parameterValueFault = (FaultException<ParameterValueFault>) error;
                //					_log.InfoFormat("Web service call is returning a ParameterValueFault with parameter [{0}] and violation description [{1}]",parameterValueFault.Detail.Parameter, parameterValueFault.Detail.ViolationDescription);
                //				}
                //				else
                //				{
                //					_log.Warn(error.Message, error);
                //				}

                // We're suposed to return true if we're satisfied that we've handled/audited the error.  Otherwise the error will continue along the chain
                // of IErrorHandlers.  Since I'm logging, we're claiming that we've handled the error.

                return true;
            }
            catch (Exception exception)
            {
                _log.Error("Error in HandleError.  " + exception.Message, exception);
                throw;
            }
        }
    }

    public class EnableCustomWebServiceHooks : Attribute, IServiceBehavior
    {
        private static readonly log4net.ILog _Log = log4net.LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType.Name);

        void IServiceBehavior.AddBindingParameters(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase, System.Collections.ObjectModel.Collection<ServiceEndpoint> endpoints, System.ServiceModel.Channels.BindingParameterCollection bindingParameters) { }

        public void Validate(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase)
        {
            //			foreach (ServiceEndpoint endpoint in serviceHostBase.Description.Endpoints)
            //			{
            //				endpoint.Behaviors.Add(new ServiceBehavior());
            //			}
        }

        void IServiceBehavior.ApplyDispatchBehavior(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase)
        {
            try
            {
                foreach (ChannelDispatcher chDisp in serviceHostBase.ChannelDispatchers)
                {
                    chDisp.ErrorHandlers.Add(new CustomWebServiceHooks());

                    foreach (EndpointDispatcher epDisp in chDisp.Endpoints)
                    {
                        epDisp.DispatchRuntime.InstanceProvider = new CustomWebServiceHooks();

                        epDisp.DispatchRuntime.MessageInspectors.Add(new CustomWebServiceHooks());

                        foreach (DispatchOperation op in epDisp.DispatchRuntime.Operations)
                            op.ParameterInspectors.Add(new CustomWebServiceHooks());
                    }
                }
            }
            catch (Exception exception)
            {
                _Log.Error("Error in ApplyDispatchBehavior.  " + exception.Message, exception);
            }
        }
    }
}