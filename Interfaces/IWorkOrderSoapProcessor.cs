using System;
using System.Xml;

namespace CW.CMMSIntegration.WorkOrderSystems.Interfaces
{
    public interface IWorkOrderSoapProcessor
    {
        #region properties

            XmlDocument XmlDoc { get; set; }

            XmlElement EnvelopeNode { get; set; }

            XmlElement BodyNode { get; set; }

        #endregion

        #region method(s)

            void CreateSoapMessage(Action<object> setSoapBody, object fields);

            string GetSoapMessage();
        
            void AddEnvelopeNodeNamespace(string prefix, string ns);

            XmlElement CreateElementAndAppendChild(string prefix, string element, string ns, XmlElement node, string value = "");

            XmlElement CreateElementAndAppendChild(string element, XmlElement node, string value = "");

            string GetResponseResult(string uri, string resultFieldName, string soapMessage);

        #endregion
    }
}
