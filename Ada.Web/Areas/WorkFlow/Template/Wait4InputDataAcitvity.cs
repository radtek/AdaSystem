using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities;

namespace WorkFlow.Template
{

    public sealed class Wait4InputDataAcitvity<T> : NativeActivity
    {
        // 定义一个字符串类型的活动输入参数
        public InArgument<string> InBookmark { get; set; }
        public OutArgument<T> OutArgument { get; set; }
        protected override bool CanInduceIdle => true;

        // 如果活动返回值，则从 CodeActivity<TResult>
        // 并从 Execute 方法返回该值。
        protected override void Execute(NativeActivityContext context)
        {
            // 获取 Text 输入参数的运行时值
            string text = context.GetValue(this.InBookmark);
            context.CreateBookmark(text, CallBack);
        }
        /// <summary>
        /// 书签继续回调函数
        /// </summary>
        /// <param name="context"></param>
        /// <param name="bookmark"></param>
        /// <param name="value"></param>
        private void CallBack(NativeActivityContext context, Bookmark bookmark, object value)
        {
            context.SetValue(OutArgument,(T)value);
        }
    }
}
