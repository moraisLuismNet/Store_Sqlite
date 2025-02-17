using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Store.DTOs;
using Store.Models;
using Store.Services;

namespace Store.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly StoreContext _context;
        private readonly IConfiguration _configuration;
        private readonly IDataProtector _dataProtector;
        private readonly HashService _hashService;

        public UsersController(StoreContext context, IConfiguration configuration,
        IDataProtectionProvider dataProtector, HashService hashService)
        {
            _context = context;
            _configuration = configuration;
            _dataProtector = dataProtector.CreateProtector(configuration["EncryptionKey"]);
            _hashService = hashService;
        }

        [HttpPost("encrypt/newUser")]
        public async Task<ActionResult> PostNewUser([FromBody] UserDTO user)
        {
            var passEncrypted = _dataProtector.Protect(user.Password);
            var newUser = new User
            {
                Email = user.Email,
                Password = passEncrypted
            };
            await _context.Users.AddAsync(newUser);
            await _context.SaveChangesAsync();

            return Ok(newUser);
        }

        [HttpPost("encrypt/checkUser")]
        public async Task<ActionResult> PostCheckUserPassEncript([FromBody] UserDTO user)
        {
            var userDB = await _context.Users.FirstOrDefaultAsync(x => x.Email == user.Email);
            if (userDB == null)
            {
                return Unauthorized();
            }

            var passDecrypted = _dataProtector.Unprotect(userDB.Password);
            if (user.Password == passDecrypted)
            {
                return Ok();
            }
            else
            {
                return Unauthorized();
            }
        }

        [HttpPost("hash/newUser")]
        public async Task<ActionResult> PostNewUserHash([FromBody] UserDTO user)
        {
            var resultHash = _hashService.Hash(user.Password);
            var newUser = new User
            {
                Email = user.Email,
                Password = resultHash.Hash,
                Salt = resultHash.Salt
            };

            await _context.Users.AddAsync(newUser);
            await _context.SaveChangesAsync();

            return Ok(newUser);
        }

        [HttpPost("hash/checkUser")]
        public async Task<ActionResult> CheckUserHash([FromBody] UserDTO user)
        {
            var userDB = await _context.Users.FirstOrDefaultAsync(x => x.Email == user.Email);
            if (userDB == null)
            {
                return Unauthorized();
            }

            var resultHash = _hashService.Hash(user.Password, userDB.Salt);
            if (userDB.Password == resultHash.Hash)
            {
                return Ok();
            }
            else
            {
                return Unauthorized();
            }

        }

        [HttpPost("login")]
        public async Task<ActionResult> Login([FromBody] UserDTO user)
        {
            var userDB = await _context.Users.FirstOrDefaultAsync(x => x.Email == user.Email);
            if (userDB == null)
            {
                return BadRequest();
            }

            var resultHash = _hashService.Hash(user.Password, userDB.Salt);
            if (userDB.Password == resultHash.Hash)
            {
                var response = GenerateToken(user);
                return Ok(response);
            }
            else
            {
                return BadRequest();
            }
        }

        [Authorize]
        [HttpPost("renewToken")]
        public async Task<ActionResult> RenewToken([FromBody] UserDTO user)
        {
            var userDB = await _context.Users.FirstOrDefaultAsync(x => x.Email == user.Email);
            if (userDB == null)
            {
                return BadRequest();
            }

            var response = GenerateToken(user);
            return Ok(response);
        }

        private LoginResponseDTO GenerateToken(UserDTO credentialsUser)
        {
            var claims = new List<Claim>()
            {
                new Claim(ClaimTypes.Email, credentialsUser.Email),
                new Claim("whatever i want", "any other value")
            };

            var key = _configuration["JWTKey"];
            var keyKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
            var signinCredentials = new SigningCredentials(keyKey, SecurityAlgorithms.HmacSha256);

            var securityToken = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.Now.AddDays(30),
                signingCredentials: signinCredentials
            );


            var tokenString = new JwtSecurityTokenHandler().WriteToken(securityToken);

            return new LoginResponseDTO()
            {
                Token = tokenString,
                Email = credentialsUser.Email
            };
        }

        [Authorize]
        [HttpPost("ChangePassword")]
        public async Task<ActionResult> ChangePassword([FromBody] UserChangePasswordDTO userChangePasswordDto)
        {
            // Check that the user exists in the database
            var userDB = await _context.Users.FirstOrDefaultAsync(x => x.Email == userChangePasswordDto.Email);

            if (userDB == null)
            {
                // If the user does not exist, return a 401 Unauthorized
                return Unauthorized("User not found");
            }

            // Check that the current password is correct
            var resultHash = _hashService.Hash(userChangePasswordDto.Password, userDB.Salt);

            if (userDB.Password != resultHash.Hash)
            {
                // If the current password is incorrect, return a 401 Unauthorized
                return Unauthorized("The current password is incorrect");
            }

            // Generate a new hash for the new password
            var newResultHash = _hashService.Hash(userChangePasswordDto.NewPassword);

            // Update the hash and salt in the database
            userDB.Password = newResultHash.Hash;
            userDB.Salt = newResultHash.Salt;

            // Save changes to the database
            _context.Users.Update(userDB);
            await _context.SaveChangesAsync();

            // Return a 200 OK indicating that the change was successful
            return Ok("Password updated successfully");
        }
    }
}
