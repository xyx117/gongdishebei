using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace GongDiJiXie.Models
{
    public class GongDiXiangMu
    {
        [Key]
        public int Id { get; set; }

        public string GongDiMingCheng { get; set; }

        public DateTime KaiShiShiJian { get; set; }

        public string BeiZhu { get; set; }

        //判断项目是否结束用于项目排序显示
        public bool End_Or { get; set; }

        //项目位置   ，项目的起点
        public string Weizhi { get; set; }


    }
}
