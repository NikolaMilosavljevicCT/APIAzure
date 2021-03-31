using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace API.DataManagement.Models
{
    public class TableList
    {
        public int Id { get; set; }
        public string TableName { get; set; }
        public string DBMSName { get; set; }
        public string Description { get; set; }
        public string DisplayName { get; set; }
        public string DisplayGroup { get; set; }
        public string FunctionGroup { get; set; }
        public string RelAttr { get; set; }
        public string CommonName { get; set; }
        public string SortBy { get; set; }
        public string Methods { get; set; }
        public string Triggers { get; set; }
        public int? IsLocal { get; set; }
        public DateTime? LastModDate { get; set; }
        public string LastModBy { get; set; }
        public int? TableType { get; set; }
        public int? TenancyType { get; set; }
        public string SlSortBy { get; set; }
        public string SLWhere { get; set; }
        public int OOTB { get; set; }//out of the box variable -> za nas uvek true, za korisnika false
        public DateTime CreateDate { get; set; }
        public string CreatedBy { get; set; }

        [Browsable(false)]
        public TableList tableList
        {
            get
            {
                return this;
            }
        }

        public override string ToString()
        {
            return TableName;
        }
    }
}
