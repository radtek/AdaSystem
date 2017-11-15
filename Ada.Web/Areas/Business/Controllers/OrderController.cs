using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ada.Core;
using Ada.Core.Domain.Business;
using Ada.Core.Domain.Resource;
using Ada.Core.ViewModel.Business;
using Ada.Core.ViewModel.Resource;
using Ada.Framework.Filter;
using Ada.Services.Business;

namespace Business.Controllers
{
    public class OrderController : BaseController
    {
        private readonly IBusinessOrderService _businessOrderService;
        private readonly IRepository<BusinessOrder> _repository;
        private readonly IRepository<MediaType> _mediaTypeRepository;
        public OrderController(IBusinessOrderService businessOrderService,
            IRepository<BusinessOrder> repository, IRepository<MediaType> mediaTypeRepository
        )
        {
            _businessOrderService = businessOrderService;
            _repository = repository;
            _mediaTypeRepository = mediaTypeRepository;
        }
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult GetList(BusinessOrderView viewModel)
        {
            var result = _businessOrderService.LoadEntitiesFilter(viewModel).ToList();
            return Json(new
            {
                viewModel.total,
                rows = result.Select(d => new BusinessOrderView
                {
                    Id = d.Id
                })
            }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Add()
        {
            BusinessOrderView viewModel = new BusinessOrderView();
            return View(viewModel);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Add(BusinessOrderView viewModel)
        {
            //if (!ModelState.IsValid)
            //{
            //    ModelState.AddModelError("message", "数据校验失败，请核对输入的信息是否准确");
            //    return View();
            //}
            //MediaType entity = new MediaType();
            //entity.Id = IdBuilder.CreateIdNum();
            //entity.AddedById = CurrentManager.Id;
            //entity.AddedBy = CurrentManager.UserName;
            //entity.AddedDate = DateTime.Now;
            //entity.TypeName = viewModel.TypeName;
            //entity.CallIndex = viewModel.CallIndex;

            //_mediaTypeService.Add(entity, viewModel.AdPositions);
            TempData["Msg"] = "添加成功";
            return RedirectToAction("Index");
        }

        public ActionResult SelectMedia()
        {
            //媒体类型
            var mediaTypes = _mediaTypeRepository.LoadEntities(d => d.IsDelete == false).ToList();
            return PartialView("SelectMedia", mediaTypes);
        }
        public ActionResult Update(string id)
        {
            var item = _repository.LoadEntities(d => d.Id == id).FirstOrDefault();
            BusinessOrderView entity = new BusinessOrderView();
            //entity.Id = item.Id;
            //entity.CallIndex = item.CallIndex;
            //entity.TypeName = item.TypeName;
            //entity.AdPositions = item.AdPositions.Select(d => d.Name).ToList();
            return View(entity);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Update(MediaTypeView viewModel)
        {
            //if (!ModelState.IsValid)
            //{
            //    ModelState.AddModelError("message", "数据校验失败，请核对输入的信息是否准确");
            //    return View(viewModel);
            //}
            //var entity = _repository.LoadEntities(d => d.Id == viewModel.Id).FirstOrDefault();
            //entity.ModifiedById = CurrentManager.Id;
            //entity.ModifiedBy = CurrentManager.UserName;
            //entity.ModifiedDate = DateTime.Now;
            //entity.TypeName = viewModel.TypeName;
            //entity.CallIndex = viewModel.CallIndex;
            //_mediaTypeService.Update(entity, viewModel.AdPositions);
            TempData["Msg"] = "更新成功";
            return RedirectToAction("Index");
        }
        [HttpPost]
        [AdaValidateAntiForgeryToken]
        public ActionResult Delete(string id)
        {
            //var entity = _repository.LoadEntities(d => d.Id == id).FirstOrDefault();
            //entity.DeletedBy = CurrentManager.UserName;
            //entity.DeletedById = CurrentManager.Id;
            //entity.DeletedDate = DateTime.Now;
            //_mediaTypeService.Delete(entity);
            return Json(new { State = 1, Msg = "删除成功" });
        }
    }
}