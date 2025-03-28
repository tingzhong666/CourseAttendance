﻿using CourseAttendance.AppDataContext;
using CourseAttendance.DtoModel.ReqDtos;
using CourseAttendance.DtoModel.ResDtos;
using CourseAttendance.mapper;
using CourseAttendance.mapper.UserExts;
using CourseAttendance.Repositories;
using CourseAttendance.Repositories.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;

namespace CourseAttendance.Controllers
{
	[Route("api/classes")]
	[ApiController]
	public class ClassesController : ControllerBase
	{

		private readonly GradeRepository _gradeRepository;
		private readonly UserRepository _userRepository;
		private readonly StudentRepository _studentRepository;

		public ClassesController(GradeRepository gradeRepository, UserRepository userRepository, StudentRepository studentRepository)
		{
			_gradeRepository = gradeRepository;
			_userRepository = userRepository;
			_studentRepository = studentRepository;
		}

		/// <summary>
		/// 1. 获取班级列表
		/// </summary>
		/// <returns></returns>
		[HttpGet]
		[Authorize(Roles = "Admin, Academic, Teacher")]
		public async Task<ActionResult<ApiResponse<List<GradeResponseDto>>>> GetClasses()
		{
			var grades = await _gradeRepository.GetAllAsync();
			var response = grades.Select(g => g.ToResponseDto()).ToList();
			return Ok(new ApiResponse<List<GradeResponseDto>> { Code = 1, Msg = "", Data = response });
		}

		/// <summary>
		/// 2. 获取单个班级信息
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		[HttpGet("{id}")]
		[Authorize(Roles = "Admin, Academic, Teacher")]
		public async Task<ActionResult<ApiResponse<GradeResponseDto>>> GetClass(int id)
		{
			var grade = await _gradeRepository.GetByIdAsync(id);
			if (grade == null)
				return Ok(new ApiResponse<GradeResponseDto> { Code = 2, Msg = "", Data = null });

			var response = grade.ToResponseDto();
			return Ok(new ApiResponse<GradeResponseDto> { Code = 1, Msg = "", Data = response });
		}

		/// <summary>
		/// 3. 创建新班级
		/// </summary>
		/// <param name="gradeRequest"></param>
		/// <returns></returns>
		[HttpPost]
		[Authorize(Roles = "Admin, Academic")]
		public async Task<ActionResult<ApiResponse<GradeResponseDto>>> CreateClass([FromBody] GradeRequestDto gradeRequest)
		{
			var grade = gradeRequest.ToModel();
			var res = await _gradeRepository.AddAsync(grade);
			if (res == 0)
				return Ok(new ApiResponse<GradeResponseDto> { Code = 2, Msg = "创建失败", Data = null });
			var dto = grade.ToResponseDto();
			return Ok(new ApiResponse<GradeResponseDto> { Code = 1, Msg = "", Data = dto });
		}

		/// <summary>
		/// 4. 更新班级信息
		/// </summary>
		/// <param name="id"></param>
		/// <param name="gradeRequest"></param>
		/// <returns></returns>
		[HttpPut("{id}")]
		[Authorize(Roles = "Admin, Academic")]
		public async Task<ActionResult<ApiResponse<object>>> UpdateClass(int id, [FromBody] GradeRequestDto gradeRequest)
		{
			var grade = gradeRequest.ToModel();
			grade.Id = id; // 确保 ID 正确

			var res = await _gradeRepository.UpdateAsync(grade);
			if (res == 0)
				return Ok(new ApiResponse<object> { Code = 2, Msg = "更新失败", Data = null });
			return Ok(new ApiResponse<object> { Code = 1, Msg = "", Data = null });
		}

		/// <summary>
		/// 5. 删除班级
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		[HttpDelete("{id}")]
		[Authorize(Roles = "Admin, Academic")]
		public async Task<ActionResult<ApiResponse<object>>> DeleteClass(int id)
		{
			var res = await _gradeRepository.DeleteAsync(id);
			if (res == 0)
				return Ok(new ApiResponse<object> { Code = 2, Msg = "删除失败", Data = null });
			return Ok(new ApiResponse<object> { Code = 1, Msg = "", Data = null });
		}

		/// <summary>
		/// 6. 获取班级的学生列表
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		[HttpGet("{id}/students")]
		[Authorize(Roles = "Admin, Academic, Teacher")]
		public async Task<ActionResult<ApiResponse<List<GetStudentResDto>>>> GetStudents(int id)
		{
			var grade = await _gradeRepository.GetByIdAsync(id);
			if (grade == null)
				return Ok(new ApiResponse<List<GetStudentResDto>> { Code = 2, Msg = "", Data = null });

			var students = grade.Students.Select(async s =>
			{
				return await s.ToGetStudentResDto(s.User, _userRepository);
			});

			var studentsRes = new List<GetStudentResDto>();
			// 全部操作同步执行，因为数据库上下文不支持多线程
			foreach (var student in grade.Students)
			{
				var studentDto = await student.ToGetStudentResDto(student.User, _userRepository);
				studentsRes.Add(studentDto);
			}

			return Ok(new ApiResponse<List<GetStudentResDto>> { Code = 1, Msg = "", Data = studentsRes });
		}

		/// <summary>
		/// 7. 更换学生班级
		/// </summary>
		/// <param name="id"></param>
		/// <param name="studentId"></param>
		/// <returns></returns>
		[HttpPost("{id}/students/{studentId}")]
		[Authorize(Roles = "Admin, Academic")]
		public async Task<ActionResult<ApiResponse<object>>> UpdateStudentToClass(int id, string studentId)
		{
			try
			{
				var grade = await _gradeRepository.GetByIdAsync(id);
				if (grade == null)
					return Ok(new ApiResponse<object> { Code = 2, Msg = "", Data = null });

				var studentModel = await _studentRepository.GetByIdAsync(studentId);
				if (studentModel == null)
					return Ok(new ApiResponse<object> { Code = 2, Msg = "未查到此学生", Data = null });

				studentModel.GradeId = id;
				var res = await _studentRepository.UpdateAsync(studentModel);
				if (res == 0)
					return Ok(new ApiResponse<object> { Code = 2, Msg = "更换失败", Data = null });

				return Ok(new ApiResponse<object> { Code = 1, Msg = "", Data = null });
			}
			catch (Exception err)
			{
				return Ok(new ApiResponse<object> { Code = 2, Msg = "更换失败", Data = null });
			}
		}

		//// 8. 移除学生从班级
		//[HttpDelete("{id}/students/{studentId}")]
		//[Authorize(Roles = "Admin, Academic")]
		//public async Task<IActionResult> RemoveStudentFromClass(int id, string studentId)
		//{
		//	var grade = await _gradeRepository.GetByIdAsync(id);
		//	if (grade == null) return NotFound();

		//	// 这里需要实现将学生添加到班级的逻辑
		//	var studentModel = await _studentRepository.GetByIdAsync(studentId);
		//	if (studentModel == null) return BadRequest("未查到此学生");

		//	studentModel.GradeId = "";
		//	var res = await _studentRepository.UpdateAsync(studentModel);
		//	if (res == 0)
		//		return BadRequest("添加失败");

		//	return NoContent(); // 204 移除成功
		//}
	}
}
