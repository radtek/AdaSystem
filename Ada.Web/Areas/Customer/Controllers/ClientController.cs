using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ada.Core;
using Ada.Core.Domain.Customer;
using Ada.Core.ViewModel.Customer;
using Ada.Framework.Filter;
using Ada.Services.Customer;

namespace Customer.Controllers
{
    /// <summary>
    /// 客户公司
    /// </summary>
    public class ClientController : BaseController
    {
        private readonly ICommpanyService _commpanyService;
        private readonly IRepository<Commpany> _repository;

        public ClientController(ICommpanyService commpanyService,
            IRepository<Commpany> repository)
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
            CommpanyView viewModel = new CommpanyView();
            viewModel.IsBusiness = true;
            return View(viewModel);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Add(CommpanyView viewModel)
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError("message", "数据校验失败，请核对输入的信息是否准确");
                return View(viewModel);
            }
            //校验公司名称唯一

            var temp = _repository.LoadEntities(d =>
                d.Name.Equals(viewModel.Name, StringComparison.CurrentCultureIgnoreCase) && d.IsDelete == false &&
                d.IsBusiness == viewModel.IsBusiness).FirstOrDefault();
            if (temp != null)
            {
                ModelState.AddModelError("message", viewModel.Name + "，此公司已存在！");
                return View(viewModel);
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
                IsBusiness = false
            };
            _commpanyService.Add(commpany);
            TempData["Msg"] = "添加成功";
            return RedirectToAction("Index");
        }
        public ActionResult Update(string id)
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
                Phone = entity.Phone,
                IsBusiness = entity.IsBusiness
            };
            return View(item);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Update(CommpanyView viewModel)
        {

            if (!ModelState.IsValid)
            {
                ModelState.AddModelError("message", "数据校验失败，请核对输入的信息是否准确");
                return View(viewModel);
            }
            //校验公司名称唯一
            var temp = _repository.LoadEntities(d =>
                d.Name.Equals(viewModel.Name, StringComparison.CurrentCultureIgnoreCase) && d.IsDelete == false &&
                d.IsBusiness == viewModel.IsBusiness && d.Id != viewModel.Id).FirstOrDefault();
            if (temp != null)
            {
                ModelState.AddModelError("message", viewModel.Name + "，此公司已存在！");
                return View(viewModel);
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
            return RedirectToAction("Index");
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