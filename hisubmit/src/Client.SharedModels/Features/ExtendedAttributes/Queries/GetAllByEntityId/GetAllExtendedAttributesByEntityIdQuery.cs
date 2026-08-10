using Hisubmit.Client.SharedModels.Contracts;

namespace Hisubmit.Client.SharedModels.Features.ExtendedAttributes.Queries.GetAllByEntityId;

public class GetAllExtendedAttributesByEntityIdQuery<TId, TEntityId, TEntity, TExtendedAttribute>
     
    where TEntity : AuditableEntity<TEntityId>, IEntityWithExtendedAttributes<TExtendedAttribute>, IEntity<TEntityId>
    where TExtendedAttribute : AuditableEntityExtendedAttribute<TId, TEntityId, TEntity>, IEntity<TId>
    where TId : IEquatable<TId>
{
    public TEntityId EntityId { get; set; }

    public GetAllExtendedAttributesByEntityIdQuery(TEntityId entityId)
    {
        EntityId = entityId;
    }
}