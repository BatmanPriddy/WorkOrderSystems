using CW.CMMSIntegration.WorkOrderSystems.Factories;
using CW.CMMSIntegration.WorkOrderSystems.Interfaces;
using CW.CMMSIntegration.WorkOrderSystems.Models;
using System;
using static CW.CMMSIntegration.WorkOrderSystems.Models.MaximoModels;

namespace CW.CMMSIntegration.WorkOrderSystems.Services.Maximo
{
    public class MaximoWorkOrderService : IWorkOrderService
    {
        #region const(s)

            const string MAX_NS = "http://www.ibm.com/maximo";

            const string MAX = "max";

        #endregion

        #region field(s)

            IWorkOrderRepository _repo;

            IWorkOrderSoapProcessor _processor;

            WoTask _woTask;

            ModelTaskRecord _taskRecord;

        #endregion

        #region constructor(s)

            public MaximoWorkOrderService(IWorkOrderRepository repo, WoTask woTask, ModelTaskRecord taskRecord)
            {
                _repo = repo;

                _processor = new WorkOrderSoapProcessorFactory().GetWorkOrderSoapProcessor();

                _woTask = woTask;

                _taskRecord = taskRecord;
            }

        #endregion
        
        #region interface method(s)

            public void RunWorkOrderService()
            {
                var workOrderId = CreateWorkOrder();

                if (!string.IsNullOrWhiteSpace(_taskRecord.WorkOrderErrorMessage)) return;

                _taskRecord.WorkOrderId = workOrderId;

                _repo.InsertWorkOrder(_taskRecord);

                _taskRecord.WorkOrderCreated = true;
            }

        #endregion

        #region method(s)

            string CreateWorkOrder()
            {
                var requestFields = GetRequestValues();

                Action<object> setSoapBody = SetSoapBody;

                _processor.CreateSoapMessage(setSoapBody, requestFields);

                var result = _processor.GetResponseResult
                (
                    "http://184.173.184.203:9080/meaweb/services/KGS_CLOCKWORKS_KGSCreateWO",
                    nameof(MaximoResponseElements.WORKORDERID),
                    _processor.GetSoapMessage()
                );

                if (result.Contains("Error")) _taskRecord.WorkOrderErrorMessage = result;

                return result;
            }

            MaximoRequestFields GetRequestValues()
            {
                //below are the minimum required fields for a Maximo Work Order to be generated
                var requestFields = new MaximoRequestFields();

                var assetId = _woTask.Equipment.CMMSIdC;

                if (!string.IsNullOrWhiteSpace(assetId)) requestFields.AssetId = assetId; //need to locate a valid asset id

                requestFields.Description = _woTask.Description.Trim();

                var locationId = _woTask.Equipment.CMMSIdB;

                if (!string.IsNullOrWhiteSpace(locationId)) requestFields.LocationId = locationId; //need to locate a valid location id

                requestFields.LongDescription = $"{_woTask.Description}\n{_woTask.Recommendations}\n{_woTask.Actions}";

                requestFields.OnBehalfOf = "WILSON";

                requestFields.ReportedBy = "MAXADMIN";

                requestFields.SiteId = _woTask.Equipment.CMMSIdA;

                return requestFields;
            }

            void SetSoapBody(object requestFields)
            {
                _processor.AddEnvelopeNodeNamespace(MAX, MAX_NS); //additional namespace "max" is necessary for the Maximo soap request message

                var fields = (MaximoRequestFields)requestFields;

                var createKGSWONode = _processor.CreateElementAndAppendChild(MAX, nameof(MaximoRequestElements.CreateKGSWO), MAX_NS, _processor.BodyNode);

                var KGSWOSetNode = _processor.CreateElementAndAppendChild(MAX, nameof(MaximoRequestElements.KGSWOSet), MAX_NS, createKGSWONode);

                var workOrderNode = _processor.CreateElementAndAppendChild(MAX, nameof(MaximoRequestElements.WORKORDER), MAX_NS, KGSWOSetNode);

                _processor.CreateElementAndAppendChild(MAX, nameof(MaximoRequestElements.ASSETNUM), MAX_NS, workOrderNode, fields.AssetId);

                _processor.CreateElementAndAppendChild(MAX, nameof(MaximoRequestElements.DESCRIPTION), MAX_NS, workOrderNode, fields.Description);

                _processor.CreateElementAndAppendChild(MAX, nameof(MaximoRequestElements.LOCATION), MAX_NS, workOrderNode, fields.LocationId);

                _processor.CreateElementAndAppendChild(MAX, nameof(MaximoRequestElements.DESCRIPTION_LONGDESCRIPTION), MAX_NS, workOrderNode, fields.LongDescription);

                _processor.CreateElementAndAppendChild(MAX, nameof(MaximoRequestElements.ONBEHALFOF), MAX_NS, workOrderNode, fields.OnBehalfOf);

                _processor.CreateElementAndAppendChild(MAX, nameof(MaximoRequestElements.REPORTEDBY), MAX_NS, workOrderNode, fields.ReportedBy);

                _processor.CreateElementAndAppendChild(MAX, nameof(MaximoRequestElements.SITEID), MAX_NS, workOrderNode, fields.SiteId);
            }

        #endregion
    }
}