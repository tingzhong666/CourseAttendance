using CourseAttendance.AppDataContext;
using CourseAttendance.Model.Users;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace CourseAttendance.Repositories.Users
{
	public class UserRepository(UserManager<User> userManager)
	{
		public readonly UserManager<User> _userManager = userManager;

		public async Task<User?> GetByIdAsync(string id)
		{
			return await _userManager.FindByIdAsync(id);
		}

		public List<User> GetAllAsync()
		{
			return [.. _userManager.Users];
		}

		public async Task<IdentityResult> AddAsync(User user, string pwd)
		{
			return await _userManager.CreateAsync(user, pwd);
		}

		public async Task<IdentityResult> UpdateAsync(User user)
		{
			var model = await _userManager.FindByIdAsync(user.Id);
			if (model == null)
				return IdentityResult.Failed([new IdentityError { Description = "未找到此用户" }]);
			model.Name = user.Name;
			model.Email = user.Email;
			model.PhoneNumber = user.PhoneNumber;
			return await _userManager.UpdateAsync(model);
		}

		public async Task<IdentityResult> DeleteAsync(string id)
		{
			var model = await _userManager.FindByIdAsync(id);
			if (model == null)
				return IdentityResult.Failed([new IdentityError { Description = "未找到此用户" }]);
			return await _userManager.DeleteAsync(model);
		}
	}
}
