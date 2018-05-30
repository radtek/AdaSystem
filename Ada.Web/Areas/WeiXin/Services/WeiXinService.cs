using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using Ada.Core;
using Ada.Core.Domain;
using Ada.Core.Domain.Admin;
using Ada.Core.Domain.Resource;
using Ada.Core.Domain.WeiXin;
using Ada.Core.Tools;
using Ada.Framework.Messaging;
using WeiXin.Models;

namespace WeiXin.Services
{
    public class WeiXinService : IWeiXinService
    {
        private readonly IRepository<WeiXinAccount> _repository;
        private readonly IRepository<Media> _mediaRepository;
        private readonly IRepository<Manager> _managerRepository;
        private readonly IMessageService _messageService;
        public WeiXinService(IRepository<WeiXinAccount> repository,
            IRepository<Media> mediaRepository,
            IRepository<Manager> managerRepository,
            IMessageService messageService)
        {
            _repository = repository;
            _mediaRepository = mediaRepository;
            _managerRepository = managerRepository;
            _messageService = messageService;
        }

        public WeiXinAccount GetWeiXinAccount(string appIdOrAccountId = null)
        {
            if (string.IsNullOrWhiteSpace(appIdOrAccountId))
            {
                return _repository.LoadEntities(d => d.Status == true).FirstOrDefault();
            }
            return _repository.LoadEntities(d => d.Id == appIdOrAccountId || d.AppId == appIdOrAccountId || d.SourceId == appIdOrAccountId).FirstOrDefault();
        }
        public string PushMedia(string key, string appId, string openId)
        {
            if (!string.IsNullOrWhiteSpace(key))
            {
                if (appId == "wxcd1a304c25e0ea53")
                {
                    var isBinding = _managerRepository
                        .LoadEntities(d => d.OpenId == openId && d.Status == Consts.StateNormal).FirstOrDefault();
                    if (isBinding != null)
                    {
                        var medias = _mediaRepository.LoadEntities(d =>
                                (d.MediaName.Contains(key) || d.MediaID.Contains(key)) && d.IsDelete == false && d.Status == Consts.StateNormal).OrderBy(d => d.MediaType.Taxis).Take(2).ToList();
                        if (medias.Any())
                        {
                            foreach (var media in medias)
                            {
                                var domin = "http://www.jxweiguang.com";
                                var url = domin + "/Resource/Media/Detail/" + media.Id;
                                //联系人信息
                                string linkStr = "经办媒介：" + media.Transactor;
                                if (media.TransactorId == isBinding.Id)
                                {
                                    StringBuilder soldsb = new StringBuilder();
                                    soldsb.AppendLine("\r\n结算人：" + media.LinkMan.Name);

                                    if (!string.IsNullOrWhiteSpace(media.LinkMan.WeiXin))
                                    {
                                        soldsb.AppendLine("联系微信：" + media.LinkMan.WeiXin);
                                    }
                                    if (!string.IsNullOrWhiteSpace(media.LinkMan.QQ))
                                    {
                                        soldsb.AppendLine("联系QQ：" + media.LinkMan.QQ);
                                    }
                                    if (!string.IsNullOrWhiteSpace(media.LinkMan.Phone))
                                    {
                                        soldsb.AppendLine("联系电话：" + media.LinkMan.Phone);
                                    }

                                    linkStr = soldsb.ToString();
                                }
                                var dic = new Dictionary<string, object>
                                {
                                    {"Title", "【"+key+"】的查询结果如下：\r\n"},
                                    {"Remark", linkStr+"\r\n更多相关信息，请点击详情"},
                                    {"Url",domin+"/weixin/login/manager?returnUrl="+Uri.EscapeDataString(url)},
                                    {"AppId", appId},
                                    {"TemplateId", "X7y8s5C5CoNpKyFkYlsLosmNjY-U62cMj7QLRcNZn-c"},
                                    {"TemplateName", "查询结果通知"},
                                    {"OpenIds", openId},
                                    {"KeyWord1", media.MediaType.TypeName},
                                    {"KeyWord2", media.MediaName},
                                    {"KeyWord3", string.Join(",",media.MediaTags.Take(6).Select(d=>d.TagName))},
                                    {"KeyWord4", Utils.ShowFansNum(media.FansNum)+" 万"}

                                };
                                _messageService.Send("Push", dic);
                            }
                            return "正在查询，请等待。。。";
                        }

                    }

                }
            }
            //微广的公众号就做预处理推送


            return null;
        }
    }
}