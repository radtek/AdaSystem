using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ada.Core;
using Ada.Core.Domain.Admin;
using Ada.Core.ViewModel;
using Ada.Core.ViewModel.Admin;

namespace Ada.Services.Admin
{
    public interface IManagerService: IDependency
    {
        /// <summary>
        /// 获取数据
        /// </summary>
        /// <param name="managerView"></param>
        /// <returns></returns>
        IQueryable<Manager> LoadEntitiesFilter(ManagerView managerView);
        /// <summary>
        /// 新增或更新
        /// </summary>
        /// <param name="entity">实体</param>
        /// <param name="isAdd">是否新增</param>
        /// <param name="roleIds">角色IDS</param>
        /// <param name="organizationIds">机构组织IDS</param>
        /// <param name="actionIds">功能权限IDS</param>
        void AddOrUpdate(Manager entity, bool isAdd, List<string> roleIds, List<string> organizationIds, List<string> actionIds);

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="entity"></param>
        void Delete(Manager entity);
        /// <summary>
        /// 修改
        /// </summary>
        /// <param name="entity"></param>
        void Edit(Manager entity);

        Manager GetMangerByOpenId(string openId);
        bool BindingOpenId(string loginName, string pwd, string openid, out string errmsg, string image = null);
        void UnBinding(string id);
        ManagerView BindingLogin(LoginModel loginModel);
        ManagerView Login(LoginModel loginModel);
        IEnumerable<ManagerView> GetByOrganizationName(string name);
    }
}
