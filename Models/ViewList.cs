using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace API.DataManagement.Models
{
    public class ViewList
    {
        public string Name { get; set; }
        public DateTime CreateDate { get; set; }
        public string CreatedBy { get; set; }
        public DateTime LastModDate { get; set; }

        [Browsable(false)]
        public ViewList viewList
        {
            get
            {
                return this;
            }
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
