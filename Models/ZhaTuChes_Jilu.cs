using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace GongDiJiXie.Models
{
    public class ZhaTuChes_Jilu
    {
        [Key]
        public int Id { get; set; }

        //车牌
        public string ChePai { get; set; }

        //车主
        public string CheZhu { get; set; }

        //工作日期
        [DisplayFormat(DataFormatString = "{0:yyyy年MM月dd日}")]
        public DateTime Time_work { get; set; }

        //工作单价
        public decimal Price { get; set; } 

        //每次记录的工作量
        public int CheCi { get; set; }

        //工作地点  或者  工作内容
        public string Gongzuodidian { get; set; }

        public string GongzuodidianB { get; set; }

        //这里关联工地项目中的 项目名
        public string XiangMuMingCheng { get; set; }

        //渣土车工作运转距离
        public string GongZuoJuLi { get; set; }

        //工作类别  分为：内转、外运、外购  三种类型
        public string GongZuoLeiBie { get; set; }

        internal object Count()
        {
            throw new NotImplementedException();
        }

        internal object ToPagedList(int page, int rows)
        {
            throw new NotImplementedException();
        }
    }
}
