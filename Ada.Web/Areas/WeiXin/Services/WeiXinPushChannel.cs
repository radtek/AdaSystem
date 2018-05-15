using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using log4net;
using Senparc.Weixin;
using Senparc.Weixin.Entities.TemplateMessage;
using Senparc.Weixin.MP.AdvancedAPIs;
using Senparc.Weixin.MP.AdvancedAPIs.TemplateMessage;
using WeiXin.Models;

namespace WeiXin.Services
{
    public class WeiXinPushChannel : IWeiXinPushChannel
    {
        public ILog Log { get; set; }
        //private readonly ILog _logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        /// <summary>
        /// 处理模板消息
        /// </summary>
        /// <param name="parameters"></param>
        public void Process(IDictionary<string, object> parameters)
        {
            Task.Factory.StartNew(async () =>
            {
                var templateMsgModel = new TemplateMsgModel();
                templateMsgModel.Url = Read(parameters, "Url");
                templateMsgModel.Title = Read(parameters, "Title");
                templateMsgModel.Remark = Read(parameters, "Remark");
                templateMsgModel.AppId = Read(parameters, "AppId");
                templateMsgModel.TemplateId = Read(parameters, "TemplateId");
                templateMsgModel.TemplateName = Read(parameters, "TemplateName");
                templateMsgModel.OpenIds = Read(parameters, "OpenIds");
                foreach (var parameter in parameters)
                {
                    if (!parameter.Key.StartsWith("KeyWord")) continue;
                    if (parameter.Value != null)
                    {
                        templateMsgModel.KeyWords.Add(parameter.Key, parameter.Value.ToString());
                    }
                }
                var openids = templateMsgModel.OpenIds.Split(',').ToList();
                var sendData = new WeiXinTemplate(templateMsgModel.Title, templateMsgModel.Remark,
                    templateMsgModel.TemplateId, templateMsgModel.Url, templateMsgModel.TemplateName);
                foreach (var keyWord in templateMsgModel.KeyWords)
                {
                    if (keyWord.Key.Contains("1"))
                    {
                        sendData.keyword1 = new TemplateDataItem(keyWord.Value);
                    }
                    if (keyWord.Key.Contains("2"))
                    {
                        sendData.keyword2 = new TemplateDataItem(keyWord.Value);
                    }
                    if (keyWord.Key.Contains("3"))
                    {
                        sendData.keyword3 = new TemplateDataItem(keyWord.Value);
                    }
                    if (keyWord.Key.Contains("4"))
                    {
                        sendData.keyword4 = new TemplateDataItem(keyWord.Value);
                    }
                    if (keyWord.Key.Contains("5"))
                    {
                        sendData.keyword5 = new TemplateDataItem(keyWord.Value);
                    }
                }
                foreach (var openid in openids)
                {
                    var result = await TemplateApi.SendTemplateMessageAsync(templateMsgModel.AppId, openid, sendData);
                    if (result.errcode!=ReturnCode.请求成功)
                    {
                        Log.Debug(openid + "发送模板 [" + templateMsgModel.TemplateName + "] 消息失败：" + result.errmsg);
                    }
                }

            });


        }

        private string Read(IDictionary<string, object> dictionary, string key)
        {
            return dictionary.ContainsKey(key) ? dictionary[key] as string : null;
        }


    }

    public class WeiXinTemplate : TemplateMessageBase
    {
        public WeiXinTemplate(string title, string note, string templateId, string url, string templateName) : base(templateId, url, templateName)
        {
            first = new TemplateDataItem(title);
            remark = new TemplateDataItem(note);
        }
        public TemplateDataItem first { get; set; }
        /// <summary>
        /// Time
        /// </summary>
        public TemplateDataItem keyword1 { get; set; }
        /// <summary>
        /// Host
        /// </summary>
        public TemplateDataItem keyword2 { get; set; }
        /// <summary>
        /// Service
        /// </summary>
        public TemplateDataItem keyword3 { get; set; }
        /// <summary>
        /// Status
        /// </summary>
        public TemplateDataItem keyword4 { get; set; }
        /// <summary>
        /// Message
        /// </summary>
        public TemplateDataItem keyword5 { get; set; }

        public TemplateDataItem remark { get; set; }
    }
}