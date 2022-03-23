using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace March17.Data
{
    public class Contributor
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string CellNumber { get; set; }
        public bool AlwaysInclude { get; set; }
        public decimal Balance { get; set; }
        public DateTime Date { get; set; }
        public bool Contributed { get; set; }
        public decimal AmountContributed { get; set; } = 5;
    }
}
