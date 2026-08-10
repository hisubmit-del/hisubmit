using System;

namespace Hisubmit.Client.SharedModels.Contracts
{
    public abstract class AuditableEntity<TId> : IAuditableEntity<TId>
    {
        public TId Id { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public string LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }
    }

    public abstract class AuditableEntity : AuditableEntity<int>
    {
        
    }
}
