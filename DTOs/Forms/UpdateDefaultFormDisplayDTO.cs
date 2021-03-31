using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.DataManagement.DTOs.Forms
{
    public class UpdateDefaultFormDisplayDTO
    {
        public string FormName { get; set; }
        public string FieldsAttributes { get; set; }
    }
}