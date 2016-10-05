using CW.CMMSIntegration.WorkOrderSystems.Interfaces;
using CW.CMMSIntegration.WorkOrderSystems.Services;
using System.Configuration;

namespace CW.CMMSIntegration.WorkOrderSystems.Factories
{
    public class WorkOrderUIServiceFactory
    {
        #region method(s)

            public IWorkOrderUIService GetWorkOrderUIService()
            {
                var connStr = ConfigurationManager.ConnectionStrings["CWConnectionString"].ConnectionString;

                return new WorkOrderUIService(connStr);
            }

        #endregion
    }
}