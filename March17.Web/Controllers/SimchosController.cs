using March17.Data;
using March17.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace March17.Web.Controllers
{
    public class SimchosController : Controller
    {

        private string _connectionString = @"Data Source=.\sqlExpress;Initial Catalog=Simchos;Integrated Security=true;";
        public IActionResult Index()
        {
            DBManager mgr = new(_connectionString);
            var vm = new SimchosViewModel
            {
                Simchos = mgr.GetAllSimchos(),
                ContributorCount = mgr.GetContributorCount()
            };
            return View(vm);
        }
        [HttpPost]
        public IActionResult AddSimcha(Simcha simcha)
        {
            DBManager mgr = new(_connectionString);
            mgr.AddSimcha(simcha);
            return Redirect("/");
        }

        public IActionResult Contributions(int id)
        {
            DBManager mgr = new(_connectionString); ;
            ContributorsViewModel vm = new()
            {
                Contributors = mgr.GetContributorsBySimcha(id).OrderBy(c => c.LastName).ToList(),
                SimchaId = id,
                SimchaName = mgr.GetSimchaName(id)
            };
            return View(vm);
        }
        [HttpPost]
        public IActionResult UpdateContributions(List<Transaction> transactions, int simchaId)
        {
            transactions = transactions.FindAll(t => t.Contributed).ToList();
            DBManager mgr = new(_connectionString);
            mgr.DeleteContributions(simchaId);
            mgr.AddContributions(transactions, simchaId);
            return Redirect("/");
        }
    }
}
