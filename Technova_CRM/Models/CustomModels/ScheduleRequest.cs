namespace Technova_CRM.Models.CustomModels
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    // ScheduleRequest class to represent the request body for scheduling the campaign
    public class ScheduleRequest
    {
        public string ScheduleTime { get; set; } // Required: in ISO 8601 format (e.g., "2024-10-17T12:00:00+00:00")
        public bool? Timewarp { get; set; } // Optional
    }
}
