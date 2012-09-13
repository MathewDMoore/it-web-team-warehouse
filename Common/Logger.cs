using System;
using System.IO;
using System.Text;
using Common.Utils;
using log4net.Config;
using ILog = log4net.ILog;
using LogManager = log4net.LogManager;


namespace Common
{
    public class Logger : ILogger
    {
        private static bool loadedXmlConfigurator;
        private string _serverName;
        private string _user;
        private string _sessionId;
        private string _ipAddress;
        private const string DEFAULT_LOGGER = "RollingLogFileAppender";

        private static void Configure()
        {
            if (!loadedXmlConfigurator)
            {
                XmlConfigurator.ConfigureAndWatch(new FileInfo(AppDomain.CurrentDomain.SetupInformation.ConfigurationFile));

                loadedXmlConfigurator = true;
            }
        }

        public Logger(string sessionId, string ipAddress, string serverName, string user)
        {
            _sessionId = sessionId;
            _ipAddress = ipAddress;
            _serverName = serverName;
            _user = user;
        }

        public void Log(Type type, LogMessage message)
        {
            Configure();

            ILog log = LogManager.GetLogger(DEFAULT_LOGGER);

            switch (message.LoggingLevel)
            {
                case LogLevel.DEBUG:
                    log.Debug(message);
                    break;
                case LogLevel.INFO:
                    log.Info(message);
                    break;
                case LogLevel.WARN:
                    log.Warn(message);
                    break;
                case LogLevel.ERROR:
                    log.Error(message);
                    break;
                case LogLevel.FATAL:
                    log.Fatal(message);
                    break;
            }
        }

        public void LogAttempt(Type methodType, OperationType operationType, string message, string keyValues)
        {
            Log(methodType,
                new LogMessage(LogLevel.INFO, _serverName, ServiceType.MARKETPLACE, operationType, StatusType.ATTEMPT,
                _user, _sessionId, _ipAddress, message, keyValues));
        }

        public void LogSuccess(Type methodType, OperationType operationType, string message, string keyValues)
        {
            Log(methodType,
                new LogMessage(LogLevel.INFO, _serverName, ServiceType.MARKETPLACE, operationType, StatusType.SUCC,
                _user, _sessionId, _ipAddress, message, keyValues));
        }

        public void LogWarning(Type methodType, OperationType operationType, string message, string keyValues)
        {
            Log(methodType,
                new LogMessage(LogLevel.INFO, _serverName, ServiceType.MARKETPLACE, operationType, StatusType.WARN,
                _user, _sessionId, _ipAddress, message, keyValues));
        }

        public void LogFailure(Type methodType, OperationType operationType, string message, string keyValues)
        {
            Log(methodType,
                new LogMessage(LogLevel.INFO, _serverName, ServiceType.MARKETPLACE, operationType, StatusType.FAIL,
                _user, _sessionId, _ipAddress, message, keyValues));
        }

        public void LogException(Type methodType, OperationType operationType, Exception exception, string keyValues)
        {
            keyValues = string.Format("{0} stacktrace={1}", keyValues,
                                      exception.InnerException != null
                                          ? exception.InnerException.StackTrace
                                          : exception.StackTrace);

            Log(methodType,
                new LogMessage(LogLevel.ERROR, _user, ServiceType.MARKETPLACE, operationType, StatusType.FAIL,
                _user, _sessionId, _ipAddress, exception.Message, keyValues));
        }
    }

    public class LogMessage
    {
        private readonly string _preFormattedKeyValuePairs;

        public LogMessage(LogLevel loglevel, string server, ServiceType serviceType, OperationType operationType, StatusType statusType, string account, string sessionId, string ipAddress, string message, string preFormattedKeyValuePairs)
        {
            LoggingLevel = loglevel;
            Server = server;
            Service = serviceType;
            Operation = operationType;
            Status = statusType;
            Account = account;
            SessionId = sessionId;
            IpAddress = ipAddress;
            Message = message;
            _preFormattedKeyValuePairs = preFormattedKeyValuePairs;
        }

        public string Server { get; set; }
        public ServiceType? Service { get; set; }
        public OperationType? Operation { get; set; }
        public StatusType? Status { get; set; }
        public string Account { get; set; }
        public string SessionId { get; set; }
        public string IpAddress { get; set; }
        public string Message { get; set; }
        public LogLevel LoggingLevel { get; set; }

        public override string ToString()
        {
            var returnMessage = new StringBuilder();
            returnMessage.AppendFormat("[{0:MM/dd/yy H:mm:ss.fff}] ", DateTime.Now);
            returnMessage.Append((string.IsNullOrEmpty(Server)) ? string.Empty : string.Format("serv={0} ", Server).ToLower());
            returnMessage.Append((Service == null) ? string.Empty : string.Format("svc={0} ", Service).ToLower());
            returnMessage.Append((Operation == null) ? string.Empty : string.Format("op={0} ", Operation).ToLower());
            returnMessage.Append((Status == null) ? string.Empty : string.Format("st={0} ", Status).ToLower());

            returnMessage.Append((string.IsNullOrEmpty(Account)) ? string.Empty : string.Format("a={0} ", Account.EscapeWhiteSpace()));
            returnMessage.Append((string.IsNullOrEmpty(SessionId)) ? string.Empty : string.Format("sess={0} ", SessionId));
            returnMessage.Append((string.IsNullOrEmpty(IpAddress)) ? string.Empty : string.Format("ip={0} ", IpAddress));

            returnMessage.Append((string.IsNullOrEmpty(_preFormattedKeyValuePairs)) ? string.Empty : string.Format("{0} ", _preFormattedKeyValuePairs));

            returnMessage.Append((string.IsNullOrEmpty(Message)) ? string.Empty : string.Format("msg={0} ", Message));

            return returnMessage.ToString();
        }
    }

    public enum ServiceType
    {
        MARKETPLACE
    } ;

    //null: Inconsistent types ADDRECORDED but no REMOVEDRECORDED, only REMOVELINKEDVIDEO
    public enum OperationType
    {
        SQLWRAPPER,
        UPLOADDATASHEET,
        UPLOADDOWNLOAD,
        ADDFEATURE,
        SAVEUSESCASES,
        SAVETESTIMONIALS,
        ADDDATASHEET,
        ADDYOUTUBEVID,
        ADDRECORDEDVID,
        SAVEWARRANTYINFO,
        SAVERMAINFO,
        SAVETSNUM,
        ADKBARTICLE,
        ADDDOWNLOAD,
        UPDATEVIEW,
        REMOVEFEATURE,
        REMOVEDATASHEET,
        REMOVEEMBEDVIDEO,
        REMOVELINKEDVIDEO,
        REMOVEKBARTICLE,
        REMOVEDOWNLOAD,
        ADDHIRESDOWNLOAD,
        SAVEPRODUCTINFO,
        ADDPRODUCTIMG,
        UPLOADPRODUCTIMG,
        HIRESDOWNLOADTHUMB,
        ADDHIRESDOWNLOADPATH,
        ADDRECPROD,
        ADDRECAPP,
        REMOVERECAPP,
        REMOVERECPROD,
        ULVARIANTADIMG,
        CCAUTH,
        MAPPERCALL,
        PAYTRACECALL,
        SAVETSINFO
    } ;

    public enum StatusType
    {
        SUCC,
        FAIL,
        ATTEMPT,
        WARN
    } ;

    public enum LogLevel
    {
        DEBUG,
        INFO,
        WARN,
        ERROR,
        FATAL
    } ;
}