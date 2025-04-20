namespace CourseAttendance.Services
{

	public class AttendanceCheckService : BackgroundService
	{
		private readonly IServiceProvider _services;
		private readonly PeriodicTimer _timer;

		public AttendanceCheckService(IServiceProvider services)
		{
			_services = services;
			// 创建一个1秒执行一次的计时器
			_timer = new PeriodicTimer(TimeSpan.FromSeconds(1));
		}

		protected override async Task ExecuteAsync(CancellationToken stoppingToken)
		{
			// 当应用程序运行时，循环执行
			while (await _timer.WaitForNextTickAsync(stoppingToken) && !stoppingToken.IsCancellationRequested)
			{
				await CheckAttendanceStatusAsync();
			}
		}

		private async Task CheckAttendanceStatusAsync()
		{
			// 使用作用域服务以确保正确的数据库连接处理
			using var scope = _services.CreateScope();
			var attendanceService = scope.ServiceProvider.GetRequiredService<AttendanceService>();
			await attendanceService.UpdateAbsentStatusAsync();
		}

		public override async Task StopAsync(CancellationToken stoppingToken)
		{

			_timer.Dispose();

			await base.StopAsync(stoppingToken);
		}
	}
}
