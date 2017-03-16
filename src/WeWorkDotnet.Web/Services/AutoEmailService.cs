using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SendGrid;
using SendGrid.Helpers.Mail;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeWorkDotnet.Web.Data;
using WeWorkDotnet.Web.Models;
using WeWorkDotnet.Web.Models.ConfigurationModels;

namespace WeWorkDotnet.Web.Services
{
    public interface IAutoEmailService
    {
        Task WeeklyUpdate();
    }

    public class AutoEmailService : IAutoEmailService
    {
        private const string UNSUBSCRIBE_LINK = "http://www.terra.com.br";
        private const string WEEKLY_UPDATE_TEMPLATE_ID = "48041a00-b116-4dec-a5a5-965db43c068c";
        private const int WEEKLY_UPDATE_UNSUBSCRIBE_GROUP_ID = 6933;

        private readonly ApplicationDbContext _context;
        private readonly SendGridConfig _sendGridConfig;


        public AutoEmailService(
            ApplicationDbContext context,
            IOptions<SendGridConfig> sendGridConfig)
        {
            _context = context;
            _sendGridConfig = sendGridConfig.Value;
        }

        public async Task WeeklyUpdate()
        {
            var jobs = await GetWeeklyUpdateJobListAsync();

            if (jobs.Any())
            {
                var htmlContent = GetWeeklyUpdateHtmlContent(jobs);
                var receiverList = await GetReceiversAsync();

                await SendGridTemplateAsync(
                    WEEKLY_UPDATE_TEMPLATE_ID,
                    WEEKLY_UPDATE_UNSUBSCRIBE_GROUP_ID,
                    htmlContent,
                    receiverList);
            }
        }

        private async Task<List<EmailAddress>> GetReceiversAsync()
        {
            var users = await _context.Set<ApplicationUser>().Where(o => o.EmailConfirmed).ToListAsync();
            return users.Select(u => new EmailAddress { Name = u.UserName, Email = u.Email }).ToList();
        }

        private async Task<List<Job>> GetWeeklyUpdateJobListAsync()
        {
            return await _context.Job.Where(o =>
                o.IsActive &&
                o.PostedAt >= DateTime.Today.AddDays(-7)).OrderByDescending(o => o.PostedAt).ThenBy(o => o.Company).ThenBy(o => o.Title).Take(10).ToListAsync();
        }

        private string GetWeeklyUpdateHtmlContent(List<Job> jobs)
        {
            var tableLines = new StringBuilder();
            foreach (var job in jobs)
            {
                var jobLink = "<a href=\"#\" target=\"_blank\">Details</a>"; // TODO RBR add correct link
                var tableLine = $"<tr><td>{job.Company}</td><td>{job.Title}</td><td>{job.Location}</td><td>{jobLink}</td></tr>";
                tableLines.Append(tableLine);
            }
            return tableLines.ToString();
        }

        private async Task SendGridTemplateAsync(string templateId, int unsubscribeGroupId, string htmlContent, List<EmailAddress> receivers)
        {
            var msg = new SendGridMessage
            {
                From = new EmailAddress(_sendGridConfig.FromEmail, _sendGridConfig.FromName),
                TemplateId = templateId,
                TrackingSettings = new TrackingSettings
                {
                    ClickTracking = new ClickTracking
                    {
                        Enable = true,
                        EnableText = true
                    }
                },
                HtmlContent = htmlContent,
                Asm = new ASM
                {
                    GroupId = unsubscribeGroupId
                },
            };

            msg.AddBccs(receivers);

            var client = new SendGridClient(_sendGridConfig.ApiKey);
            var response = await client.SendEmailAsync(msg);
        }
    }
}
