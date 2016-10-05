using CW.CMMSIntegration.WorkOrderSystems.Interfaces;

namespace CW.CMMSIntegration.WorkOrderSystems.Services
{
    public class WorkOrderUIService : IWorkOrderUIService
    {
        #region field(s)

            ConfigManager _configManager;

        #endregion

        #region constructor(s)

            public WorkOrderUIService(string connStr) { _configManager = new ConfigManager(connStr); }

        #endregion

        #region interface method(s)
        
            public CMMSConfig GetSettings(ConfigTypeEnum type, int id) => _configManager.GetConfig(type, id);

            public WoTypeMap[] GetWoTypes() => _configManager.GetWoTypes();
            
            public WoTypeMap[] GetWoTypeMappings(CMMSConfig settings) => _configManager.GetWoTypeMappings(settings);
        
        #endregion
    }
}