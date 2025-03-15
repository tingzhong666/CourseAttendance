using CourseAttendance.DtoModel.ReqDtos;
using CourseAttendance.Model;
using CourseAttendance.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CourseAttendance.Controllers
{
	[Route("api/attendance")]
	[ApiController]
	public class AttendanceController : ControllerBase
	{
		private readonly AttendanceRepository _attendanceRepository;

		public AttendanceController(AttendanceRepository attendanceRepository)
		{
			_attendanceRepository = attendanceRepository;
		}

		[HttpGet]
		[Authorize(Roles = "Admin,Academic,Teacher")]
		public async Task<ActionResult<IEnumerable<Attendance>>> GetAttendances()
		{
			var attendances = await _attendanceRepository.GetAllAsync();
			return Ok(attendances);
		}

		[HttpGet("{courseId}/{studentId}")]
		[Authorize(Roles = "Admin,Academic,Teacher,Student")]
		public async Task<ActionResult<Attendance>> GetAttendance(int courseId, string studentId)
		{
			var attendance = await _attendanceRepository.GetByIdAsync(courseId, studentId);
			if (attendance == null)
			{
				return NotFound();
			}
			return Ok(attendance);
		}

		[HttpPost]
		[Authorize(Roles = "Admin,Academic,Teacher")]
		public async Task<ActionResult<Attendance>> CreateAttendance(Attendance attendance)
		{
			await _attendanceRepository.AddAsync(attendance);
			return CreatedAtAction(nameof(GetAttendance), new { courseId = attendance.CourseId, studentId = attendance.StudentId }, attendance);
		}

		[HttpPut("{courseId}/{studentId}")]
		[Authorize(Roles = "Admin,Academic,Teacher")]
		public async Task<IActionResult> UpdateAttendance(int courseId, string studentId, Attendance attendance)
		{
			if (courseId != attendance.CourseId || studentId != attendance.StudentId)
			{
				return BadRequest();
			}

			await _attendanceRepository.UpdateAsync(attendance);
			return NoContent();
		}

		[HttpDelete("{courseId}/{studentId}")]
		[Authorize(Roles = "Admin,Academic")]
		public async Task<IActionResult> DeleteAttendance(int courseId, string studentId)
		{
			var attendance = await _attendanceRepository.GetByIdAsync(courseId, studentId);
			if (attendance == null)
			{
				return NotFound();
			}

			await _attendanceRepository.DeleteAsync(courseId, studentId);
			return NoContent();
		}

		[HttpGet("filter")]
		[Authorize(Roles = "Admin,Academic,Teacher,Student")]
		public async Task<ActionResult<IEnumerable<Attendance>>> FilterAttendances([FromQuery] AttendanceFilter filter)
		{
			var filteredAttendances = await _attendanceRepository.FilterAttendancesAsync(filter);
			return Ok(filteredAttendances);
		}
	}
}
