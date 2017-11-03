using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ada.Core;
using Ada.Core.Domain.Admin;
using Action = System.Action;

namespace Ada.Services.Admin
{
    public class MenuService : IMenuService
    {
        private readonly IDbContext _dbContext;
        private readonly IRepository<Menu> _repository;
        public MenuService(IDbContext dbContext,
            IRepository<Menu> repository)
        {
            _dbContext = dbContext;
            _repository = repository;
        }
        public void Add(Menu entity)
        {
            var parent = _repository.LoadEntities(d => d.Id == entity.ParentId).FirstOrDefault();
            if (parent != null)
            {
                entity.Level = parent.Level + 1;
                entity.TreePath = parent.TreePath + entity.Id + "/";
                parent.IsLeaf = false;
            }
            else
            {
                entity.Level = 1;
                entity.TreePath = "/" + entity.Id + "/";
            }
            entity.IsLeaf = true;
            _repository.Add(entity);
            _dbContext.SaveChanges();
        }

        public void Update(Menu entity)
        {
            //先判断选中的父节点是否被包含
            if (IsContainNode(entity.Id, entity.ParentId)) return;
            //更新子节点
            var parent = _repository.LoadEntities(d => d.Id == entity.ParentId).FirstOrDefault();
            if (parent != null)
            {
                entity.TreePath = parent.TreePath + entity.Id + "/";
                entity.Level = parent.Level + 1;
            }
            else
            {
                entity.Level = 1;
                entity.TreePath = "/" + entity.Id + "/";

            }
            _repository.Update(entity);
            _dbContext.SaveChanges();
            //更新子节点
            UpdateChilds(entity.Id);
            //更新叶子
            var allEntities = _repository.LoadEntities(d => d.IsDelete==false).ToList();
            foreach (var item in allEntities)
            {
                var temp = _repository.LoadEntities(d => d.ParentId == item.Id).FirstOrDefault();
                item.IsLeaf = temp == null;
            }
            _dbContext.SaveChanges();
        }

        public void Delete(Menu entity)
        {
            _repository.Delete(entity);
            _dbContext.SaveChanges();
            //更新叶子
            var allEntities = _repository.LoadEntities(d => d.IsDelete == false).ToList();
            foreach (var item in allEntities)
            {
                var temp = _repository.LoadEntities(d => d.ParentId == item.Id).FirstOrDefault();
                item.IsLeaf = temp == null;
            }
            _dbContext.SaveChanges();
        }


        /// <summary>
        /// 验证节点是否被包含
        /// </summary>
        /// <param name="id">待查询的节点</param>
        /// <param name="parentId">父节点</param>
        /// <returns></returns>
        private bool IsContainNode(string id, string parentId)
        {
            var temp =
                _repository.LoadEntities(d => d.TreePath.Contains(id) && d.Id == parentId).ToList();
            return temp.Any();
        }
        /// <summary>
        /// 更新子节点
        /// </summary>
        /// <param name="parentId"></param>
        private void UpdateChilds(string parentId)
        {
            var parent = _repository.LoadEntities(d => d.Id == parentId).FirstOrDefault();
            if (parent != null)
            {
                var childs = _repository.LoadEntities(d => d.ParentId == parentId).ToList();
                foreach (var menuInfo in childs)
                {
                    menuInfo.TreePath = parent.TreePath + menuInfo.Id + "/";
                    menuInfo.Level = parent.Level + 1;
                    _dbContext.SaveChanges();
                    UpdateChilds(menuInfo.Id);
                }
            }
        }
    }
}
