using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Logger.Models
{
    public class User
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonElement("username")]
        public string UserName { get; set; }

        [BsonElement("password")]
        public string Password { get; set; }

        [BsonElement("alias")]
        public string Alias { get; set; }

        [BsonElement("roles")]
        public List<string> Roles { get; set; }
    }

    public class UserRegisterRequest
    {
        [Required]
        public string UserName { get; set; }
        [Required]
        public string Password { get; set; }
        [Required]
        public string Alias { get; set; }
    }

    public class UserSignInRequest
    {
        [Required]
        public string UserName { get; set; }
        [Required]
        public string Password { get; set; }
    }

    public class RenewTokenRequest
    {
        [Required]
        public string UserName { get; set; }
    }

    public class AuthResult
    {
        public bool Success { get; set; }
        public string UserName { get; set; }
        public string Alias { get; set; }
        public List<string> Roles { get; set; }
        public string Token { get; set; }
        public string Error { get; set; }
    }

    public class AuthSuccessResult
    {
        public string UserName { get; set; }
        public string Alias { get; set; }
        public List<string> Roles { get; set; }
        public string Token { get; set; }
    }
    public class AuthFailedResult
    {
        public string Error { get; set; }
    }

    public class UserProfile
    {
        public string UserName { get; set; }
        public string Alias { get; set; }
        public List<string> Roles { get; set; }
    }
    
    public class UserList
    {
        public IEnumerable<UserProfile> Profiles { get; set; }
    }

    public class UserUpdateRequest
    {
        [Required]
        public string UserName { get; set; }
        public string Password { get; set; }
        public string Alias { get; set; }
        public List<string> Roles { get; set; }
    }

    public class ChangePasswordRequest
    {
        [Required]
        public string OldPassword { get; set; }
        [Required]
        public string NewPassword { get; set; }
    }
}