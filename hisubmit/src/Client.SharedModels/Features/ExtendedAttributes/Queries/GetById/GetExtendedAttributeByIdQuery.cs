using Hisubmit.Client.SharedModels.Contracts;

namespace Hisubmit.Client.SharedModels.Features.ExtendedAttributes.Queries.GetById
{
    public class GetExtendedAttributeByIdQuery<TId, TEntityId, TEntity, TExtendedAttribute>
       
        where TEntity : AuditableEntity<TEntityId>, IEntityWithExtendedAttributes<TExtendedAttribute>, IEntity<TEntityId>
        where TExtendedAttribute : AuditableEntityExtendedAttribute<TId, TEntityId, TEntity>, IEntity<TId>
        where TId : IEquatable<TId>
    {
        public TId Id { get; set; }
    }

  
}