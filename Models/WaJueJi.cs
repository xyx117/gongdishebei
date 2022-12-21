using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace GongDiJiXie.Models
{
    //挖掘机信息
    public class WaJueJi
    {
        [Key]
        public int Id { get; set; }

        //名称   就是机牌
        public string JiPai { get; set; }

        //型号
        public string XinHao { get; set; }

        //车主
        public string CheZhu { get; set; }

        //产权方式
        public string ChanQuan { get; set; }

        //吨位
        //public int DunWei { get; set; }

        //联系方式
        public string LianXiFangShi { get; set; }

        //这里关联工地项目中的 项目名
        public string XiangMuMingCheng { get; set; }
    }
}
