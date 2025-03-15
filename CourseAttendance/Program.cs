using CourseAttendance.AppDataContext;
using CourseAttendance.Repositories.Users;
using CourseAttendance.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using CourseAttendance.Model.Users;
using CourseAttendance.Services;
using Microsoft.OpenApi.Models;
using CourseAttendance.Controllers.Account;
using CourseAttendance.DtoModel.ReqDtos;
using CourseAttendance.mapper.CreateUserReqDtoExts;
using Microsoft.AspNetCore.Http.Features;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSwaggerGen(option =>
{
	option.SwaggerDoc("v1", new OpenApiInfo { Title = "Demo API", Version = "v1" });
	option.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
	{
		In = ParameterLocation.Header,
		Description = "Please enter a valid token",
		Name = "Authorization",
		Type = SecuritySchemeType.Http,
		BearerFormat = "JWT",
		Scheme = "Bearer"
	});
	option.AddSecurityRequirement(new OpenApiSecurityRequirement
	{
		{
			new OpenApiSecurityScheme
			{
				Reference = new OpenApiReference
				{
					Type=ReferenceType.SecurityScheme,
					Id="Bearer"
				}
			},
			new string[]{}
		}
	});
});

builder.Services.AddControllers().AddNewtonsoftJson(options =>
{
	options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
});

builder.Services.AddDbContext<AppDBContext>(options =>
{
	options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});

builder.Services.AddIdentity<User, IdentityRole>(options =>
{
	options.Password.RequireDigit = true;
	options.Password.RequireLowercase = true;
	options.Password.RequireUppercase = true;
	options.Password.RequireNonAlphanumeric = true;
	options.Password.RequiredLength = 12;
})
	.AddEntityFrameworkStores<AppDBContext>();

builder.Services.AddAuthentication(options =>
{
	options.DefaultAuthenticateScheme =
	options.DefaultChallengeScheme =
	options.DefaultForbidScheme =
	options.DefaultScheme =
	options.DefaultSignInScheme =
	options.DefaultSignOutScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
	options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
	{
		ValidateIssuer = true,
		ValidIssuer = builder.Configuration["JWT:Issuer"],
		ValidateAudience = true,
		ValidAudience = builder.Configuration["JWT:Audience"],
		ValidateIssuerSigningKey = true,
		IssuerSigningKey = new SymmetricSecurityKey(
			System.Text.Encoding.UTF8.GetBytes(builder.Configuration["JWT:SigningKey"])
			)
	};
});

// 存储库注入
builder.Services.AddScoped<AttendanceRepository>();
builder.Services.AddScoped<CourseRepository>();
builder.Services.AddScoped<CourseStudentRepository>();
builder.Services.AddScoped<GradeRepository>();
builder.Services.AddScoped<AcademicRepository>();
builder.Services.AddScoped<AdminRepository>();
builder.Services.AddScoped<StudentRepository>();
builder.Services.AddScoped<TeacherRepository>();
builder.Services.AddScoped<UserRepository>();

builder.Services.AddScoped<TokenService, TokenService>();


// 配置文件上传限制
builder.Services.Configure<FormOptions>(options =>
{
	options.MultipartBodyLengthLimit = 200 * 1024 * 1024; // 设置文件大小限制 200MB
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseStaticFiles(); // 启用静态文件
app.UseAuthentication();
app.UseAuthorization();


app.MapControllers();

// 创建初始管理员账号
using (var scope = app.Services.CreateScope())
{
	var services = scope.ServiceProvider;
	var _userRepository = services.GetRequiredService<UserRepository>();
	var _adminRepository = services.GetRequiredService<AdminRepository>();

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

app.Run();
