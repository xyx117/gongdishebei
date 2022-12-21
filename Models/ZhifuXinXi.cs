using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace GongDiJiXie.Models
{
    public class ZhifuXinXi
    {
        [Key]
        public int Id { get; set; }

        public DateTime Zhifushijian { get; set; }        

        //此次支付的金额
        public decimal Zhifujine { get; set; }

        public string Chezhu { get; set; }

        public string Zhifubeizhu { get; set; }

        //这里关联工地项目中的 项目名
        public string XiangMuMingCheng { get; set; }
    }
}
