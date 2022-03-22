using March17.Data;
using March17.Web.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace March17.Web.Controllers
{
    public class ContributorsController : Controller
    {
        private string _connectionString = @"Data Source=.\sqlExpress;Initial Catalog=Simchos;Integrated Security=true;";
        public IActionResult Index()
        {
            DBManager mgr = new(_connectionString);
            ContributorsViewModel vm = new()
            {
                Contributors = mgr.GetContributors()
            };
            return View(vm);
        }
        public IActionResult AddContributor(Contributor c, Transaction t)
        {
            DBManager mgr = new(_connectionString);
            int id = mgr.AddContributor(c);
            t.ContributorId = id;
            mgr.AddDeposit(t);
            return Redirect("/contributors");
        }
        public IActionResult AddDeposit(Transaction t)
        {
            DBManager mgr = new(_connectionString);
            mgr.AddDeposit(t);
            return Redirect("/contributors");
        }
        public IActionResult History(int id)
        {
            DBManager mgr = new(_connectionString);
            HistoryViewModel vm = new()
            {
                Transactions = mgr.GetTransactionsById(id).OrderByDescending(t => t.Date).ToList(),
                Name = mgr.GetContributorName(id)
            };
            vm.Balance = vm.Transactions.Sum(t => t.Amount);
            return View(vm);
        }
        public IActionResult Edit(Contributor c)
        {
            DBManager mgr = new(_connectionString);
            mgr.EditContributor(c);
            return Redirect("/contributors");
        }
    }
}
