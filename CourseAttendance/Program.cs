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
using CourseAttendance.DtoModel.ReqDtos;
using CourseAttendance.mapper.CreateUserReqDtoExts;
using Microsoft.AspNetCore.Http.Features;
using Newtonsoft.Json.Converters;

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

	options.SerializerSettings.Converters.Add(new StringEnumConverter());
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
	.AddEntityFrameworkStores<AppDBContext>()
	.AddDefaultTokenProviders();

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

// �洢��ע��
builder.Services.AddScoped<AttendanceRepository>();
builder.Services.AddScoped<CourseRepository>();
builder.Services.AddScoped<CourseStudentRepository>();
builder.Services.AddScoped<GradeRepository>();
builder.Services.AddScoped<AcademicRepository>();
builder.Services.AddScoped<AdminRepository>();
builder.Services.AddScoped<StudentRepository>();
builder.Services.AddScoped<TeacherRepository>();
builder.Services.AddScoped<UserRepository>();
builder.Services.AddScoped<WebSystemConfigRepository>();
builder.Services.AddScoped<TimeTableRepository>();
builder.Services.AddScoped<CourseTimeRepository>();
builder.Services.AddScoped<MajorsCategoryRepository>();
builder.Services.AddScoped<MajorsSubcategoryRepository>();

builder.Services.AddScoped<TokenService>();
builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<InitService>();


// �����ļ��ϴ�����
builder.Services.Configure<FormOptions>(options =>
{
	options.MultipartBodyLengthLimit = 200 * 1024 * 1024; // �����ļ���С���� 200MB
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI();
}

app.UseHttpsRedirection();
if (app.Environment.IsDevelopment())
{
	app.UseCors(x => x
		 .AllowAnyMethod()
		 .AllowAnyHeader()
		 .AllowCredentials()
		  //.WithOrigins("https://localhost:44351))
		  .SetIsOriginAllowed(origin => true)); // ��������
}
app.UseStaticFiles(); // ���þ�̬�ļ�
app.UseAuthentication();
app.UseAuthorization();


app.MapControllers();

// ĳЩ���ݳ�ʼ��
using (var scope = app.Services.CreateScope())
{
	var services = scope.ServiceProvider;
	var initService = services.GetRequiredService<InitService>();
	await initService.Run();
}

app.Run();
