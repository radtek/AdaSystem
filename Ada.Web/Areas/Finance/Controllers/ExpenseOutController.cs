using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ada.Core;
using Ada.Core.Domain.Finance;
using Ada.Core.Tools;
using Ada.Core.ViewModel.Finance;
using Ada.Framework.Filter;
using Ada.Services.Finance;
using Newtonsoft.Json;

namespace Finance.Controllers
{
    /// <summary>
    /// 费用支出
    /// </summary>
    public class ExpenseOutController : BaseController
    {
        private readonly IRepository<Expense> _repository;
        public ExpenseOutController(IRepository<Expense> repository)
        {
            _repository = repository;
        }
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Add()
        {
            ExpenseView viewModel = new ExpenseView();
            viewModel.BillDate = DateTime.Now;
            viewModel.IsIncom = false;
            return View(viewModel);
        }
        public ActionResult Update(string id)
        {
            var entity = _repository.LoadEntities(d => d.Id == id).FirstOrDefault();
            ExpenseView viewModel = new ExpenseView();
            viewModel.Id = id;
            viewModel.AccountBank = entity.AccountBank;
            viewModel.AccountName = entity.AccountName;
            viewModel.AccountNum = entity.AccountNum;
            viewModel.BillNum = entity.BillNum;
            viewModel.BillDate = entity.BillDate;
            viewModel.LinkManName = entity.LinkManName;
            viewModel.LinkManId = entity.LinkManId;
            viewModel.Employe = entity.Employe;
            viewModel.EmployerId = entity.EmployerId;
            viewModel.Remark = entity.Remark;
            viewModel.Image = entity.Image;
            viewModel.ThumbnailImage = Thumbnail.MakeThumbnailImageToBase64(Utils.GetMapPath(entity.Image));
            var paydetails = entity.ExpenseDetails.Select(d => new
            {
                d.Id,
                d.SettleAccountId,
                d.IncomeExpendId,
                d.Money
            });
            viewModel.IsIncom = (bool)entity.IsIncom;
            viewModel.PayDetails = JsonConvert.SerializeObject(paydetails);
            return View(viewModel);
        }
    }
}