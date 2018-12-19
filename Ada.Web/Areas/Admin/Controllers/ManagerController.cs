using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ada.Core;
using Ada.Core.Domain;
using Ada.Core.Domain.Admin;
using Ada.Core.Domain.Wages;
using Ada.Core.Tools;
using Ada.Core.ViewModel;
using Ada.Core.ViewModel.Admin;
using Ada.Framework.Caching;
using Ada.Framework.Filter;
using Ada.Framework.UploadFile;
using Ada.Services.Admin;
using Newtonsoft.Json.Linq;
using Action = Ada.Core.Domain.Admin.Action;

namespace Admin.Controllers
{
    public class ManagerController : BaseController
    {
        private readonly IRepository<Action> _actionRepository;
        private readonly IRepository<Role> _roleRepository;
        private readonly IRepository<Manager> _managerRepository;
        private readonly IManagerService _managerService;
        private readonly IRepository<Quarters> _quartersRepository;
        private readonly IRepository<Organization> _organizationRepository;

        public ManagerController(IManagerService managerService,
            IRepository<Manager> managerRepository,
            IRepository<Role> roleRepository,
            IRepository<Action> actionRepository,
            IRepository<Organization> organizationRepository,
            IRepository<Quarters> quartersRepository)
        {
            _actionRepository = actionRepository;
            _roleRepository = roleRepository;
            _managerRepository = managerRepository;
            _managerService = managerService;
            _organizationRepository = organizationRepository;
            _quartersRepository = quartersRepository;
        }
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult GetList(ManagerView viewModel)
        {
            viewModel.Managers = PremissionData();
            var result = _managerService.LoadEntitiesFilter(viewModel).ToList();
            return Json(new
            {
                viewModel.total,
                rows = result.Select(d => new ManagerView
                {
                    Id = d.Id,
                    UserName = d.UserName,
                    Password = Encrypt.Decode(d.Password),
                    Phone = d.Phone,
                    Image = d.Image,
                    Status = d.Status,
                    RealName = d.RealName,
                    AddDate = d.AddedDate?.ToString("yyyy年MM月dd日") ?? "",
                    Roles = d.Roles.Count > 0 ? string.Join(",", d.Roles.Select(r => r.RoleName)) : "",
                    Organizations = GetOrganization(d),
                    LastLoginDate = d.ManagerLoginLogs.OrderByDescending(l => l.LoginTime).FirstOrDefault() == null ? "" : Utils.ToRead(d.ManagerLoginLogs.OrderByDescending(l => l.LoginTime).FirstOrDefault().LoginTime)
                })
            }, JsonRequestBehavior.AllowGet);
        }

        private string GetOrganization(Manager manager)
        {
            List<string> names = new List<string>();
            foreach (var organization in manager.Organizations)
            {
                List<string> list = new List<string>();
                var arry = organization.TreePath.Trim('/').Split('/');
                foreach (var s in arry)
                {
                    var temp = _organizationRepository.LoadEntities(d => d.Id == s).FirstOrDefault();
                    if (temp.Level != 1)
                    {
                        list.Add(temp.OrganizationName);
                    }
                }
                var str = string.Join(" → ", list);
                names.Add(str);
            }
            return string.Join(",", names);

        }
        private List<TreeView> GetTree(string parentId, List<Organization> list)
        {
            var newlist = list.Where(d => d.ParentId == parentId).ToList();
            List<TreeView> treeViews = new List<TreeView>();
            newlist.ForEach(item =>
            {
                treeViews.Add(new TreeView
                {
                    Id = item.Id,
                    Children = GetTree(item.Id, list),
                    ParentId = item.ParentId,
                    Text = item.OrganizationName
                });
            });
            return treeViews;
        }
        private List<TreeView> GetTree(string parentId, List<Action> list)
        {
            var newlist = list.Where(d => d.ParentId == parentId).ToList();
            List<TreeView> treeViews = new List<TreeView>();
            newlist.ForEach(item =>
            {
                treeViews.Add(new TreeView
                {
                    Id = item.Id,
                    Children = GetTree(item.Id, list),
                    ParentId = item.ParentId,
                    Text = item.ActionName
                });
            });
            return treeViews;
        }

        private void InitViewModel(ManagerView viewModel)
        {
            var roles = _roleRepository.LoadEntities(d => d.IsDelete == false).ToList();
            viewModel.RoleList = roles.Select(d => new RoleView() { Id = d.Id, RoleName = d.RoleName });
            var organizations = _organizationRepository.LoadEntities(d => d.IsDelete == false).OrderBy(d => d.Taxis)
                .ToList();
            viewModel.OrganizationList = GetTree(null, organizations);
            var actions = _actionRepository.LoadEntities(d => d.IsDelete == false).OrderBy(d => d.Taxis).ToList();
            viewModel.ActionList = GetTree(null, actions);
            viewModel.RoleIds = new List<string>();
        }
        public ActionResult Add()
        {
            ManagerView viewModel = new ManagerView();
            InitViewModel(viewModel);
            viewModel.Status = Consts.StateNormal;
            viewModel.IsLunar = false;
            return View(viewModel);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Add(ManagerView viewModel)
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError("message", "数据校验失败，请核对输入的信息是否准确");
                InitViewModel(viewModel);
                return View(viewModel);
            }
            var organizationIds = string.IsNullOrWhiteSpace(viewModel.OrganizationIds)
                ? null
                : viewModel.OrganizationIds.Split(',').ToList();
            var actionIds = string.IsNullOrWhiteSpace(viewModel.ActionIds)
                ? null
                : viewModel.ActionIds.Split(',').ToList();
            //校验唯一性
            var temp = _managerRepository
                .LoadEntities(d => d.UserName.Equals(viewModel.UserName, StringComparison.CurrentCultureIgnoreCase) && d.IsDelete == false)
                .FirstOrDefault();
            if (temp != null)
            {
                ModelState.AddModelError("message", "用户名：" + viewModel.UserName + "，已被占用！");
                InitViewModel(viewModel);
                return View(viewModel);
            }
            var manager = new Manager()
            {
                UserName = viewModel.UserName,
                RealName = viewModel.RealName,
                Phone = viewModel.Phone,
                Status = viewModel.Status,
                IsLunar = viewModel.IsLunar,
                IdCard = viewModel.IdCard,
                Birthday = viewModel.Birthday,
                EntryDate = viewModel.EntryDate,
                QuitDate = viewModel.QuitDate,
                PromotionDate = viewModel.PromotionDate,
                BankAccount = viewModel.BankAccount,
                BankName = viewModel.BankName,
                BankNum = viewModel.BankNum,
                QuartersId = viewModel.QuartersId,
                Password = Encrypt.Encode(viewModel.Password),
                IsInsurance = viewModel.IsInsurance,
                Id = IdBuilder.CreateIdNum(),
                AddedBy = CurrentManager.UserName,
                AddedById = CurrentManager.Id,
                AddedDate = DateTime.Now
            };
            _managerService.AddOrUpdate(manager, true, viewModel.RoleIds, organizationIds, actionIds);
            TempData["Msg"] = "添加成功";
            return RedirectToAction("Index");
        }
        public ActionResult Update(string id)
        {
            ManagerView viewModel = new ManagerView();
            InitViewModel(viewModel);
            var manager = _managerRepository.LoadEntities(d => d.Id == id).FirstOrDefault();
            viewModel.UserName = manager.UserName;
            viewModel.Phone = manager.Phone;
            viewModel.Password = Encrypt.Decode(manager.Password);
            viewModel.Status = manager.Status;
            viewModel.IdCard = manager.IdCard;
            viewModel.EntryDate = manager.EntryDate;
            viewModel.QuitDate = manager.QuitDate;
            viewModel.Birthday = manager.Birthday;
            viewModel.IsLunar = manager.IsLunar ?? false;
            viewModel.RealName = manager.RealName;
            viewModel.QuartersId = manager.QuartersId;
            viewModel.PromotionDate = manager.PromotionDate;
            viewModel.BankAccount = manager.BankAccount;
            viewModel.BankName = manager.BankName;
            viewModel.BankNum = manager.BankNum;
            viewModel.IsInsurance = manager.IsInsurance;
            viewModel.Roles = string.Join(",", manager.Roles.Select(r => r.Id));
            viewModel.OrganizationIds = manager.Organizations.Count > 0
                ? string.Join(",", manager.Organizations.Select(r => r.Id))
                : "";
            viewModel.ActionIds = manager.ManagerActions.Count > 0 ? SetActionIds(manager.ManagerActions) : "";
            return View(viewModel);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Update(ManagerView viewModel)
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError("message", "数据校验失败，请核对输入的信息是否准确");
                InitViewModel(viewModel);
                return View(viewModel);
            }
            //校验唯一性
            var temp = _managerRepository
                .LoadEntities(d => d.UserName.Equals(viewModel.UserName, StringComparison.CurrentCultureIgnoreCase) && d.IsDelete == false && d.Id != viewModel.Id)
                .FirstOrDefault();
            if (temp != null)
            {
                ModelState.AddModelError("message", "用户名：" + viewModel.UserName + "，已被占用！");
                InitViewModel(viewModel);
                return View(viewModel);
            }
            var organizationIds = string.IsNullOrWhiteSpace(viewModel.OrganizationIds)
                ? null
                : viewModel.OrganizationIds.Split(',').ToList();
            var actionIds = string.IsNullOrWhiteSpace(viewModel.ActionIds)
                ? null
                : viewModel.ActionIds.Split(',').ToList();

            var manager = _managerRepository.LoadEntities(d => d.Id == viewModel.Id).FirstOrDefault();
            manager.UserName = viewModel.UserName;
            manager.RealName = viewModel.RealName;
            manager.Phone = viewModel.Phone;
            manager.Status = viewModel.Status;
            manager.IdCard = viewModel.IdCard;
            manager.Birthday = viewModel.Birthday;
            manager.IsLunar = viewModel.IsLunar;
            manager.EntryDate = viewModel.EntryDate;
            manager.QuitDate = viewModel.QuitDate;
            manager.QuartersId = viewModel.QuartersId;
            manager.BankAccount = viewModel.BankAccount;
            manager.BankName = viewModel.BankName;
            manager.BankNum = viewModel.BankNum;
            manager.PromotionDate = viewModel.PromotionDate;
            manager.Password = Encrypt.Encode(viewModel.Password);
            manager.IsInsurance = viewModel.IsInsurance;
            manager.ModifiedById = CurrentManager.Id;
            manager.ModifiedBy = CurrentManager.UserName;
            manager.ModifiedDate = DateTime.Now;
            _managerService.AddOrUpdate(manager, false, viewModel.RoleIds, organizationIds, actionIds);
            TempData["Msg"] = "更新成功";
            return RedirectToAction("Index");
        }

        [HttpPost]
        [AdaValidateAntiForgeryToken]
        public ActionResult Export(ManagerView viewModel)
        {
            viewModel.Managers = PremissionData();
            var managers = _managerService.LoadEntitiesFilter(viewModel).ToList();
            var quarters = _quartersRepository.LoadEntities(d => d.IsDelete == false).ToList();
            var result = from m in managers
                         from q in quarters
                         where q.Id == m.QuartersId
                         select new
                         {
                             m.UserName,
                             m.Phone,
                             m.IdCard,
                             m.Birthday,
                             m.IsLunar,
                             m.BankName,
                             m.BankAccount,
                             m.BankNum,
                             m.EntryDate,
                             m.IsInsurance,
                             q.Title,
                             Group = GetOrganization(m)
                         };
            var enumerable = result.ToList();
            if (!enumerable.Any())
            {
                return Json(new { State = 0, Msg = "未找到相关的数据！" });
            }
            JArray jObjects = new JArray();
            foreach (var item in enumerable.ToList())
            {
                var jo = new JObject();
                jo.Add("姓名", item.UserName);
                jo.Add("所属部门", item.Group);
                jo.Add("岗位", item.Title);
                jo.Add("联系手机", item.Phone);
                jo.Add("身份证", item.IdCard);
                if (item.Birthday != null)
                {
                    var str = item.Birthday.Value.ToString("yyyy-MM-dd") + (item.IsLunar == true ? " (农历)" : "");
                    jo.Add("生日", str);
                }
                else
                {
                    jo.Add("生日", "");
                }
                jo.Add("入职日期", item.EntryDate == null ? "" : item.EntryDate.Value.ToString("yyyy-MM-dd"));
                if (viewModel.EntryDate != null)
                {
                    if (item.EntryDate != null)
                    {
                        var temp = item.EntryDate.Value.AddMonths(1);
                        var span = viewModel.EntryDate.Value - new DateTime(temp.Year,
                                       temp.Month, 1);
                        jo.Add("入职天数（截至" + viewModel.EntryDate.Value.ToString("yyyy-MM-dd") + "）", span.TotalDays);
                    }
                }
                jo.Add("五险一金", item.IsInsurance == true ? "是" : "否");
                jo.Add("开户行", item.BankName);
                jo.Add("开户名", item.BankAccount);
                jo.Add("开户号", item.BankNum);
                jObjects.Add(jo);
            }
            return Json(new { State = 1, Msg = ExportFile(jObjects.ToString()) });
        }
        [HttpPost]
        [AdaValidateAntiForgeryToken]
        public ActionResult Delete(string id)
        {
            if (id == CurrentManager.Id)
            {
                return Json(new { State = 0, Msg = "不能删除当前账号，如需删除，请登陆其他账户进行操作。" });
            }
            var manager = _managerRepository.LoadEntities(d => d.Id == id).FirstOrDefault();
            manager.DeletedById = CurrentManager.Id;
            manager.DeletedBy = CurrentManager.UserName;
            manager.DeletedDate = DateTime.Now;
            _managerService.Delete(manager);
            return Json(new { State = 1, Msg = "删除成功" });
        }
        [HttpPost]
        [AdaValidateAntiForgeryToken]
        public ActionResult UnBinding(string id)
        {
            _managerService.UnBinding(id);
            return Json(new { State = 1, Msg = "解绑成功" });
        }
        public ActionResult ChangePassword()
        {
            return PartialView("ChangePassword");
        }
        public ActionResult BaseInfo()
        {
            var manager = _managerRepository.LoadEntities(d => d.Id == CurrentManager.Id).FirstOrDefault();
            return PartialView("MangerInfo", manager);
        }
        [HttpPost]
        [AdaValidateAntiForgeryToken]
        public ActionResult ChangePassword(string old, string fresh, string refresh)
        {
            if (string.IsNullOrWhiteSpace(old) || string.IsNullOrWhiteSpace(fresh) || string.IsNullOrWhiteSpace(refresh))
            {
                return Json(new { State = 0, Msg = "请正确输入密码信息" });
            }
            if (fresh != refresh)
            {
                return Json(new { State = 0, Msg = "两次密码不一致" });
            }
            var oldpwd = Encrypt.Encode(old);
            var manager = _managerRepository.LoadEntities(d => d.Id == CurrentManager.Id && d.Password == oldpwd).FirstOrDefault();
            if (manager == null)
            {
                return Json(new { State = 0, Msg = "当前密码有误" });
            }
            manager.Password = Encrypt.Encode(fresh);
            _managerService.Edit(manager);
            return Json(new { State = 1, Msg = "修改成功" });
        }
        public ActionResult UploadImage(string id)
        {
            return PartialView("UploadImage");
        }
        [HttpPost]
        public ActionResult UploadImage()
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
            var uploadFileName = uploadConfig.Base64Filename;
            var base64Str = Request[uploadConfig.UploadFieldName];
            var uploadFileBytes = Convert.FromBase64String(base64Str.Replace("data:image/png;base64,", ""));
            var savePath = PathFormatter.Format(uploadFileName, uploadConfig.PathFormat);
            var localPath = Server.MapPath(savePath);
            try
            {
                if (!Directory.Exists(Path.GetDirectoryName(localPath)))
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(localPath));
                }
                System.IO.File.WriteAllBytes(localPath, uploadFileBytes);
                var imgPath = savePath;
                var manager = _managerRepository.LoadEntities(d => d.Id == CurrentManager.Id).FirstOrDefault();
                manager.Image = imgPath;
                _managerService.Edit(manager);
                return Json(new { State = 1, Msg = imgPath });

            }
            catch (Exception e)
            {
                return Json(new { State = 0, Msg = e.Message });
            }

        }
        /// <summary>
        /// 切换角色
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult ChangeRole(string id)
        {
            var role = CurrentManager.RoleList.FirstOrDefault(d => d.Id == id);
            CurrentManager.RoleName = role.RoleName;
            CurrentManager.RoleId = role.Id;
            if (Session["LoginManager"] == null)
            {
                return RedirectToAction("Index", "Login");
            }
            Session["LoginManager"] = SerializeHelper.SerializeToString(CurrentManager);
            return RedirectToAction("Index", "Home", new { area = "Dashboards" });
        }
        private string SetActionIds(IEnumerable<ManagerAction> list)
        {
            List<string> temp = new List<string>();
            foreach (var managerAction in list)
            {
                var ispass = managerAction.IsPass ? "true" : "false";
                temp.Add(managerAction.ActionInfoId + "^" + ispass);
            }
            return string.Join(",", temp);
        }
    }
}