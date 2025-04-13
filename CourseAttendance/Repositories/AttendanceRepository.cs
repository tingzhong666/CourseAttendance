﻿using CourseAttendance.AppDataContext;
using CourseAttendance.DtoModel.ReqDtos;
using CourseAttendance.Model;
using CourseAttendance.Model.Users;
using Microsoft.EntityFrameworkCore;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace CourseAttendance.Repositories
{
	public class AttendanceRepository
	{
		private readonly AppDBContext _context;

		public AttendanceRepository(AppDBContext context)
		{
			_context = context;
		}

		public async Task<Attendance?> GetByIdAsync(int id)
		{
			return await _context.Attendances
				.Include(a => a.Course)
				.Include(a => a.Student)
				.FirstOrDefaultAsync(a => a.Id == id);
		}

		public async Task<(List<Attendance> queryRes, int total)> GetAllAsync(AttendanceReqQueryDto query)
		{
			var queryable = _context.Attendances.AsQueryable();
			// 课程名
			if (query.q != null && query.q != "")
				queryable = queryable.Where(x => x.Course.Name.Contains(query.q));

			// 学生id 与 学生名 优先id
			if (query.StudentId != null && query.StudentId.Count != 0)
				queryable = queryable.Where(x => query.StudentId.Contains(x.StudentId));
			if (query.StudentName != null && query.StudentName != "" && query.StudentId == null)
				queryable = queryable.Where(x => x.Student.User.Name.Contains(query.StudentName));

			// 老师id 与 学生名 优先id
			if (query.TeacherId != null && query.TeacherId.Count != 0)
				queryable = queryable.Where(x => query.TeacherId.Contains(x.Course.Teacher.UserId));
			if (query.TeacherName != null && query.TeacherName != "" && query.TeacherId == null)
				queryable = queryable.Where(x => x.Course.Teacher.User.Name.Contains(query.TeacherName));

			// 时间范围
			if (query.StartTime != null)
				queryable = queryable.Where(x => query.StartTime < x.StartTime);
			if (query.EndTime != null)
				queryable = queryable.Where(x => x.EndTime < query.EndTime);


			// 执行查询
			var queryRes = await queryable
				.Include(a => a.Course)
				.Include(a => a.Student)
				.ToListAsync();
			var total = queryRes.Count;
			// 分页
			queryRes = queryRes.Skip(query.Limit * (query.Page - 1)).Take(query.Limit).ToList();
			return (queryRes, total);
		}

		public async Task<int> AddAsync(Attendance attendance)
		{
			await _context.Attendances.AddAsync(attendance);
			return await _context.SaveChangesAsync();
		}

		public async Task<int> UpdateAsync(Attendance attendance)
		{
			var model = await GetByIdAsync(attendance.Id);
			if (model == null) return 0;

			model.UpdatedAt = DateTime.Now;

			model.CheckMethod = attendance.CheckMethod;
			model.StartTime = attendance.StartTime;
			model.EndTime = attendance.EndTime;
			model.Status = attendance.Status;
			model.SignInTime = attendance.SignInTime;
			model.Remark = attendance.Remark;
			model.PassWord = attendance.PassWord;

			return await _context.SaveChangesAsync();
		}

		public async Task<int> DeleteAsync(int id)
		{
			var attendance = await GetByIdAsync(id);
			if (attendance != null)
			{
				_context.Attendances.Remove(attendance);
				return await _context.SaveChangesAsync();
			}
			return 0;
		}
	}
}
