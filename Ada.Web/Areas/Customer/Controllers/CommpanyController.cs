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
        public ActionResult Business()
        {
            return View();
        }
        public ActionResult Custom()
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
        public ActionResult AddCustom()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddCustom(CommpanyView viewModel)
        {
            return Add(viewModel, "Custom");
        }
        public ActionResult AddBusiness()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddBusiness(CommpanyView viewModel)
        {
            return Add(viewModel, "Business");
        }

        private ActionResult Add(CommpanyView viewModel,string returnView)
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError("message", "数据校验失败，请核对输入的信息是否准确");
                return View();
            }
            //校验公司名称唯一

            var temp = _repository.LoadEntities(d =>
                d.Name.Equals(viewModel.Name, StringComparison.CurrentCultureIgnoreCase) && d.IsDelete == false &&
                d.IsBusiness == viewModel.IsBusiness).FirstOrDefault();
            if (temp!=null)
            {
                ModelState.AddModelError("message", viewModel.Name+ "，此公司已存在！");
                return View();
            }
            Commpany commpany = new Commpany
            {
                Id = IdBuilder.CreateIdNum(),
                AddedById = CurrentManager.Id,
                AddedBy = CurrentManager.UserName,
                AddedDate = DateTime.Now,
                Name = viewModel.Name.Trim(),
                Address = viewModel.Address,
                City = viewModel.City,
                CommpanyType = viewModel.CommpanyType,
                CommpanyGrade = viewModel.CommpanyGrade,
                Phone = viewModel.Phone,
                IsBusiness = viewModel.IsBusiness
            };
            _commpanyService.Add(commpany);
            TempData["Msg"] = "添加成功";
            return RedirectToAction(returnView);
        }
        public ActionResult UpdateCustom(string id)
        {
            var entity = _repository.LoadEntities(d => d.Id == id).FirstOrDefault();
            CommpanyView item = new CommpanyView
            {
                Id = entity.Id,
                Name = entity.Name,
                CommpanyType = entity.CommpanyType,
                CommpanyGrade = entity.CommpanyGrade,
                City = entity.City,
                Address = entity.Address,
                Phone = entity.Phone
            };
            return View(item);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UpdateCustom(CommpanyView viewModel)
        {

            return Update(viewModel, "Custom");
        }
        public ActionResult UpdateBusiness(string id)
        {
            var entity = _repository.LoadEntities(d => d.Id == id).FirstOrDefault();
            CommpanyView item = new CommpanyView
            {
                Id = entity.Id,
                Name = entity.Name.Trim(),
                CommpanyType = entity.CommpanyType,
                CommpanyGrade = entity.CommpanyGrade,
                City = entity.City,
                Address = entity.Address,
                Phone = entity.Phone
            };
            return View(item);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UpdateBusiness(CommpanyView viewModel)
        {
            return Update(viewModel, "Business");
        }
        private ActionResult Update(CommpanyView viewModel, string returnView)
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError("message", "数据校验失败，请核对输入的信息是否准确");
                return View(viewModel);
            }
            //校验公司名称唯一
            var temp = _repository.LoadEntities(d =>
                d.Name.Equals(viewModel.Name, StringComparison.CurrentCultureIgnoreCase) && d.IsDelete == false &&
                d.IsBusiness == viewModel.IsBusiness&&d.Id!=viewModel.Id).FirstOrDefault();
            if (temp != null)
            {
                ModelState.AddModelError("message", viewModel.Name + "，此公司已存在！");
                return View();
            }
            var entity = _repository.LoadEntities(d => d.Id == viewModel.Id).FirstOrDefault();
            entity.ModifiedById = CurrentManager.Id;
            entity.ModifiedBy = CurrentManager.UserName;
            entity.ModifiedDate = DateTime.Now;
            entity.Name = viewModel.Name;
            entity.Address = viewModel.Address;
            entity.City = viewModel.City;
            entity.CommpanyType = viewModel.CommpanyType;
            entity.CommpanyGrade = viewModel.CommpanyGrade;
            entity.Phone = viewModel.Phone;
            _commpanyService.Update(entity);
            TempData["Msg"] = "更新成功";
            return RedirectToAction(returnView);
        }

        [HttpPost]
        [AdaValidateAntiForgeryToken]
        public ActionResult Delete(string id)
        {
            var entity = _repository.LoadEntities(d => d.Id == id).FirstOrDefault();
            entity.DeletedBy = CurrentManager.UserName;
            entity.DeletedById = CurrentManager.Id;
            entity.DeletedDate = DateTime.Now;
            _commpanyService.Delete(entity);
            return Json(new { State = 1, Msg = "删除成功" });
        }
    }
}