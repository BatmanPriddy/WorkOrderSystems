using CW.CMMSIntegration.WorkOrderSystems.Factories;
using CW.CMMSIntegration.WorkOrderSystems.Interfaces;
using System;
using static CW.CMMSIntegration.WorkOrderSystems.Models.MCSModels;

namespace CW.CMMSIntegration.WorkOrderSystems.Services.MCS
{
    public class MCSWorkOrderConnectionService : IWorkOrderConnectionService
    {
        #region field(s)

            IWorkOrderSoapProcessor _processor;

            CMMSConfig _settings;

        #endregion

        #region constructor(s)

            public MCSWorkOrderConnectionService(CMMSConfig settings)
            {
                _processor = new WorkOrderSoapProcessorFactory().GetWorkOrderSoapProcessor();

                _settings = settings;
            }

        #endregion

        #region interface method(s)

            public string SetConnection()
            {
                var requestFields = new MCSLoginRequestFields();

                requestFields.Username = _settings.Identifier;

                requestFields.Password = _settings.Passkey;

                Action<object> setSoapBody = SetSoapBody;

                _processor.CreateSoapMessage(setSoapBody, requestFields);

                return _processor.GetResponseResult
                (
                    "https://fms.iss.biz/testmthpglobal/custom/systemintegrator/APISecurity.php",
                    nameof(MCSResponseElements.Result),
                    _processor.GetSoapMessage()
                );
            }

            void SetSoapBody(object fields)
            {
                var requestFields = (MCSLoginRequestFields)fields;

                //annoying hack. the login soap message requires the prefix "apis", but no specific namespace, so we just supply a fake one
                var loginNode = _processor.CreateElementAndAppendChild
                (
                    nameof(MCSLoginRequestElements.apis), 
                    nameof(MCSLoginRequestElements.login), 
                    "http://default/", 
                    _processor.BodyNode
                );

                _processor.CreateElementAndAppendChild(nameof(MCSLoginRequestElements.username), loginNode, requestFields.Username);

                _processor.CreateElementAndAppendChild(nameof(MCSLoginRequestElements.password), loginNode, requestFields.Password);
            }

        #endregion
    }
}