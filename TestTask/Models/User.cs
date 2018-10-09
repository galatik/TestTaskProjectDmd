using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace TestTask.Models
{
    public class User : IdentityUser
    {
        [MaxLength(36)]
        public override string Id { get; set; }



    }
}
