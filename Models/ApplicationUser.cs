using GongDiJiXie.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace GongDiJiXie.Models
{
    //这是一个 自定义用户字段 类
    //public class ApplicationUser : IdentityUser<Guid>
    public class ApplicationUser : IdentityUser
    {
        //迁移命令
        //Add-Migration -Context ApplicationDbContext 20221023
        //Update-Database -Context ApplicationDbContext

        public string Role { get; set; }         
        
        /// <summary>
        /// 可以浏览的目录权限
        /// </summary>
        public string Mulu_access { get; set; }
        
        public string CC { get; set; }        
        
        public string DD { get; set; }

        public string EE { get; set; }

    }


    
}
