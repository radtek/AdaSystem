using System;
using System.Linq;
using System.Web.Mvc;
using Ada.Core;
using Ada.Core.Domain.WorkFlow;
using Ada.Core.ViewModel.WorkFlow;
using Ada.Framework.Filter;
using Ada.Services.WorkFlow;

namespace WorkFlow.Controllers
{
    public class DefinitionController : BaseController
    {
        private readonly IWorkFlowDefinitionService _service;
        private readonly IRepository<WorkFlowDefinition> _repository;
        public DefinitionController(IWorkFlowDefinitionService service, IRepository<WorkFlowDefinition> repository)
        {
            _service = service;
            _repository = repository;
        }
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult GetList(WorkFlowDefinitionView viewModel)
        {
            var result = _service.LoadEntitiesFilter(viewModel).ToList();
            return Json(new
            {
                viewModel.total,
                rows = result.Select(d => new
                {
                    d.Id,
                    d.Name
                   
                })
            }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Add()
        {
            return View(new WorkFlowDefinitionView(){Enabled = true});
        }
        [HttpPost,ValidateInput(false)]
        public ActionResult Add(WorkFlowDefinitionView viewModel)
        {
            WorkFlowDefinition workFlowDefinition=new WorkFlowDefinition();
            workFlowDefinition.Id = IdBuilder.CreateIdNum();
            workFlowDefinition.Name = viewModel.Name;
            workFlowDefinition.ActityType = viewModel.ActityType;
            workFlowDefinition.WFType = viewModel.WFType;
            workFlowDefinition.Description = viewModel.Description;
            workFlowDefinition.TempForm = viewModel.TempForm;
            workFlowDefinition.Enabled = viewModel.Enabled;
            workFlowDefinition.AddedDate = DateTime.Now;
            workFlowDefinition.AddedBy = CurrentManager.UserName;
            workFlowDefinition.AddedById = CurrentManager.Id;
            _service.Add(workFlowDefinition);
            TempData["Msg"] = "保存成功";
            return RedirectToAction("Index");
        }
        //public ActionResult PropertiesView(string id)
        //{
        //    if (string.IsNullOrWhiteSpace(id))
        //    {
        //        return PartialView("ParopertiesView",new WorkFlowDefinitionView());
        //    }
        //    var entity = _repository.LoadEntities(d => d.Id == id).FirstOrDefault();
        //    return PartialView("ParopertiesView", new WorkFlowDefinitionView(){Name = entity.Name,Id = entity.Id});
        //}
        //[HttpPost]
        //public ActionResult Properties(WorkFlowDefinitionView viewModel)
        //{
        //    if (string.IsNullOrWhiteSpace(viewModel.Name))
        //    {
        //        return Json(new { State = 0, Msg = "名称不能为空" });
        //    }
        //    WorkFlowDefinition workFlowDefinition;
        //    if (string.IsNullOrWhiteSpace(viewModel.Id))
        //    {
        //        workFlowDefinition = new WorkFlowDefinition();
        //        workFlowDefinition.Name = viewModel.Name;
        //        workFlowDefinition.Enabled = true;
        //        workFlowDefinition.Id = IdBuilder.CreateIdNum();
        //        workFlowDefinition.AddedDate = DateTime.Now;
        //        workFlowDefinition.AddedBy = CurrentManager.UserName;
        //        workFlowDefinition.AddedById = CurrentManager.Id;
        //        _service.Add(workFlowDefinition);
        //    }
        //    else
        //    {
        //        workFlowDefinition = _repository.LoadEntities(d => d.Id == viewModel.Id).FirstOrDefault();
        //        workFlowDefinition.Name = viewModel.Name;
        //        _service.Update(workFlowDefinition);
        //    }
        //    return Json(new { State = 1, Msg = "操作成功" });
        //}
        /// <summary>
        /// 编辑
        /// </summary>
        /// <returns></returns>
        public ActionResult Update(string id)
        {
            var entity= _repository.LoadEntities(d => d.Id == id).FirstOrDefault();
            WorkFlowDefinitionView viewModel=new WorkFlowDefinitionView();
            viewModel.Id = id;
            viewModel.Name = entity.Name;
            viewModel.ActityType = entity.ActityType;
            viewModel.WFType = entity.WFType;
            viewModel.Description = entity.Description;
            viewModel.TempForm = entity.TempForm;
            viewModel.Enabled = entity.Enabled;
            return View(viewModel);
        }
        [HttpPost, ValidateInput(false)]
        public ActionResult Update(WorkFlowDefinitionView viewModel)
        {
            var entity = _repository.LoadEntities(d => d.Id == viewModel.Id).FirstOrDefault();
            entity.Name = viewModel.Name;
            entity.ActityType = viewModel.ActityType;
            entity.WFType = viewModel.WFType;
            entity.Description = viewModel.Description;
            entity.TempForm = viewModel.TempForm;
            entity.Enabled = viewModel.Enabled;
            entity.ModifiedBy = CurrentManager.UserName;
            entity.ModifiedById = CurrentManager.Id;
            entity.ModifiedDate = DateTime.Now;
            _service.Update(entity);
            TempData["Msg"] = "保存成功";
            return RedirectToAction("Index");
        }
        [HttpPost]
        
        public ActionResult Delete(string id)
        {
            var entity = _repository.LoadEntities(d => d.Id == id).FirstOrDefault();
            _service.Delete(entity);
            return Json(new { State = 1, Msg = "删除成功" });
        }
    }
}