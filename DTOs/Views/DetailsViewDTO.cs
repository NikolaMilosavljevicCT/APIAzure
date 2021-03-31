using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.DataManagement.DTOs.Views
{
    public class DetailsViewDTO
    {
        public string FirstTable { get; set; }
        public string SecondTable { get; set; }
        public Dictionary<string, bool> FirstTableColumns { get; set; }
        public Dictionary<string, bool> SecondTableColumns { get; set; }

    }
}
