using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ada.Core;
using Ada.Core.Domain.Admin;
using Ada.Core.Tools;
using Ada.Core.ViewModel;
using Ada.Core.ViewModel.Admin;
using Ada.Framework.Caching;
using Ada.Framework.Filter;
using Ada.Framework.UploadFile;
using Ada.Services.Admin;
using Action = Ada.Core.Domain.Admin.Action;

namespace Admin.Controllers
{
    public class ManagerController : BaseController
    {
        private readonly IRepository<Action> _actionRepository;
        private readonly IRepository<Role> _roleRepository;
        private readonly IRepository<Manager> _managerRepository;
        private readonly IManagerService _managerService;
        private readonly IRepository<Organization> _organizationRepository;

        public ManagerController(IManagerService managerService,
            IRepository<Manager> managerRepository,
            IRepository<Role> roleRepository,
            IRepository<Action> actionRepository,
            IRepository<Organization> organizationRepository)
        {
            _actionRepository = actionRepository;
            _roleRepository = roleRepository;
            _managerRepository = managerRepository;
            _managerService = managerService;
            _organizationRepository = organizationRepository;
        }
        public ActionResult Index()
        {
            var roles = _roleRepository.LoadEntities(d => d.IsDelete == false).ToList();
            ViewBag.Roles = roles.Select(d => new RoleView() { Id = d.Id, RoleName = d.RoleName });
            var organizations = _organizationRepository.LoadEntities(d => d.IsDelete == false).OrderBy(d => d.Taxis)
                .ToList();
            ViewBag.Organizations = GetTree(null, organizations);
            var actions = _actionRepository.LoadEntities(d => d.IsDelete == false).OrderBy(d => d.Taxis).ToList();
            ViewBag.Actions = GetTree(null, actions);
            return View();
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
        public ActionResult GetEntity(string id)
        {
            var d = _managerRepository.LoadEntities(m => m.Id == id).FirstOrDefault();
            return Json(new ManagerView
            {
                Id = d.Id,
                UserName = d.UserName,
                Password = Encrypt.Decode(d.Password),
                Phone = d.Phone,
                Status = d.Status,
                RealName = d.RealName,
                AddDate = d.AddedDate?.ToString("yyyy年MM月dd日") ?? "",
                RoleIds = d.Roles.Count > 0 ? d.Roles.Select(r => r.Id).ToList() : null,
                OrganizationIds = d.Organizations.Count > 0 ? string.Join(",", d.Organizations.Select(r => r.Id)) : null,
                ActionIds = d.ManagerActions.Count > 0 ? SetActionIds(d.ManagerActions) : null
            }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetList(ManagerView viewModel)
        {
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
                    Status = d.Status,
                    RealName = d.RealName,
                    AddDate = d.AddedDate?.ToString("yyyy年MM月dd日") ?? "",
                    Roles = d.Roles.Count > 0 ? string.Join(",", d.Roles.Select(r => r.RoleName)) : "",
                    Organizations = d.Organizations.Count > 0 ? string.Join(" → ", d.Organizations.Select(r => r.OrganizationName)) : "",
                    LastLoginDate = d.ManagerLoginLogs.OrderByDescending(l => l.LoginTime).FirstOrDefault() == null ? "" : Utils.ToRead(d.ManagerLoginLogs.OrderByDescending(l => l.LoginTime).FirstOrDefault().LoginTime)
                })
            }, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        [AdaValidateAntiForgeryToken]
        public ActionResult AddOrUpdate(ManagerView viewModel)
        {
            if (!ModelState.IsValid)
            {
                return Json(new { State = 0, Msg = "请核对输入的用户信息是否正确" });
            }
            var msg = string.Empty;
            var organizationIds = string.IsNullOrWhiteSpace(viewModel.OrganizationIds)
                ? null
                : viewModel.OrganizationIds.Split(',').ToList();
            var actionIds = string.IsNullOrWhiteSpace(viewModel.ActionIds)
                ? null
                : viewModel.ActionIds.Split(',').ToList();
            if (!string.IsNullOrWhiteSpace(viewModel.Id))
            {
                //校验唯一性
                var temp = _managerRepository
                    .LoadEntities(d => d.UserName.Equals(viewModel.UserName, StringComparison.CurrentCultureIgnoreCase) && d.IsDelete == false && d.Id != viewModel.Id)
                    .FirstOrDefault();
                if (temp != null)
                {
                    return Json(new { State = 0, Msg = "用户名：" + viewModel.UserName + "，已被占用！" });
                }
                var manager = _managerRepository.LoadEntities(d => d.Id == viewModel.Id).FirstOrDefault();
                manager.UserName = viewModel.UserName;
                manager.RealName = viewModel.RealName;
                manager.Phone = viewModel.Phone;
                manager.Status = viewModel.Status;
                manager.Password = Encrypt.Encode(viewModel.Password);
                manager.ModifiedById = CurrentManager.Id;
                manager.ModifiedBy = CurrentManager.UserName;
                manager.ModifiedDate = DateTime.Now;
                _managerService.AddOrUpdate(manager, false, viewModel.RoleIds, organizationIds, actionIds);
                msg = "更新成功";
               
            }
            else
            {
                //校验唯一性
                var temp = _managerRepository
                      .LoadEntities(d => d.UserName.Equals(viewModel.UserName, StringComparison.CurrentCultureIgnoreCase) && d.IsDelete == false)
                      .FirstOrDefault();
                if (temp != null)
                {
                    return Json(new { State = 0, Msg = "用户名：" + viewModel.UserName + "，已被占用！" });
                }
                var manager = new Manager()
                {
                    UserName = viewModel.UserName,
                    RealName = viewModel.RealName,
                    Phone = viewModel.Phone,
                    Status = viewModel.Status,
                    Password = Encrypt.Encode(viewModel.Password),
                    Id = IdBuilder.CreateIdNum(),
                    AddedBy = CurrentManager.UserName,
                    AddedById = CurrentManager.Id,
                    AddedDate = DateTime.Now
                };
                _managerService.AddOrUpdate(manager, true, viewModel.RoleIds, organizationIds, actionIds);
                msg = "添加成功";
            }

            return Json(new { State = 1, Msg = msg });
        }
        [HttpPost]
        [AdaValidateAntiForgeryToken]
        public ActionResult Delete()
        {
            var ids = Request["Ids"].Split(',');
            List<Manager> list = new List<Manager>();
            foreach (var id in ids)
            {
                if (id == CurrentManager.Id)
                {
                    return Json(new { State = 0, Msg = "不能删除当前账号，如需删除，请登陆其他账户进行操作。" });
                }
                var manager = _managerRepository.LoadEntities(d => d.Id == id).FirstOrDefault();
                manager.DeletedById = CurrentManager.Id;
                manager.DeletedBy = CurrentManager.UserName;
                manager.DeletedDate = DateTime.Now;
                list.Add(manager);
            }
            _managerService.Delete(list);
            return Json(new { State = 1, Msg = "删除成功" });
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