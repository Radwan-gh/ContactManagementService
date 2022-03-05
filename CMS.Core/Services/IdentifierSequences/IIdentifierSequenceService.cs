using CMS.Core.Entities;

namespace CMS.Core.Services.IdentifierSequences
{
    public interface IIdentifierSequenceService
    {
        int GetIdentifier(EntityType type);
    }
}