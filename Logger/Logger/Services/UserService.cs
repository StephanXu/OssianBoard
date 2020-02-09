using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using Logger.Models;

namespace Logger.Services
{
    public class UserService
    {
        private readonly IMongoCollection<Models.User> _users;
        private readonly string _secret;
        
        public UserService(IConfiguration config)
        {
            var client = new MongoClient(config.GetConnectionString("OnlineLogger"));
            var database = client.GetDatabase("OnlineLogger");
            _users = database.GetCollection<Models.User>("users");
            _secret = config.GetSection("AppSettings")["Secret"];
        }

        public IEnumerable<UserProfile> GetUsers() =>
            _users.AsQueryable()
                .Select(p => new Models.UserProfile {UserName = p.UserName, Alias = p.Alias, Roles = p.Roles});


        public async Task<bool> Create(Models.User user)
        {
            if (0 < await _users.CountDocumentsAsync(item => user.UserName == item.UserName))
            {
                return false;
            }

            user.Password = BCrypt.Net.BCrypt.HashPassword(user.Password);
            await _users.InsertOneAsync(user);
            return true;
        }

        public Models.AuthResult Authenticate(Models.UserSignInRequest signInInfo)
        {
            var (status, message, user) = VerifyPassword(signInInfo.UserName, signInInfo.Password);
            if (!status)
            {
                return new AuthResult
                {
                    Success = false,
                    Error = message
                };
            }

            return GenerateToken(user);
        }

        public Models.AuthResult RenewToken(string userName)
        {
            var user = _users.Find(user => user.UserName == userName).SingleOrDefault();
            if (null == user)
                return new Models.AuthResult
                {
                    Success = false,
                    Error = "User dose not exist."
                };
            return GenerateToken(user);
        }

        private Models.AuthResult GenerateToken(Models.User user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_secret);

            List<Claim> getClaims()
            {
                var claims = new List<Claim> {new Claim(ClaimTypes.Name, user.UserName)};
                claims.AddRange(user.Roles.Select(role => new Claim(ClaimTypes.Role, role)));
                return claims;
            }

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(getClaims()),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return new Models.AuthResult
            {
                Success = true,
                UserName = user.UserName,
                Alias = user.Alias,
                Roles = user.Roles,
                Token = tokenHandler.WriteToken(token),
                Error = null
            };
        }

        public Models.UserProfile Update(string userName, Models.User updateInfo)
        {
            var user = _users.Find(user => user.UserName == userName).SingleOrDefault();
            if (null == user)
                return null;
            user.Password = updateInfo.Password == null
                ? user.Password
                : BCrypt.Net.BCrypt.HashPassword(updateInfo.Password);
            user.Roles = updateInfo.Roles == null
                ? user.Roles
                : updateInfo.Roles;
            user.Alias = updateInfo.Alias == null ? user.Alias : updateInfo.Alias;
            _users.ReplaceOne(item => item.Id == user.Id, user);
            return new Models.UserProfile
            {
                UserName = user.UserName,
                Alias = user.Alias,
                Roles = user.Roles
            };
        }

        public Models.UserProfile GetProfile(string username)
        {
            var user = _users.Find(user => user.UserName == username).SingleOrDefault();
            if (null == user)
                return null;
            return new Models.UserProfile
            {
                UserName = user.UserName,
                Alias = user.Alias,
                Roles = user.Roles
            };
        }

        public (bool status, string message, User user) VerifyPassword(string userName, string password)
        {
            var user = _users.Find(user => user.UserName == userName).SingleOrDefault();
            if (null == user)
                return (status: false, message: "User dose not exist.", null);
            return !BCrypt.Net.BCrypt.Verify(password, user.Password)
                ? (status: false, message: "Password is not correct.", null)
                : (status: true, message: "", user);
        }
    }
}