namespace CW.CMMSIntegration.WorkOrderSystems.Interfaces
{
    public interface IWorkOrderUIService
    {
        #region method(s)

            CMMSConfig GetSettings(ConfigTypeEnum type, int id);

            WoTypeMap[] GetWoTypes();

            WoTypeMap[] GetWoTypeMappings(CMMSConfig settings);

        #endregion
    }
}