using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using GongDiJiXie.Data;
using GongDiJiXie.Models;
using System.Transactions;
using PagedList;

namespace GongDiJiXie.Controllers
{
    public class ZhaTuChes_JiluController : Controller
    {
        private readonly GongDiContext _context;

        public ZhaTuChes_JiluController(GongDiContext context)
        {
            _context = context;
        }

        // GET: ZhaTuChes_Jilu
        public IActionResult Index(string xm)
        {
            //渣土车工作记录录入时获取渣土车车牌信息
            var GenreLst = new List<string>();
            var GenreQry = from c in _context.ZhaTuChes
                           where c.XiangMuMingCheng == xm         //筛选条件是 这个项目里 的渣土车
                           orderby c.Id
                           select c.ChePai;                                                   
            GenreLst.AddRange(GenreQry.Distinct());
            ViewBag.chepai = GenreLst;

            var chezhuLst = new List<string>();
            var chezhuQry = from c in _context.ZhaTuChes
                           where c.XiangMuMingCheng == xm         //筛选条件是 这个项目里 的渣土车
                           orderby c.Id
                           select c.CheZhu;
            chezhuLst.AddRange(chezhuQry.Distinct());
            ViewBag.chezhu = chezhuLst;

            //渣土车工作记录录入时获取工地地块信息
            var GongdiLst = new List<string>();
            var GongdiQry = from c in _context.GongDiXinxis
                            where c.XiangMuMingCheng ==xm
                           orderby c.Id
                           select c.GongDiLeiXin;
            GongdiLst.AddRange(GongdiQry.Distinct());

            //页面初始加载时用于在datagrid底部显示提示信息
            var zhatuche = _context.ZhaTuChes_Jilus.Where(s => s.XiangMuMingCheng == xm);
            decimal jine_zhatuche = 0;//每条记录 车次*单价的 合计金额
            int chechi_sum = 0;  //所有记录 车次 的合计
                                 
            foreach (var item in zhatuche)
            {
                var checi = item.CheCi;
                var danjia = item.Price;
                jine_zhatuche += danjia * checi;
                chechi_sum += item.CheCi;
            }
            var msg = "项目" + xm + "挖掘机合计车次" + chechi_sum + "(次)。费用（车次*单价）合计：" + jine_zhatuche + "(元)。";

            ViewBag.msg = msg;
            ViewBag.gongdi = GongdiLst;
            ViewBag.xm = xm;   //把从sidemenu item.url 取来的 项目名  传值

            return View();
        }        

        /// <summary>
        ///   获取所有渣土车的信息
        /// </summary>        
        [HttpPost]
#pragma warning disable CS8632 // 只能在 "#nullable" 注释上下文内的代码中使用可为 null 的引用类型的注释。
        public JsonResult Get_zhatuchesjilu(string xm, DateTime? kaishi, DateTime? jieshu, string? chezhu_filt, string? chepai_filt)
#pragma warning restore CS8632 // 只能在 "#nullable" 注释上下文内的代码中使用可为 null 的引用类型的注释。        
        {
            int page = (Request.Form["page"] != "") ? int.Parse(Request.Form["page"]) : 1;
            int rows = (Request.Form["rows"] != "") ? int.Parse(Request.Form["rows"]) : 10;
            //在linq的where判断中用三元表达式，判断参数值是否需要过滤
            var zhatuchejilu = from c in _context.ZhaTuChes_Jilus
                                    where c.XiangMuMingCheng == xm && (kaishi != null ? c.Time_work > kaishi : c.Time_work < DateTime.Now) && (jieshu != null ? c.Time_work < jieshu : c.Time_work < DateTime.Now) && (chezhu_filt != null ? c.CheZhu == chezhu_filt : c.CheZhu != null) && (chepai_filt != null ? c.ChePai == chepai_filt : c.ChePai != null)
                                    orderby c.Id
                                    select new
                                    {
                                        id = c.Id,
                                        chepai = c.ChePai,
                                        time_work = c.Time_work,
                                        price = c.Price,
                                        checi = c.CheCi,
                                        gongzuodidian = c.Gongzuodidian,
                                        gongzuodidianb = c.GongzuodidianB,
                                        xm = c.XiangMuMingCheng,
                                        gongzuoleibie = c.GongZuoLeiBie,
                                        gongzuojuli = c.GongZuoJuLi,
                                        chezhu = c.CheZhu
                                    };
            try
            {
                var easyUIPages = new Dictionary<string, object>
                    {
                        { "total", zhatuchejilu.Count() },
                        { "rows", zhatuchejilu.ToPagedList(page, rows) }
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
        public ContentResult ZhaTuChe_tishi(string xm, DateTime? kaishi, DateTime? jieshu, string? chezhu_filt, string? chepai_filt)
#pragma warning restore CS8632 // 只能在 "#nullable" 注释上下文内的代码中使用可为 null 的引用类型的注释。        
        {            
            //在linq的where判断中用三元表达式，判断参数值是否需要过滤           
            var zhatuche = _context.ZhaTuChes_Jilus.Where(c => c.XiangMuMingCheng == xm && (kaishi != null ? c.Time_work > kaishi : c.Time_work < DateTime.Now) && (jieshu != null ? c.Time_work < jieshu : c.Time_work < DateTime.Now) && (chezhu_filt != null ? c.CheZhu == chezhu_filt : c.CheZhu != null) && (chepai_filt != null ? c.ChePai == chepai_filt : c.ChePai != null));
            decimal jine_zhatuche = 0;//每条记录 车次*单价的 合计金额
            int chechi_sum = 0;  //所有记录 车次 的合计

            foreach (var item in zhatuche)
            {
                var checi = item.CheCi;
                var danjia = item.Price;
                jine_zhatuche += danjia * checi;
                chechi_sum += item.CheCi;
            }
            var msg = "项目" + xm + "渣土车录入合计车次" + chechi_sum + "(次)。费用（车次*单价）合计：" + jine_zhatuche + "(元)。";

            return Content(msg);
        }

        //搜索功能中，车主 和 车牌 下拉框的二联动，当选取车主时，车牌下拉框的 联动数据
#pragma warning disable CS8632 // 只能在 "#nullable" 注释上下文内的代码中使用可为 null 的引用类型的注释。
        public JsonResult Chepai_filt(string xm, string? chezhu)
#pragma warning restore CS8632 // 只能在 "#nullable" 注释上下文内的代码中使用可为 null 的引用类型的注释。
        {
            if (chezhu == "" || chezhu == null)
            {
                var zhatuche = from c in _context.ZhaTuChes
                               where c.XiangMuMingCheng == xm
                               orderby c.Id
                               select new
                               {
                                   id = c.ChePai,
                                   chepai = c.ChePai
                               };
                return Json(zhatuche);
            }
            else
            {
                var zhatuche = from c in _context.ZhaTuChes
                               where c.XiangMuMingCheng == xm && c.CheZhu == chezhu
                               orderby c.Id
                               select new
                               {
                                   id = c.ChePai,
                                   chepai = c.ChePai
                               };
                return Json(zhatuche);
            }            
        }

        //这里是添加记录时，车主，车牌输入框的联动
        public JsonResult Chepai(string xm)
        {
            var zhatuche = from c in _context.ZhaTuChes
                               where c.XiangMuMingCheng == xm
                               orderby c.Id
                               select new
                               {
                                   id = c.ChePai,
                                   chepai = c.ChePai
                               };
            return Json(zhatuche);
        }

        //这里是 车牌输入框和车主输入框的 联动，车主输入框的返回结果
        public JsonResult Chezhu(string xm,string chepai) 
        {
            var zhatuche = from c in _context.ZhaTuChes
                           where c.XiangMuMingCheng == xm && c.ChePai == chepai
                           orderby c.Id
                           select new
                           { 
                               id = c.CheZhu,
                               chezhu = c.CheZhu
                           };
            return Json(zhatuche);
        }

        //这里是 工作类别、工作地点A、工作地点B、工作距离   四个combobx框的联动，内转时 工作地点A的返回结果
        public JsonResult Gongzuodidian(string xm) 
        {
            var zhatuche = from c in _context.GongDiXiangMus
                           where c.GongDiMingCheng == xm 
                           orderby c.Id            //同一个场地 内转  时返回距离最小的结果
                           select new
                           {
                               id = c.Weizhi, 
                               gongzuodidian = c.Weizhi
                           };
            return Json(zhatuche);
        }

        //外运时 地点B  的下拉选项，距离降序排列
        public JsonResult Gongzuodidian_b(string xm) 
        {
            var zhatuche = from c in _context.GongDiXinxis
                           where c.XiangMuMingCheng == xm && c.Juli >= 2
                           orderby c.Juli descending      //同一个场地 内转  时返回距离最小的结果
                           select new
                           {
                               id = c.GongDiLeiXin,
                               gongzuodidian = c.GongDiLeiXin
                           };
            return Json(zhatuche);
        }

        //这里是 工作类别、工作地点A、工作地点B、工作距离   四个combobx框的联动，内转时 距离的返回结果
        //[HttpPost]
        public JsonResult Gongzuojuli(string xm,string b) 
        {
            var zhatuche = from c in _context.GongDiXinxis
                           where c.XiangMuMingCheng == xm && c.GongDiLeiXin == b
                           orderby c.Id
                           select c.Juli;
            return Json(zhatuche);
        }

        /// <summary>
        /// 保存新增的渣土车信息
        /// </summary>
        [HttpPost] 
        public JsonResult Save_zhatuchejilu([Bind("ChePai,CheZhu,Time_work,Price,CheCi,GongZuoLeiBie,Gongzuodidian,GongzuodidianB,GongZuoJuLi")] ZhaTuChes_Jilu zhatuche,string xm)
        {
            if (ModelState.IsValid)
            {
                using (TransactionScope transaction = new())//原子操作，事物错误回滚
                {
                    try
                    {
                        zhatuche.XiangMuMingCheng = xm;
                        _context.ZhaTuChes_Jilus.Add(zhatuche);
                        _context.SaveChanges();

                        transaction.Complete();
                    }
                    catch (Exception ex)
                    {
                        return Json(new { success = false, msg = ex.ToString() }, "text/html");
                    }
                    finally
                    {
                        transaction.Dispose();
                    }
                }
                return Json(new { success = true, msg = "添加渣土车记录成功" });/*, "text/html"*/
            }
            return Json(new { success = false, msg = "添加记录失败，请再试一试！" });
        }


        /// <summary>
        /// 编辑渣土车信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult Update_zhatuche(int id)  //string bmname 好像多余
        {
            var zhatuche = _context.ZhaTuChes_Jilus.Where(c => c.Id == id).FirstOrDefault();

            zhatuche.ChePai = Request.Form["chepai"];

            zhatuche.Time_work = Convert.ToDateTime(Request.Form["time_work"]);            

            zhatuche.Price = Convert.ToInt32( Request.Form["price"]);

            zhatuche.Gongzuodidian = Request.Form["gongzuodidian"];

            zhatuche.GongzuodidianB = Request.Form["gongzuodidianb"];

            zhatuche.CheCi = Convert.ToInt32(Request.Form["CheCi"]);

            zhatuche.GongZuoLeiBie = Request.Form["gongzuoleibie"];

            zhatuche.GongZuoJuLi = Request.Form["gongzuojuli"];

            using (TransactionScope transaction = new())//原子操作，事物错误回滚
            {
                try
                {
                    _context.Entry(zhatuche).State = EntityState.Modified;
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
            return Json(new { success = true, msg = "更改渣土车信息成功！" });
        }

        /// <summary>
        /// 删除渣土车信息
        /// </summary>
        /// <param name="id"></param>
        /// <param name="xmid"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult Del_zhatuche(int id)
        {
            ZhaTuChes_Jilu zhatuche = _context.ZhaTuChes_Jilus.Find(id);

            using (TransactionScope transaction = new())//原子操作，事物错误回滚
            {
                try
                {
                    _context.ZhaTuChes_Jilus.Remove(zhatuche);
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
























        // GET: ZhaTuChes_Jilu/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var zhaTuChes_Jilu = await _context.ZhaTuChes_Jilus
                .FirstOrDefaultAsync(m => m.Id == id);
            if (zhaTuChes_Jilu == null)
            {
                return NotFound();
            }

            return View(zhaTuChes_Jilu);
        }

        // GET: ZhaTuChes_Jilu/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: ZhaTuChes_Jilu/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,ChePai,Time_work,Price,Gongzuoliang,Gongzuodidian")] ZhaTuChes_Jilu zhaTuChes_Jilu)
        {
            if (ModelState.IsValid)
            {
                _context.Add(zhaTuChes_Jilu);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(zhaTuChes_Jilu);
        }

        // GET: ZhaTuChes_Jilu/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var zhaTuChes_Jilu = await _context.ZhaTuChes_Jilus.FindAsync(id);
            if (zhaTuChes_Jilu == null)
            {
                return NotFound();
            }
            return View(zhaTuChes_Jilu);
        }

        // POST: ZhaTuChes_Jilu/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,ChePai,Time_work,Price,Gongzuoliang,Gongzuodidian")] ZhaTuChes_Jilu zhaTuChes_Jilu)
        {
            if (id != zhaTuChes_Jilu.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(zhaTuChes_Jilu);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ZhaTuChes_JiluExists(zhaTuChes_Jilu.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(zhaTuChes_Jilu);
        }

        // GET: ZhaTuChes_Jilu/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var zhaTuChes_Jilu = await _context.ZhaTuChes_Jilus
                .FirstOrDefaultAsync(m => m.Id == id);
            if (zhaTuChes_Jilu == null)
            {
                return NotFound();
            }

            return View(zhaTuChes_Jilu);
        }

        // POST: ZhaTuChes_Jilu/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var zhaTuChes_Jilu = await _context.ZhaTuChes_Jilus.FindAsync(id);
            _context.ZhaTuChes_Jilus.Remove(zhaTuChes_Jilu);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ZhaTuChes_JiluExists(int id)
        {
            return _context.ZhaTuChes_Jilus.Any(e => e.Id == id);
        }
    }
}
