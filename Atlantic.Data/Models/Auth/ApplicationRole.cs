using AspNetCore.Identity.MongoDbCore.Models;
using MongoDbGenericRepository.Attributes;

namespace Atlantic.Data.Models.Auth
{
    [CollectionName("Auth_Roles")]
    public class ApplicationRole : MongoIdentityRole<Guid>
    {
    }
}
