using System;

namespace WeWorkDotnet.Web.Models
{
    public class Job
    {
        public Guid Id { get; set; }
        public Guid ContractTypeId { get; set; }
        public string PostedByUserId { get; set; }

        public bool IsRemote { get; set; }
        public bool IsVisaSponsor { get; set; }
        public bool IsActive { get; set; } = true;

        public DateTime PostedAt { get; set; }

        public string Title { get; set; }
        public string Company { get; set; }
        public string Location { get; set; }
        public string Description { get; set; }
        public string ExternalUrl { get; set; }
        public string Contact { get; set; }

        public ContractType ContractType { get; set; }
        public ApplicationUser User { get; set; }
    }
}
