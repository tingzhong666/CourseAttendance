using CourseAttendance.DtoModel.ReqDtos;
using CourseAttendance.DtoModel.ResDtos;
using CourseAttendance.DtoModel;
using CourseAttendance.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using CourseAttendance.mapper;
using CourseAttendance.AppDataContext;

namespace CourseAttendance.Controllers
{
	[Route("api/majors-subcategory")]
	[ApiController]
	public class MajorsSubcategoryController : ControllerBase
	{
		public readonly MajorsSubcategoryRepository _majorsSubcategoryRepository;
		private readonly AppDBContext _context;

		public MajorsSubcategoryController(MajorsSubcategoryRepository majorsSubcategoryRepository, AppDBContext context)
		{
			_majorsSubcategoryRepository = majorsSubcategoryRepository;
			_context = context;
		}


		[HttpGet("{id}")]
		[Authorize]
		public async Task<ActionResult<ApiResponse<MajorsSubcategoryResDto>>> GetById(int id)
		{
			var model = await _majorsSubcategoryRepository.GetByIdAsync(id);
			if (model == null)
				return Ok(new ApiResponse<MajorsSubcategoryResDto> { Code = 2, Msg = "不存在", Data = null });

			return Ok(new ApiResponse<MajorsSubcategoryResDto> { Code = 1, Msg = "不存在", Data = model.ToDto() });
		}

		[HttpGet]
		[Authorize]
		public async Task<ActionResult<ApiResponse<ListDto<MajorsSubcategoryResDto>>>> GetAll([FromQuery] MajorsSubReqQueryDto query)
		{
			try
			{
				var (queryRes, total) = await _majorsSubcategoryRepository.GetAllAsync(query);
				return Ok(new ApiResponse<ListDto<MajorsSubcategoryResDto>>
				{
					Code = 1,
					Msg = "",
					Data = new ListDto<MajorsSubcategoryResDto>
					{
						DataList = queryRes.Select(x => x.ToDto()).ToList(),
						Total = total
					}
				});
			}
			catch (Exception err)
			{
				return Ok(new ApiResponse<ListDto<MajorsSubcategoryResDto>> { Code = 2, Msg = err.Message, Data = null });
			}
		}


		[HttpDelete]
		[Authorize(Roles = "Admin,Academic")]
		public async Task<ActionResult<ApiResponse<object>>> Del([FromQuery] int id)
		{
			var transaction = await _context.Database.BeginTransactionAsync();
			try
			{
				var res = await _majorsSubcategoryRepository.DelAsync(id);
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
		public async Task<ActionResult<ApiResponse<object>>> Update([FromBody] MajorsSubcategoryReqDto dto, [FromQuery] int id)
		{
			var transaction = await _context.Database.BeginTransactionAsync();
			try
			{
				var model = dto.ToModel();
				model.Id = id;
				var res = await _majorsSubcategoryRepository.UpdateAsync(model);
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
		public async Task<ActionResult<ApiResponse<object>>> Create([FromBody] MajorsSubcategoryReqDto dto)
		{
			var transaction = await _context.Database.BeginTransactionAsync();
			try
			{
				var model = dto.ToModel();
				var res = await _majorsSubcategoryRepository.AddAsync(model);
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
