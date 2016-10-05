using CW.CMMSIntegration.WorkOrderSystems.Interfaces;
using CW.CMMSIntegration.WorkOrderSystems.Models;
using CW.Data;
using System.Data.Linq;
using System.Linq;

namespace CW.CMMSIntegration.WorkOrderSystems.Repositories
{
    public class WorkOrderRepository : IWorkOrderRepository
    {
        #region field(s)

            string _connStr;

        #endregion

        #region constructor(s)

            public WorkOrderRepository(string connStr) { _connStr = connStr; }

        #endregion

        #region interface method(s)

            public int InsertTaskRecord(TaskRecord taskRecord)
            {
                using (var sqlWrapper = new SqlQueryWrapper(_connStr))
                {
                    sqlWrapper.Run((db) =>
                    {
                        db.TaskRecords.InsertOnSubmit(taskRecord);
                        db.SubmitChanges();
                    });

                    return taskRecord.TaskID;
                }
            }

            public TaskRecord GetTaskRecord(int id)
            {
                using (var sqlWrapper = new SqlQueryWrapper(_connStr))
                {
                    TaskRecord taskRecord = null;

                    sqlWrapper.Run((db) =>
                    {
                        var options = new DataLoadOptions();

                        options.LoadWith<TaskRecord>(t => t.User);
                        options.LoadWith<TaskRecord>(t => t.User1);
                        options.LoadWith<TaskRecord>(t => t.Client);
                        options.LoadWith<TaskRecord>(t => t.Analyses_Equipment);
                        options.LoadWith<Analyses_Equipment>(ae => ae.Equipment);
                        options.LoadWith<Equipment>(e => e.Building);
                        options.LoadWith<Equipment>(e => e.EquipmentType);
                        options.LoadWith<EquipmentType>(et => et.EquipmentClass);
                        options.LoadWith<Building>(b => b.State);
                        options.LoadWith<Building>(b => b.Country);

                        db.LoadOptions = options;

                        taskRecord = db.TaskRecords.SingleOrDefault(t => t.TaskID == id);
                    });

                    return taskRecord;
                }
            }

            public void UpdateTaskRecord(ModelTaskRecord modelTaskRecord)
            {
                using (var sqlWrapper = new SqlQueryWrapper(_connStr))
                {
                    sqlWrapper.Run((db) =>
                    {
                        var existingTaskRecord = db.TaskRecords.Single(t => t.TaskID == modelTaskRecord.TaskId);

                        existingTaskRecord.Summary = modelTaskRecord.Summary;

                        existingTaskRecord.AssignedUID = modelTaskRecord.AssignedUid;

                        existingTaskRecord.StatusID = modelTaskRecord.StatusId;

                        existingTaskRecord.DateCompleted = modelTaskRecord.DateCompleted;

                        existingTaskRecord.Description = modelTaskRecord.Description;

                        existingTaskRecord.Recommendations = modelTaskRecord.Recommendations;

                        existingTaskRecord.Actions = modelTaskRecord.Actions;

                        existingTaskRecord.AnnualAvoidableCost = modelTaskRecord.AnnualAvoidableCost;

                        existingTaskRecord.AnnualAvoidableHeatingUse = modelTaskRecord.AnnualAvoidableHeatingUse;

                        existingTaskRecord.AnnualAvoidableCoolingUse = modelTaskRecord.AnnualAvoidableCoolingUse;

                        existingTaskRecord.AnnualAvoidableElectricUse = modelTaskRecord.AnnualAvoidableElectricUse;

                        existingTaskRecord.DateModified = modelTaskRecord.DateModified;

                        existingTaskRecord.LastModifiedUID = modelTaskRecord.LastModifiedUid;

                        if (modelTaskRecord.CreateWorkOrder) existingTaskRecord.WorkOrderID = modelTaskRecord.WorkOrderId;

                        db.SubmitChanges();
                    });
                }
            }
        
            public void SetNextClientTaskId(ModelTaskRecord taskRecord)
            {
                var clientTaskId = 1;

                using (var sqlWrapper = new SqlQueryWrapper(_connStr))
                {
                    sqlWrapper.Run((db) =>
                    {
                        var tr = db.TaskRecords.Where(_ => _.CID == taskRecord.Cid);

                        if (tr.Any()) clientTaskId = tr.Max(_ => _.ClientTaskID) + 1;
                    });

                    taskRecord.ClientTaskId = clientTaskId;
                }
            }

            public void InsertWorkOrder(ModelTaskRecord taskRecord)
            {
                using (var sqlWrapper = new SqlQueryWrapper(_connStr))
                {
                    sqlWrapper.Run((db) =>
                    {
                        var existingTaskRecord = db.TaskRecords.Single(t => t.TaskID == taskRecord.TaskId);

                        existingTaskRecord.WorkOrderID = taskRecord.WorkOrderId;

                        db.SubmitChanges();
                    });
                }
            }

            public Equipment GetEquipment(int id)
            {
                using (var sqlWrapper = new SqlQueryWrapper(_connStr))
                {
                    return sqlWrapper.Run((db) => db.Equipments.SingleOrDefault(e => e.EID == id));
                }
            }

            public CMMSConfig GetSettings(int id) => new ConfigManager(_connStr).GetConfig(ConfigTypeEnum.Building, id);
            
        #endregion
    }
}