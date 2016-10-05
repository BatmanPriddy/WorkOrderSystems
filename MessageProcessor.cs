using System.IO;
using System.Net;

namespace CW.CMMSIntegration.WorkOrderSystems
{
    //This class handles processing of data based on the uri supplied as well as request headers for outbound request
    public sealed class MessageProcessor
    {
        #region field(s)

            string _contentType, _accept, _method;

        #endregion

        #region constructor(s)

            public MessageProcessor(string contentType, string accept, string method)
            {
                _contentType = contentType;

                _accept = accept;

                _method = method;
            }

        #endregion
              
        #region method(s)

            public HttpWebRequest CreateRequest(string uri, string soapMsg)
            {
                var request = (HttpWebRequest)WebRequest.Create(uri);

                request.ContentType = _contentType;

                request.Accept = _accept;

                request.Method = _method;

                request.ContentLength = soapMsg.Length;

                using (var stream = request.GetRequestStream())
                {
                    using (var sw = new StreamWriter(stream))
                        sw.Write(soapMsg);
                }

                return request;
            }

            public WebResponse ProcessRequest(HttpWebRequest request)
            {
                //Try catch here is necessary since there might be a problem with the end point
                //right now we only deal with invalid uris and 500 server errors (to catch error message for user and bubble up)
                //perhaps we may add more later
                try
                {
                    return request.GetResponse(); //Perhaps add some retries?
                }
                catch (WebException ex)
                {
                    var webResponse = (HttpWebResponse)ex.Response;

                    if (webResponse.StatusCode == HttpStatusCode.NotFound)
                        throw new WebException($"{webResponse.ResponseUri} is an invalid Url");

                    if (webResponse.StatusCode == HttpStatusCode.InternalServerError)
                    {
                        var errorResponse = ProcessResponse(ex.Response);

                        throw new WebException(errorResponse);
                    }

                    //catch the generic error separately, so we can inform the user with a generic error message
                    ex.Data.Add("genericError", "Unspecified Error. Please contact administrator");

                    throw;
                }
            }

            public string ProcessResponse(WebResponse response)
            {
                var responseStream = response.GetResponseStream();

                var responseText = string.Empty;

                using (var reader = new StreamReader(responseStream))
                    responseText = reader.ReadToEnd();

                return responseText;
            }

        #endregion
    }
}