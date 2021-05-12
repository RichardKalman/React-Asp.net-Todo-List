using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TodosApplication.Model
{
    public class ItemMove
    {
        public int Id { get; set; } = -1;
        public string DestinationTableName { get; set; }
        public int DestinationIndex { get; set; } = -1;
        public int SourceIndex { get; set; } = -1;
    }
}
