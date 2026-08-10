using Hisubmit.Client.SharedModels.Contracts;

namespace Hisubmit.Client.SharedModels.Features.ExtendedAttributes.Queries.Export;

    internal class ExportExtendedAttributesQueryLocalization
    {
        // for localization
    }

    public class ExportExtendedAttributesQuery<TId, TEntityId, TEntity, TExtendedAttribute>
       
            where TEntity : AuditableEntity<TEntityId>, IEntityWithExtendedAttributes<TExtendedAttribute>, IEntity<TEntityId>
            where TExtendedAttribute : AuditableEntityExtendedAttribute<TId, TEntityId, TEntity>, IEntity<TId>
            where TId : IEquatable<TId>
    {
        public string SearchString { get; set; }
        public TEntityId EntityId { get; set; }
        public bool IncludeEntity { get; set; }
        public bool OnlyCurrentGroup { get; set; }
        public string CurrentGroup { get; set; }

        public ExportExtendedAttributesQuery(string searchString = "", TEntityId entityId = default, bool includeEntity = false, bool onlyCurrentGroup = false, string currentGroup = "")
        {
            SearchString = searchString;
            EntityId = entityId;
            IncludeEntity = includeEntity;
            OnlyCurrentGroup = onlyCurrentGroup;
            CurrentGroup = currentGroup;
        }
    }

  
