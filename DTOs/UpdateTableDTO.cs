using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.DataManagement.DTOs
{
    public class UpdateTableDTO
    {
        public string OldTableName { get; set; }
        public string NewTableName { get; set; }
    }
}
