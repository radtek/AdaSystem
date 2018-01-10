using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Ada.Core.Tools
{
    public class FastPropertyComparer<T> : IEqualityComparer<T>
    {
        private readonly Func<T, Object> _getPropertyValueFunc;

        /// <summary>
        /// 通过propertyName 获取PropertyInfo对象
        /// </summary>
        /// <param name="propertyName"></param>
        public FastPropertyComparer(string propertyName)
        {
            PropertyInfo propertyInfo = typeof(T).GetProperty(propertyName,
                BindingFlags.GetProperty | BindingFlags.Instance | BindingFlags.Public);
            if (propertyInfo == null)
            {
                throw new ArgumentException($"{propertyName} is not a property of type {typeof(T)}.");
            }

            ParameterExpression expPara = Expression.Parameter(typeof(T), "obj");
            MemberExpression me = Expression.Property(expPara, propertyInfo);
            _getPropertyValueFunc = Expression.Lambda<Func<T, object>>(me, expPara).Compile();
        }

        #region IEqualityComparer<T> Members

        public bool Equals(T x, T y)
        {
            var xValue = _getPropertyValueFunc(x);
            var yValue = _getPropertyValueFunc(y);

            if (xValue == null)
                return yValue == null;

            return xValue.Equals(yValue);
        }

        public int GetHashCode(T obj)
        {
            var propertyValue = _getPropertyValueFunc(obj);

            return propertyValue == null ? 0 : propertyValue.GetHashCode();
        }

        #endregion
    }
}
