using System.Linq;
using Ada.Core;
using Ada.Core.Domain.Content;

namespace Ada.Services.Content
{
   public class ColumnService : IColumnService
    {
        private readonly IDbContext _dbContext;
        private readonly IRepository<Column> _repository;
        public ColumnService(IDbContext dbContext,
            IRepository<Column> repository)
        {
            _dbContext = dbContext;
            _repository = repository;
        }
        public void Add(Column entity)
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

        public void Update(Column entity)
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
            var allEntities = _repository.LoadEntities(d => d.IsDelete == false).ToList();
            foreach (var item in allEntities)
            {
                var temp = _repository.LoadEntities(d => d.ParentId == item.Id).FirstOrDefault();
                item.IsLeaf = temp == null;
            }
            _dbContext.SaveChanges();
        }

        public void Delete(Column entity)
        {
            _repository.Remove(entity);
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
