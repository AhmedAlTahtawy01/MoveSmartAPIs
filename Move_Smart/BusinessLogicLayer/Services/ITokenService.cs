using DataAccessLayer.Repositories;

namespace BusinessLogicLayer.Services
{
    public interface ITokenService
    {
        string GenerateToken(UserDTO user);
    }
}
