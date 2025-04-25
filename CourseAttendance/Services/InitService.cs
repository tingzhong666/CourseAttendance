using CourseAttendance.Controllers;
using CourseAttendance.DtoModel.ReqDtos;
using CourseAttendance.mapper.CreateUserReqDtoExts;
using CourseAttendance.Repositories;
using CourseAttendance.Repositories.Users;

namespace CourseAttendance.Services
{
	public class InitService
	{
		private readonly UserRepository _userRepository;
		private readonly WebSystemConfigRepository _webSystemConfigRepository;
		private readonly TimeTableRepository _timeTableRepository;
		private readonly UserService _userService;

		public InitService(UserRepository userRepository, WebSystemConfigRepository webSystemConfigRepository, TimeTableRepository timeTableRepository, UserService userService)
		{
			_userRepository = userRepository;
			_webSystemConfigRepository = webSystemConfigRepository;
			_timeTableRepository = timeTableRepository;
			_userService = userService;
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
			var datas = new List<Model.TimeTable>
			{
				new Model.TimeTable { Name="第1节", Start=new TimeSpan(8,50,0), End=new TimeSpan(9,35,0)},
				new Model.TimeTable { Name="第2节", Start=new TimeSpan(9,45,0), End=new TimeSpan(10,30,0)},
				new Model.TimeTable { Name="第3节", Start=new TimeSpan(10,45,0), End=new TimeSpan(11,30,0)},
				new Model.TimeTable { Name="第4节", Start=new TimeSpan(11,40,0), End=new TimeSpan(12,25,0)},
				new Model.TimeTable { Name="第5节", Start=new TimeSpan(13,50,0), End=new TimeSpan(14,35,0)},
				new Model.TimeTable { Name="第6节", Start=new TimeSpan(14,45,0), End=new TimeSpan(15,30,0)},
				new Model.TimeTable { Name="第7节", Start=new TimeSpan(15,45,0), End=new TimeSpan(16,30,0)},
				new Model.TimeTable { Name="第8节", Start=new TimeSpan(16,40,0), End=new TimeSpan(17,25,0)},
				new Model.TimeTable { Name="第9节", Start=new TimeSpan(18,30,0), End=new TimeSpan(19,15,0)},
				new Model.TimeTable { Name="第10节", Start=new TimeSpan(19,25,0), End=new TimeSpan(20,10,0)},
				new Model.TimeTable { Name="第11节", Start=new TimeSpan(20,20,0), End=new TimeSpan(21,5,0)},
				new Model.TimeTable { Name="第12节", Start=new TimeSpan(21,15,0), End=new TimeSpan(22,0,0)},
			};
			var resTask = new List<int>();
			foreach (var item in datas)
			{
				var (queryRes, total) = await _timeTableRepository.GetAllAsync(new ReqQueryDto
				{
					Limit = 999,
					Page = 0,
					q = item.Name,
				});
				if (total != 0) continue;

				resTask.Add(await _timeTableRepository.Add(item));
			}
			var isSucceed = resTask.All(x => x == 1); // 是否都成功
		}

		private async Task CreateAdmin()
		{
			var model = await _userRepository._userManager.FindByNameAsync("admin");
			if (model == null)
			{
				await _userService.CreateUserAsync(new CreateUserReqDto
				{
					Name = "admin",
					UserName = "admin",
					PassWord = "Admin123456!",
					Roles = [Enums.UserRole.Admin],
					CreateAdminExt = new CreateUserAdminReqDto { },
				});
			}
		}
	}
}
