using March17.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace March17.Web.Models
{
    public class HistoryViewModel
    {
        public List<Transaction> Transactions { get; set; }
        public string Name { get; set; }        
        public decimal Balance { get; set; }
    }
}
