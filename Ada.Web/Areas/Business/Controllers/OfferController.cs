using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ada.Core;
using Ada.Core.Domain;
using Ada.Core.Domain.Business;
using Ada.Core.ViewModel.Business;
using Ada.Framework.Filter;
using Ada.Framework.UploadFile;
using Ada.Services.Business;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Newtonsoft.Json;

namespace Business.Controllers
{
    /// <summary>
    /// 报价
    /// </summary>
    public class OfferController : BaseController
    {
        private readonly IBusinessOfferService _businessOfferService;
        private readonly IRepository<BusinessOffer> _repository;
        public OfferController(IBusinessOfferService businessOfferService, IRepository<BusinessOffer> repository)
        {
            _businessOfferService = businessOfferService;
            _repository = repository;
        }
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult GetList(BusinessOfferView viewModel)
        {
            viewModel.Managers = PremissionData();
            var result = _businessOfferService.LoadEntitiesFilter(viewModel).ToList();
            return Json(new
            {
                viewModel.total,
                rows = result.Select(d => new BusinessOfferView
                {
                    Id = d.Id,
                    OfferNum = d.OfferNum,
                    LinkManName = d.LinkMan.Name,
                    TotalMoney = d.TotalMoney,
                    Transactor = d.Transactor,
                    Status = d.Status,
                    OfferDate = d.OfferDate,
                    TotalSellMoney = d.TotalSellMoney,
                    TotalTaxMoney = d.TotalTaxMoney,
                    DiscountMoney = d.DiscountMoney
                })
            }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Add()
        {
            BusinessOfferView viewModel = new BusinessOfferView();
            viewModel.DiscountRate = 100;
            viewModel.Tax = 0;
            viewModel.DiscountMoney = 0;
            viewModel.ValidDays = 7;
            viewModel.OfferDate = DateTime.Now;
            viewModel.Transactor = CurrentManager.UserName;
            viewModel.TransactorId = CurrentManager.Id;
            return View(viewModel);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Add(BusinessOfferView viewModel)
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError("message", "数据校验失败，请核对输入的信息是否准确");
                return View(viewModel);
            }
            var details = JsonConvert.DeserializeObject<List<BusinessOfferDetail>>(viewModel.Details);
            if (details.Count <= 0)
            {
                ModelState.AddModelError("message", "请录入媒体资源！");
                return View(viewModel);
            }
            BusinessOffer entity = new BusinessOffer();
            entity.Id = IdBuilder.CreateIdNum();
            entity.OfferNum = IdBuilder.CreateOrderNum("BJ");
            entity.OfferDate = viewModel.OfferDate;
            entity.ValidDays = viewModel.ValidDays;
            entity.Status = Consts.StateLock;//用户转化为订单的状态
            entity.AddedBy = CurrentManager.UserName;
            entity.AddedById = CurrentManager.Id;
            entity.AddedDate = DateTime.Now;
            entity.OfferType = viewModel.OfferType;
            entity.Remark = viewModel.Remark;
            entity.LinkManId = viewModel.LinkManId;
            entity.Transactor = viewModel.Transactor;
            entity.TransactorId = viewModel.TransactorId;
            entity.Tax = viewModel.Tax;
            entity.DiscountRate = viewModel.DiscountRate;
            entity.DiscountMoney = viewModel.DiscountMoney;
            foreach (var item in details)
            {
                item.Id = IdBuilder.CreateIdNum();
                entity.BusinessOfferDetails.Add(item);
            }
            entity.TotalTaxMoney = details.Sum(d => d.TaxMoney);
            entity.TotalMoney = details.Sum(d => d.Money) - viewModel.DiscountMoney;
            entity.TotalSellMoney = details.Sum(d => d.SellMoney);

            _businessOfferService.Add(entity);
            TempData["Msg"] = "添加成功";
            return RedirectToAction("Index");
        }
        /// <summary>
        /// 预览
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Preview(string id)
        {
            var entity = _repository.LoadEntities(d => d.Id == id).FirstOrDefault();
            return View(entity);
        }
        /// <summary>
        /// 生成报价文件
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [AdaValidateAntiForgeryToken]
        public ActionResult ToPdf()
        {
            UEditorModel uploadConfig = new UEditorModel()
            {
                AllowExtensions = new[] { ".png" },
                PathFormat = UEditorConfig.GetString("scrawlPathFormat"),
                SizeLimit = UEditorConfig.GetInt("scrawlMaxSize"),
                UploadFieldName = UEditorConfig.GetString("scrawlFieldName"),
                Base64 = true,
                Base64Filename = "scrawl.png"
            };
            var base64Str = Request[uploadConfig.UploadFieldName];
            var offerNum = Request["offer"];
            var uploadFileBytes = Convert.FromBase64String(base64Str.Replace("data:image/png;base64,", ""));
            var savePath = "~/upload/offer/" + offerNum + ".pdf";
            var localPath = Server.MapPath(savePath);
            PdfWriter writer=null;
            string msg = savePath;
            bool sucess = true;
            try
            {
                if (!Directory.Exists(Path.GetDirectoryName(localPath)))
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(localPath));
                }
                using (var stream = new FileStream(localPath, FileMode.Create, FileAccess.Write, FileShare.None))
                {
                    using (Document doc = new Document(PageSize.A2))
                    {
                        writer = PdfWriter.GetInstance(doc, stream);
                        doc.Open();
                        var image = Image.GetInstance(uploadFileBytes);
                        if (image.Height > PageSize.A2.Height)
                        {
                            image.ScaleToFit(PageSize.A2.Width, PageSize.A2.Height);
                        }
                        else if (image.Width > PageSize.A2.Width)
                        {
                            image.ScaleToFit(PageSize.A2.Width, PageSize.A2.Height);
                        }
                        image.Alignment = Image.ALIGN_MIDDLE;
                        doc.Add(image);
                    }
                }

            }
            catch (Exception e)
            {
                sucess = false;
                msg = e.Message;
            }
            finally
            {
                writer?.Close();
            }
            return Json(new { State = sucess ? 1 : 0, Msg = msg });
        }
        /// <summary>
        /// 下载报价文件
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public FileResult DownPdf(string path)
        {
            var fullpath = Server.MapPath(path);
            return File(fullpath, "application/pdf", Path.GetFileName(path));
        }
    }

}