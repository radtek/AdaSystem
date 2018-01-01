using Ada.Core;


namespace Ada.Services.Setting
{
   public interface ISettingService : IDependency
   {
       void AddOrUpdate(Core.Domain.Admin.Setting entity);
       T GetSetting<T>() where T : class, new();
   }
}
