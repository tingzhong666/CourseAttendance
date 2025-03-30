using CourseAttendance.DtoModel.ResDtos;
using CourseAttendance.mapper;
using CourseAttendance.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace CourseAttendance.Controllers
{
	[Route("api/time-table")]
	[ApiController]
	public class TimeTableController : ControllerBase
	{
		private readonly TimeTableRepository _timeTableRepository;

		public TimeTableController(TimeTableRepository timeTableRepository)
		{
			_timeTableRepository = timeTableRepository;
		}

		[HttpGet("{id}")]
		[Authorize]
		public async Task<ActionResult<ApiResponse<TimeTableResDto>>> GetById(int id)
		{
			var model = await _timeTableRepository.GetById(id);
			if (model == null)
				return Ok(new ApiResponse<TimeTableResDto> { Code = 2, Msg = "不存在", Data = null });

			return Ok(new ApiResponse<TimeTableResDto> { Code = 1, Msg = "不存在", Data = model.ToDto() });
		}
	}
}
