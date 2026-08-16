#nullable enable
using Hisubmit.Client.SharedModels.Enums;
using System.ComponentModel.DataAnnotations;
using Hisubmit.Client.SharedModels.Contracts;

namespace Hisubmit.Client.SharedModels.Features.ExtendedAttributes.Commands.AddEdit
{
    internal class AddEditExtendedAttributeCommandLocalization
    {
        // for localization
    }
    
    public class AddEditExtendedAttributeCommand<TId, TEntityId, TEntity, TExtendedAttribute>
        where TEntity : AuditableEntity<TEntityId>, IEntityWithExtendedAttributes<TExtendedAttribute>, IEntity<TEntityId>
        where TExtendedAttribute : AuditableEntityExtendedAttribute<TId, TEntityId, TEntity>, IEntity<TId>
        where TId : IEquatable<TId>
    {
        public TId Id { get; set; }
        public TEntityId EntityId { get; set; }
        public EntityExtendedAttributeType Type { get; set; }
        [Required(AllowEmptyStrings = false)]
        public string Key { get; set; }
        public string? Text { get; set; }
        public decimal? Decimal { get; set; }
        public DateTime? DateTime { get; set; }
        public string? Json { get; set; }
        public string? ExternalId { get; set; }
        public string? Group { get; set; }
        public string? Description { get; set; }
        public bool IsActive { get; set; }
    }

   
    
}