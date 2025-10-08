using Treazr_Backend.Common;
using Treazr_Backend.Data;
using Treazr_Backend.Models;
using Treazr_Backend.Repository.interfaces;
using Treazr_Backend.Services.interfaces;

namespace Treazr_Backend.Services.implementation
{
    public class UserService:IUserService
    {
        private readonly IGenericRepository<User> _userrepo;
        private readonly AppDbContext _context;

        public UserService(IGenericRepository<User> userrepo ,AppDbContext context)
        {
            _userrepo = userrepo;
            _context = context;
        }

        public async Task<ApiResponse<IEnumerable<User>>> GetAllUsersAsync()
        {
            var users = await _userrepo.GetAllAsync();
            return new ApiResponse<IEnumerable<User>>(200, "users retrieved succesfully", users);
        }

        public async Task<ApiResponse<User>> GetUserByIdAsync(int id)
        {
            var user = await _userrepo.GetByIdAsync(id);

            if (user == null || user.IsDeleted)
                return new ApiResponse<User>(404, "User not found");

            return new ApiResponse<User>(200, "User retrieved successfully", user);
        }

        public async Task<ApiResponse<string>> BlockUnblockUserAsync(int id)
        {
            var user = await _userrepo.GetByIdAsync(id);
            if (user == null || user.IsDeleted)
                return new ApiResponse<string>(404, "User not found");

            user.IsBlocked = !user.IsBlocked;
            user.ModifiedOn = DateTime.UtcNow;
            _context.Users.Update(user);
            await _context.SaveChangesAsync();


            return new ApiResponse<string>(200,  $"User {(user.IsBlocked ? "blocked" : "unblocked")}  successfully");

        }

        public async Task<ApiResponse<string>> SoftDeleteUserAsync(int id)
        {
            var user = await _userrepo.GetByIdAsync(id);
            if (user == null || user.IsDeleted)
                return new ApiResponse<string>(404, "User not found");

            user.IsDeleted = !user.IsDeleted;
            user.DeletedOn = DateTime.UtcNow;
            _context.Users.Update(user);
            await _context.SaveChangesAsync();


            return new ApiResponse<string>(200, "user soft-deleted successfully");
        }

    }
}
