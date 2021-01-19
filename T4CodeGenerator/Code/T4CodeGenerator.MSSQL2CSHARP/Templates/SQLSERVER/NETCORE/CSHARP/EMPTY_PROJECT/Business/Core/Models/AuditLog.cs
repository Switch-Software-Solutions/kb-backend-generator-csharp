
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Security.Cryptography;
using System.Text;

namespace Core.Models
{
    public partial class AuditLog
    {
        public Int32 Id { get; set; }

        public String Token { get; set; }

        public DateTime EventDate { get; set; }

        public String EventType { get; set; }

        public String TableName { get; set; }

        public String RecordValues { get; set; }

        public Byte[] TimeStamp { get; set; }

        public String HashValue { get; set; }

        public AuditLog()
        {
        }

        public AuditLog(string token, string eventType, string entityName, object entity)
        {
            Token = token;
            EventDate = DateTime.Now;
            EventType = eventType;
            TableName = entityName;
            RecordValues = JsonConvert.SerializeObject
            (
                entity, 
                Formatting.Indented,
                new JsonSerializerSettings()
                {
                    ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore
                }
            );
        }
    }
}
