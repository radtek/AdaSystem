using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Ada.Core;
using Ada.Core.Domain.WeiXin;
using WeiXin.Models;

namespace WeiXin.Services
{
    public class WeiXinService : IWeiXinService
    {
        private readonly IRepository<WeiXinAccount> _repository;
        public WeiXinService(IRepository<WeiXinAccount> repository)
        {
            _repository = repository;
        }

        public WeiXinAccount GetWeiXinAccount(string appIdOrAccountId = null)
        {
            if (string.IsNullOrWhiteSpace(appIdOrAccountId))
            {
                return _repository.LoadEntities(d => d.Status == true).FirstOrDefault();
            }
            return _repository.LoadEntities(d => d.Id == appIdOrAccountId || d.AppId == appIdOrAccountId).FirstOrDefault();
        }

    }
}