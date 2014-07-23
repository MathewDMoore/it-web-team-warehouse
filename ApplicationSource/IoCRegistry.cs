using System.Diagnostics;
using System.Reflection;
using System.Web;
using Common;
using log4net;
using Persistence;
using Persistence.Repositories;
using Persistence.Repositories.Interfaces;
using StructureMap.Configuration.DSL;

namespace ApplicationSource
{
    public class IoCRegistry : Registry
    {
            private readonly ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType.Name);

            public IoCRegistry()
            {
                var stopwatch = Stopwatch.StartNew();
                For<ISqlMapperFactory>().Use((context) => new SqlMapperFactory(new SqlMapperWrapper(), context.GetInstance<ILogger>()));
                For<IInventoryRepository>().Use((context) => new InventoryRepository(context.GetInstance<ISqlMapperFactory>()));
                //            For<ISession>().Use<SessionHelper>();
                //            For<ISettings>().Singleton().Use<SettingsWrapper>();

                For<ILogger>().Use((context) => new Logger(HttpContext.Current.Session != null ? HttpContext.Current.Session.SessionID : "", HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"] ?? HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"], HttpContext.Current.Server.MachineName, ""));


                stopwatch.Stop();
                _log.Debug("Completed IoC container configuration in " + stopwatch.Elapsed + " milliseconds.");
            }
        } 
    
}