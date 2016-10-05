using CW.CMMSIntegration.WorkOrderSystems.Interfaces;
using System;
using System.Linq;
using System.Xml;
using System.Xml.Linq;

namespace CW.CMMSIntegration.WorkOrderSystems
{
    //Base class for generation/processing of Work Orders using SOAP messaging
    public sealed class WorkOrderSoapProcessor : IWorkOrderSoapProcessor
    {
        #region const(s)

            const string SOAPENV_NS = "http://schemas.xmlsoap.org/soap/envelope/"; //this is the default schema

            const string SOAPENV = "soapenv"; //this is the default xml tag prefix

        #endregion

        #region enum(s)

            enum ErrorMessageNode { faultstring }

            enum SoapMessageFields { Envelope, Header, Body }

        #endregion

        #region interface properties

            public XmlDocument XmlDoc { get; set; }

            public XmlElement EnvelopeNode { get; set; }

            public XmlElement BodyNode { get; set; }

        #endregion

        #region interface method(s)

            public void CreateSoapMessage(Action<object> setSoapBody, object fields)
            {
                XmlDoc = new XmlDocument();

                var xmlDeclaration = XmlDoc.CreateXmlDeclaration("1.0", "utf-8", null);

                EnvelopeNode = XmlDoc.CreateElement(SOAPENV, nameof(SoapMessageFields.Envelope), SOAPENV_NS);

                var headerNode = XmlDoc.CreateElement(SOAPENV, nameof(SoapMessageFields.Header), SOAPENV_NS);

                BodyNode = XmlDoc.CreateElement(SOAPENV, nameof(SoapMessageFields.Body), SOAPENV_NS);

                setSoapBody(fields);

                XmlDoc.AppendChild(EnvelopeNode);

                EnvelopeNode.AppendChild(headerNode);

                EnvelopeNode.AppendChild(BodyNode);
            }

            public string GetSoapMessage() => XmlDoc.InnerXml;

            //There may be additional namespaces required for the soap message, this method handles dynamically adding them
            public void AddEnvelopeNodeNamespace(string prefix, string ns) => EnvelopeNode.SetAttribute($"xmlns:{prefix}", ns);

            //The methods below are overloaded to allow creation of an element with or without a prefix           
            public XmlElement CreateElementAndAppendChild(string prefix, string element, string ns, XmlElement node, string value = "")
            {
                var xmlElement = XmlDoc.CreateElement(prefix, element, ns);

                if (!string.IsNullOrWhiteSpace(value)) xmlElement.InnerText = value.Trim();

                node.AppendChild(xmlElement);

                return xmlElement;
            }
               
            public XmlElement CreateElementAndAppendChild(string element, XmlElement node, string value = "")
            {
                var xmlElement = XmlDoc.CreateElement(element);

                if (!string.IsNullOrWhiteSpace(value)) xmlElement.InnerText = value.Trim();

                node.AppendChild(xmlElement);

                return xmlElement;
            }            
           
            public string GetResponseResult(string uri, string resultFieldName, string soapMessage)
            {
                var xml = "text/xml";

                var processor = new MessageProcessor(xml, $"{xml}, text/html", "POST");

                var request = processor.CreateRequest(uri, soapMessage);

                var responseText = string.Empty;

                //Try catch here is necessary since there might have been a problem processing the request in MessageProcessor
                //right now we only deal with invalid uris and 500 server errors (to catch error message for user and bubble up)
                try
                {
                    var response = processor.ProcessRequest(request);

                    responseText = processor.ProcessResponse(response);
                }
                catch (Exception ex)
                {
                    //an unspecified error was set in the exception data object (if there was one), so we simply display a
                    //generic message to user. otherwise we must read the error from the response returned to us by the work
                    //order system.
                    var errorTxt = "Error:";

                    if (ex.Data.Count > 0) return $"{errorTxt} {ex.Data["genericError"]}";

                    return $"{errorTxt} {GetFieldValue(ex.Message, nameof(ErrorMessageNode.faultstring))}";
                }
            
                return GetFieldValue(responseText, resultFieldName);
            }

        #endregion

        #region method(s)

            string GetFieldValue(string xml, string fieldName)
            {
                if (string.IsNullOrWhiteSpace(xml)) return null;

                var doc = XDocument.Parse(xml);

                var descendents = doc.Root.Descendants();

                if (descendents.Count() == 0) return null;

                var item = descendents.Where(e => e.Name.LocalName == fieldName);

                if (!item.Any()) return null;

                return item.First().Value;
            }

        #endregion
    }
}