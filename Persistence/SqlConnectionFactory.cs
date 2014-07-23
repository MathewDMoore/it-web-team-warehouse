using System;
using System.Collections.Specialized;
using System.Configuration;
using System.Xml;
using IBatisNet.Common.Utilities;
using IBatisNet.DataMapper.Configuration;

namespace Persistence
{
    public interface ISqlConnectionFactoryData
    {
        ConnectionStringSettings ConnectionStringsSettings(string key);
        XmlDocument GetEmbeddedResourceAsXmlDocument(string name);
        IDomSqlMapBuilder CreateDomSqlMapBuilder();
    }

    public interface IDomSqlMapBuilder
    {
        NameValueCollection Properties { set; }
        IBatisNet.DataMapper.ISqlMapper Configure(XmlDocument mapConfig);
    }

    public class DomSqlMapBuilderAdapter : IDomSqlMapBuilder
    {
        public static DomSqlMapBuilderAdapter GetInstance(DomSqlMapBuilder builder)
        {
            return new DomSqlMapBuilderAdapter(builder);
        }
        private DomSqlMapBuilderAdapter(DomSqlMapBuilder builder)
        {
            _builder = builder;
        }
        private readonly DomSqlMapBuilder _builder;

        public NameValueCollection Properties
        {
            set { _builder.Properties = value; }
        }

        public IBatisNet.DataMapper.ISqlMapper Configure(XmlDocument mapConfig)
        {
            return _builder.Configure(mapConfig);
        }
    }

    public class ProductionSqlConnectionFactoryData : ISqlConnectionFactoryData
    {
        public static ProductionSqlConnectionFactoryData GetInstance()
        {
            return new ProductionSqlConnectionFactoryData();
        }
        private ProductionSqlConnectionFactoryData()
        {
        }

        public ConnectionStringSettings ConnectionStringsSettings(string key)
        {
            return ConfigurationManager.ConnectionStrings[key];
        }

        public XmlDocument GetEmbeddedResourceAsXmlDocument(string name)
        {
            return Resources.GetEmbeddedResourceAsXmlDocument(name);
        }

        public IDomSqlMapBuilder CreateDomSqlMapBuilder()
        {
            return DomSqlMapBuilderAdapter.GetInstance(new DomSqlMapBuilder());
        }
    }

    public class SqlConnectionFactory
    {
        private const string CONNECTION_STRING = "connectionString";
        private static volatile IBatisNet.DataMapper.ISqlMapper _inventoryMapper;
        private static IBatisNet.DataMapper.ISqlMapper _dataMiningMapper;

        public static IBatisNet.DataMapper.ISqlMapper GetMapperForConnectionString(string connectionStringKey, string configName, ISqlConnectionFactoryData data)
        {
            ConnectionStringSettings settings = data.ConnectionStringsSettings(connectionStringKey);
            if (settings != null)
            {
                IDomSqlMapBuilder builder = data.CreateDomSqlMapBuilder();
                XmlDocument sqlMapConfig = data.GetEmbeddedResourceAsXmlDocument(String.Format("Configs.{0}.config,Persistence", configName));
                NameValueCollection properties = new NameValueCollection();
                properties.Add(CONNECTION_STRING, settings.ConnectionString);
                builder.Properties = properties;
                return builder.Configure(sqlMapConfig);
            }
            return null;
        }

        static SqlConnectionFactory()
        {
            ISqlConnectionFactoryData data = ProductionSqlConnectionFactoryData.GetInstance();
            _inventoryMapper = GetMapperForConnectionString("InventoryConnectionString", "sqlMap", data);            
        }

        public static IBatisNet.DataMapper.ISqlMapper GetInventoryRepository()
        {
            return _inventoryMapper;
        }
    }
}