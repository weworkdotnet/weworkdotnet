using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using WeWorkDotnet.Web.Data;
using WeWorkDotnet.Web.Models;

namespace WeWorkDotnet.Web.ViewComponents
{
    public class JobsViewComponent : ViewComponent
    {
        private readonly ApplicationDbContext _db;

        public JobsViewComponent(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var jobs = await _db.Set<Job>().Include(i => i.ContractType).OrderByDescending(o => o.PostedAt).ToListAsync();
            return View(jobs);
        }
    }
}
