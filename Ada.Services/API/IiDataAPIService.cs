using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ada.Core;
using Ada.Core.ViewModel.API.iDataAPI;

namespace Ada.Services.API
{
  public  interface IiDataAPIService: IDependency
  {
      string GetWeiXinArticles(WeiXinProParams wxparams);
      string GetWeiBoArticles(WeiBoParams wbparams);
  }
}
