using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ada.Core;
using Ada.Core.Domain.Admin;
using Ada.Core.Domain.Wages;
using Ada.Core.ViewModel.Business;
using Ada.Core.ViewModel.Wages;
using Ada.Framework.Filter;
using Ada.Services.Admin;
using Ada.Services.Business;
using Ada.Services.Salary;
using Ada.Services.Setting;
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
        private readonly IRepository<SalaryDetail> _salaryRepository;
        private readonly IBusinessWriteOffService _businessWriteOffService;
        private readonly ISettingService _settingService;
        public SalaryDetailController(ISalaryDetailService service,
            IManagerService managerService,
            IRepository<Manager> managerRepository,
            IBusinessWriteOffService businessWriteOffService,
            IRepository<Quarters> quartersRepository,
            IRepository<SalaryDetail> salaryRepository,
            ISettingService settingService)
        {
            _service = service;
            _managerService = managerService;
            _managerRepository = managerRepository;
            _businessWriteOffService = businessWriteOffService;
            _quartersRepository = quartersRepository;
            _salaryRepository = salaryRepository;
            _settingService = settingService;
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
                    d.Total

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
        [ValidateAntiForgeryToken]
        public ActionResult Add(AttendanceDetailView viewModel)
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError("message", "数据校验失败，请核对输入的信息是否准确");
                return View();
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
            var jt = quarters.Allowance;//津贴
            var config = _settingService.GetSetting<SalarySet>();
            //未打卡
            var dk = config.NoClock;
            var dkYh = config.Derate;
            var dkTemp = viewModel.NoClockTimes > 0 ? viewModel.NoClockTimes - dkYh : viewModel.NoClockTimes;
            var noClock = dkTemp * dk;
            //迟到
            var cd = config.Late;
            var cdYh = config.Derate;
            var cdTemp = viewModel.LateTimes > 0 ? viewModel.LateTimes - cdYh : viewModel.LateTimes;
            var late = cdTemp * cd;
            bool isBad = false;
            //请假
            var qj = (double)config.OffWork;
            double offwork = 0;
            if (viewModel.OffWork < 5)
            {
                offwork = viewModel.OffWork <= 2 ? viewModel.OffWork / 0.5 * qj : 2 * qj * Math.Pow(2, viewModel.OffWork - 1);
            }
            else
            {
                isBad = true;
            }
            //旷工
            var kuanggong = (double)config.Absenteeism;
            double absenteeism = 0;
            if (viewModel.Absenteeism < 5)
            {
                absenteeism = viewModel.Absenteeism <= 2 ? viewModel.Absenteeism / 0.5 * kuanggong : 2 * kuanggong * Math.Pow(2, viewModel.Absenteeism - 1);
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
            //扣款  旷工+请假+未打卡+迟到
            decimal deductTotal = (decimal) (absenteeism + offwork) + noClock + late; //(decimal)(absenteeism + noClock + late + offwork);
            if (!isQc)
            {
                deductTotal += kq;
            }
            if (isBad)
            {
                deductTotal += gw;
            }
            //奖金 岗位工资+全勤奖+津贴+其他奖金+红包提成+销售提成
            //销售提成
            var quare = new BusinessWriteOffDetailView();
            quare.WriteOffDateStar = new DateTime(viewModel.Date.Value.Year, viewModel.Date.Value.Month, 1);
            quare.WriteOffDateEnd = new DateTime(viewModel.Date.Value.Year, viewModel.Date.Value.Month, DateTime.DaysInMonth(viewModel.Date.Value.Year, viewModel.Date.Value.Month));
            quare.TransactorId = viewModel.ManagerId;
            var saleCommission = _businessWriteOffService.LoadEntitiesFilter(quare).Sum(d => d.Commission) ?? 0;
            var total = gw + jt + kq + viewModel.Bonus + viewModel.Commission + saleCommission;
            //合计
            salaryDetail.Total =Math.Round(total - deductTotal - viewModel.DeductMoney) ;
            salaryDetail.Commission = viewModel.Commission;
            salaryDetail.Bonus = viewModel.Bonus;
            salaryDetail.Date = viewModel.Date;
            salaryDetail.DeductMoney = viewModel.DeductMoney;
            salaryDetail.SaleCommission = saleCommission;
            salaryDetail.AttendanceTotal = deductTotal;
            salaryDetail.Remark = viewModel.Remark;
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
        [HttpPost]
        [AdaValidateAntiForgeryToken]
        public ActionResult Delete(string id)
        {
            _service.Delete(id);
            return Json(new { State = 1, Msg = "删除成功" });
        }
    }
}