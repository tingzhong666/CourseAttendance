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
builder.Services.AddScoped<StudentRepository>();
builder.Services.AddScoped<TeacherRepository>();
builder.Services.AddScoped<UserRepository>();

builder.Services.AddScoped<TokenService, TokenService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();


app.MapControllers();

// 创建初始管理员账号
using (var scope = app.Services.CreateScope())
{
	var services = scope.ServiceProvider;
	var userManager = services.GetRequiredService<UserManager<User>>();

	// 创建管理员用户
	var adminName = "admin";
	var adminPassword = "Admin123456!";

	var adminUser = await userManager.FindByNameAsync(adminName);
	if (adminUser == null)
	{
		adminUser = new User { UserName = adminName };
		var result = await userManager.CreateAsync(adminUser, adminPassword);
		if (result.Succeeded)
		{
			await userManager.AddToRoleAsync(adminUser, "Admin");
		}
	}
}

app.Run();
