using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ada.Core.Domain
{
    public class Consts
    {
        /// <summary>
        /// 状态开启|运行中 1
        /// </summary>
        public const short StateNormal = (short)StatusEnum.Normal;
        /// <summary>
        /// 状态关闭 0
        /// </summary>
        public const short StateLock = (short)StatusEnum.Locked;
        /// <summary>
        /// 错误|失败|待确认 -1
        /// </summary>
        public const short StateError = (short)StatusEnum.Error;
        /// <summary>
        /// 成功|完成  2
        /// </summary>
        public const short StateOK = (short)StatusEnum.Success;
        /// <summary>
        /// 成功|完成  2
        /// </summary>
        public const short StateFail = (short)StatusEnum.Fail;
        /// <summary>
        /// 订单确认申请 -1
        /// </summary>
        public const short AuditConfirmState = (short)AuditEnum.Confirm;
        /// <summary>
        /// 价格修改申请 -2
        /// </summary>
        public const short AuditPriceState = (short)AuditEnum.Price;
        /// <summary>
        /// 待处理
        /// </summary>
        public const short PurchaseStatusWait = (short)PurchaseStatusEnum.Wait;
        /// <summary>
        /// 处理
        /// </summary>
        public const short PurchaseStatusTodo = (short)PurchaseStatusEnum.Todo;
        /// <summary>
        /// 确认
        /// </summary>
        public const short PurchaseStatusConfirm = (short)PurchaseStatusEnum.Confirm;
        /// <summary>
        /// 成功
        /// </summary>
        public const short PurchaseStatusSuccess = (short)PurchaseStatusEnum.Success;
        /// <summary>
        /// 失败
        /// </summary>
        public const short PurchaseStatusFail = (short)PurchaseStatusEnum.Fail;
    }


}
