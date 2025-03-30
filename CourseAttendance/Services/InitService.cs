using CourseAttendance.Controllers.Account;
using CourseAttendance.DtoModel.ReqDtos;
using CourseAttendance.mapper.CreateUserReqDtoExts;
using CourseAttendance.Repositories;
using CourseAttendance.Repositories.Users;

namespace CourseAttendance.Services
{
	public class InitService
	{
		private readonly UserRepository _userRepository;
		private readonly AdminRepository _adminRepository;
		private readonly WebSystemConfigRepository _webSystemConfigRepository;
		private readonly TimeTableRepository _timeTableRepository;

		public InitService(UserRepository userRepository, AdminRepository adminRepository, WebSystemConfigRepository webSystemConfigRepository, TimeTableRepository timeTableRepository)
		{
			_userRepository = userRepository;
			_adminRepository = adminRepository;
			_webSystemConfigRepository = webSystemConfigRepository;
			_timeTableRepository = timeTableRepository;
		}

		public async Task Run()
		{
			// 创建默认管理员
			await CreateAdmin();

			// 作息表
			await CreateInitTimeTable();
		}

		private async Task CreateInitTimeTable()
		{
			var resTask = new List<int>
			{
				await _timeTableRepository.Add(new Model.TimeTable { Name="第1节", Start=new TimeSpan(8,50,0), End=new TimeSpan(9,35,0)}),
				await _timeTableRepository.Add(new Model.TimeTable { Name="第2节", Start=new TimeSpan(9,45,0), End=new TimeSpan(10,30,0)}),
				await _timeTableRepository.Add(new Model.TimeTable { Name="第3节", Start=new TimeSpan(10,45,0), End=new TimeSpan(11,30,0)}),
				await _timeTableRepository.Add(new Model.TimeTable { Name="第4节", Start=new TimeSpan(11,40,0), End=new TimeSpan(12,25,0)}),
				await _timeTableRepository.Add(new Model.TimeTable { Name="第5节", Start=new TimeSpan(13,50,0), End=new TimeSpan(14,35,0)}),
				await _timeTableRepository.Add(new Model.TimeTable { Name="第6节", Start=new TimeSpan(14,45,0), End=new TimeSpan(15,30,0)}),
				await _timeTableRepository.Add(new Model.TimeTable { Name="第7节", Start=new TimeSpan(15,45,0), End=new TimeSpan(16,30,0)}),
				await _timeTableRepository.Add(new Model.TimeTable { Name="第8节", Start=new TimeSpan(16,40,0), End=new TimeSpan(17,25,0)}),
				await _timeTableRepository.Add(new Model.TimeTable { Name="第9节", Start=new TimeSpan(18,30,0), End=new TimeSpan(19,15,0)}),
				await _timeTableRepository.Add(new Model.TimeTable { Name="第10节", Start=new TimeSpan(19,25,0), End=new TimeSpan(20,10,0)}),
				await _timeTableRepository.Add(new Model.TimeTable { Name="第11节", Start=new TimeSpan(20,20,0), End=new TimeSpan(21,5,0)}),
				await _timeTableRepository.Add(new Model.TimeTable { Name="第12节", Start=new TimeSpan(21,15,0), End=new TimeSpan(22,0,0)}),
			};
			var isSucceed = resTask.All(x => x == 1); // 是否都成功
		}

		private async Task CreateAdmin()
		{
			var dto = new CreateUserAdminReqDto
			{
				Name = "admin",
				UserName = "admin",
				PassWord = "Admin123456!"
			};
			var userModel = await AccountController.CreateUser(dto, _userRepository);
			if (userModel != null)
			{
				var resRole = await _userRepository._userManager.AddToRoleAsync(userModel, "Admin");

				var adminModel = dto.ToModel();
				adminModel.UserId = userModel.Id;
				var result = await _adminRepository.AddAdminAsync(adminModel);
			}
		}
	}
}
