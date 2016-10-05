using CW.CMMSIntegration.WorkOrderSystems.Interfaces;

namespace CW.CMMSIntegration.WorkOrderSystems.Factories
{
    public class WorkOrderSoapProcessorFactory
    {
        #region method(s)

            public IWorkOrderSoapProcessor GetWorkOrderSoapProcessor() => new WorkOrderSoapProcessor();

        #endregion
    }
}