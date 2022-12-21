using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using GongDiJiXie.Data;
using GongDiJiXie.Models;
using PagedList;
using System.Transactions;

namespace GongDiJiXie.Controllers
{
    public class WaJueJis_JiLuController : Controller
    {
        private readonly GongDiContext _context;

        public WaJueJis_JiLuController(GongDiContext context)
        {
            _context = context;
        }

        
        public IActionResult Index(string xm)
        {
            //渣土车工作记录录入时获取渣土车车牌信息
            var GenreLst = new List<string>();
            var GenreQry = from c in _context.WaJueJis
                           where c.XiangMuMingCheng == xm
                           orderby c.Id
                           select c.JiPai;
            GenreLst.AddRange(GenreQry.Distinct());
            ViewBag.wajueji = GenreLst;

            //渣土车工作记录录入时获取渣土车车主信息
            var GenreLst_chezhu = new List<string>();
            var GenreQry_chezhu = from c in _context.WaJueJis
                                  where c.XiangMuMingCheng == xm
                                  orderby c.Id
                                  select c.CheZhu;
            GenreLst_chezhu.AddRange(GenreQry_chezhu.Distinct());
            ViewBag.wajueji_chezhu = GenreLst_chezhu;

            //渣土车工作记录录入时获取工地信息
            var GongdiLst = new List<string>();
            var GongdiQry = from c in _context.GongDiXinxis
                            where c.XiangMuMingCheng == xm
                            orderby c.Id
                            select c.GongDiLeiXin;
            GongdiLst.AddRange(GongdiQry.Distinct());

            //页面初始加载时用于在datagrid底部显示提示信息
            var zhatuche = _context.WaJueJis_JiLus.Where(c => c.XiangMuMingCheng == xm);
            decimal jine_zhatuche = 0;//每条记录 车次*单价的 合计金额
            decimal chechi_sum = 0;  //所有记录 车次 的合计

            foreach (var item in zhatuche)
            {
                var checi = item.Gongzuoliang;
                var danjia = item.Price;
                jine_zhatuche += danjia * checi;
                chechi_sum += item.Gongzuoliang;
            }
            var msg = "项目" + xm + "挖掘机录入合计车次" + chechi_sum + "(次)。费用（车次*单价）合计：" + jine_zhatuche + "(元)。";

            ViewBag.msg = msg;
            ViewBag.gongdi = GongdiLst;
            ViewBag.xm = xm;
            return View();
        }

        /// <summary>
        ///   获取所有渣土车的信息
        /// </summary>        
        [HttpPost]
#pragma warning disable CS8632 // 只能在 "#nullable" 注释上下文内的代码中使用可为 null 的引用类型的注释。
        public JsonResult Get_wajuejijilu(string xm, DateTime? kaishi, DateTime? jieshu, string? chezhu_filt, string? chepai_filt)/*string searchquery*/
#pragma warning restore CS8632 // 只能在 "#nullable" 注释上下文内的代码中使用可为 null 的引用类型的注释。
        {
            int page = (Request.Form["page"] != "") ? int.Parse(Request.Form["page"]) : 1;
            int rows = (Request.Form["rows"] != "") ? int.Parse(Request.Form["rows"]) : 10;

            var xm1 = from c in _context.WaJueJis_JiLus
                     where c.XiangMuMingCheng == xm && (kaishi != null ? c.Time_work > kaishi : c.Time_work < DateTime.Now) && (jieshu != null ? c.Time_work < jieshu : c.Time_work < DateTime.Now) && (chezhu_filt != null ? c.Chezhu == chezhu_filt : c.Chezhu != null) && (chepai_filt != null ? c.JiPai == chepai_filt : c.JiPai != null)
                      orderby c.Id
                     select new
                     {
                         id = c.Id,
                         jipai = c.JiPai,
                         chezhu = c.Chezhu,
                         time_work = c.Time_work,
                         price = c.Price,
                         gongzuoliang = c.Gongzuoliang,
                         gongzuodidian = c.Gongzuodidian                         
                     };
            try
            {
                var easyUIPages = new Dictionary<string, object>
                {
                    { "total", xm1.Count() },
                    { "rows", xm1.ToPagedList(page, rows) }
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
        public ContentResult Wajueji_tishi(string xm, DateTime? kaishi, DateTime? jieshu, string? chezhu_filt, string? chepai_filt)
#pragma warning restore CS8632 // 只能在 "#nullable" 注释上下文内的代码中使用可为 null 的引用类型的注释。        
        {
            //在linq的where判断中用三元表达式，判断参数值是否需要过滤           
            var zhatuche = _context.WaJueJis_JiLus.Where(c => c.XiangMuMingCheng == xm && (kaishi != null ? c.Time_work > kaishi : c.Time_work < DateTime.Now) && (jieshu != null ? c.Time_work < jieshu : c.Time_work < DateTime.Now) && (chezhu_filt != null ? c.Chezhu == chezhu_filt : c.Chezhu != null) && (chepai_filt != null ? c.JiPai == chepai_filt : c.JiPai != null));
            decimal jine_zhatuche = 0;//每条记录 车次*单价的 合计金额
            decimal chechi_sum = 0;  //所有记录 车次 的合计

            foreach (var item in zhatuche)
            {
                var checi = item.Gongzuoliang;
                var danjia = item.Price;
                jine_zhatuche += danjia * checi;
                chechi_sum += item.Gongzuoliang;
            }
            var msg = "项目" + xm + "挖掘机录入合计车次" + chechi_sum + "(次)。费用（车次*单价）合计：" + jine_zhatuche + "(元)。";

            return Content(msg);
        }


        //搜索功能中，车主 和 车牌 下拉框的二联动，当选取车主时，车牌下拉框的 联动数据
#pragma warning disable CS8632 // 只能在 "#nullable" 注释上下文内的代码中使用可为 null 的引用类型的注释。
        public JsonResult Jipai_filt(string xm, string? chezhu)
#pragma warning restore CS8632 // 只能在 "#nullable" 注释上下文内的代码中使用可为 null 的引用类型的注释。
        {
            if (chezhu == "" || chezhu == null)
            {
                var zhatuche = from c in _context.WaJueJis
                               where c.XiangMuMingCheng == xm
                               orderby c.Id
                               select new
                               {
                                   id = c.JiPai,
                                   jipai = c.JiPai
                               };
                return Json(zhatuche);
            }
            else
            {
                var zhatuche = from c in _context.WaJueJis
                               where c.XiangMuMingCheng == xm && c.CheZhu == chezhu
                               orderby c.Id
                               select new
                               {
                                   id = c.JiPai,
                                   jipai = c.JiPai
                               };
                return Json(zhatuche);
            }
        }

        //在 combobox联动时候，需要返回的数据
        public JsonResult Jipai(string xm) 
        {
            var zhatuche = from c in _context.WaJueJis
                           where c.XiangMuMingCheng == xm
                           orderby c.Id
                           select new
                           {
                               id = c.JiPai,
                               jipai = c.JiPai 
                           };
            return Json(zhatuche);
        }

        //在 combobox联动时候，需要返回的数据
        public JsonResult Chezhu(string xm, string jipai)
        {
            var zhatuche = from c in _context.WaJueJis
                           where c.XiangMuMingCheng == xm && c.JiPai == jipai
                           orderby c.Id
                           select new
                           {
                               id = c.CheZhu,
                               chezhu = c.CheZhu
                           };
            return Json(zhatuche);
        }

        /// <summary>
        /// 保存新增的挖掘机记录信息
        /// </summary>
        [HttpPost]
        public JsonResult Save_wajuejijilu([Bind("JiPai,Chezhu,Time_work,Price,Gongzuodidian,Gongzuoliang")] WaJueJis_JiLu wajueji,string xm)
        {
            if (ModelState.IsValid)
            {
                using (TransactionScope transaction = new())//原子操作，事物错误回滚
                {
                    try
                    {
                        wajueji.XiangMuMingCheng = xm;
                        _context.WaJueJis_JiLus.Add(wajueji);
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
                return Json(new { success = true, msg = "添加挖掘机记录成功" });/*, "text/html"*/
            }
            return Json(new { success = false, msg = "添加挖掘机记录失败，请再试一试！" });
        }


        /// <summary>
        /// 编辑挖掘机信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult Update_wajueji(int id)  
        {
            var wajueji = _context.WaJueJis_JiLus.Where(c => c.Id == id).FirstOrDefault();

            wajueji.JiPai = Request.Form["jipai"];

            wajueji.Time_work = Convert.ToDateTime(Request.Form["time_work"]);

            wajueji.Price = Convert.ToInt32(Request.Form["price"]);

            wajueji.Gongzuodidian = Request.Form["gongzuodidian"];            

            wajueji.Gongzuoliang = Convert.ToDecimal(Request.Form["gongzuoliang"]);

            using (TransactionScope transaction = new())//原子操作，事物错误回滚
            {
                try
                {
                    _context.Entry(wajueji).State = EntityState.Modified;
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
        /// 删除挖掘机信息
        /// </summary>
        /// <param name="id"></param>
        /// <param name="xmid"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult Del_wajueji(int id)
        {  
            WaJueJis_JiLu wajueji = _context.WaJueJis_JiLus.Find(id);

            using (TransactionScope transaction = new())//原子操作，事物错误回滚
            {
                try
                {
                    _context.WaJueJis_JiLus.Remove(wajueji);
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




        // GET: WaJueJis_JiLu/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var waJueJis_JiLu = await _context.WaJueJis_JiLus
                .FirstOrDefaultAsync(m => m.Id == id);
            if (waJueJis_JiLu == null)
            {
                return NotFound();
            }

            return View(waJueJis_JiLu);
        }

        // GET: WaJueJis_JiLu/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: WaJueJis_JiLu/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Mingcheng,Time_work,Price,Gongzuoliang,Gongzuodidian")] WaJueJis_JiLu waJueJis_JiLu)
        {
            if (ModelState.IsValid)
            {
                _context.Add(waJueJis_JiLu);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(waJueJis_JiLu);
        }

        // GET: WaJueJis_JiLu/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var waJueJis_JiLu = await _context.WaJueJis_JiLus.FindAsync(id);
            if (waJueJis_JiLu == null)
            {
                return NotFound();
            }
            return View(waJueJis_JiLu);
        }

        // POST: WaJueJis_JiLu/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Mingcheng,Time_work,Price,Gongzuoliang,Gongzuodidian")] WaJueJis_JiLu waJueJis_JiLu)
        {
            if (id != waJueJis_JiLu.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(waJueJis_JiLu);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!WaJueJis_JiLuExists(waJueJis_JiLu.Id))
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
            return View(waJueJis_JiLu);
        }

        // GET: WaJueJis_JiLu/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var waJueJis_JiLu = await _context.WaJueJis_JiLus
                .FirstOrDefaultAsync(m => m.Id == id);
            if (waJueJis_JiLu == null)
            {
                return NotFound();
            }

            return View(waJueJis_JiLu);
        }

        // POST: WaJueJis_JiLu/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var waJueJis_JiLu = await _context.WaJueJis_JiLus.FindAsync(id);
            _context.WaJueJis_JiLus.Remove(waJueJis_JiLu);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool WaJueJis_JiLuExists(int id)
        {
            return _context.WaJueJis_JiLus.Any(e => e.Id == id);
        }
    }
}
