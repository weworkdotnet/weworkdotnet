using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using WeWorkDotnet.Web.Data;
using WeWorkDotnet.Web.Models;

namespace WeWorkDotnet.Web.Controllers
{
    public class JobsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public JobsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var job = await _context.Job.Include(j => j.ContractType).SingleOrDefaultAsync(m => m.Id == id);
            if (job == null)
            {
                return NotFound();
            }

            return View(job);
        }

        [Authorize]
        public IActionResult Create()
        {
            PopulateContractTypes();
            return View();
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,ContractTypeId,IsRemote,IsVisaSponsor,PostedAt,Title,Company,Location,Description,ExternalUrl,Contact")] Job job)
        {
            var user = await _userManager.FindByNameAsync(HttpContext.User.Identity.Name);

            job.PostedByUserId = user.Id;
            job.PostedAt = DateTime.Now;

            Validate(ModelState, job);

            if (ModelState.IsValid)
            {
                _context.Add(job);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            PopulateContractTypes(job.ContractTypeId);

            return View(job);
        }

        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var job = await _context.Job.SingleOrDefaultAsync(m => m.Id == id);
            if (job == null)
            {
                return NotFound();
            }

            PopulateContractTypes(job.ContractTypeId);

            return View(job);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("Id,ContractTypeId,IsRemote,IsVisaSponsor,PostedAt,Title,Company,Location,Description,ExternalUrl,Contact")] Job job)
        {
            if (id != job.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(job);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!JobExists(job.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }

                return RedirectToAction("Index");
            }

            PopulateContractTypes(job.ContractTypeId);

            return View(job);
        }

        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var job = await _context.Job.Include(j => j.ContractType).SingleOrDefaultAsync(m => m.Id == id);
            if (job == null)
            {
                return NotFound();
            }

            return View(job);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var job = await _context.Job.SingleOrDefaultAsync(m => m.Id == id);
            _context.Job.Remove(job);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        private bool JobExists(Guid id)
        {
            return _context.Job.Any(e => e.Id == id);
        }

        private void PopulateContractTypes(Guid? contractTypeId = null)
        {
            ViewBag.ContractTypeId = new SelectList(_context.Set<ContractType>(), "Id", "Name", contractTypeId);
        }

        private void Validate(ModelStateDictionary modelState, Job job)
        {
            if (string.IsNullOrEmpty(job.Company))
            {
                ModelState.AddModelError("Company", "The Company field is required.");
            }

            if (string.IsNullOrEmpty(job.Title))
            {
                ModelState.AddModelError("Title", "The Title field is required.");
            }

            if (string.IsNullOrEmpty(job.Description))
            {
                ModelState.AddModelError("Description", "The Description field is required.");
            }

            if (string.IsNullOrEmpty(job.Contact))
            {
                ModelState.AddModelError("Contact", "The Contact field is required.");
            }
        }
    }
}
