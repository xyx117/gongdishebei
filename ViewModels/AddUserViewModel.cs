using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace GongDiJiXie.ViewModels
{
    public class AddUserViewModel
    {
        [Required]
        [Display(Name = "用户名")]
        public string UserName { get; set; }

        //[Required]
        [DataType(DataType.Password)]
        [Display(Name = "密码")]
        public string Password { get; set; }

        public string Role { get; set; }

        public string PhoneNumber { get; set; }


    }
}
