using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using System.Text.Json.Serialization;

namespace DBSQLClient.Models
{
    public class User
    {
        [PrimaryKey]
        public int Id { get; set; }

        public string Name { get; set; }

        [OneToMany(typeof(UserRole))]
        public List<UserRole> Roles { get; set; } = new();
    }
    public class Role
    {
        [PrimaryKey]
        public int Id { get; set; }

        public string Name { get; set; }
    }
    public class UserRole
    {
        [ForeignKey(typeof(User))]
        public int UserId { get; set; }

        [ForeignKey(typeof(Role))]
        public int RoleId { get; set; }
    }

}
