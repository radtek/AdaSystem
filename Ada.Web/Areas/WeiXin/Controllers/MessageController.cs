﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Ada.Core;
using Ada.Core.Domain;
using Ada.Core.Domain.Admin;
using Ada.Core.Domain.WeiXin;
using Ada.Framework.Messaging;
using log4net;
using Senparc.Weixin.MP;
using Senparc.Weixin.MP.AdvancedAPIs;
using Senparc.Weixin.MP.AdvancedAPIs.TemplateMessage;
using Senparc.Weixin.MP.Entities.Request;
using Senparc.Weixin.MP.MvcExtension;
using WeiXin.Models;
using WeiXin.Services;
using WeiXin.Services.MessageHandlers;

namespace WeiXin.Controllers
{
    public class MessageController : Controller
    {
        private readonly IRepository<WeiXinAccount> _repository;
        
        public ILog Log { get; set; }
        public MessageController(IRepository<WeiXinAccount> repository)
        {
            _repository = repository;
        }
        [HttpGet]
        [ActionName("Index")]
        public ActionResult Get(string id, PostModel postModel, string echostr)
        {
            var account = _repository.LoadEntities(d => d.Id == id).FirstOrDefault();
            if (account == null)
            {
                return Content("非法请求");
            }
            if (CheckSignature.Check(postModel.Signature, postModel.Timestamp, postModel.Nonce, account.Token))
            {
                return Content(echostr); //返回随机字符串则表示验证通过
            }

            return Content("failed:" + postModel.Signature + "," + CheckSignature.GetSignature(postModel.Timestamp, postModel.Nonce, account.Token) + "。" +
                           "如果你在浏览器中看到这句话，说明此地址可以被作为微信公众账号后台的Url，请注意保持Token一致。");
        }

        /// <summary>
        /// 用户发送消息后，微信平台自动Post一个请求到这里，并等待响应XML。
        /// PS：此方法为简化方法，效果与OldPost一致。
        /// v0.8之后的版本可以结合Senparc.Weixin.MP.MvcExtension扩展包，使用WeixinResult，见MiniPost方法。
        /// </summary>
        [HttpPost]
        [ActionName("Index")]
        public ActionResult Post(string id, PostModel postModel)
        {
            var account = _repository.LoadEntities(d => d.Id == id).FirstOrDefault();
            if (account == null)
            {
                return Content("非法请求");
            }
            if (!CheckSignature.Check(postModel.Signature, postModel.Timestamp, postModel.Nonce, account.Token))
            {
                return Content("参数错误！");
            }
            postModel.Token = account.Token;//根据自己后台的设置保持一致
            postModel.EncodingAESKey = account.EncodingAESKey;//根据自己后台的设置保持一致
            postModel.AppId = account.AppId;//根据自己后台的设置保持一致
            //v4.2.2之后的版本，可以设置每个人上下文消息储存的最大数量，防止内存占用过多，如果该参数小于等于0，则不限制
            var maxRecordCount = 10;

            //自定义MessageHandler，对微信请求的详细判断操作都在这里面。
            var messageHandler = new CustomMessageHandler(Request.InputStream, postModel, maxRecordCount);

            #region 设置消息去重

            /* 如果需要添加消息去重功能，只需打开OmitRepeatedMessage功能，SDK会自动处理。
             * 收到重复消息通常是因为微信服务器没有及时收到响应，会持续发送2-5条不等的相同内容的RequestMessage*/
            messageHandler.OmitRepeatedMessage = true;//默认已经开启，此处仅作为演示，也可以设置为false在本次请求中停用此功能

            #endregion

            try
            {

                #region 记录 Request 日志

                //var logPath = Server.MapPath(string.Format("~/App_Data/MP/{0}/", DateTime.Now.ToString("yyyy-MM-dd")));
                //if (!Directory.Exists(logPath))
                //{
                //    Directory.CreateDirectory(logPath);
                //}

                ////测试时可开启此记录，帮助跟踪数据，使用前请确保App_Data文件夹存在，且有读写权限。
                //messageHandler.RequestDocument.Save(Path.Combine(logPath, string.Format("{0}_Request_{1}_{2}.txt", _getRandomFileName(),
                //    messageHandler.RequestMessage.FromUserName,
                //    messageHandler.RequestMessage.MsgType)));
                //if (messageHandler.UsingEcryptMessage)
                //{
                //    messageHandler.EcryptRequestDocument.Save(Path.Combine(logPath, string.Format("{0}_Request_Ecrypt_{1}_{2}.txt", _getRandomFileName(),
                //        messageHandler.RequestMessage.FromUserName,
                //        messageHandler.RequestMessage.MsgType)));
                //}

                #endregion

                //执行微信处理过程
                messageHandler.Execute();

                #region 记录 Response 日志

                //测试时可开启，帮助跟踪数据

                //if (messageHandler.ResponseDocument == null)
                //{
                //    throw new Exception(messageHandler.RequestDocument.ToString());
                //}
                //if (messageHandler.ResponseDocument != null)
                //{
                //    messageHandler.ResponseDocument.Save(Path.Combine(logPath, string.Format("{0}_Response_{1}_{2}.txt", _getRandomFileName(),
                //        messageHandler.ResponseMessage.ToUserName,
                //        messageHandler.ResponseMessage.MsgType)));
                //}

                //if (messageHandler.UsingEcryptMessage && messageHandler.FinalResponseDocument != null)
                //{
                //    //记录加密后的响应信息
                //    messageHandler.FinalResponseDocument.Save(Path.Combine(logPath, string.Format("{0}_Response_Final_{1}_{2}.txt", _getRandomFileName(),
                //        messageHandler.ResponseMessage.ToUserName,
                //        messageHandler.ResponseMessage.MsgType)));
                //}

                #endregion
                return new FixWeixinBugWeixinResult(messageHandler);//为了解决官方微信5.0软件换行bug暂时添加的方法，平时用下面一个方法即可
            }
            catch (Exception ex)
            {
                #region 异常处理
                Log.Error("MessageHandler异常", ex);
                return Content("");
                #endregion
            }
        }

        
    }
}