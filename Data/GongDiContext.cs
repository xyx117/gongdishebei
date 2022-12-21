using GongDiJiXie.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace GongDiJiXie.Data 
{
    public class GongDiContext : DbContext
    {
        public GongDiContext(DbContextOptions<GongDiContext> options)
            : base(options)
        {
        }

        public DbSet<FeiYongXinXi> FeiYongXinXis { get; set; }
        public DbSet<GongDiXinxi> GongDiXinxis { get; set; }
        public DbSet<WaJueJi> WaJueJis { get; set; }         
        public DbSet<ZhaTuChe> ZhaTuChes { get; set; }
        public DbSet<ZhaTuChes_Jilu> ZhaTuChes_Jilus { get; set;} 
        public DbSet<WaJueJis_JiLu> WaJueJis_JiLus { get; set; } 
        public DbSet<FeiYongJiLu> FeiYongJiLus { get; set; }
        public DbSet<GongDiXiangMu> GongDiXiangMus { get; set; } 
        public DbSet<ZhifuXinXi> ZhifuXins { get; set; }


        //protected override void OnModelCreating(ModelBuilder modelBuilder)
        //{
        //    modelBuilder.Entity<FeiYongXinXi>().ToTable("FeiYongXinXi");
        //    modelBuilder.Entity<GongDiXinxi>().ToTable("GongDiXinxi");
        //    modelBuilder.Entity<WaJueJi>().ToTable("WaJueJi");
        //    modelBuilder.Entity<ZhaTuChe>().ToTable("ZhaTuChe");
        //}
    }
}