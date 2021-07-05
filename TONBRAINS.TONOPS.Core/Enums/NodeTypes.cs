using System.ComponentModel;

namespace TONBRAINS.TONOPS.Core.Handlers
{
    /// <summary>
    /// Types node
    /// </summary>
    public enum NodeTypes
    {
        [Description("Undefined")]
        Undefined = 1,
        [Description("Zabbix Server")]
        ZabbixServer = 2,
        [Description("ElasticSearch + Kibana")]
        ElasticSearcjKibana = 3, 
        [Description("QUANTON Ready")]
        QUANTONReady = 4,
        [Description("QUANTON Active")]
        QUANTONActive = 5,
    }
}
