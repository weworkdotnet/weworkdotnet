using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WeWorkDotnet.Web.Data;
using WeWorkDotnet.Web.Models;

namespace WeWorkDotnet.Web.ViewComponents
{
    public class JobsViewComponent : ViewComponent
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<ApplicationUser> _userManager;

        public JobsViewComponent(ApplicationDbContext db, UserManager<ApplicationUser> userManager)
        {
            _db = db;
            _userManager = userManager;
        }

        public async Task<IViewComponentResult> InvokeAsync(string userName)
        {
            var jobs = new List<Job>();

            if (string.IsNullOrEmpty(userName))
            {
                jobs = await _db.Job
                    .Include(i => i.ContractType)
                    .OrderByDescending(o => o.PostedAt)
                    .ToListAsync();
            }
            else
            {
                var user = await _userManager.FindByNameAsync(userName);

                jobs = await _db.Job
                    .Include(i => i.ContractType)
                    .Where(w => w.PostedByUserId == user.Id)
                    .OrderByDescending(o => o.PostedAt)
                    .ToListAsync();
            }


            return View(jobs);
        }
    }
}
