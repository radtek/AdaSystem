using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Ada.Core.Infrastructure;
using Ada.Services.Setting;
using Aliyun.Acs.Core;
using Aliyun.Acs.Core.Profile;
using Aliyun.Acs.Dysmsapi.Model.V20170525;
using log4net;

namespace Message.Models
{
    public class AliYunSmsChannel : IAliYunSmsChannel
    {
        private readonly ISettingService _settingService;
        private readonly ILog _logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public AliYunSmsChannel()
        {
            _settingService = EngineContext.Current.Resolve<ISettingService>();
        }
        public void Process(IDictionary<string, object> parameters)
        {
            var aliYunMessage = new AliYunMessage
            {
                PhoneNumbers = Read(parameters, "PhoneNumbers"),
                TemplateCode = Read(parameters, "TemplateCode"),
                TemplateParam = Read(parameters, "TemplateParam"),
                OutId = Read(parameters, "OutId")
            };
            var aliYunSet = _settingService.GetSetting<AliYunSet>();
            IClientProfile profile = DefaultProfile.GetProfile(aliYunSet.Area, aliYunSet.AccessKey, aliYunSet.AccessKeySecret);
            DefaultProfile.AddEndpoint(aliYunSet.Area, aliYunSet.Area, aliYunSet.Product, aliYunSet.Domain);
            IAcsClient acsClient = new DefaultAcsClient(profile);
            SendSmsRequest request = new SendSmsRequest();
            SendSmsResponse response = null;
            //必填:待发送手机号。支持以逗号分隔的形式进行批量调用，批量上限为1000个手机号码,批量调用相对于单条调用及时性稍有延迟,验证码类型的短信推荐使用单条调用的方式
            request.PhoneNumbers = aliYunMessage.PhoneNumbers;
            //必填:短信签名-可在短信控制台中找到
            request.SignName = aliYunSet.SignName;
            //必填:短信模板-可在短信控制台中找到
            request.TemplateCode = aliYunMessage.TemplateCode;
            if (!string.IsNullOrWhiteSpace(aliYunMessage.TemplateParam))
            {
                //可选:模板中的变量替换JSON串,如模板内容为"亲爱的${name},您的验证码为${code}"时,此处的值为
                request.TemplateParam = aliYunMessage.TemplateParam;
            }
            if (!string.IsNullOrWhiteSpace(aliYunMessage.TemplateParam))
            {
                //可选:outId为提供给业务方扩展字段,最终在短信回执消息中将此值带回给调用者
                request.OutId = aliYunMessage.OutId;
            }
            //请求失败这里会抛ClientException异常
            response = acsClient.GetAcsResponse(request);
            if (response.Code != "OK")
            {
                _logger.Error("阿里云短信发送失败["+ aliYunMessage.PhoneNumbers + "]，错误码：" + response.Code+"，错误描述："+response.Message);
            }
        }
        private string Read(IDictionary<string, object> dictionary, string key)
        {
            return dictionary.ContainsKey(key) ? dictionary[key] as string : null;
        }
    }
}