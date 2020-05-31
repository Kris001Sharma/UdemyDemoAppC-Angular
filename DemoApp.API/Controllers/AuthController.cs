using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Threading.Tasks;
using DemoApp.API.Data;
using DemoApp.API.Dtos;
using DemoApp.API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace DemoApp.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthRepository _repo;
        private readonly IConfiguration _config;
        public AuthController(IAuthRepository repo, IConfiguration config)
        {
            _config = config;
            _repo = repo;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] UserForRegisterDto userForRegisterDto)
        {
            // we will receive userName n password as one json serialized obj, not separate strings
            // since request coming from user, validate request

            // without [ApiController] that handles infer automatically, and also allows Validatiion
            // if(!ModelState.IsValid)
            //     return BadRequest(ModelState);

            userForRegisterDto.Username = userForRegisterDto.Username.ToLower();
            if (await _repo.UserExists(userForRegisterDto.Username))
                return BadRequest("User name already exists");

            var userToCreate = new User
            {
                Username = userForRegisterDto.Username
            };
            var newUserCreated = await _repo.Register(userToCreate, userForRegisterDto.Password);

            // return CreatedAtRoute(string routeName, object value) to be implemented 
            // send back to clien the location of the new created entity            
            return StatusCode(201);
        }


        [HttpPost("login")]
        public async Task<IActionResult> Login(UserForLoginDto userForLoginDto)
        {
            var userFromRepo = await _repo.Login(userForLoginDto.Username.ToLower(), userForLoginDto.Password);
            if (userFromRepo == null)
                return Unauthorized();

            // the token would be processed by the server without making db call
            // additional pieces of info can be added to it so, server would process token n get Id n Username
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, userFromRepo.Id.ToString()),
                new Claim(ClaimTypes.Name, userFromRepo.Username)
            };

         // secret key (3rd section of Token) 
            // to be stored in appsetting to use at multiple places, need to bring IConfigrations
            // the token should be randomly generated strong key for security
            var key = new SymmetricSecurityKey(System.Text.Encoding.UTF8
                .GetBytes(_config.GetSection("AppSettings:Token").Value));

            // generating credentials using the key and hashing algo to encrypt
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            // Need security token descriptor that will contain
            // claims, expirydate for token and signin creds
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddDays(1),
                SigningCredentials = creds
            };

            // Token handler for the descriptor
            var tokenHandler = new JwtSecurityTokenHandler();
            
            // token contains Jwt token that we need to return to the client
            var token = tokenHandler.CreateToken(tokenDescriptor);

            // return as Obj by writing into the resp that is sent to client
            return Ok(new {
                token = tokenHandler.WriteToken(token)
            });

        }
    }
}