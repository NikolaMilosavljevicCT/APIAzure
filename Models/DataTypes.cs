using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace API.DataManagement.Models
{
    public class DataTypes
    {
        
        public int Id { get; set; }
        public string TypeName { get; set; }
        public int Enum { get; set; }
        public string SqlDataType { get; set; }
        public int? AttrMaxLength { get; set; }
        public int? AttrMaxSize { get; set; }

    }
}
