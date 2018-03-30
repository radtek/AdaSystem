

using System;
using System.Text;
using System.Web;
using System.Web.Mvc;


namespace Ada.Web.Models
{
    public static class HtmlHelperExt
    {
        public static HtmlString ShowPageNavigate(this HtmlHelper htmlHelper, int totalCount, int currentPage, int pageSize = 10)
        {
            //var redirectTo = htmlHelper.ViewContext.RequestContext.HttpContext.Request.Url.AbsolutePath;

            var totalPages = Math.Max((totalCount + pageSize - 1) / pageSize, 1); //总页数

            var output = new StringBuilder();
            if (totalPages > 1)
            {

                if (currentPage > 1)
                {//处理上一页的连接
                    output.AppendFormat("<li class='list-inline-item float-sm-left'><a class='u-pagination-v1__item u-pagination-v1-2 u-pagination-v1-2--nav g-pa-12-21' href='javascript:nextPage({0});' aria-label='Previous'><span aria-hidden='true'><i class='fa fa-angle-left g-mr-5'></i>上一页</span><span class='sr-only'>上一页</span></a></li>", currentPage - 1);
                }
                int currint = 5;
                int pageBtn = 10;
                if (currentPage > (currint + 1))
                {
                    output.Append("<li class='list-inline-item g-hidden-sm-down'><a class='u-pagination-v1__item u-pagination-v1-2 g-brd-gray-light-v3 bilan g-brd-primary--hover g-pa-12-19' href='javascript:nextPage(1);'>1</a></li>");
                    output.Append("<li class='list-inline-item g-hidden-sm-down'><span class='g-brd-gray-light-v3 bilan g-brd-primary--hover g-pa-12-19'>...</span></li>");
                }


                for (int i = 0; i <= pageBtn; i++)
                {//一共最多显示10个页码，前面5个，后面5个
                    if ((currentPage + i - currint) >= 1 && (currentPage + i - currint) <= totalPages)
                    {
                        if (currint == i)
                        {//当前页处理 激活
                            //output.Append(string.Format("[{0}]", currentPage));
                            output.AppendFormat("<li class='list-inline-item g-hidden-sm-down'><a class='u-pagination-v1__item u-pagination-v1-2 u-pagination-v1-2--active g-brd-gray-light-v3 bilan g-brd-primary--hover g-pa-12-19' href='javascript:;'>{0}</a></li>", currentPage);
                        }
                        else
                        {//一般页处理
                            output.AppendFormat("<li class='list-inline-item g-hidden-sm-down'><a class='u-pagination-v1__item u-pagination-v1-2 g-brd-gray-light-v3 bilan g-brd-primary--hover g-pa-12-19' href='javascript:nextPage({0});'>{0}</a></li>", currentPage + i - currint);
                        }
                    }
                }
                if (currentPage != totalPages)
                {
                    output.Append("<li class='list-inline-item g-hidden-sm-down'><span class='g-brd-gray-light-v3 bilan g-brd-primary--hover g-pa-12-19'>...</span></li>");
                    output.AppendFormat("<li class='list-inline-item g-hidden-sm-down'><a class='u-pagination-v1__item u-pagination-v1-2 g-brd-gray-light-v3 bilan g-brd-primary--hover g-pa-12-19' href='javascript:nextPage({0});'>{0}</a></li>", totalPages);
                }
                if (currentPage < totalPages)
                {//处理下一页的链接
                    output.AppendFormat("<li class='list-inline-item float-sm-right'><a class='u-pagination-v1__item u-pagination-v1-2 u-pagination-v1-2--nav g-pa-12-21' href='javascript:nextPage({0});' aria-label='Next'><span aria-hidden='true'>下一页<i class='fa fa-angle-right g-ml-5'></i></span><span class='sr-only'>下一页</span></a></li>", currentPage + 1);
                }

            }
            //output.AppendFormat("第{0}页 / 共{1}页", currentPage, totalPages);//这个统计加不加都行

            return new HtmlString(output.ToString());
        }
    }
}