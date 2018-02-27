using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ada.Core.ViewModel.API.iDataAPI
{
    public interface IJsonResult
    {
        /// <summary>
        /// 返回结果信息
        /// </summary>
        string message { get; set; }

        /// <summary>
        /// errcode的
        /// </summary>
        int CodeValue { get; }
    }
    [Serializable]
    public abstract class BaseJsonResult : IJsonResult
    {
        /// <summary>
        /// 返回结果信息
        /// </summary>
        public virtual string message { get; set; }

        /// <summary>
        /// errcode的
        /// </summary>
        public abstract int CodeValue { get; }
    }
    [Serializable]
    public class iDataJsonResult : BaseJsonResult
    {

        public ReturnCode retcode { get; set; }
        /// <summary>
        /// 接口别名
        /// </summary>
        public string appCode { get; set; }
        /// <summary>
        /// 请求方式
        /// </summary>
        public string dataType { get; set; }
        /// <summary>
        /// 下一页码
        /// </summary>
        public string pageToken { get; set; }
        /// <summary>
        /// 是否有下一页
        /// </summary>
        public bool? hasNext { get; set; }
        /// <summary>
        /// 返回消息代码数字（同errcode枚举值）
        /// </summary>
        public override int CodeValue => (int)retcode;

        public override string ToString()
        {
            return string.Format("iDataAPI结果：{{返回码:'{0}',错误中文信息:'{1}',错误信息:'{2}'}}",
                (int)retcode, retcode.ToString(), message);
        }


    }

    public enum ReturnCode
    {
        请求成功 = 000000,
        服务器内部错误 = 100000,
        网络错误 = 100001,
        目标参数搜索没结果 = 100002,
        目标服务器错误 = 100004,
        用户输入参数错误 = 100005,
        授权失败 = 100700,
        您的当前API已停用 = 100701,
        您的账户已停用 = 100702,
        并发已达上限 = 100703,
        API维护中 = 100704,
        API不存在 = 100705,
        请先添加api = 100706,
        调用次数超限 = 100707,
        请求路径错误或者缺少time参数 = 100802,
        参数pageToken有误 = 100803,
    }
}
