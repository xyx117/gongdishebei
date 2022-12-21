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
using Microsoft.AspNetCore.Authorization;

namespace GongDiJiXie.Controllers
{
    //[Authorize]
    public class FeiYongJiLusController : Controller
    {
        private readonly GongDiContext _context;

        public FeiYongJiLusController(GongDiContext context)
        {
            _context = context;
        }

        // GET: FeiYongJiLus        
        public IActionResult Index(string xm)
        {          
            //获取费用分类
            var FeiyongfenleiLst = new List<string>();
            var FeiyongfenleiQry = from c in _context.FeiYongXinXis
                           orderby c.Id
                           select c.FeiYongLeiXing;
            FeiyongfenleiLst.AddRange(FeiyongfenleiQry.Distinct());
            ViewBag.feiyongfenlei = FeiyongfenleiLst;

            //给视图传递车主集合
            var chezhu_list = new List<string>();
            var chezhu_qry = from c in _context.FeiYongJiLus
                             where c.XiangMuMingCheng == xm
                             orderby c.Id
                             select c.Chezhu;
            chezhu_list.AddRange(chezhu_qry.Distinct());

            //给视图传递 牌号集合（机牌和车牌）
            var paihao_list = new List<string>();
            var paihao_qry = from c in _context.FeiYongJiLus
                             where c.XiangMuMingCheng == xm
                             orderby c.Id
                             select c.Feiyongduixiang;
            paihao_list.AddRange(paihao_qry.Distinct());

            //初始化加载时，datagrid底部的合计显示
            //在linq的where判断中用三元表达式，判断参数值是否需要过滤           
            var zhatuche = _context.FeiYongJiLus.Where(c => c.XiangMuMingCheng == xm );
            decimal total = 0;//每条记录 车次*单价的 合计金额           
            foreach (var item in zhatuche)
            {
                total += item.Total;
            }
            var msg = "项目" + xm + "挖掘机录入合计车次" + total + "(元)。";

            //传递项目名参数
            ViewBag.xm = xm;
            ViewBag.chezhu = chezhu_list;
            ViewBag.paihao = paihao_list;
            ViewBag.msg = msg;

            return View();
        }

        /// <summary>
        ///   获取所有渣土车的信息
        ///   xm  : 项目名
        /// </summary>        
        [HttpPost]
#pragma warning disable CS8632 // 只能在 "#nullable" 注释上下文内的代码中使用可为 null 的引用类型的注释。
        public JsonResult Get_feiyongjilu(string xm, DateTime? kaishi, DateTime? jieshu, string? chezhu_filt, string? chepai_filt)/*string searchquery*/
#pragma warning restore CS8632 // 只能在 "#nullable" 注释上下文内的代码中使用可为 null 的引用类型的注释。
        {
            int page = (Request.Form["page"] != "") ? int.Parse(Request.Form["page"]) : 1;
            int rows = (Request.Form["rows"] != "") ? int.Parse(Request.Form["rows"]) : 10;

            var feiyong = from c in _context.FeiYongJiLus
                          where c.XiangMuMingCheng ==xm && (kaishi != null ? c.Time > kaishi : c.Time < DateTime.Now) && (jieshu != null ? c.Time < jieshu : c.Time < DateTime.Now) && (chezhu_filt != null ? c.Chezhu == chezhu_filt : c.Chezhu != null) && (chepai_filt != null ? c.Feiyongduixiang == chepai_filt : c.Feiyongduixiang != null)
                          orderby c.Id
                         select new
                         {
                             id = c.Id,
                             feiyongduixiang = c.Feiyongduixiang,
                             leixing = c.Leixing,
                             chezhu = c.Chezhu,
                             time = c.Time,
                             total = c.Total,
                             feiyongleixing = c.Feiyongleixing                         
                         };           
            try
            {
                var easyUIPages = new Dictionary<string, object>
                {
                    { "total", feiyong.Count() },
                    { "rows", feiyong.ToPagedList(page, rows) }
                };
                return Json(easyUIPages);
            }
            catch (Exception)
            {
                throw;
            }
        }

        //搜索功能中，车主 和 车牌 下拉框的二联动，当选取车主时，车牌下拉框的 联动数据
#pragma warning disable CS8632 // 只能在 "#nullable" 注释上下文内的代码中使用可为 null 的引用类型的注释。
        public JsonResult Jipai_filt(string xm, string? chezhu)
#pragma warning restore CS8632 // 只能在 "#nullable" 注释上下文内的代码中使用可为 null 的引用类型的注释。
        {
            if (chezhu == "" || chezhu == null)
            {
                var zhatuche = from c in _context.FeiYongJiLus
                               where c.XiangMuMingCheng == xm
                               orderby c.Id
                               select new
                               {
                                   id = c.Feiyongduixiang,
                                   jipai = c.Feiyongduixiang
                               };
                return Json(zhatuche);
            }
            else
            {
                var zhatuche = from c in _context.FeiYongJiLus
                               where c.XiangMuMingCheng == xm && c.Chezhu == chezhu
                               orderby c.Id
                               select new
                               {
                                   id = c.Feiyongduixiang,
                                   jipai = c.Feiyongduixiang
                               };
                return Json(zhatuche);
            }
        }

        //前端页面点击搜索按钮后触发ajax事件，返回搜索结果的 车次 合计、金额合计 
        [HttpPost]
#pragma warning disable CS8632 // 只能在 "#nullable" 注释上下文内的代码中使用可为 null 的引用类型的注释。
        public ContentResult Feiyong_tishi(string xm, DateTime? kaishi, DateTime? jieshu, string? chezhu_filt, string? chepai_filt)
#pragma warning restore CS8632 // 只能在 "#nullable" 注释上下文内的代码中使用可为 null 的引用类型的注释。        
        {
            //在linq的where判断中用三元表达式，判断参数值是否需要过滤           
            var zhatuche = _context.FeiYongJiLus.Where(c => c.XiangMuMingCheng == xm && (kaishi != null ? c.Time > kaishi : c.Time < DateTime.Now) && (jieshu != null ? c.Time < jieshu : c.Time < DateTime.Now) && (chezhu_filt != null ? c.Chezhu == chezhu_filt : c.Chezhu != null) && (chepai_filt != null ? c.Feiyongduixiang == chepai_filt : c.Feiyongduixiang != null));
            decimal total = 0;//每条记录 车次*单价的 合计金额            

            foreach (var item in zhatuche)
            {                
                total += item.Total;             
            }
            var msg = "项目" + xm + "挖掘机录入合计车次" + total + "(元)。";

            return Content(msg);
        }

        //添加信息时，类型，机器牌号，车主 ，三个下拉框的联动，这里是获取  机器牌号的值
        public JsonResult Feiyongduixiang(string xm, string leixing)
        {            
            if(leixing == null)
            {                
                return Json(null);
            }
            else
            {
                if(leixing == "渣土车")
                {
                    var zhatuche = from c in _context.ZhaTuChes
                                   where c.XiangMuMingCheng == xm
                                   orderby c.Id
                                   select new
                                   {
                                       id = c.ChePai,
                                       feiyongduixiang = c.ChePai
                                   };
                    return Json(zhatuche);
                }
                else
                {
                    var wajueji = from c in _context.WaJueJis
                                   where c.XiangMuMingCheng == xm
                                   orderby c.Id
                                   select new
                                   {
                                       id = c.JiPai,
                                       feiyongduixiang = c.JiPai
                                   };
                    return Json(wajueji);
                }                
            }            
        }

        //添加信息时，类型，机器牌号，车主 ，三个下拉框的联动,这里是获取车主的 值
        public JsonResult Chezhu(string xm,string paihao, string leixing)
        {
            if (leixing == null)
            {
                return Json(null);
            }
            else
            {
                if (leixing == "渣土车")
                {
                    var zhatuche = from c in _context.ZhaTuChes
                                   where c.XiangMuMingCheng == xm && c.ChePai == paihao
                                   orderby c.Id
                                   select new
                                   {
                                       id = c.CheZhu,
                                       chezhu = c.CheZhu
                                   };
                    return Json(zhatuche);
                }
                else
                {
                    var wajueji = from c in _context.WaJueJis
                                   where c.XiangMuMingCheng == xm && c.JiPai == paihao
                                   orderby c.Id
                                   select new
                                   {
                                       id = c.CheZhu,
                                       chezhu = c.CheZhu
                                   };
                    return Json(wajueji);
                }
            }
        }

        /// <summary>
        /// 保存新增的挖掘机记录信息
        /// </summary>
        [HttpPost]
        public JsonResult Save_feiyongjilu([Bind("Leixing,Feiyongduixiang,Chezhu,Time,Total,Feiyongleixing")] FeiYongJiLu feiyongjilu,string xm)
        {
            if (ModelState.IsValid)
            {
                using (TransactionScope transaction = new())//原子操作，事物错误回滚
                {
                    try
                    {
                        feiyongjilu.XiangMuMingCheng = xm;
                        _context.FeiYongJiLus.Add(feiyongjilu);
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
                return Json(new { success = true, msg = "添加费用记录成功" });/*, "text/html"*/
            }
            return Json(new { success = false, msg = "添加费用记录失败，请再试一试！" });
        }


        /// <summary>
        /// 编辑挖掘机信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult Update_feiyongjilu(int id)
        {
            var feiyongjilu = _context.FeiYongJiLus.Where(c => c.Id == id).FirstOrDefault();
            feiyongjilu.Feiyongduixiang = Request.Form["feiyongduixiang"];
            feiyongjilu.Time = Convert.ToDateTime(Request.Form["time"]);
            feiyongjilu.Total = Convert.ToDecimal(Request.Form["total"]);
            feiyongjilu.Feiyongleixing = Request.Form["feiyongleixing"];
            feiyongjilu.Leixing = Request.Form["leixing"];
            feiyongjilu.Chezhu = Request.Form["chezhu"];
            using (TransactionScope transaction = new())//原子操作，事物错误回滚
            {
                try
                {
                    _context.Entry(feiyongjilu).State = EntityState.Modified;
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
            return Json(new { success = true, msg = "更改费用记录信息成功！" });
        }

        /// <summary>
        /// 删除费用记录信息
        /// </summary>
        /// <param name="id"></param>
        /// <param name="xmid"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult Del_feiyongjilu(int id)
        {
            FeiYongJiLu feiyongjilu = _context.FeiYongJiLus.Find(id);
            using (TransactionScope transaction = new())//原子操作，事物错误回滚
            {
                try
                {
                    _context.FeiYongJiLus.Remove(feiyongjilu);
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



        // GET: FeiYongJiLus/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var feiYongJiLu = await _context.FeiYongJiLus
                .FirstOrDefaultAsync(m => m.Id == id);
            if (feiYongJiLu == null)
            {
                return NotFound();
            }

            return View(feiYongJiLu);
        }

        // GET: FeiYongJiLus/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: FeiYongJiLus/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Feiyongduixiang,Time,Total,Feiyongleixing")] FeiYongJiLu feiYongJiLu)
        {
            if (ModelState.IsValid)
            {
                _context.Add(feiYongJiLu);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(feiYongJiLu);
        }

        // GET: FeiYongJiLus/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var feiYongJiLu = await _context.FeiYongJiLus.FindAsync(id);
            if (feiYongJiLu == null)
            {
                return NotFound();
            }
            return View(feiYongJiLu);
        }

        // POST: FeiYongJiLus/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Feiyongduixiang,Time,Total,Feiyongleixing")] FeiYongJiLu feiYongJiLu)
        {
            if (id != feiYongJiLu.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(feiYongJiLu);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!FeiYongJiLuExists(feiYongJiLu.Id))
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
            return View(feiYongJiLu);
        }

        // GET: FeiYongJiLus/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var feiYongJiLu = await _context.FeiYongJiLus
                .FirstOrDefaultAsync(m => m.Id == id);
            if (feiYongJiLu == null)
            {
                return NotFound();
            }

            return View(feiYongJiLu);
        }

        // POST: FeiYongJiLus/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var feiYongJiLu = await _context.FeiYongJiLus.FindAsync(id);
            _context.FeiYongJiLus.Remove(feiYongJiLu);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool FeiYongJiLuExists(int id)
        {
            return _context.FeiYongJiLus.Any(e => e.Id == id);
        }
    }
}
