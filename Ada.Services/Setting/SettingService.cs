using System;
using System.Linq;
using Ada.Core;
using Ada.Core.Domain.Customer;
using Ada.Core.ViewModel.Customer;
using Ada.Core.ViewModel.WorkLog;
using Newtonsoft.Json;

namespace Ada.Services.Setting
{
   public class SettingService : ISettingService
    {
        private readonly IDbContext _dbContext;
        private readonly IRepository<Core.Domain.Admin.Setting> _repository;
        public SettingService(IDbContext dbContext,
            IRepository<Core.Domain.Admin.Setting> repository)
        {
            _dbContext = dbContext;
            _repository = repository;
        }
        
        public void AddOrUpdate(Core.Domain.Admin.Setting entity)
        {
            var setting = _repository.LoadEntities(d => d.SettingName == entity.SettingName).FirstOrDefault();
            if (setting==null)
            {
                setting = new Core.Domain.Admin.Setting
                {
                    Id = IdBuilder.CreateIdNum(),
                    SettingName = entity.SettingName,
                    Content = entity.Content
                };
                _repository.Add(setting);
            }
            else
            {
                setting.Content = entity.Content;
                _repository.Update(setting);
            }
            _dbContext.SaveChanges();
        }
        public T GetSetting<T>() where T : class, new()
        {
            var setting= _repository.LoadEntities(d=>d.SettingName.Equals(typeof(T).Name, StringComparison.OrdinalIgnoreCase)).FirstOrDefault();
            return setting!=null ? JsonConvert.DeserializeObject<T>(setting.Content) : new T();
        }
        
    }
}
