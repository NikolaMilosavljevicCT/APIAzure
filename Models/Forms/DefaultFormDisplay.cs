using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.DataManagement.Models.Forms
{
    public class DefaultFormDisplay
    {
        public int Id { get; set; }
        public string FormId { get; set; }
        public string FormName { get; set; }
        public string TableName { get; set; }
        public string FieldsAttributes { get; set; }
    }
}
