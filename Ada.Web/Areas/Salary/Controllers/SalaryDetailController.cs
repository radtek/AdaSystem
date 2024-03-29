﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ada.Core;
using Ada.Core.Domain.Admin;
using Ada.Core.Domain.Demand;
using Ada.Core.Domain.Wages;
using Ada.Core.ViewModel.Business;
using Ada.Core.ViewModel.Setting;
using Ada.Core.ViewModel.Wages;
using Ada.Framework.Filter;
using Ada.Services.Admin;
using Ada.Services.Business;
using Ada.Services.Salary;
using Ada.Services.Setting;
using Newtonsoft.Json.Linq;
using Salary.Models;

namespace Salary.Controllers
{
    /// <summary>
    /// 考勤奖金
    /// </summary>
    public class SalaryDetailController : BaseController
    {
        private readonly ISalaryDetailService _service;
        private readonly IManagerService _managerService;
        private readonly IRepository<Manager> _managerRepository;
        private readonly IRepository<Quarters> _quartersRepository;
        private readonly IRepository<AttendanceDetail> _attendanceRepository;
        private readonly IRepository<SalaryDetail> _salaryRepository;
        private readonly IRepository<SubjectDetail> _subjectRepository;
        private readonly IBusinessWriteOffService _businessWriteOffService;
        private readonly ISettingService _settingService;
        public SalaryDetailController(ISalaryDetailService service,
            IManagerService managerService,
            IRepository<Manager> managerRepository,
            IBusinessWriteOffService businessWriteOffService,
            IRepository<Quarters> quartersRepository,
            IRepository<SalaryDetail> salaryRepository,
            ISettingService settingService,
            IRepository<AttendanceDetail> attendanceRepository,
            IRepository<SubjectDetail> subjectRepository)
        {
            _service = service;
            _managerService = managerService;
            _managerRepository = managerRepository;
            _businessWriteOffService = businessWriteOffService;
            _quartersRepository = quartersRepository;
            _salaryRepository = salaryRepository;
            _settingService = settingService;
            _attendanceRepository = attendanceRepository;
            _subjectRepository = subjectRepository;
        }
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult GetList(AttendanceDetailView viewModel)
        {
            var p = PremissionData();
            viewModel.ManagerId = p.Any() ? CurrentManager.Id : null;
            var result = _service.LoadEntitiesFilter(viewModel).ToList();
            return Json(new
            {
                viewModel.total,
                rows = result.Select(d => new
                {
                    d.Id,
                    ManagerName = d.Manager.UserName,
                    d.Date,
                    d.Total,
                    Sum = viewModel.TotalSum
                })
            }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 新增工资
        /// </summary>
        /// <returns></returns>
        public ActionResult Add()
        {
            AttendanceDetailView viewModel = new AttendanceDetailView();
            return View(viewModel);
        }
        [HttpPost]
        
        public ActionResult Add(AttendanceDetailView viewModel)
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError("message", "数据校验失败，请核对输入的信息是否准确");
                return View(viewModel);
            }

            var manager = _managerRepository.LoadEntities(d => d.Id == viewModel.ManagerId).FirstOrDefault();
            //考勤 奖金
            AttendanceDetail entity = manager.AttendanceDetails.FirstOrDefault(d => d.Date == viewModel.Date);
            bool isAddAttendance = false;
            if (entity == null)
            {
                entity = new AttendanceDetail();
                entity.Id = IdBuilder.CreateIdNum();
                isAddAttendance = true;
            }
            entity.Date = viewModel.Date;
            entity.OffWork = viewModel.OffWork;
            entity.NoClockTimes = viewModel.NoClockTimes;
            entity.LateTimes = viewModel.LateTimes;
            entity.Absenteeism = viewModel.Absenteeism;
            //entity.Overtime = viewModel.Overtime;
            var quarters = _quartersRepository.LoadEntities(d => d.Id == manager.QuartersId).FirstOrDefault();
            var gw = quarters.BaseSalary;//岗位工资
            var kq = quarters.Attendance;//全勤
            var jt = quarters.Allowance;//岗位津贴
            var zwjt = quarters.Post;//职务津贴
            var px = quarters.Training;//培训费
            var config = _settingService.GetSetting<SalarySet>();
            var sb = config.SocialSecurity;//社保补贴
            var hs = config.FoodFee;//伙食费
            //未打卡
            var jmTimes = config.Derate;
            bool isJm = true;
            var dk = config.NoClock;
            int dkTemp;
            if (viewModel.NoClockTimes > 0)
            {
                dkTemp = viewModel.NoClockTimes - jmTimes;
                isJm = false;
            }
            else
            {
                dkTemp = viewModel.NoClockTimes;
            }
            var noClock = dkTemp * dk;
            //迟到
            var cd = config.Late;
            int cdTemp;
            if (viewModel.LateTimes > 0)
            {
                if (isJm)
                {
                    cdTemp = viewModel.LateTimes - jmTimes;
                }
                else
                {
                    cdTemp = viewModel.LateTimes;
                }
            }
            else
            {
                cdTemp = viewModel.LateTimes;
            }
            var late = cdTemp * cd;
            bool isBad = false;
            //请假
            var qj = (double)config.OffWork;
            double offwork = 0;
            if (viewModel.OffWork <= 5)
            {
                //offwork = viewModel.OffWork <= 2 ? viewModel.OffWork / 0.5 * qj : 2 * qj * Math.Pow(2, viewModel.OffWork - 1);
                if (viewModel.OffWork <= 2)
                {
                    offwork = viewModel.OffWork / 0.5 * qj;
                }
                else
                {
                    if (viewModel.OffWork.ToString().Contains(".5"))
                    {
                        offwork = 2 * qj * Math.Pow(2, viewModel.OffWork - 0.5 - 1) + qj;
                    }
                    else
                    {
                        offwork = 2 * qj * Math.Pow(2, viewModel.OffWork - 1);
                    }
                }
            }
            else
            {
                isBad = true;
            }
            //旷工
            var kuanggong = (double)config.Absenteeism;
            double absenteeism = 0;
            if (viewModel.Absenteeism <= 5)
            {
                //absenteeism = viewModel.Absenteeism <= 2 ? viewModel.Absenteeism / 0.5 * kuanggong : 2 * kuanggong * Math.Pow(2, viewModel.Absenteeism - 1);
                if (viewModel.Absenteeism <= 2)
                {
                    absenteeism = viewModel.Absenteeism / 0.5 * kuanggong;
                }
                else
                {

                    if (viewModel.Absenteeism.ToString().Contains(".5"))
                    {
                        absenteeism = 2 * kuanggong * Math.Pow(2, viewModel.Absenteeism - 0.5 - 1) + kuanggong;
                    }
                    else
                    {
                        absenteeism = 2 * kuanggong * Math.Pow(2, viewModel.Absenteeism - 1);
                    }

                }
            }
            else
            {
                isBad = true;
            }
            //全勤
            bool isQc = !(viewModel.OffWork > 0 || viewModel.Absenteeism > 0 || viewModel.NoClockTimes >= 5 || viewModel.LateTimes >= 5);
            //算工资
            SalaryDetail salaryDetail = manager.SalaryDetails.FirstOrDefault(d => d.Date == viewModel.Date);
            bool isAddSalary = false;
            if (salaryDetail == null)
            {
                salaryDetail = new SalaryDetail();
                salaryDetail.Id = IdBuilder.CreateIdNum();
                isAddSalary = true;
            }
            //考勤扣款  旷工+请假+未打卡+迟到
            decimal deductTotal = (decimal)(absenteeism + offwork) + noClock + late; //(decimal)(absenteeism + noClock + late + offwork);
            if (!isQc)
            {
                deductTotal += kq;
            }
            if (isBad)
            {
                deductTotal += gw;
            }
            //奖金 岗位工资+全勤奖+岗位津贴+职务津贴+其他奖金+红包提成+销售提成+社保补贴
            //销售提成
            var quare = new BusinessWriteOffDetailView();
            quare.WriteOffDateStar = new DateTime(viewModel.Date.Value.Year, viewModel.Date.Value.Month, 1);
            quare.WriteOffDateEnd = new DateTime(viewModel.Date.Value.Year, viewModel.Date.Value.Month, DateTime.DaysInMonth(viewModel.Date.Value.Year, viewModel.Date.Value.Month));
            quare.TransactorId = viewModel.ManagerId;
            var saleCommissionList = _businessWriteOffService.LoadEntitiesFilters(quare);
            var saleCommission = saleCommissionList.Any() ? saleCommissionList.Sum(d => d.Commission) : 0;
            saleCommission = saleCommission * quarters.Commission;//乘以系数
            //制作提成
            var endDay = quare.WriteOffDateEnd.Value.AddDays(1);
            var subjects = _subjectRepository.LoadEntities(d =>
                d.IsDelete == false && d.CompletDate >= quare.WriteOffDateStar && d.CompletDate < endDay&&d.Subject.IsDelete==false&&d.Status==3);
            var getCount = subjects.Count(d => d.TransactorId == viewModel.ManagerId);
            var makeCount = subjects.Count(d => d.ProducerById == viewModel.ManagerId);
            var makeCommission = getCount * 10+ makeCount * 20;
            string subjectRemark = string.Empty;
            if (getCount > 0)
            {
                var money1 = getCount * 10;
                subjectRemark = "\r\n需求提成(认领)：总数[" + getCount + "]条，提成" + money1 + "元；";
            }
            if (makeCount > 0)
            {
                var money2 = makeCount * 20;
                subjectRemark += "\r\n需求提成(制作)：总数[" + makeCount + "]条，提成" + money2 + "元；";
            }
            var total = gw + jt + kq + viewModel.Bonus + viewModel.Commission + saleCommission + sb + zwjt+ makeCommission;
            //五险+个人所得税
            decimal tax = 0;
            decimal wx = 0;
            if (manager.IsInsurance == true)
            {
                wx = config.Endowment + config.Childbirth + config.Health + config.Injury + config.Unemployment + config.HousingFund;//五险
                var gs = total - wx - config.IncomeTaxBase;
                if (gs > 0)
                {
                    tax = Tax(config.TaxRange, gs);
                }
                salaryDetail.Endowment = config.Endowment;
                salaryDetail.Childbirth = config.Childbirth;
                salaryDetail.Health = config.Health;
                salaryDetail.Injury = config.Injury;
                salaryDetail.Unemployment = config.Unemployment;
                salaryDetail.HousingFund = config.HousingFund;
                salaryDetail.Tax = tax;
            }
            //合计
            salaryDetail.Total = Math.Round(total - wx - tax - deductTotal - viewModel.DeductMoney - hs - px);
            salaryDetail.Commission = viewModel.Commission;
            salaryDetail.Bonus = viewModel.Bonus;
            salaryDetail.Date = viewModel.Date;
            salaryDetail.DeductMoney = viewModel.DeductMoney;
            salaryDetail.SaleCommission = saleCommission+ makeCommission;
            salaryDetail.AttendanceTotal = deductTotal;
            
            salaryDetail.Remark = viewModel.Remark+ subjectRemark;
            if (isAddAttendance)
            {
                manager.AttendanceDetails.Add(entity);
            }
            if (isAddSalary)
            {
                manager.SalaryDetails.Add(salaryDetail);
            }
            _managerService.Edit(manager);
            TempData["Msg"] = "添加成功";
            //AttendanceDetailView temp = new AttendanceDetailView();
            return RedirectToAction("Add");
        }
        public ActionResult Detail(string id)
        {
            var entity = _salaryRepository.LoadEntities(d => d.Id == id).FirstOrDefault();
            return PartialView("Detail", entity);
        }

        /// <summary>
        /// 编辑工资
        /// </summary>
        /// <returns></returns>
        public ActionResult Update(string id)
        {
            //明细
            var entity = _salaryRepository.LoadEntities(d => d.Id == id).FirstOrDefault();
            //考勤
            var managerId = entity.ManagerId;
            var date = entity.Date;
            var detail = _attendanceRepository
                .LoadEntities(d => d.ManagerId == managerId && d.Date == date).FirstOrDefault();
            AttendanceDetailView viewModel = new AttendanceDetailView();
            viewModel.Date = date;
            viewModel.Absenteeism = detail.Absenteeism;
            viewModel.OffWork = detail.OffWork;
            viewModel.NoClockTimes = detail.NoClockTimes;
            viewModel.LateTimes = detail.LateTimes;
            viewModel.DeductMoney = entity.DeductMoney;
            viewModel.ManagerId = entity.ManagerId;
            viewModel.ManagerName = entity.Manager.UserName;
            viewModel.Bonus = entity.Bonus;
            viewModel.Commission = entity.Commission;
            viewModel.Remark = entity.Remark;
            return View(viewModel);
        }
        [HttpPost]
        
        public ActionResult Update(AttendanceDetailView viewModel)
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError("message", "数据校验失败，请核对输入的信息是否准确");
                return View(viewModel);
            }

            var manager = _managerRepository.LoadEntities(d => d.Id == viewModel.ManagerId).FirstOrDefault();
            //考勤 奖金
            AttendanceDetail entity = manager.AttendanceDetails.FirstOrDefault(d => d.Date == viewModel.Date);
            entity.OffWork = viewModel.OffWork;
            entity.NoClockTimes = viewModel.NoClockTimes;
            entity.LateTimes = viewModel.LateTimes;
            entity.Absenteeism = viewModel.Absenteeism;
            var quarters = _quartersRepository.LoadEntities(d => d.Id == manager.QuartersId).FirstOrDefault();
            var gw = quarters.BaseSalary;//岗位工资
            var kq = quarters.Attendance;//全勤
            var jt = quarters.Allowance;//岗位津贴
            var zwjt = quarters.Post;//职务津贴
            var px = quarters.Training;//培训费
            var config = _settingService.GetSetting<SalarySet>();
            var sb = config.SocialSecurity;//社保补贴
            var hs = config.FoodFee;//伙食费
            //未打卡
            var jmTimes = config.Derate;
            bool isJm = true;
            var dk = config.NoClock;
            int dkTemp;
            if (viewModel.NoClockTimes > 0)
            {
                dkTemp = viewModel.NoClockTimes - jmTimes;
                isJm = false;
            }
            else
            {
                dkTemp = viewModel.NoClockTimes;
            }
            var noClock = dkTemp * dk;
            //迟到
            var cd = config.Late;
            int cdTemp;
            if (viewModel.LateTimes > 0)
            {
                if (isJm)
                {
                    cdTemp = viewModel.LateTimes - jmTimes;
                }
                else
                {
                    cdTemp = viewModel.LateTimes;
                }
            }
            else
            {
                cdTemp = viewModel.LateTimes;
            }
            var late = cdTemp * cd;
            bool isBad = false;
            //请假
            var qj = (double)config.OffWork;
            double offwork = 0;
            if (viewModel.OffWork <= 5)
            {
                //offwork = viewModel.OffWork <= 2 ? viewModel.OffWork / 0.5 * qj : 2 * qj * Math.Pow(2, viewModel.OffWork - 1);
                if (viewModel.OffWork <= 2)
                {
                    offwork = viewModel.OffWork / 0.5 * qj;
                }
                else
                {
                    if (viewModel.OffWork.ToString().Contains(".5"))
                    {
                        offwork = 2 * qj * Math.Pow(2, viewModel.OffWork - 0.5 - 1) + qj;
                    }
                    else
                    {
                        offwork = 2 * qj * Math.Pow(2, viewModel.OffWork - 1);
                    }
                }
            }
            else
            {
                isBad = true;
            }
            //旷工
            var kuanggong = (double)config.Absenteeism;
            double absenteeism = 0;
            if (viewModel.Absenteeism <= 5)
            {
                //absenteeism = viewModel.Absenteeism <= 2 ? viewModel.Absenteeism / 0.5 * kuanggong : 2 * kuanggong * Math.Pow(2, viewModel.Absenteeism - 1);
                if (viewModel.Absenteeism <= 2)
                {
                    absenteeism = viewModel.Absenteeism / 0.5 * kuanggong;
                }
                else
                {

                    if (viewModel.Absenteeism.ToString().Contains(".5"))
                    {
                        absenteeism = 2 * kuanggong * Math.Pow(2, viewModel.Absenteeism - 0.5 - 1) + kuanggong;
                    }
                    else
                    {
                        absenteeism = 2 * kuanggong * Math.Pow(2, viewModel.Absenteeism - 1);
                    }

                }
            }
            else
            {
                isBad = true;
            }
            //全勤
            bool isQc = !(viewModel.OffWork > 0 || viewModel.Absenteeism > 0 || viewModel.NoClockTimes >= 5 || viewModel.LateTimes >= 5);
            //算工资

            //考勤扣款  旷工+请假+未打卡+迟到
            decimal deductTotal = (decimal)(absenteeism + offwork) + noClock + late; //(decimal)(absenteeism + noClock + late + offwork);
            if (!isQc)
            {
                deductTotal += kq;
            }
            if (isBad)
            {
                deductTotal += gw;
            }
            //奖金 岗位工资+全勤奖+岗位津贴+职务津贴+其他奖金+红包提成+销售提成+社保补贴
            //销售提成
            var quare = new BusinessWriteOffDetailView();
            quare.WriteOffDateStar = new DateTime(viewModel.Date.Value.Year, viewModel.Date.Value.Month, 1);
            quare.WriteOffDateEnd = new DateTime(viewModel.Date.Value.Year, viewModel.Date.Value.Month, DateTime.DaysInMonth(viewModel.Date.Value.Year, viewModel.Date.Value.Month));
            quare.TransactorId = viewModel.ManagerId;
            var saleCommissionList = _businessWriteOffService.LoadEntitiesFilters(quare);
            var saleCommission = saleCommissionList.Any() ? saleCommissionList.Sum(d => d.Commission) : 0;
            saleCommission = saleCommission * quarters.Commission;//乘以系数
            //制作提成
            var endDay = quare.WriteOffDateEnd.Value.AddDays(1);
            var subjects = _subjectRepository.LoadEntities(d =>
                d.IsDelete == false && d.CompletDate >= quare.WriteOffDateStar && d.CompletDate < endDay && d.Subject.IsDelete == false && d.Status == 3);
            var getCount = subjects.Count(d => d.TransactorId == viewModel.ManagerId);
            var makeCount = subjects.Count(d => d.ProducerById == viewModel.ManagerId);
            var makeCommission = getCount * 10 + makeCount * 20;
            string subjectRemark = string.Empty;
            if (getCount > 0)
            {
                var money1 = getCount * 10;
                subjectRemark = "\r\n需求提成(认领)：总数[" + getCount + "]条，提成" + money1 + "元；";
            }
            if (makeCount > 0)
            {
                var money2 = makeCount * 20;
                subjectRemark += "\r\n需求提成(制作)：总数[" + makeCount + "]条，提成" + money2 + "元；";
            }
            var total = gw + jt + kq + viewModel.Bonus + viewModel.Commission + saleCommission + sb + zwjt+ makeCommission;
            SalaryDetail salaryDetail = manager.SalaryDetails.FirstOrDefault(d => d.Date == viewModel.Date);
            //五险+个人所得税
            decimal tax = 0;
            decimal wx = 0;
            if (manager.IsInsurance==true)
            {
                wx = config.Endowment + config.Childbirth + config.Health + config.Injury + config.Unemployment + config.HousingFund;//五险
                var gs = total - wx - config.IncomeTaxBase;
                if (gs > 0)
                {
                    tax = Tax(config.TaxRange, gs);
                }
                salaryDetail.Endowment = config.Endowment;
                salaryDetail.Childbirth = config.Childbirth;
                salaryDetail.Health = config.Health;
                salaryDetail.Injury = config.Injury;
                salaryDetail.Unemployment = config.Unemployment;
                salaryDetail.HousingFund = config.HousingFund;
                salaryDetail.Tax = tax;
            }
            //合计
            salaryDetail.Total = Math.Round(total - wx - tax - deductTotal - viewModel.DeductMoney - hs - px);
            salaryDetail.Commission = viewModel.Commission;
            salaryDetail.Bonus = viewModel.Bonus;
            salaryDetail.DeductMoney = viewModel.DeductMoney;
            salaryDetail.SaleCommission = saleCommission+ makeCommission;
            salaryDetail.AttendanceTotal = deductTotal;
            salaryDetail.Remark = viewModel.Remark+ subjectRemark;
            _managerService.Edit(manager);
            TempData["Msg"] = "编辑成功";
            //AttendanceDetailView temp = new AttendanceDetailView();
            return View(viewModel);
        }
        [HttpPost]
        
        public ActionResult Export(string date)
        {
            if (string.IsNullOrWhiteSpace(date))
            {
                return Json(new { State = 0, Msg = "请选择要导出的月份！" });
            }
            if (!DateTime.TryParse(date, out var exportDate))
            {
                return Json(new { State = 0, Msg = "日期格式有误！" });
            }
            var p = PremissionData();
            var viewModel = new AttendanceDetailView();
            viewModel.Date = exportDate;
            viewModel.ManagerId = p.Any() ? CurrentManager.Id : null;
            viewModel.limit = 500;
            var result = _service.LoadEntitiesFilter(viewModel).ToList();
            if (!result.Any())
            {
                return Json(new { State = 0, Msg = "未找到相关的数据！" });
            }
            JArray jObjects = new JArray();
            var config = _settingService.GetSetting<SalarySet>();
            var site = _settingService.GetSetting<Site>();
            foreach (var salaryDetail in result)
            {
                var jo = new JObject();
                var quarters = _quartersRepository.LoadEntities(d => d.Id == salaryDetail.Manager.QuartersId).FirstOrDefault();
                var company = string.Join(",",
                    salaryDetail.Manager.Organizations.Where(o => o.ParentId == null).Select(n => n.OrganizationName));
                jo.Add("公司",string.IsNullOrWhiteSpace(company)?site.CompanyName:company );
                jo.Add("姓名", salaryDetail.Manager.UserName);
                jo.Add("岗位名称", quarters.Title);
                jo.Add("基本工资", quarters.BaseSalary);
                jo.Add("岗位津贴", quarters.Allowance);
                jo.Add("职务津贴", quarters.Post);
                jo.Add("全勤奖金", quarters.Attendance);
                jo.Add("销售提成", salaryDetail.SaleCommission);
                jo.Add("社保补贴", config.SocialSecurity);
                jo.Add("红包提成", salaryDetail.Commission);
                jo.Add("其他奖金", salaryDetail.Bonus);
                var attendance = salaryDetail.Manager.AttendanceDetails.FirstOrDefault(d => d.Date == exportDate);
                jo.Add("请假天数", attendance.OffWork);
                jo.Add("旷工天数", attendance.Absenteeism);
                jo.Add("迟到次数", attendance.LateTimes);
                jo.Add("未打卡次数", attendance.NoClockTimes);
                jo.Add("考勤扣款", salaryDetail.AttendanceTotal);
                jo.Add("养老保险", salaryDetail.Endowment);
                jo.Add("医疗保险", salaryDetail.Health);
                jo.Add("生育保险", salaryDetail.Childbirth);
                jo.Add("工伤保险", salaryDetail.Injury);
                jo.Add("失业保险", salaryDetail.Unemployment);
                jo.Add("住房公积金", salaryDetail.HousingFund);
                jo.Add("个人所得税", salaryDetail.Tax);
                jo.Add("培训费用", quarters.Training);
                jo.Add("伙食费用", config.FoodFee);
                jo.Add("其他扣款", salaryDetail.DeductMoney);
                jo.Add("实发工资", salaryDetail.Total);
                jo.Add("工资银行", salaryDetail.Manager.BankName);
                jo.Add("工资卡号", salaryDetail.Manager.BankNum);
                jo.Add("备注信息", salaryDetail.Remark);
                jObjects.Add(jo);
            }
            return Json(new { State = 1, Msg = ExportFile(jObjects.ToString(), exportDate.ToString("yyyy年MM月")) });
        }

        [HttpPost]
        
        public ActionResult Delete(string id)
        {
            _service.Delete(id);
            return Json(new { State = 1, Msg = "删除成功" });
        }

        private decimal Tax(string param, decimal money)
        {
            IList<TaxModel> taxModels = new List<TaxModel>();
            var arrs = param.Trim().Replace("\r\n", ",").Split(',').Where(d => !string.IsNullOrWhiteSpace(d)).ToList();
            foreach (var item in arrs)
            {
                var temp = item.Split(':');
                var range = temp[0].Split('-');
                var taxModel = new TaxModel();
                taxModel.Min = decimal.Parse(range[0]);
                taxModel.Max = decimal.Parse(range[1]);
                taxModel.Tax = decimal.Parse(temp[1]);
                taxModels.Add(taxModel);
            }
            taxModels = taxModels.OrderBy(d => d.Tax).ToList();
            //计算速算扣除数
            List<decimal> list = new List<decimal>();
            decimal last = 0;
            list.Add(last);
            for (int i = 0; i < taxModels.Count - 1; i++)
            {
                last = taxModels[i].Max * (taxModels[i + 1].Tax - taxModels[i].Tax) + last;
                list.Add(last);
            }
            list = list.OrderBy(d => d).ToList();
            //计算个人所得税
            int currentLeve = 0;
            decimal currentTax = 0;
            for (int i = 0; i < taxModels.Count; i++)
            {
                if (money > taxModels[i].Min && money <= taxModels[i].Max)
                {
                    currentLeve = i;
                    currentTax = taxModels[i].Tax;
                    break;
                }
            }
            return money * currentTax - list[currentLeve];
        }
    }

    class TaxModel
    {
        public TaxModel()
        {
            Min = 0;
            Max = 0;
            Tax = 0;
        }
        public decimal Min { get; set; }
        public decimal Max { get; set; }
        public decimal Tax { get; set; }
    }
}