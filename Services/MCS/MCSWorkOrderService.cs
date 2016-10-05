using CW.CMMSIntegration.WorkOrderSystems.Factories;
using CW.CMMSIntegration.WorkOrderSystems.Interfaces;
using CW.CMMSIntegration.WorkOrderSystems.Models;
using System;
using static CW.CMMSIntegration.WorkOrderSystems.Models.MCSModels;

namespace CW.CMMSIntegration.WorkOrderSystems.Services.MCS
{
    public sealed class MCSWorkOrderService : IWorkOrderService
    {
        #region const(s)

            const string APIW_NS = "http://localhost/SystemIntegrator/APIWorkorders.php";

            const string APIW = "apiw";

            const string MCS_NS = "MCSeFMSWorkordersAPI";

            const string MCS = "mcs";

        #endregion

        #region field(s)

            IWorkOrderRepository _repo;

            IWorkOrderSoapProcessor _processor;

            CMMSConfig _settings;

            WoTask _woTask;

            ModelTaskRecord _taskRecord;

        #endregion
        
        #region constructor(s)

            public MCSWorkOrderService(IWorkOrderRepository repo, CMMSConfig settings, WoTask woTask, ModelTaskRecord taskRecord)
            {
                _repo = repo;
            
                _processor = new WorkOrderSoapProcessorFactory().GetWorkOrderSoapProcessor();

                _settings = settings;

                _woTask = woTask;

                _taskRecord = taskRecord;
            }

        #endregion

        #region interface method(s)
        
            public void RunWorkOrderService()
            {
                var connResult = SetConnection();

                if (!string.IsNullOrWhiteSpace(_taskRecord.WorkOrderErrorMessage)) return;
                
                var workOrderId = CreateWorkOrder(connResult);

                if (!string.IsNullOrWhiteSpace(_taskRecord.WorkOrderErrorMessage)) return;
                
                _taskRecord.WorkOrderId = workOrderId;

                _repo.InsertWorkOrder(_taskRecord);

                _taskRecord.WorkOrderCreated = true;
            }

        #endregion

        #region method(s)

            string SetConnection()
            {
                var connectionSvc = new WorkOrderConnectionServiceFactory().GetWorkOrderConnectionService(_settings);

                var result = connectionSvc.SetConnection();

                SetMessageIfError(result);    

                return result;
            }

            string CreateWorkOrder(string sessionId)
            {
                var requestFields = GetRequestValues(sessionId);

                Action<object> setSoapBody = SetSoapBody;

                 _processor.CreateSoapMessage(setSoapBody, requestFields);

                var result = _processor.GetResponseResult
                (
                    "https://fms.iss.biz/testmthpglobal/custom/systemintegrator/APIWorkorders.php", 
                    nameof(MCSResponseElements.Result), 
                    _processor.GetSoapMessage()
                );

                SetMessageIfError(result);

                return result;
            }

            void SetMessageIfError(string result) { if (result.Contains("Error")) _taskRecord.WorkOrderErrorMessage = result; }

            MCSWorkOrderRequestFields GetRequestValues(string sessionId)
            {
                //below are the minimum required fields for an MCS Work Order to be generated
                var requestFields = new MCSWorkOrderRequestFields();

                requestFields.SessionId = sessionId;

                //TODO: decide what should be the default values for these fields
                requestFields.TypeId = "0000000002"; //means "Repair Equipment" on the MCS system

                requestFields.PriorityId = "0000000004"; //means "P3-Regular Work" on the MCS system

                requestFields.SeverityId = "0000000001"; //means "Default Severity" on the MCS system

                requestFields.NatureId = "0000000256"; //means "Other/Issue Request" on the MCS system

                var locationId = _woTask.Equipment.CMMSIdB;

                if (!string.IsNullOrWhiteSpace(locationId)) requestFields.LocationId = locationId; //need to find a valid location id

                requestFields.Description = _woTask.Description.Trim();

                return requestFields;
            }

            void SetSoapBody(object fields)
            {
                var requestFields = (MCSWorkOrderRequestFields)fields;

                _processor.AddEnvelopeNodeNamespace(APIW, APIW_NS); //additional namespace "apiw" is necessary for the MCS soap request message

                _processor.AddEnvelopeNodeNamespace(MCS, MCS_NS); //additional namespace "mcs" is necessary for the MCS soap request message

                var insertOrUpdateWorkOrderNode = _processor.CreateElementAndAppendChild
                (
                    APIW,
                    nameof(MCSWorkOrderRequestElements.insertOrUpdateWorkorder),
                    APIW_NS,
                    _processor.BodyNode
                );

                _processor.CreateElementAndAppendChild(nameof(MCSWorkOrderRequestElements.sessionId), insertOrUpdateWorkOrderNode, requestFields.SessionId);

                var workOrderNode = _processor.CreateElementAndAppendChild(nameof(MCSWorkOrderRequestElements.workorder), insertOrUpdateWorkOrderNode);

                _processor.CreateElementAndAppendChild(MCS, nameof(MCSWorkOrderRequestElements.reference), MCS_NS, workOrderNode, "?"); //need better default than "?"

                var typeNode = _processor.CreateElementAndAppendChild(MCS, nameof(MCSWorkOrderRequestElements.type), MCS_NS, workOrderNode);

                _processor.CreateElementAndAppendChild(MCS, nameof(MCSWorkOrderRequestElements.id), MCS_NS, typeNode, requestFields.TypeId);

                var priorityNode = _processor.CreateElementAndAppendChild(MCS, nameof(MCSWorkOrderRequestElements.priority), MCS_NS, workOrderNode);

                _processor.CreateElementAndAppendChild(MCS, nameof(MCSWorkOrderRequestElements.id), MCS_NS, priorityNode, requestFields.PriorityId);

                var severityNode = _processor.CreateElementAndAppendChild(MCS, nameof(MCSWorkOrderRequestElements.severity), MCS_NS, workOrderNode);

                _processor.CreateElementAndAppendChild(MCS, nameof(MCSWorkOrderRequestElements.id), MCS_NS, severityNode, requestFields.SeverityId);

                var natureNode = _processor.CreateElementAndAppendChild(MCS, nameof(MCSWorkOrderRequestElements.nature), MCS_NS, workOrderNode);

                _processor.CreateElementAndAppendChild(MCS, nameof(MCSWorkOrderRequestElements.id), MCS_NS, natureNode, requestFields.NatureId);

                var locationId = requestFields.LocationId;

                if (!string.IsNullOrWhiteSpace(locationId))
                {
                    var locationNode = _processor.CreateElementAndAppendChild(MCS, nameof(MCSWorkOrderRequestElements.location), MCS_NS, workOrderNode);

                    _processor.CreateElementAndAppendChild(MCS, nameof(MCSWorkOrderRequestElements.id), MCS_NS, locationNode, locationId);
                }

                var description = requestFields.Description;

                if (!string.IsNullOrWhiteSpace(description))
                    _processor.CreateElementAndAppendChild(MCS, nameof(MCSWorkOrderRequestElements.description), MCS_NS, workOrderNode, description);
            }

        #endregion
    }
}