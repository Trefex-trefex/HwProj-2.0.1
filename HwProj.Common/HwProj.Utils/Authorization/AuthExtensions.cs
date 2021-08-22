﻿using System;
using System.Linq;
using System.Security.Claims;
using HwProj.Models.Roles;
using Microsoft.AspNetCore.Http;

namespace HwProj.Utils.Authorization
{
    public static class AuthExtensions
    {
        public static string GetUserId(this HttpRequest request)
        {   
            var id =  request.HttpContext.User.Claims.FirstOrDefault(claim => claim.Type.ToString() == "_id");
            return id == null
                ? null
                : id.Value;
        }

        public static string GetUserName(this HttpRequest request)
        {
            return request.Query.First(x => x.Key == "_userName").Value.ToString();
        }
        
        public static string GetUserRole(this HttpRequest request)
        {
            var claimRole = "http://schemas.microsoft.com/ws/2008/06/identity/claims/role";
            var role = request.HttpContext.User.Claims.FirstOrDefault(claim => claim.Type.ToString() == claimRole);
            return role == null
                ? null
                : role.Value;
        }

        public static bool IsLecturer(this string role)
        {
            return role == Roles.LecturerRole;
        }
    }
}