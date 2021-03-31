using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace API.DataManagement.Models
{
    public class ColumnsList
    {
        [Browsable(false)]
        public int Id { get; set; }
        [Browsable(false)]
        public string TableName { get; set; }
        public string ColumnName { get; set; }
        [Browsable(false)]
        public string SchemaName { get; set; }
        [Browsable(false)]
        public string DBMSName { get; set; }
        [Browsable(false)]
        public string Description { get; set; }
        [Browsable(false)]
        public string DisplayName { get; set; }
        [Browsable(false)]
        public int? Type { get; set; }
        [Browsable(false)]
        public int? MaxStringLength { get; set; }
        [Browsable(false)]
        public string XRELTable { get; set; }
        [Browsable(false)]
        public string ADDLInfo { get; set; }
        [Browsable(false)]
        public string OnNewDefault { get; set; }
        [Browsable(false)]
        public string OnCiSet { get; set; }
        [Browsable(false)]
        public int? ISIndexed { get; set; }
        [Browsable(false)]
        public int? ISUnique { get; set; }
        [Browsable(false)]
        public int? ISLocal { get; set; }
        [Browsable(false)]
        public int? ISNotNull { get; set; }
        [Browsable(false)]
        public int? ISRequired { get; set; }
        [Browsable(false)]
        public int? ISWriteNew { get; set; }
        [Browsable(false)]
        public DateTime? LastModDate { get; set; }
        [Browsable(false)]
        public string LastModBy { get; set; }
        [Browsable(false)]
        public int? ISServProv { get; set; }
        [Browsable(false)]
        public int? ServProvCode { get; set; }
        [Browsable(false)]
        public string UiInfo { get; set; }
        [Browsable(false)]
        public DateTime CreateDate { get; set; }
        [Browsable(false)]
        public string CreatedBy { get; set; }

        [Browsable(false)]
        public ColumnsList columnsList
        {
            get
            {
                return this;
            }
        }

        public override string ToString()
        {
            return ColumnName;
        }
    }
}
