using GongDiJiXie.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;

namespace GongDiJiXie.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser,ApplicationRole, string>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
        {
        }
        
       

        //protected override void OnModelCreating(ModelBuilder builder)
        //{
        //    base.OnModelCreating(builder);
        //}
    }

    // .net core Identity 的 Seed() 方法，添加种子数据，  .net core 的播种





    //public class IdentityDbInit : DropCreateDatabaseIfModelChanges<ApplicationDbContext>
    //{
    //    protected override void Seed(ApplicationDbContext context)
    //    {
    //        PerformInitialSetup(context);
    //        base.Seed(context);
    //    }
    //    public void PerformInitialSetup(ApplicationDbContext context)
    //    {
    //        //初始化

    //        ApplicationUserManager userMgr = new ApplicationUserManager(new UserStore<ApplicationUser>(context));
    //        AppRoleManager roleMgr = new AppRoleManager(new RoleStore<AppRole>(context));

    //        //int[] numbers = new int[]{1,2,3,4,5,6};
    //        string[] roleNames = new string[] { "员工", "部门负责人", "业务专员", "财务主管", "分管领导", "领导", "评委", "校内评审" };

    //        for (int i = 0; i < roleNames.Length; i++)
    //        {
    //            if (!roleMgr.RoleExists(roleNames[i]))
    //            {
    //                roleMgr.Create(new AppRole(roleNames[i]));
    //            }
    //        }

    //        string userName = "admin";
    //        string password = "123456";
    //        string email = "admin@example.com";

    //        ApplicationUser user = userMgr.FindByName(userName);
    //        if (user == null)
    //        {

    //            //UserName = model.name, suoshuxueyuan = xueyuan, Email = model.name + "@qq.com", zhenshiname = model.zhenshiname, role = model.role, parentID = -1, usercount = 0
    //            userMgr.Create(new ApplicationUser { UserName = userName, Email = email, suoshuxueyuan = "all", zhenshiname = "admin", role = "业务专员", parentID = -1, usercount = 0 },
    //                password);
    //            user = userMgr.FindByName(userName);
    //        }

    //        if (!userMgr.IsInRole(user.Id, roleNames[2]))//在这里判断admin是不是在数组中的角色，不是就添加到第三下标的角色
    //        {
    //            userMgr.AddToRole(user.Id, roleNames[2]);
    //        }

    //        //foreach (ApplicationUser dbUser in userMgr.Users)
    //        //{
    //        //    dbUser.City = Cities.PARIS;
    //        //}
    //        context.SaveChanges();

    //    }
    //}
}
