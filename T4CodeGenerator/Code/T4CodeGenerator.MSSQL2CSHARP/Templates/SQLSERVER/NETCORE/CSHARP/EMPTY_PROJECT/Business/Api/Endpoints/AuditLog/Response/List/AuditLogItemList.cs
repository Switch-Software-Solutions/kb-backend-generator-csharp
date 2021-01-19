 
using System;
using System.Collections.Generic;

namespace Api.Endpoints.AuditLog.Response.List
{
    public class AuditLogItemList
    {
        public Int32 Id { get; set; }

        public String Token { get; set; }

        public DateTime EventDate { get; set; }

        public String EventType { get; set; }

        public String TableName { get; set; }

        public String RecordValues { get; set; }
    }
}


