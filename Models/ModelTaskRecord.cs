using CW.Business;
using CW.Data;
using CW.Logging;
using System;

namespace CW.CMMSIntegration.WorkOrderSystems.Models
{
    public class ModelTaskRecord
    {
        #region enum(s)

            public enum DateType { CreatedOrCompleted, Modified }

        #endregion

        #region properties

            public int Bid { get; set; }

            public int Eid { get; set; }

            public int Aid { get; set; }

            public int TaskId { get; set; }

            public int ClientTaskId { get; set; }

            public string Summary { get; set; }

            public int Aeid { get; set; }

            public int? ReporterUid { get; set; }

            public string ReporterEmail { get; set; }

            public int? AssignedUid { get; set; }

            public string AssigneeEmail { get; set; }

            public string SecondaryEmail { get; set; }

            public DateTime AnalysisStartDate { get; set; }

            public string Interval { get; set; }

            public string Description { get; set; }

            public string Recommendations { get; set; }

            public string Actions { get; set; }

            public int LastModifiedUid { get; set; }

            public int StatusId { get; set; }

            public DateTime? DateCompleted { get; set; }

            public DateTime DateModified { get; set; }

            public DateTime DateCreated { get; set; }

            public string Email { get; set; }

            public string FullName { get; set; }

            public bool IsSchneiderTheme { get; set; }

            public string CultureName { get; set; }

            public int Uid { get; set; }

            public int Cid { get; set; }

            public int WorkOrderTypeId { get; set; }

            public string WorkOrderType { get; set; }

            public string WorkOrderId { get; set; }

            public bool CreateWorkOrder { get; set; }
            
            public string WorkOrderErrorMessage { get; set; }

            public bool WorkOrderCreated { get; set; }

            public bool RecalculateMetrics { get; set; }

            public string AnnualAvoidableCost { get; set; }

            public string AnnualAvoidableHeatingUse { get; set; }

            public string AnnualAvoidableCoolingUse { get; set; }

            public string AnnualAvoidableElectricUse { get; set; }

            public string TimeZone { get; set; }

            public string TaskMessage { get; set; }

            public string EmailSubject { get; set; }

            public string EmailBodyTop { get; set; }

            public bool EmailSent { get; set; }

            public string EmailFailedMessage { get; set; }

            public bool IsTaskCompleted { get; set; }

            public bool HasTaskChanged { get; set; }

            public User User { get; set; }

            public User User1 { get; set; }

            public DataManager DataMgr { get; set; }

            public Client Client { get; set; }

            public Building Building { get; set; }

            public Equipment Equipment { get; set; }

            public EquipmentClass EquipmentClass { get; set; }

            public Analyse Analyse { get; set; }

            public Status Status { get; set; }

            public State State { get; set; }

            public Country Country { get; set; }

            public ISectionLogManager LogMgr { get; set; }

        #endregion
    }
}