using GongDiJiXie.Data;
using GongDiJiXie.Models;
using Highsoft.Web.Mvc.Charts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace GongDiJiXie.Controllers
{
    [Authorize]
    
    public class HomeController : Controller
    {
        private readonly GongDiContext _context;
        private readonly ApplicationDbContext _Dbcontext;
        private readonly UserManager<ApplicationUser> userManager;

        public HomeController(GongDiContext context, ApplicationDbContext dbcontext, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _Dbcontext = dbcontext;
            this.userManager = userManager;
        }
        //private readonly ILogger<HomeController> _logger;

        //public HomeController(ILogger<HomeController> logger)
        //{
        //    _logger = logger;
        //}

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult BarChart_admin() 
        {
            return View();
        }

        public IActionResult BarChart() 
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        //获取用户 Mulu_access
        public async Task<string> Mulu_access_getAsync(string userId)
        {
            ApplicationUser user = await userManager.FindByIdAsync(userId);
            var mulu_access = user.Mulu_access;
            return mulu_access;
        }


        [HttpPost]
        public JsonResult Index_json()
        {
            // admin 用户需要显示全部 项目名称，还要做个 角色 或者 username 的判断   
            // 添加用户的时候，就应该给个 input 让可以选 目录的权限，就是说可以看哪些项目

            string[] xm = { };

            //获取登录用户的Id以及 Mulu_access 信息
            var userId = userManager.GetUserId(HttpContext.User);

            ApplicationUser user = userManager.Users.Where(s => s.Id == userId).FirstOrDefault();
            string mulu_access = user.Mulu_access;

            //string mulu_access = Mulu_access_getAsync(userId).ToString();

            //判断mulu_access 是否包含 多个项目名称组成的 字符串
            //判断mulu_access 是 null 的情况下有问题
            if (mulu_access==null)    //新增的用户，在不分配目录权限情况下， mulu_access 的值就是 null
            {
                //admin 用户的 mulu_access 值是null，但是admin 可以看全部
                if (user.UserName=="admin")
                {
                    xm = _context.GongDiXiangMus.Select(s => s.GongDiMingCheng).ToArray();
                }
                else
                {
                    xm = _context.GongDiXiangMus.Where(s => s.GongDiMingCheng.Contains(mulu_access)).Select(s => s.GongDiMingCheng).ToArray();
                }                                          
            }
            else
            {
                if (mulu_access.Contains(","))
                {
                    //拆分mulu_access
                    string[] mulu_arry = mulu_access.Split(',');

                    //匹配  mulu_access 中的每一项，把值累加赋给 变量 xm
                    var mulu_list = new List<string>();
                    var mulu_list_get = new List<string>();

                    foreach (var item in mulu_arry)
                    {
                        mulu_list = _context.GongDiXiangMus.Where(s => s.GongDiMingCheng.Contains(item)).Select(s => s.GongDiMingCheng).ToList();
                        mulu_list_get.AddRange(mulu_list);
                    }
                    xm = mulu_list_get.ToArray();

                    //xm = _context.GongDiXiangMus.Where(s => s.GongDiMingCheng.Contains(mulu_arry)).Select(s => s.GongDiMingCheng).ToArray();
                }
                else
                {
                    xm = _context.GongDiXiangMus.Where(s => s.GongDiMingCheng.Contains(mulu_access)).Select(s => s.GongDiMingCheng).ToArray();
                }
            }                     

            int xm_count = xm.Length;
            
            var json2 = new JObject();
            var json3 = new JObject();             

            JArray base_lst = new(
                                        new JObject(
                                        new JProperty("text", "渣土车录入"),
                                        new JProperty("iconCls", "icon-more"),
                                        new JProperty("url", "/ZhaTuChes_Jilu/Index")),
                                        new JObject(
                                            new JProperty("text", "挖掘机录入"),
                                            new JProperty("iconCls", "icon-more"),
                                            new JProperty("url", "/WaJueJis_Jilu/Index")
                                            ),
                                        new JObject(
                                            new JProperty("text", "费用录入"),
                                            new JProperty("iconCls", "icon-more"),
                                            new JProperty("url", "/FeiYongJiLus/Index")
                                            ),
                                        new JObject(
                                            new JProperty("text", "支付录入"),
                                            new JProperty("iconCls", "icon-more"),
                                            new JProperty("url", "/ZhifuXinXi/Index")
                                            ),
                                        new JObject(
                                            new JProperty("text", "统计分析"),
                                            new JProperty("iconCls", "icon-more"),
                                            new JProperty("url", "/Tongji/Index")       
                                            ),
                                        new JObject(
                                            new JProperty("text", "渣土车设置"),
                                            new JProperty("iconCls", "icon-bumen"),
                                            new JProperty("url", "/ZhaTuChes/Index")
                                            ),
                                        new JObject(
                                            new JProperty("text", "挖掘机设置"),
                                            new JProperty("iconCls", "icon-yonghu"),
                                            new JProperty("url", "/WaJueJis/Index")
                                            ),
                                        new JObject(
                                            new JProperty("text", "地块信息"),
                                            new JProperty("iconCls", "icon-liucheng"),
                                            new JProperty("url", "/GongDiXinxis/Index")
                                            )
                                        );

            JArray xm_list = new();            
             
            for (int i = 0; i < xm_count; i++)
            {
                JObject xm_list_ = new()                
                {
                    new JProperty("text", xm[i]),
                    new JProperty("iconCls", "icon-listmanage"),
                    new JProperty("state", "closed"),
                    //new JProperty("children", base_lst)   //最理想的是 用这种方式加载，但是这样没办法 传递 项目名 参数
                    new JProperty("children", new JArray(
                                        new JObject(
                                        new JProperty("text", "渣土车录入"),
                                        new JProperty("iconCls", "icon-more"),
                                        new JProperty("title", xm[i]),
                                        new JProperty("url", "/ZhaTuChes_Jilu/Index?xm="+ xm[i] + "")),
                                        new JObject(
                                            new JProperty("text", "挖掘机录入"),
                                            new JProperty("iconCls", "icon-more"),
                                            new JProperty("title", xm[i]),
                                            new JProperty("url", "/WaJueJis_Jilu/Index?xm=" + xm[i] + "")
                                            ),
                                        new JObject(
                                            new JProperty("text", "费用录入"),
                                            new JProperty("iconCls", "icon-more"),
                                            new JProperty("title", xm[i]),
                                            new JProperty("url", "/FeiYongJiLus/Index?xm=" + xm[i] + "")
                                            ),
                                        new JObject(
                                            new JProperty("text", "支付录入"),
                                            new JProperty("iconCls", "icon-more"),
                                            new JProperty("title", xm[i]),
                                            new JProperty("url", "/ZhifuXinXi/Index?xm=" + xm[i] + "")
                                            ),
                                        new JObject(
                                            new JProperty("text", "支付查询"),
                                            new JProperty("iconCls", "icon-more"),
                                            new JProperty("title", xm[i]),
                                            new JProperty("url", "/ZhifuXinXi/Zhifuchaxun?xm=" + xm[i] + "")
                                            ),
                                        new JObject(
                                            new JProperty("text", "统计分析"),
                                            new JProperty("iconCls", "icon-more"),
                                            new JProperty("title", xm[i]),
                                            new JProperty("url", "/Tongji/Index?xm=" + xm[i] + "")
                                            ),
                                        new JObject(
                                            new JProperty("text", "渣土车设置"),
                                            new JProperty("iconCls", "icon-bumen"),
                                            new JProperty("title", xm[i]),
                                            new JProperty("url", "/ZhaTuChes/Index?xm=" + xm[i] + "")
                                            ),
                                        new JObject(
                                            new JProperty("text", "挖掘机设置"),
                                            new JProperty("iconCls", "icon-yonghu"),
                                            new JProperty("title", xm[i]),
                                            new JProperty("url", "/WaJueJis/Index?xm=" + xm[i] + "")
                                            ),
                                        new JObject(
                                            new JProperty("text", "地块信息"),
                                            new JProperty("iconCls", "icon-liucheng"),
                                            new JProperty("title", xm[i]),
                                            new JProperty("url", "/GongDiXinxis/Index?xm=" + xm[i] + "")
                                            )
                        ))
                };
                xm_list.Add(xm_list_);
            };            

            JObject json1 = new()                //这里在末尾  tostring(),返回页面无法显示
            { 
                new JProperty("text", "项目管理"), 
                new JProperty("iconCls", "icon-listmanage"),
                new JProperty("state", "open"),
                new JProperty("children", xm_list)
            };            

            json3 = new JObject                //这里在末尾  tostring(),返回页面无法显示
                {
                    new JProperty("text","基本设置"),
                    new JProperty("iconCls","icon-hammer"),
                    new JProperty("state","closed"),
                    new JProperty("children",new JArray(                                                        
                                                        new JObject(
                                                            new JProperty("text","费用分类"),
                                                            new JProperty("iconCls","icon-user"),
                                                            new JProperty("title", "基本设置"),
                                                            new JProperty("url","/FeiYongXinXis/Index")
                                                            ),
                                                        new JObject(
                                                            new JProperty("text","用户管理"),
                                                            new JProperty("iconCls","icon-lock"),
                                                            new JProperty("title", "基本设置"),
                                                            new JProperty("url","/Account/Index")
                                                            ),
                                                        new JObject(
                                                            new JProperty("text","角色管理"),
                                                            new JProperty("iconCls","icon-lock"),
                                                            new JProperty("title", "基本设置"),
                                                            new JProperty("url","/Role/Index")
                                                            ),
                                                        new JObject(
                                                            new JProperty("text","项目设置"),
                                                            new JProperty("iconCls","icon-lock"),
                                                            new JProperty("title", "基本设置"),
                                                            new JProperty("url","/GongDiXiangMu/Index")
                                                            )
                                                        ))
                };
            var json1_ = (JsonConvert.SerializeObject(json1)).ToString();            
            var json3_ = (JsonConvert.SerializeObject(json3)).ToString();
            return Json(new { a = json1_, c = json3_ });      
        }


        //统计页面柱形图 返回的数据
        public JsonResult GetDataList()
        {        
            //取出项目名称集合
            var xmList = new List<string>();
            var xmQry = from c in _context.GongDiXiangMus
                        orderby c.Id descending
                        select c.GongDiMingCheng;
            xmList.AddRange(xmQry.Distinct());

            var chejin_list = new List<decimal>();  //渣土车  车主的 金额合计
            var waji_jinelist = new List<decimal>();//挖掘机  车主的 金额合计
            var chejin_feiyong_list = new List<decimal>(); //渣土车  费用 合计
            var wajijin_feiyong_list = new List<decimal>();  //挖掘机  费用 合计
            var zhifu_jilu_list = new List<decimal>();//项目的支付 合计，这里的支付没有分为 挖掘机支付  或者渣土车支付

            //计算车主的 渣土车合金
            foreach (var item in xmList)
            {
                var chatuche_jilu = _context.ZhaTuChes_Jilus.Where(s => s.XiangMuMingCheng == item);
                var wajueji_jilu = _context.WaJueJis_JiLus.Where(s => s.XiangMuMingCheng == item);                
                var chatuche_feiyong = _context.FeiYongJiLus.Where(t => t.XiangMuMingCheng == item&&t.Leixing=="渣土车");
                var wajueji_feiyong = _context.FeiYongJiLus.Where(s => s.XiangMuMingCheng == item && s.Leixing == "挖掘机");
                var zhifu_jilu = _context.ZhifuXins.Where(s => s.XiangMuMingCheng == item);

                decimal jin_che = 0;
                decimal jin_feiyong_zhatuche = 0;
                decimal jin_waji = 0;
                decimal jin_waji_feiyong = 0;
                decimal jin_zhifu = 0;
                //计算渣土车的 工作 报酬金额 合计
                foreach (var item_che in chatuche_jilu)
                {
                    var danjia = item_che.Price;
                    var checi = item_che.CheCi;
                    jin_che += danjia * checi;
                }
                chejin_list.Add(jin_che);

                //计算 渣土车 费用录入  合计
                foreach (var item_che_feiyong in chatuche_feiyong)
                {
                    jin_feiyong_zhatuche += item_che_feiyong.Total;
                }
                chejin_feiyong_list.Add(jin_feiyong_zhatuche);

                //计算挖掘机 工作的 报酬金额合计
                foreach (var item_che in wajueji_jilu)
                {
                    var danjia = item_che.Price;
                    var gongzuoliang = item_che.Gongzuoliang;
                    jin_waji += danjia * gongzuoliang;
                }
                waji_jinelist.Add(jin_waji);

                //计算挖掘机的 费用录入合计
                foreach (var item_waji_feiyong in wajueji_feiyong)
                {
                    jin_waji_feiyong += item_waji_feiyong.Total;
                }
                wajijin_feiyong_list.Add(jin_waji_feiyong);

                //计算整个项目的  支付合计
                foreach (var item_zhifu in zhifu_jilu)
                {
                    jin_zhifu += item_zhifu.Zhifujine;
                }
                zhifu_jilu_list.Add(jin_zhifu);
            }
            
            var Obj = new
            {
                xm = xmList,
                che_jin = chejin_list,
                che_jin_feiyong = chejin_feiyong_list,                
                waji_jine = waji_jinelist,
                waji_jine_feiyong = wajijin_feiyong_list,
                zhifu_jin = zhifu_jilu_list
            };
            return Json(Obj);
        }


    }
}
