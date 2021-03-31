using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.DataManagement.DTOs
{
    public class UpdateColumnDTO
    {
        public string OldColumnName { get; set; }
        public string TableName { get; set; }
        public string NewColumnName { get; set; }
        public string NewColumnType { get; set; }
        public string OldColumnType { get; set; }
    }
}
