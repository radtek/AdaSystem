using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ada.Core;
using Ada.Core.Domain.Admin;
using Ada.Core.Domain.Customer;
using Ada.Core.ViewModel.Admin;
using Ada.Core.ViewModel.Customer;
using Ada.Framework.Filter;
using Ada.Services.Customer;

namespace Customer.Controllers
{
    public class CommpanyController : BaseController
    {
        private readonly ICommpanyService _commpanyService;
        private readonly IRepository<Commpany> _repository;
        public CommpanyController(ICommpanyService commpanyService,
            IRepository<Commpany> repository
           )
        {
            _commpanyService = commpanyService;
            _repository = repository;
        }
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult GetList(CommpanyView viewModel)
        {
            var result = _commpanyService.LoadEntitiesFilter(viewModel).ToList();
            return Json(new
            {
                viewModel.total,
                rows = result.Select(d => new CommpanyView
                {
                    Id = d.Id,
                    Name = d.Name,
                    CommpanyType = d.CommpanyType,
                    CommpanyGrade = d.CommpanyGrade,
                    City = d.City,
                    Address = d.Address
                })
            }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Add()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Add(CommpanyView viewModel)
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError("message", "数据校验失败，请核对输入的信息是否准确");
                return View();
            }
            Commpany commpany = new Commpany
            {
                Id = IdBuilder.CreateIdNum(),
                AddedById = CurrentManager.Id,
                AddedBy = CurrentManager.UserName,
                AddedDate = DateTime.Now,
                Name = viewModel.Name,
                Address = viewModel.Address,
                City = viewModel.City,
                CommpanyType = viewModel.CommpanyType,
                CommpanyGrade = viewModel.CommpanyGrade,
                Phone = viewModel.Phone,
                IsBusiness = viewModel.IsBusiness
            };
            _commpanyService.Add(commpany);
            TempData["Msg"] = "添加成功";
            return RedirectToAction("Index");
        }
    }
}