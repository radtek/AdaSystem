using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ada.Core;
using Ada.Core.Domain.Customer;
using Ada.Core.ViewModel.Customer;
using Ada.Framework.Filter;
using Ada.Framework.UploadFile;
using Ada.Services.Customer;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;

namespace Customer.Controllers
{
    /// <summary>
    /// 供应商
    /// </summary>
    public class SupplierController : BaseController
    {
        private readonly ICommpanyService _commpanyService;
        private readonly IRepository<Commpany> _repository;
        private readonly IDbContext _dbContext;

        public SupplierController(ICommpanyService commpanyService,
            IRepository<Commpany> repository, IDbContext dbContext)
        {
            _commpanyService = commpanyService;
            _repository = repository;
            _dbContext = dbContext;
        }
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult GetList(CommpanyView viewModel)
        {
            viewModel.Managers = PremissionData();
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
                    Address = d.Address,
                    Transactor = d.Transactor
                })
            }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Add()
        {
            CommpanyView viewModel = new CommpanyView();
            viewModel.IsBusiness = true;
            viewModel.Transactor = CurrentManager.UserName;
            viewModel.TransactorId = CurrentManager.Id;
            return View(viewModel);
        }
        [HttpPost]
        
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
                d.IsBusiness).FirstOrDefault();
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
                IsBusiness = true,
                Transactor = viewModel.Transactor,
                TransactorId = viewModel.TransactorId
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
                IsBusiness = entity.IsBusiness,
                Transactor = entity.Transactor,
                TransactorId = entity.TransactorId
            };
            return View(item);
        }
        [HttpPost]
        
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
                d.IsBusiness  && d.Id != viewModel.Id).FirstOrDefault();
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
            entity.Transactor = viewModel.Transactor;
            entity.TransactorId = viewModel.TransactorId;
            _commpanyService.Update(entity);
            TempData["Msg"] = "更新成功";
            return RedirectToAction("Index");
        }
        [HttpPost]
        
        public ActionResult Delete(string id)
        {
            var entity = _repository.LoadEntities(d => d.Id == id).FirstOrDefault();
            if (entity.LinkMans.Count(d => d.IsDelete == false) > 0)
            {
                return Json(new { State = 0, Msg = "该公司下还有其他联系人信息，无法删除" });
            }
            entity.DeletedBy = CurrentManager.UserName;
            entity.DeletedById = CurrentManager.Id;
            entity.DeletedDate = DateTime.Now;
            _commpanyService.Delete(entity);
            return Json(new { State = 1, Msg = "删除成功" });
        }
        /// <summary>
        /// 导入
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Import()
        {
            UEditorModel uploadConfig = new UEditorModel()
            {
                AllowExtensions = UEditorConfig.GetStringList("fileAllowFiles"),
                PathFormat = UEditorConfig.GetString("filePathFormat"),
                SizeLimit = UEditorConfig.GetInt("fileMaxSize"),
                UploadFieldName = UEditorConfig.GetString("fileFieldName")
            };
            var file = Request.Files[uploadConfig.UploadFieldName];
            if (file == null)
            {
                return Json(new { State = 0, Msg = "请选择要导入的文件" });
            }
            var uploadFileName = file.FileName;
            var fileExtension = Path.GetExtension(uploadFileName).ToLower();
            if (!uploadConfig.AllowExtensions.Select(x => x.ToLower()).Contains(fileExtension))
            {
                return Json(new { State = 0, Msg = "文件类型不匹配" });
            }
            if (!(file.ContentLength < uploadConfig.SizeLimit))
            {
                return Json(new { State = 0, Msg = "上传的文件最大只能为：" + uploadConfig.SizeLimit + "B" });
            }
            //创建工作薄
            IWorkbook wk = new XSSFWorkbook(file.InputStream);
            //1.获取第一个工作表
            ISheet sheet = wk.GetSheetAt(0);
            if (sheet.LastRowNum <= 1)
            {
                return Json(new { State = 0, Msg = "此文件没有导入数据，请填充数据再进行导入" });
            }

            for (int i = 1; i <= sheet.LastRowNum; i++)
            {
                IRow row = sheet.GetRow(i);
                var companyName = row.GetCell(0)?.ToString();
                var linkManName = row.GetCell(1)?.ToString();
                var transactor = row.GetCell(5)?.ToString();
                var transactorId = row.GetCell(6)?.ToString();
                if (companyName == null || transactor == null || transactorId == null || linkManName == null)
                {
                    continue;
                }
                var company = _repository.LoadEntities(d =>
                    d.Name.Equals(companyName.Trim(), StringComparison.CurrentCultureIgnoreCase) && d.IsDelete == false &&
                    d.IsBusiness).FirstOrDefault();
                var linkMan = new LinkMan
                {
                    Id = IdBuilder.CreateIdNum(),
                    AddedById = transactorId.Trim(),
                    AddedBy = transactor.Trim(),
                    AddedDate = DateTime.Now,
                    Name = linkManName.Trim(),
                    QQ = row.GetCell(4)?.ToString(),
                    Phone = row.GetCell(2)?.ToString(),
                    WeiXin = row.GetCell(3)?.ToString(),
                    Transactor = transactor.Trim(),
                    TransactorId = transactorId.Trim()
                };
                if (company == null)
                {
                    company = new Commpany
                    {
                        Id = IdBuilder.CreateIdNum(),
                        AddedById = transactorId.Trim(),
                        AddedBy = transactor.Trim(),
                        AddedDate = DateTime.Now,
                        Name = companyName.Trim(),
                        IsBusiness = true,
                        Transactor = transactor.Trim(),
                        TransactorId = transactorId.Trim()
                    };
                    company.LinkMans.Add(linkMan);
                    _repository.Add(company);
                }
                else
                {
                    var temp = company.LinkMans.FirstOrDefault(d =>
                          d.Name.Equals(linkManName.ToString().Trim(), StringComparison.CurrentCultureIgnoreCase) &&
                          d.IsDelete == false);
                    if (temp != null) continue;
                    company.LinkMans.Add(linkMan);
                    _repository.Update(company);
                }
            }
            _dbContext.SaveChanges();
            return Json(new { State = 1, Msg = "导入成功" + (sheet.LastRowNum - 1) + "条数据" });
        }
    }
}