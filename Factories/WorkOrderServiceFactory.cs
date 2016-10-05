using CW.CMMSIntegration.WorkOrderSystems.Interfaces;
using CW.CMMSIntegration.WorkOrderSystems.Models;
using CW.CMMSIntegration.WorkOrderSystems.Services.Maximo;
using CW.CMMSIntegration.WorkOrderSystems.Services.MCS;
using System.Configuration;

namespace CW.CMMSIntegration.WorkOrderSystems.Factories
{
    public class WorkOrderServiceFactory
    {
        #region method(s)

            public IWorkOrderService GetWorkOrderService(WoTask woTask, ModelTaskRecord taskRecord)
            {
                var connStr = ConfigurationManager.ConnectionStrings["CWConnectionString"].ConnectionString;

                var _repo = new WorkOrderRepositoryFactory().GetWorkOrderRepository(connStr);

                var settings = _repo.GetSettings(taskRecord.Bid);

                switch (settings.VendorId)
                {
                    case 1:
                        return null;
                    case 2:
                        return new MaximoWorkOrderService(_repo, woTask, taskRecord);
                    case 3:
                        return new MCSWorkOrderService(_repo, settings, woTask, taskRecord);
                    default:
                        return null;
                }
            }

        #endregion
    }
}