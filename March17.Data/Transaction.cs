using System;

namespace March17.Data
{
    public class Transaction
    {
        public int ContributorId { get; set; }
        public int SimchaId { get; set; }
        public string Name { get; set; }
        public DateTime Date { get; set; }
        public decimal Amount { get; set; }
        public bool Contributed { get; set; }
    }
}
