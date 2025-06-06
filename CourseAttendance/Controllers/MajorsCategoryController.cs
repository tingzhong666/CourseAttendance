﻿using CourseAttendance.DtoModel.ReqDtos;
using CourseAttendance.DtoModel;
using CourseAttendance.DtoModel.ResDtos;
using CourseAttendance.mapper;
using CourseAttendance.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using CourseAttendance.AppDataContext;

namespace CourseAttendance.Controllers
{
	[Route("api/majors-category")]
	[ApiController]
	public class MajorsCategoryController : ControllerBase
	{
		public readonly MajorsCategoryRepository _majorsCategoryRepository;
		private readonly AppDBContext _context;

		public MajorsCategoryController(MajorsCategoryRepository majorsCategoryRepository, AppDBContext context)
		{
			_majorsCategoryRepository = majorsCategoryRepository;
			_context = context;
		}


		[HttpGet("{id}")]
		[Authorize]
		public async Task<ActionResult<ApiResponse<MajorsCategoryResDto>>> GetById(int id)
		{
			var model = await _majorsCategoryRepository.GetByIdAsync(id);
			if (model == null)
				return Ok(new ApiResponse<MajorsCategoryResDto> { Code = 2, Msg = "不存在", Data = null });

			return Ok(new ApiResponse<MajorsCategoryResDto> { Code = 1, Msg = "不存在", Data = model.ToDto() });
		}

		[HttpGet]
		[Authorize]
		public async Task<ActionResult<ApiResponse<ListDto<MajorsCategoryResDto>>>> GetAll([FromQuery] ReqQueryDto query)
		{
			try
			{
				var (queryRes, total) = await _majorsCategoryRepository.GetAllAsync(query);
				return Ok(new ApiResponse<ListDto<MajorsCategoryResDto>>
				{
					Code = 1,
					Msg = "",
					Data = new ListDto<MajorsCategoryResDto>
					{
						DataList = queryRes.Select(x => x.ToDto()).ToList(),
						Total = total
					}
				});
			}
			catch (Exception err)
			{
				return Ok(new ApiResponse<ListDto<MajorsCategoryResDto>> { Code = 2, Msg = err.Message, Data = null });
			}
		}

		[HttpDelete]
		[Authorize(Roles = "Admin,Academic")]
		public async Task<ActionResult<ApiResponse<object>>> Del([FromQuery] int id)
		{
			var transaction = await _context.Database.BeginTransactionAsync();
			try
			{
				var res = await _majorsCategoryRepository.DelAsync(id);
				if (res == 0) throw new Exception("删除失败");
				await transaction.CommitAsync();
				return Ok(new ApiResponse<object> { Code = 1, Msg = "", Data = null });
			}
			catch (Exception err)
			{
				await transaction.RollbackAsync();
				return Ok(new ApiResponse<object> { Code = 2, Msg = err.Message, Data = null });
			}
		}

		[HttpPut]
		[Authorize(Roles = "Admin,Academic")]
		public async Task<ActionResult<ApiResponse<object>>> Update([FromBody] MajorsCategoryReqDto dto, [FromQuery] int id)
		{
			var transaction = await _context.Database.BeginTransactionAsync();
			try
			{
				var model = dto.ToModel();
				model.Id = id;
				var res = await _majorsCategoryRepository.UpdateAsync(model);
				if (res == 0) throw new Exception("更新失败");
				await transaction.CommitAsync();
				return Ok(new ApiResponse<object> { Code = 1, Msg = "", Data = null });
			}
			catch (Exception err)
			{
				await transaction.RollbackAsync();
				return Ok(new ApiResponse<object> { Code = 2, Msg = err.Message, Data = null });
			}
		}


		[HttpPost]
		[Authorize(Roles = "Admin,Academic")]
		public async Task<ActionResult<ApiResponse<object>>> Create([FromBody] MajorsCategoryReqDto dto)
		{
			var transaction = await _context.Database.BeginTransactionAsync();
			try
			{
				var model = dto.ToModel();
				var res = await _majorsCategoryRepository.AddAsync(model);
				if (res == 0) throw new Exception("创建失败");
				await transaction.CommitAsync();
				return Ok(new ApiResponse<object> { Code = 1, Msg = "", Data = null });
			}
			catch (Exception err)
			{
				await transaction.RollbackAsync();
				return Ok(new ApiResponse<object> { Code = 2, Msg = err.Message, Data = null });
			}
		}
	}
}
