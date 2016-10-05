namespace CW.CMMSIntegration.WorkOrderSystems.Models
{
    public struct CMMSConfigSettings
    {
        //Not used yet, need to simplify ConfigManager code to leverage this model
        #region properties

            public bool IsEnabled { get; set; }

            public ConfigTypeEnum ConfigType { get; set; }

            public int ConfigId { get; set; }

            public int VendorId { get; set; }

            public string Username { get; set; }

            public string Password { get; set; }

            public string Notes { get; set; }

            public string Version { get; set; }

            public string[] WoStatuses { get; set; }

            public string[] WoTypes { get; set; }

        #endregion
    }
}