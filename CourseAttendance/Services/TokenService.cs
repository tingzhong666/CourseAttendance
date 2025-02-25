using CourseAttendance.Model.Users;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace CourseAttendance.Services
{
	public class TokenService
	{
		private readonly IConfiguration _config;
		private readonly SymmetricSecurityKey _key;
		private readonly UserManager<User> _userManager;
		public TokenService(IConfiguration config, UserManager<User> userManager)
		{
			_config = config;
			_key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["JWT:SigningKey"]));
			_userManager = userManager;
		}

		public async Task<string> CreateToken(User user)
		{
			var claims = new List<Claim>()
			{
				//new Claim(JwtRegisteredClaimNames.Email,user.Email),
				new Claim(JwtRegisteredClaimNames.GivenName,user.UserName)
			};

			// 获取用户角色并添加到声明中
			var roles = await _userManager.GetRolesAsync(user);
			foreach (var role in roles)
			{
				claims.Add(new Claim(ClaimTypes.Role, role)); // 添加角色声明
			}

			var creds = new SigningCredentials(_key, SecurityAlgorithms.HmacSha512Signature);

			var tokenDescriptor = new SecurityTokenDescriptor()
			{
				Subject = new ClaimsIdentity(claims),
				Expires = DateTime.Now.AddDays(7),
				SigningCredentials = creds,
				Issuer = _config["JWT:Issuer"],
				Audience = _config["JWT:Audience"]
			};


			var tokenHandler = new JwtSecurityTokenHandler();

			var token = tokenHandler.CreateToken(tokenDescriptor);

			return tokenHandler.WriteToken(token);
		}
	}
}
