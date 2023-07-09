using DemoBed.Base.Entities;

namespace DemoBed.Base.Services
{
    public interface ILoggedInUserService
    {
        User? User { get; }
        bool? IsAdmin { get; }
    }
}