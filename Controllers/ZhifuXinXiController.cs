using GongDiJiXie.Data;
using GongDiJiXie.Models;
using GongDiJiXie.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;

namespace GongDiJiXie.Controllers
{
    public class ZhifuXinXiController : Controller
    {
        private readonly GongDiContext _context;

        public ZhifuXinXiController(GongDiContext context)
        {
            _context = context;
        }

        // GET: ZhaTuChes_Jilu
        public IActionResult Index(string xm)
        {
            //渣土车工作记录录入时获取渣土车车牌信息
            var zhatucheLst = new List<string>();
            var zhatucheQry = from c in _context.ZhaTuChes_Jilus
                           where c.XiangMuMingCheng == xm         //筛选条件是 这个项目里 的渣土车
                           orderby c.Id
                           select c.CheZhu;
            zhatucheLst.AddRange(zhatucheQry.Distinct());

            //找挖掘机工作时的车主
            var wajuejiLst = new List<string>();
            var wajuejiQry = from c in _context.WaJueJis_JiLus 
                              where c.XiangMuMingCheng == xm         //筛选条件是 这个项目里 的渣土车
                              orderby c.Id
                              select c.Chezhu;
            wajuejiLst.AddRange(wajuejiQry.Distinct());                   
            zhatucheLst.AddRange(wajuejiLst);   //渣土车的车主 加上 挖掘机的 车主，最后再做个去重，返回一个集合     
            var chezhu_all = zhatucheLst.Distinct().ToList();   //渣土车和挖掘机车主合并后，做一个去重,这里查了好久的资料

            //这里是要在  datagrid 底部 显示一个合计信息
            //这里要做一个合计，就是渣土车和挖掘机工作总金额的合计
            //是否要做到针对个人？？？？            
            var zhatuche = _context.ZhaTuChes_Jilus.Where(s=>s.XiangMuMingCheng==xm);
            var wajueji = _context.WaJueJis_JiLus.Where(s => s.XiangMuMingCheng == xm);
            var feiyiong = _context.FeiYongJiLus.Where(s => s.XiangMuMingCheng == xm);


            decimal jine_zhatuche=0;
            decimal jine_wajueji=0;
            decimal jine_feiyong = 0;
            foreach (var item in zhatuche)
            {
                var checi = item.CheCi;
                var danjia = item.Price;
                jine_zhatuche += danjia * checi;
            }
            foreach (var item in wajueji)
            {
                var gongzuoliang = item.Gongzuoliang;                              
                var price = item.Price;
                jine_wajueji += gongzuoliang * price;
            }
            foreach (var item in feiyiong)
            {
                jine_feiyong += item.Total;
            }
            //项目的 渣土车和挖掘机总 金额
            decimal heji = jine_wajueji + jine_zhatuche;            

            //还要往页面传递，已经自付记录的 总金额
            var yifu_jine = _context.ZhifuXins.Where(s => s.XiangMuMingCheng == xm).Select(s => s.Zhifujine);
            decimal jine_zhifu = 0;

            //接下来要做一个遍历集合的自加
            foreach (var item in yifu_jine)
            {
                jine_zhifu += item;
            }
            decimal shenyu_jine = heji - jine_zhifu - jine_feiyong;
            var msg = "项目" + xm + "在渣土车、挖掘机合计工作金额" + heji + "(元)，其中费用录入支付：" + jine_feiyong + "(元)、支付录入支付" + jine_zhifu + "(元)," + "剩余未支付" + shenyu_jine + "(元)。";
            ViewBag.xm = xm;   //把从sidemenu item.url 取来的 项目名  传值
            ViewBag.chezhu = chezhu_all;            
            ViewBag.msg = msg;

            return View();
        }

        /// <summary>
        ///   获取所有渣土车的信息
        /// </summary>        
        [HttpPost]
#pragma warning disable CS8632 // 只能在 "#nullable" 注释上下文内的代码中使用可为 null 的引用类型的注释。
        public JsonResult Get_zhifu(string xm, DateTime? kaishi, DateTime? jieshu, string? chezhu_filt)/*string searchquery*/
#pragma warning restore CS8632 // 只能在 "#nullable" 注释上下文内的代码中使用可为 null 的引用类型的注释。
        {
            int page = (Request.Form["page"] != "") ? int.Parse(Request.Form["page"]) : 1;
            int rows = (Request.Form["rows"] != "") ? int.Parse(Request.Form["rows"]) : 10;

            var zhifu = from c in _context.ZhifuXins
                        where c.XiangMuMingCheng == xm && (kaishi != null ? c.Zhifushijian > kaishi : c.Zhifushijian < DateTime.Now) && (jieshu != null ? c.Zhifushijian < jieshu : c.Zhifushijian < DateTime.Now) && (chezhu_filt != null ? c.Chezhu == chezhu_filt : c.Chezhu != null)
                        orderby c.Id
                               select new
                               {
                                   id = c.Id,
                                   xiangmumingcheng = c.XiangMuMingCheng,
                                   zhifushijian = c.Zhifushijian,
                                   zhifujine = c.Zhifujine,
                                   zhifubeizhu = c.Zhifubeizhu,                                                                                                                                     
                                   chezhu = c.Chezhu
                               };
            try
            {
                var easyUIPages = new Dictionary<string, object>
                {
                    { "total", zhifu.Count() },
                    { "rows", zhifu.ToPagedList(page, rows) }
                };
                return Json(easyUIPages);
            }
            catch (Exception)
            {
                throw;
            }
        }

        //前端页面点击搜索按钮后触发ajax事件，返回搜索结果的 车次 合计、金额合计 
        [HttpPost]
#pragma warning disable CS8632 // 只能在 "#nullable" 注释上下文内的代码中使用可为 null 的引用类型的注释。
        public ContentResult Zhifu_tishi(string xm, DateTime? kaishi, DateTime? jieshu, string? chezhu_filt)
#pragma warning restore CS8632 // 只能在 "#nullable" 注释上下文内的代码中使用可为 null 的引用类型的注释。        
        {
            if (kaishi==null&&jieshu==null&&chezhu_filt==null)
            {
                var zhatuche = _context.ZhaTuChes_Jilus.Where(s => s.XiangMuMingCheng == xm);
                var wajueji = _context.WaJueJis_JiLus.Where(s => s.XiangMuMingCheng == xm);
                var feiyiong = _context.FeiYongJiLus.Where(s => s.XiangMuMingCheng == xm);

                decimal jine_zhatuche = 0;
                decimal jine_wajueji = 0;
                decimal jine_feiyong = 0;
                foreach (var item in zhatuche)
                {
                    var checi = item.CheCi;
                    var danjia = item.Price;
                    jine_zhatuche += danjia * checi;
                }
                foreach (var item in wajueji)
                {
                    var gongzuoliang = item.Gongzuoliang;
                    var price = item.Price;
                    jine_wajueji += gongzuoliang * price;
                }
                foreach (var item in feiyiong)
                {
                    jine_feiyong += item.Total;
                }
                //项目的 渣土车和挖掘机总 金额
                decimal heji = jine_wajueji + jine_zhatuche;

                //还要往页面传递，已经自付记录的 总金额
                var yifu_jine = _context.ZhifuXins.Where(s => s.XiangMuMingCheng == xm).Select(s => s.Zhifujine);
                decimal jine_zhifu = 0;

                //接下来要做一个遍历集合的自加
                foreach (var item in yifu_jine)
                {
                    jine_zhifu += item;
                }
                decimal shenyu_jine = heji - jine_zhifu - jine_feiyong;
                var msg = "项目" + xm + "在渣土车、挖掘机合计工作金额" + heji + "(元)，其中费用录入支付：" + jine_feiyong + "(元)、支付录入支付" + jine_zhifu + "(元)," + "剩余未支付" + shenyu_jine + "(元)。";                                
                return Content(msg);
            }
            else
            {
                //在linq的where判断中用三元表达式，判断参数值是否需要过滤           
                var zhatuche = _context.ZhifuXins.Where(c => c.XiangMuMingCheng == xm && (kaishi != null ? c.Zhifushijian > kaishi : c.Zhifushijian < DateTime.Now) && (jieshu != null ? c.Zhifushijian < jieshu : c.Zhifushijian < DateTime.Now) && (chezhu_filt != null ? c.Chezhu == chezhu_filt : c.Chezhu != null));
                decimal jine_zhifu = 0;//每条记录 车次*单价的 合计金额

                foreach (var item in zhatuche)
                {
                    jine_zhifu += item.Zhifujine;
                }
                var msg = "项目" + xm + "支付金额合计：" + jine_zhifu + "(元)。";

                return Content(msg);
            }                                              
        }

        /// <summary>
        /// 保存新增的渣土车信息
        /// </summary> 
        [HttpPost]
        public JsonResult Save_zhifu([Bind("Chezhu,Zhifushijian,Zhifujine,Zhifubeizhu")] ZhifuXinXi zhifu, string xm)
        {
            if (ModelState.IsValid)
            {
                using (TransactionScope transaction = new())//原子操作，事物错误回滚
                {
                    try
                    {
                        //添加判断，添加的记录 支付金额 是否超过了 还未支付的钱数
                        var zhifu_jine = zhifu.Zhifujine;
                        var chezhu = zhifu.Chezhu;

                        
                        //总工资  = 渣土车记录工资 + 挖掘机记录工资                       
                        // 未付工资 = 总工资 - 支付记录工资
                        var zhatuche = _context.ZhaTuChes_Jilus.Where(s => s.XiangMuMingCheng == xm & s.CheZhu == chezhu);
                        var wajueji = _context.WaJueJis_JiLus.Where(s => s.XiangMuMingCheng == xm & s.Chezhu == chezhu);

                        decimal jine_zhatuche = 0;
                        decimal jine_wajueji = 0;
                        foreach (var item in zhatuche)
                        {
                            var checi = item.CheCi;
                            var danjia = item.Price;
                            jine_zhatuche += danjia * checi;
                        }
                        foreach (var item in wajueji)
                        {
                            var gongzuoliang = item.Gongzuoliang;

                            var price = item.Price;

                            jine_wajueji += gongzuoliang * price;
                        }
                        //项目的 渣土车和挖掘机总 金额
                        decimal heji = jine_wajueji + jine_zhatuche;


                        //还要往页面传递，已经自付记录的 总金额
                        var yifu_jine = _context.ZhifuXins.Where(s => s.XiangMuMingCheng == xm & s.Chezhu == chezhu).Select(s => s.Zhifujine);
                        decimal jine = 0;
                        //接下来要做一个遍历集合的自加
                        foreach (var item in yifu_jine)
                        {
                            jine += item;
                        }
                        ViewBag.jine = jine;  //项目已经支付金额总额合计
                        ViewBag.heji = heji;                        
                        ViewBag.chezhu = chezhu;
                        var weifu = heji - jine;
                        if (weifu< zhifu_jine)
                        {
                            return Json(new { success = false, msg ="此次支付金额已经超出 车主 应得合计金额，再检查一下！" }); /*, "text/html"*/
                        }


                        zhifu.XiangMuMingCheng = xm;
                        _context.ZhifuXins.Add(zhifu);
                        _context.SaveChanges();

                        transaction.Complete();
                    }
                    catch (Exception ex)
                    {
                        return Json(new { success = false, msg = ex.ToString() });
                    }
                    finally
                    {
                        transaction.Dispose();
                    }
                }
                return Json(new { success = true, msg = "添加支付记录成功" });/*, "text/html"*/
            }
            return Json(new { success = false, msg = "添加记录失败，请再试一试！" });
        }


        /// <summary>
        /// 编辑渣土车信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult Update_zhifu(int id)  //string bmname 好像多余
        {
            var zhifu = _context.ZhifuXins.Where(c => c.Id == id).FirstOrDefault();

            zhifu.Chezhu = Request.Form["chezhu"];

            zhifu.Zhifushijian = Convert.ToDateTime(Request.Form["zhifushijian"]);

            zhifu.Zhifujine = Convert.ToDecimal(Request.Form["zhifujine"]);

            zhifu.Zhifubeizhu = Request.Form["zhifubeizhu"];            

            using (TransactionScope transaction = new())//原子操作，事物错误回滚
            {
                try
                {
                    _context.Entry(zhifu).State = EntityState.Modified;
                    _context.SaveChanges();
                    transaction.Complete();
                }
                catch (Exception ex)
                {
                    return Json(new { success = false, msg = ex.ToString() });
                }
                finally
                {
                    transaction.Dispose();
                }
            }
            return Json(new { success = true, msg = "更改支付信息成功！" });
        }

        /// <summary>
        /// 删除渣土车信息
        /// </summary>
        /// <param name="id"></param>
        /// <param name="xmid"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult Del_zhifu(int id)
        {
            ZhifuXinXi zhifu = _context.ZhifuXins.Find(id);

            using (TransactionScope transaction = new())//原子操作，事物错误回滚
            {
                try
                {
                    _context.ZhifuXins.Remove(zhifu);
                    _context.SaveChanges();

                    transaction.Complete();
                }
                catch (Exception ex)
                {
                    return Json(new { success = false, msg = ex.ToString() });
                }
                finally
                {
                    transaction.Dispose();
                }
            }
            return Json(new { success = true });
        }

        //鼠标滑动到  datagrid 的车主列时，显示这个车主的支付信息，包括总金额、已支付金额、未支付金额
        [HttpPost]
        public JsonResult Chezhu_zhifu(string xm, string chezhu)
        {
            //要找出车主在这个项目中的总工资  和  已经支付 的工资，做一个百分比
            //总工资  = 渣土车记录工资 + 挖掘机记录工资
            //这里要做一个合计，就是渣土车和挖掘机工作总金额的合计
            //是否要做到针对个人，这里针对到了个人
            var zhatuche = _context.ZhaTuChes_Jilus.Where(s => s.XiangMuMingCheng == xm & s.CheZhu == chezhu);
            var wajueji = _context.WaJueJis_JiLus.Where(s => s.XiangMuMingCheng == xm & s.Chezhu == chezhu);
            var feiyong = _context.FeiYongJiLus.Where(s => s.XiangMuMingCheng == xm && s.Chezhu == chezhu);
            var yifu_jine = _context.ZhifuXins.Where(s => s.XiangMuMingCheng == xm & s.Chezhu == chezhu).Select(s => s.Zhifujine);

            decimal jine_zhatuche = 0;
            decimal jine_wajueji = 0;
            decimal jine_feiyong = 0;
            decimal jine_yifu = 0;
            //渣土车做一个合计
            foreach (var item in zhatuche)
            {
                var checi = item.CheCi;
                var danjia = item.Price;
                jine_zhatuche += danjia * checi;
            }
            //挖掘机做一个合计
            foreach (var item in wajueji)
            {
                var gongzuoliang = item.Gongzuoliang;
                var price = item.Price;
                jine_wajueji += gongzuoliang * price;
            }
            //费用录入 做一个合计
            foreach (var item in feiyong)
            {
                jine_feiyong += item.Total;
            }
            //支付记录 做一个合计
            foreach (var item in yifu_jine)
            {
                jine_yifu += item;
            }
            //项目的 渣土车和挖掘机总 金额
            decimal heji = jine_wajueji + jine_zhatuche;                                  
            //ViewBag.jine = jine_yifu;  //项目已经支付金额总额合计
            //ViewBag.heji = heji;
            //ViewBag.xm = xm;
            //ViewBag.chezhu = chezhu;

            //剩余未付金额 = 挖掘机金额 + 渣土车金额 - 已支付金额 - 费用金额
            var weifu = heji - jine_yifu-jine_feiyong;

            string tip = chezhu + "在" + xm + "项目中渣土车、挖掘机工作合计应获得：" + heji + "（元)，其中已支付：" + jine_yifu + "(元),费用支付："+jine_feiyong+"(元),剩余未付：" + weifu + "(元)";
            return Json(new { success = true, msg= tip });
        }


        /// <summary>
        /// 支付查询页面，显示每一个车主收款信息
        /// </summary>
        /// <param name="xm"></param>
        /// <param name="chezhu"></param>
        /// <returns></returns> 
        public IActionResult Zhifuchaxun(string xm)
        {
            //只要是在 渣土车记录表  和  挖掘机记录表  有记录的车主都有钱
            //渣土车工作记录录入时获取渣土车车牌信息
            var zhatucheLst = new List<string>();
            var zhatucheQry = from c in _context.ZhaTuChes_Jilus
                              where c.XiangMuMingCheng == xm         //筛选条件是 这个项目里 的渣土车
                              orderby c.Id
                              select c.CheZhu;
            zhatucheLst.AddRange(zhatucheQry.Distinct());

            //找挖掘机工作时的车主
            var wajuejiLst = new List<string>();
            var wajuejiQry = from c in _context.WaJueJis_JiLus
                             where c.XiangMuMingCheng == xm         //筛选条件是 这个项目里 的渣土车
                             orderby c.Id
                             select c.Chezhu;
            wajuejiLst.AddRange(wajuejiQry.Distinct());
            zhatucheLst.AddRange(wajuejiLst);   //渣土车的车主 加上 挖掘机的 车主，最后再做个去重，返回一个集合     
            var chezhu_all = zhatucheLst.Distinct().ToList();   //渣土车和挖掘机车主合并后，做一个去重,这里查了好久的资料

            
            var chaxun_list = new List<Zhifuchaxun>();
            //计算车主的 渣土车、挖掘机合金
            foreach (var item in chezhu_all)
            {
                Zhifuchaxun zhifu = new();  //这是要返回给视图的 模型
                var chatuche_jilu = _context.ZhaTuChes_Jilus.Where(s => s.XiangMuMingCheng == xm&&s.CheZhu==item);
                var wajueji_jilu = _context.WaJueJis_JiLus.Where(d => d.XiangMuMingCheng == xm && d.Chezhu == item);
                //支付金
                var zhifu_jilu = _context.ZhifuXins.Where(x => x.XiangMuMingCheng == xm && x.Chezhu == item);
                //费用金额
                var feiyong_jine = _context.FeiYongJiLus.Where(y => y.XiangMuMingCheng == xm && y.Chezhu == item);

                decimal jin_che = 0;
                decimal jin_ji = 0;
                decimal jin_zhifu = 0;
                decimal jin_feiyong = 0;
                foreach (var item_che in chatuche_jilu)
                {
                    var danjia = item_che.Price;
                    var checi = item_che.CheCi;
                    jin_che += danjia * checi;
                }
                foreach (var item_ji in wajueji_jilu)
                {
                    var danjia = item_ji.Price;
                    var gongzuoliang = item_ji.Gongzuoliang;
                    jin_ji += danjia * gongzuoliang;
                }
                foreach (var item_feiyon in feiyong_jine)
                {
                    jin_feiyong += item_feiyon.Total;
                }
                //支付金要先判断是否有这里个记录, 不做判读其实也可以，没有记录 金额 就是初始值0；
                foreach (var item_zhifu in zhifu_jilu)
                {
                    jin_zhifu += item_zhifu.Zhifujine;
                }

                // 应付金额 = 挖掘机+渣土车-费用-已付金额
                zhifu.Chezhu = item;
                zhifu.Jine = jin_che + jin_ji;                
                zhifu.Yifu = jin_zhifu;
                zhifu.Yue = jin_che + jin_ji - jin_zhifu - jin_feiyong;   //支付余额
                zhifu.Feiyong = jin_feiyong;
                zhifu.Chaoe = jin_feiyong + jin_zhifu - jin_che - jin_ji;

                chaxun_list.Add(zhifu);
            }                       
            ViewBag.xm = xm;
            return View(chaxun_list);
        }



        //跳转到查看这车主工资支付情况页面  ，这个页面作废了
        public IActionResult Chezhu_zhifu_page(string xm, string chezhu)
        {           
            //要找出车主在这个项目中的总工资  和  已经支付 的工资，做一个百分比
            //总工资  = 渣土车记录工资 + 挖掘机记录工资
            //这里要做一个合计，就是渣土车和挖掘机工作总金额的合计
            //是否要做到针对个人？？？？
            var zhatuche = _context.ZhaTuChes_Jilus.Where(s => s.XiangMuMingCheng == xm & s.CheZhu == chezhu);
            var wajueji = _context.WaJueJis_JiLus.Where(s => s.XiangMuMingCheng == xm & s.Chezhu == chezhu);
            
            decimal jine_zhatuche = 0;
            decimal jine_wajueji = 0;
            foreach (var item in zhatuche)
            {
                var checi = item.CheCi;
                var danjia = item.Price;
                jine_zhatuche += danjia * checi;
            }
            foreach (var item in wajueji)
            {
                var gongzuoliang = item.Gongzuoliang;

                var price = item.Price;

                jine_wajueji += gongzuoliang * price;
            }
            //项目的 渣土车和挖掘机总 金额
            decimal heji = jine_wajueji + jine_zhatuche;
            

            //还要往页面传递，已经自付记录的 总金额
            var yifu_jine = _context.ZhifuXins.Where(s => s.XiangMuMingCheng == xm & s.Chezhu==chezhu).Select(s => s.Zhifujine);
            decimal jine = 0;
            //接下来要做一个遍历集合的自加
            foreach (var item in yifu_jine)
            {
                jine += item;
            }
            ViewBag.jine = jine;  //项目已经支付金额总额合计
            ViewBag.heji = heji;
            ViewBag.xm = xm;
            ViewBag.chezhu = chezhu;
            return View();
        }


    }
}
