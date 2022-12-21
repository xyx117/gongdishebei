using GongDiJiXie.Data;
using GongDiJiXie.Models;
using GongDiJiXie.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GongDiJiXie.Controllers
{
    //[Authorize(Roles ="admin")]
    //[Authorize(Policy ="仅限管理员")]
    public class AccountController : Controller
    {        
        //三个依赖注入
        private readonly SignInManager<ApplicationUser> signInManager;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly RoleManager<ApplicationRole> roleManager;
        private readonly ApplicationDbContext _context;
        private readonly GongDiContext _Gdcontext;
        public AccountController(SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager, ApplicationDbContext context, GongDiContext gdcontext = null)
        {
            this.signInManager = signInManager;
            this.userManager = userManager;
            this.roleManager = roleManager;
            _context = context;

            //User name '' is invalid, can only contain letters or digits的解决方式  因为用户名带有中文，在indetity Server4中报错
            roleManager.RoleValidators.Clear();
            userManager.UserValidators.Clear();
            _Gdcontext = gdcontext;
        }

        public IActionResult Index()
        {
            var userId = userManager.GetUserId(HttpContext.User);
            
             

            //这里要把角色列表转到前端
            var role = from c in _context.Roles
                       orderby c.Name descending
                       select c.Name;
            ViewBag.role = role;
            var mulu = from s in _Gdcontext.GongDiXiangMus
                       orderby s.Id descending
                       select s.GongDiMingCheng;
            ViewBag.mulu = mulu;

            return View();
        }

        //Getuser
        [HttpPost]
        public JsonResult Getuser() 
        {                     
            int page = (Request.Form["page"] != "") ? int.Parse(Request.Form["page"]) : 1;
            int rows = (Request.Form["rows"] != "") ? int.Parse(Request.Form["rows"]) : 10;

            //var xm = _context.Users.Where(c=>c.UserName=="admin").Select(c=>c.PasswordHash);
            var xm = from c in _context.Users                     
                     orderby c.Id 
                     select new { id = c.Id,role= c.Role, username = c.UserName, phonenumber = c.PhoneNumber,mulu_access = c.Mulu_access };                               
            try
            {
                // 返回到前台的值必须按照如下的格式包括 total and rows 
                var easyUIPages = new Dictionary<string, object>
                {
                    { "total", xm.Count() },
                    { "rows", xm.ToPagedList(page, rows) }
                };
                return Json(easyUIPages);
            }
            catch (Exception)
            {
                throw;
            }
        }

        [AllowAnonymous]
        public IActionResult Login(string returnUrl)
        {            
            ViewBag.returnUrl = returnUrl;
            return View();            
        }        

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel loginViewModel)
        {
            if (!ModelState.IsValid)
            {
                return View(loginViewModel);
            }
            var user = await userManager.FindByNameAsync(loginViewModel.UserName);

            if (user != null)
            {
                //var result = await signInManager.PasswordSignInAsync(user, loginViewModel.Password, false, false);
                var result = await signInManager.PasswordSignInAsync(user, loginViewModel.Password, false, false);

                if (result.Succeeded)
                {
                    return RedirectToAction("Index", "Home");    
                }
            }
            ModelState.AddModelError("", "用户名/密码不正确");
            return View(loginViewModel);
        }

        public IActionResult Register()
        {
            return View();
        }

        //注册用户，用于初始化添加用户，判断用户名是不是叫 admin ，是的话就置为  super 管理员
        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel registerViewModel)
        {
            if (ModelState.IsValid)
            {
                //var name = registerViewModel.UserName;//只有admin可以初始化注册，其余的注册要做个 重名检测 都给屏蔽掉
                
                var user = new ApplicationUser
                {
                    UserName = registerViewModel.UserName
                };
                var result = await userManager.CreateAsync(user, registerViewModel.Password);
                if (result.Succeeded)
                {
                    return RedirectToAction("Index", "Home");
                }
            }
            return View(registerViewModel);
        }

        //添加用户，这里添加的用户 全部 设置为 一般用户
        [HttpPost]
        public async Task<IActionResult> Adduser(AddUserViewModel addUserViewModel)/*RegisterViewModel registerViewModel*/
        {
            if (ModelState.IsValid)
            {
                var role = Request.Form["role"];
                if (addUserViewModel.Password == "" || addUserViewModel.Password == null)
                {
                    var user = new ApplicationUser
                    {
                        UserName = addUserViewModel.UserName,
                        Role = addUserViewModel.Role,
                        PhoneNumber = addUserViewModel.PhoneNumber
                    };
                    var result = await userManager.CreateAsync(user, "Abc@123456");  //添加用户                    
                    var result2 = await userManager.AddToRoleAsync(user, role);    //添加用户到角色，这里要先创建好用户角色才能添加用户到角色
                    if (result.Succeeded && result2.Succeeded)
                    {                        
                        return Json(new { success = true, msg = "添加用户成功" });
                    }
                }
                else
                {
                    var user = new ApplicationUser
                    {
                        UserName = addUserViewModel.UserName,
                        Role = addUserViewModel.Role,
                        PhoneNumber = addUserViewModel.PhoneNumber
                    };
                    var result = await userManager.CreateAsync(user, addUserViewModel.Password);   //添加用户
                    var result2 = await userManager.AddToRoleAsync(user, role);                   //添加用户到角色
                    if (result.Succeeded && result2.Succeeded)
                    {                     
                        return Json(new { success = true, msg = "添加用户成功" });
                    }
                }                
            }
            return View("Index");
        }

        //编辑用户    有bug
        [HttpPost]
        public async Task<IActionResult> Edituser(EditUserViewModel edituserViewModel)
        {
            
            if (edituserViewModel != null && ModelState.IsValid)
            {                
                try
                {
                    //var user = await userManager.FindByIdAsync(edituserViewModel.Id);
                    ApplicationUser user = await userManager.FindByIdAsync(edituserViewModel.Id);
                    var username = user.UserName;                    
                    if (user == null)
                    {
                        return Json(new { success = false, msg = "用户不存在" });
                    }
                    var role = user.Role;
                    IdentityResult result, result2, result3;

                    user = new ApplicationUser 
                    {
                        //Id = edituserViewModel.Id,
                        UserName = username,
                        Role = edituserViewModel.Role,
                        PhoneNumber = edituserViewModel.PhoneNumber                        
                    };
                    result = await userManager.UpdateAsync(user);  //添加用户
                                                                                       
                    if (role != null)
                    {
                        result2 = await userManager.RemoveFromRoleAsync(user, role);    //添加用户到角色，这里要先创建好用户角色才能添加用户到角色
                        result3 = await userManager.AddToRoleAsync(user, edituserViewModel.Role);
                        if (result.Succeeded&& result2.Succeeded && result3.Succeeded)
                        {
                            return Json(new { success = true, msg = "编辑用户成功" });
                        }
                    }
                    else
                    {
                        result3 = await userManager.AddToRoleAsync(user, edituserViewModel.Role);
                        if (result.Succeeded && result3.Succeeded)
                        {
                            return Json(new { success = true, msg = "编辑用户成功" });
                        }
                    }                                                                                                           
                }
                catch (Exception ex)
                {
                    //return Json(new { success = false, msg = "编辑用户出现问题" });
                    return Json(new { success = false, msg = ex.ToString() });
                }
            }
            return View("Index");
        }

        //删除用户  是不是还要删除 Role 中的对应关系
        [HttpPost]        
        public async Task<JsonResult> DelUser(string id)
        {
            string errormsg2 = "";            
            ApplicationUser user = await userManager.FindByIdAsync(id);
            try
            {
                IdentityResult result = await userManager.DeleteAsync(user);//删除选出的员工
                if (result.Succeeded)
                {
                    return Json(new { success = true, msg = "删除用户成功！" });
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        errormsg2 += error;
                    }
                    return Json(new { success = false, msg = errormsg2.ToString() });
                }
            }
            catch (Exception ex) 
            {
                return Json(new { success = false, msg = ex.ToString() });
            }
        }

        //登录人员重置密码
        [HttpPost]
        //[AllowAnonymous]
        public async Task<IActionResult> Setpwd(string id)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    string errormsg = "";
                    ApplicationUser user = await userManager.FindByIdAsync(id);//这里多一步，可以直接通过登录人信息取得用户id（主键）
                    string password = "Abc@123456";
                    //获取 令牌时报错：No IUserTwoFactorTokenProvider<TUser> named 'Default' is registered.  
                    //这里要在 startup.cs 中添加 .AddDefaultTokenProviders() 的配置
                    string code = await userManager.GeneratePasswordResetTokenAsync(user);//根据user.id产生code,在forgotpassword中有
                    if (user == null)
                    {
                        return Json(new { success = false, msg = "找不到用户。" });
                    }
                    IdentityResult result = await userManager.ResetPasswordAsync(user, code, password);//ResetPasswordAsync方法需要有三个参数
                    if (result.Succeeded)
                    {
                        return Json(new { success = true, msg = "修改密码成功！" });
                    }
                    else
                    {
                        foreach (var error in result.Errors)
                        {
                            errormsg += error;
                        }
                        return Json(new { success = false, msg = errormsg });
                    }
                }
                catch (Exception ex)
                {
                    return Json(new { success = false, msg = ex.ToString() });
                }                
            }
            // 如果我们进行到这一步时某个地方出错，则重新显示表单            
            return Json(new { success = false, msg = "重置密码有误！" });
        }


        //由 Id 获取登录人员 UserName 和 Role
        [HttpPost]
        //[AllowAnonymous]
        public async Task<JsonResult> GetNameRole(string id)
        {
            if (id!= "")
            {
                try
                {
                    ApplicationUser user = await userManager.FindByIdAsync(id);//这里多一步，可以直接通过登录人信息取得用户id（主键）
                    string username = user.UserName; 
                    string userrole = user.Role;
                    return Json(new { success = true, name= username, role= userrole });                    
                }
                catch (Exception ex)
                {
                    return Json(new { success = false, name = ex.ToString(), role = ex.ToString() });
                }
            }
            // 如果我们进行到这一步时某个地方出错，则重新显示表单            
            return Json(new { success = false });
        }

        //返回用户可以编辑目录的 列表, Mulu_access 字段已经有内容情况下  的加载已有内容
        public JsonResult Load__access(string id)
        {
            var user = _context.Users.Where(s => s.Id == id).FirstOrDefault();
            //取出id用户的 可以编辑的 权限,这可能是 一串包含 分隔符 ， 的字符串
            var mulu = user.Mulu_access;

            var GenreLst = new List<string>();
            var GenreQry = from s in _context.Users.Where(s => s.Id == id)
                           orderby s.Id
                           select new { id = s.Id, it = s.Id };
            //GenreLst.AddRange(GenreQry.Distinct());

            return Json(GenreLst.ToList());

        }

        //提交按钮，保存 用户可以 访问 的目录
        public JsonResult Set_mulu_access(string id)
        {
            var user = _context.Users.Where(s => s.Id == id).FirstOrDefault();
            string mulu = Request.Form["calim_input"];
            user.Mulu_access = mulu;
           
            _context.Entry(user).State = EntityState.Modified;
            _context.SaveChanges();
            return Json(new { success = true, msg="更改访问目录成功！" });            
        }

        //判断用户名存在就返回false,不存在就返回true
        [HttpPost]
        public async Task<String> CheckNameIsSame(string name) 
        {
            string isOk = "False";
            var xm = await userManager.FindByNameAsync(name);
            if (xm == null)   //不存在重名
            {
                isOk = "True";
            }
            return isOk + "";
        }

        [HttpPost]
        public async Task<IActionResult> LogOff() 
        {
            await signInManager.SignOutAsync();
            return RedirectToAction("Login", "Account");
        }
    }
}
