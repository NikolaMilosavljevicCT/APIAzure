using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.DataManagement.DTOs.Views
{
    public class AlterViewDTO
    {
        public string ViewName { get; set; }
        public string Table1 { get; set; }
        public string Table2 { get; set; }
        public List<string> ListOfFieldsTable1 { get; set; }
        public List<string> ListOfFieldsTable2 { get; set; }
        public string Condition { get; set; }
    }
}
