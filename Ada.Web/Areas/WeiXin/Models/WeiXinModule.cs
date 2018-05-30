using System.Linq;
using Ada.Core;
using Ada.Core.Domain.WeiXin;
using Ada.Core.Infrastructure;
using Senparc.Weixin.Context;
using Senparc.Weixin.MP;
using Senparc.Weixin.MP.Entities;
using Senparc.Weixin.MP.MessageHandlers;
using Senparc.Weixin.RegisterServices;

namespace WeiXin.Models
{
    public class WeiXinModule : IAppStart
    {
        private readonly IRepository<WeiXinAccount> _repository;

        public WeiXinModule()
        {
            _repository = EngineContext.Current.Resolve<IRepository<WeiXinAccount>>();
        }
        public void Register()
        {
            var accounts = _repository.LoadEntities(d => d.IsDelete == false && (d.AccountType == 0 || d.AccountType == 1 || d.AccountType == 2)).ToList();
            var register = RegisterService.Start();
            //var register = RegisterService.Start().ChangeDefaultCacheNamespace("DefaultWeixinCache").RegisterCacheRedis(
            //    ConfigurationManager.AppSettings["RedisConnectionString"],
            //    redisConfiguration => (!string.IsNullOrEmpty(redisConfiguration) && redisConfiguration != "Redis配置")
            //        ? RedisObjectCacheStrategy.Instance
            //        : null);
            foreach (var weiXinAccount in accounts)
            {
                register.RegisterMpAccount(weiXinAccount.AppId, weiXinAccount.AppSecret, weiXinAccount.Name);
            }
            MessageHandler<MessageContext<IRequestMessageBase, IResponseMessageBase>>.GlobalWeixinContext.ExpireMinutes = 3;
        }
    }
}