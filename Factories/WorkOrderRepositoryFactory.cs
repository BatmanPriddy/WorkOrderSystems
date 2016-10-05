using CW.CMMSIntegration.WorkOrderSystems.Interfaces;
using CW.CMMSIntegration.WorkOrderSystems.Repositories;

namespace CW.CMMSIntegration.WorkOrderSystems.Factories
{
    public class WorkOrderRepositoryFactory
    {
        #region method(s)

            public IWorkOrderRepository GetWorkOrderRepository(string connStr) => new WorkOrderRepository(connStr);
        
        #endregion
    }
}