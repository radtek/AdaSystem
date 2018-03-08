using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ada.Core;
using Ada.Core.ViewModel.API;
using Ada.Core.ViewModel.API.iDataAPI;

namespace Ada.Services.API
{
  public  interface IiDataAPIService: IDependency
  {
      RequestResult GetWeiXinArticles(WeiXinProParams wxparams);
      RequestResult GetWeiBoArticles(WeiBoParams wbparams);
      string TestApi(TestParams testParams);
      RequestResult GetWeinXinInfo(BaseParams baseParams);
      RequestResult GetWeiXinInfoPro(WeiXinProParams wxparams);
  }
}
