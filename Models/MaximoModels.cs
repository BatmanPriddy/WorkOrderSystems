namespace CW.CMMSIntegration.WorkOrderSystems.Models
{
    public static class MaximoModels
    {
        #region enum(s)

            //Element casing needs to match the format for the outgoing/incoming soap request/response
            public enum MaximoRequestElements
            {
                CreateKGSWO,
                KGSWOSet,
                WORKORDER,
                ASSETNUM,
                DESCRIPTION,
                LOCATION,
                DESCRIPTION_LONGDESCRIPTION,
                ONBEHALFOF,
                REPORTEDBY,
                SITEID
            }

            public enum MaximoResponseElements { SITEID, WONUM, WORKORDERID }

        #endregion

        #region struct(s)

            public struct MaximoRequestFields
            {
                public string AssetId;

                public string Description;

                public string LocationId;

                public string LongDescription;

                public string OnBehalfOf;

                public string ReportedBy;

                public string SiteId;
            }

        #endregion
    }
}