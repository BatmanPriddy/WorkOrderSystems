using CW.CMMSIntegration.WorkOrderSystems.Models;
using CW.Data;

namespace CW.CMMSIntegration.WorkOrderSystems.Interfaces
{
    public interface IWorkOrderRepository
    {
        #region method(s)

            int InsertTaskRecord(TaskRecord taskRecord);

            TaskRecord GetTaskRecord(int id);

            void UpdateTaskRecord(ModelTaskRecord taskRecord);

            void SetNextClientTaskId(ModelTaskRecord taskRecord);

            void InsertWorkOrder(ModelTaskRecord taskRecord);

            Equipment GetEquipment(int id);

            CMMSConfig GetSettings(int id);

        #endregion
    }
}