using CW.CMMSIntegration.WorkOrderSystems.Interfaces;
using CW.CMMSIntegration.WorkOrderSystems.Services.MCS;

namespace CW.CMMSIntegration.WorkOrderSystems.Factories
{
    public class WorkOrderConnectionServiceFactory
    {
        #region method(s)

            public IWorkOrderConnectionService GetWorkOrderConnectionService(CMMSConfig settings)
            {
                switch (settings.VendorId)
                {
                    case 3:
                        return new MCSWorkOrderConnectionService(settings);
                    default:
                        return null;
                }
            }

        #endregion
    }
}