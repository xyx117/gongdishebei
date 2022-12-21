using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace GongDiJiXie.Models
{
    public class WaJueJis_JiLu
    {
        [Key]
        public int Id { get; set; }

        //机牌
        public string JiPai { get; set; }

        //车主，一个车主可能有多辆  挖掘机
        public string Chezhu { get; set; }

        //工作日期        
        public DateTime Time_work { get; set; }

        //工作单价
        public decimal Price { get; set; }

        //每次记录的工作量,就是工时
        public decimal Gongzuoliang { get; set; }

        //工作地点  或者  工作内容
        public string Gongzuodidian { get; set; }

        

        //这里关联工地项目中的 项目名
        public string XiangMuMingCheng { get; set; }
    }
}
