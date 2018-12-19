using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Salary.Models
{
    public class SalarySet
    {
        public SalarySet()
        {
            NoClock = 20;
            Late = 20;
            OffWork = 50;
            Absenteeism = 100;
            Derate = 1;
            SocialSecurity = 500;
            FoodFee = 400;
            Endowment = 0;
            Health = 0;
            Childbirth = 0;
            Unemployment = 0;
            IncomeTaxBase = 0;
            HousingFund = 0;
        }
        /// <summary>
        /// 未打卡扣款（元/次）
        /// </summary>
        [Display(Name = "未打卡扣款（元/次）")]
        public decimal NoClock { get; set; }
        /// <summary>
        /// 迟到扣款（元/次）
        /// </summary>
        [Display(Name = "迟到扣款（元/次）")]
        public decimal Late { get; set; }
        /// <summary>
        /// 请假扣款（元/半天）
        /// </summary>
        [Display(Name = "请假扣款（元/半天）")]
        public decimal OffWork { get; set; }
        /// <summary>
        /// 旷工扣款（元/半天）
        /// </summary>
        [Display(Name = "旷工扣款（元/半天）")]
        public decimal Absenteeism { get; set; }
        /// <summary>
        /// 未打卡/迟到减免次数
        /// </summary>
        [Display(Name = "未打卡/迟到减免次数")]
        public int Derate { get; set; }
        /// <summary>
        /// 社保补贴
        /// </summary>
        [Display(Name = "社保补贴")]
        public decimal SocialSecurity { get; set; }
        /// <summary>
        /// 伙食费
        /// </summary>
        [Display(Name = "伙食费")]
        public decimal FoodFee { get; set; }
        /// <summary>
        /// 养老保险
        /// </summary>
        [Display(Name = "养老保险")]
        public decimal Endowment { get; set; }
        /// <summary>
        /// 医疗保险
        /// </summary>
        [Display(Name = "医疗保险")]
        public decimal Health { get; set; }
        /// <summary>
        /// 工伤保险
        /// </summary>
        [Display(Name = "工伤保险")]
        public decimal Injury { get; set; }
        /// <summary>
        /// 生育保险
        /// </summary>
        [Display(Name = "生育保险")]
        public decimal Childbirth { get; set; }
        /// <summary>
        /// 失业保险
        /// </summary>
        [Display(Name = "失业保险")]
        public decimal Unemployment { get; set; }
        /// <summary>
        /// 住房公积金
        /// </summary>
        [Display(Name = "住房公积金")]
        public decimal HousingFund { get; set; }
        /// <summary>
        /// 个人所得税基数
        /// </summary>
        [Display(Name = "个人所得税基数")]
        public decimal IncomeTaxBase { get; set; }
        /// <summary>
        /// 个人所得税基数
        /// </summary>
        [Display(Name = "扣税系数")]
        public string TaxRange { get; set; }

    }
}