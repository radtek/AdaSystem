using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ada.Core;
using Ada.Core.Domain.Admin;
using Action = Ada.Core.Domain.Admin.Action;

namespace Ada.Services.Admin
{
   public class ActionService:IActionService
    {
        private readonly IDbContext _dbContext;
        private readonly IRepository<Action> _repository;
        public ActionService(IDbContext dbContext,
            IRepository<Action> repository)
        {
            _dbContext = dbContext;
            _repository = repository;
        }
        public void Add(Action action,bool isCurd=false)
        {
            if (isCurd)
            {
                var addpage = new Action
                {
                    ActionName = "新增页面",
                    Area = action.Area,
                    ControllerName = action.ControllerName,
                    HttpMethod = "GET",
                    MethodName = "Add",
                    Taxis = 1,
                    ParentId = action.Id,
                    Id = IdBuilder.CreateIdNum(),
                    AddedBy = action.AddedBy,
                    AddedById = action.AddedById,
                    AddedDate = DateTime.Now
                };
                var add = new Action
                {
                    ActionName = "新增数据",
                    Area = action.Area,
                    ControllerName = action.ControllerName,
                    HttpMethod = "POST",
                    MethodName = "Add",
                    Taxis = 2,
                    ParentId = action.Id,
                    IsButton = true,
                    Id = IdBuilder.CreateIdNum(),
                    AddedBy = action.AddedBy,
                    AddedById = action.AddedById,
                    AddedDate = DateTime.Now
                };
                var updatepage = new Action
                {
                    ActionName = "编辑页面",
                    Area = action.Area,
                    ControllerName = action.ControllerName,
                    HttpMethod = "GET",
                    MethodName = "Update",
                    Taxis = 3,
                    ParentId = action.Id,
                    //IsButton = true,
                    Id = IdBuilder.CreateIdNum(),
                    AddedBy = action.AddedBy,
                    AddedById = action.AddedById,
                    AddedDate = DateTime.Now
                };
                var update = new Action
                {
                    ActionName = "编辑数据",
                    Area = action.Area,
                    ControllerName = action.ControllerName,
                    HttpMethod = "Post",
                    MethodName = "Update",
                    Taxis = 4,
                    ParentId = action.Id,
                    IsButton = true,
                    Id = IdBuilder.CreateIdNum(),
                    AddedBy = action.AddedBy,
                    AddedById = action.AddedById,
                    AddedDate = DateTime.Now
                };
                var getlist = new Action
                {
                    ActionName = "显示数据",
                    Area = action.Area,
                    ControllerName = action.ControllerName,
                    HttpMethod = "GET",
                    MethodName = "GetList",
                    Taxis = 5,
                    ParentId = action.Id,
                    //IsButton = true,
                    Id = IdBuilder.CreateIdNum(),
                    AddedBy = action.AddedBy,
                    AddedById = action.AddedById,
                    AddedDate = DateTime.Now
                };
                var delete = new Action
                {
                    ActionName = "删除数据",
                    Area = action.Area,
                    ControllerName = action.ControllerName,
                    HttpMethod = "POST",
                    MethodName = "Delete",
                    Taxis = 6,
                    ParentId = action.Id,
                    IsButton = true,
                    Id = IdBuilder.CreateIdNum(),
                    AddedBy = action.AddedBy,
                    AddedById = action.AddedById,
                    AddedDate = DateTime.Now
                };
                _repository.Add(add);
                _repository.Add(addpage);
                _repository.Add(getlist);
                _repository.Add(update);
                _repository.Add(updatepage);
                _repository.Add(delete);
            }
            
            _repository.Add(action);
            _dbContext.SaveChanges();
        }

        public void Update(Action action)
        {
            _repository.Update(action);
            _dbContext.SaveChanges();
        }

        public void Delete(Action action)
        {
            _repository.Delete(action);
            _dbContext.SaveChanges();
        }
    }
}
