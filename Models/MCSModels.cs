namespace CW.CMMSIntegration.WorkOrderSystems.Models
{
    public static class MCSModels
    {
        //Element casing needs to match the format for the outgoing/incoming soap request/response
        #region enum(s)

            public enum MCSLoginRequestElements { apis, login, username, password }

            public enum MCSWorkOrderRequestElements { insertOrUpdateWorkorder, sessionId, workorder, reference, type, priority, severity, nature, location, description, id }

            public enum MCSResponseElements { Result } //the element is the same for both the login and work order responses
        
        #endregion

        #region struct(s)

            public struct MCSLoginRequestFields
            {
                public string Username;

                public string Password;
            }

            public struct MCSWorkOrderRequestFields
            {
                public string SessionId;

                public string StatusId;

                public string Reference;

                public string TypeId;

                public string PriorityId;

                public string SeverityId;

                public string NatureId;

                public string LocationId;

                public string Description;
            }

        #endregion
    }
}