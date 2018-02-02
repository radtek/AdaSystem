using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ada.Core;
using Ada.Core.Domain.API;
using Ada.Core.Domain.Resource;
using Ada.Core.Tools;
using Ada.Core.ViewModel.API.iDataAPI;
using Newtonsoft.Json;

namespace Ada.Services.API
{
    public class iDataAPIService : IiDataAPIService
    {
        private readonly IDbContext _dbContext;
        private readonly IAPIInterfacesService _apiInterfacesService;
        private readonly IRepository<Media> _mediaRepository;
        public iDataAPIService(IAPIInterfacesService apiInterfacesService, IDbContext dbContext, IRepository<Media> mediaRepository)
        {
            _apiInterfacesService = apiInterfacesService;
            _dbContext = dbContext;
            _mediaRepository = mediaRepository;
        }

        public async Task<string> GetArticlesAsync(WeiXinProParams wxparams)
        {
            var apiInfo = _apiInterfacesService.GetAPIInterfacesByCallIndex(wxparams.CallIndex);
            string url = string.Format(apiInfo.APIUrl + "?apikey={0}&uid={1}", apiInfo.Token, wxparams.WeiXinId);
            if (!string.IsNullOrWhiteSpace(wxparams.Range))
            {
                url += "&range=" + wxparams.Range;
            }
            int page = wxparams.PageNum ?? 1;
            int addCount = 0;
            int updateCount = 0;
            for (int i = 1; i <= page; i++)
            {
                var apiUrl = url + "&pageToken=" + i;
                var jsonStr = await HttpUtility.GetAsync(apiUrl);
                var result = JsonConvert.DeserializeObject<WeiXinProJSON>(jsonStr);
                if (result.hasNext == false)
                {
                    break;
                }
                //增加请求记录
                APIRequestRecord record = new APIRequestRecord();
                record.Id = IdBuilder.CreateIdNum();
                record.IsSuccess = result.retcode == ReturnCode.请求成功;
                //record.ReponseContent = ;
                record.RequestParameters = apiUrl;
                record.Retcode = result.retcode.GetHashCode().ToString();
                record.Retmsg = result.message;
                record.ReponseDate = DateTime.Now;
                apiInfo.APIRequestRecords.Add(record);
                if (result.retcode != ReturnCode.请求成功)
                    continue;
                if (result.data.Count > 0)
                {
                    var media = _mediaRepository.LoadEntities(d => d.MediaID == wxparams.WeiXinId).FirstOrDefault();
                    foreach (var articleData in result.data)
                    {
                        var article = media.MediaArticles.FirstOrDefault(d => d.ArticleId == articleData.id);
                        if (article != null)
                        {
                            article.ArticleIdx = articleData.idx;
                            article.ArticleUrl = articleData.url;
                            article.IsOriginal = articleData.original;
                            article.OriginUrl = articleData.originUrl;
                            article.Biz = articleData.biz;
                            article.CommentCount = articleData.commentCount;
                            article.Content = articleData.content;
                            article.IsTop = articleData.isTop;
                            article.PublishDate = string.IsNullOrWhiteSpace(articleData.publishDateStr)
                                ? (DateTime?)null
                                : DateTime.Parse(articleData.publishDateStr);
                            article.LikeCount = articleData.likeCount;
                            article.ViewCount = articleData.viewCount;
                            article.Title = articleData.title;
                            updateCount++;
                        }
                        else
                        {
                            article = new MediaArticle();
                            article.Id = IdBuilder.CreateIdNum();
                            article.ArticleId = articleData.id;
                            article.ArticleIdx = articleData.idx;
                            article.ArticleUrl = articleData.url;
                            article.IsOriginal = articleData.original;
                            article.OriginUrl = articleData.originUrl;
                            article.Biz = articleData.biz;
                            article.CommentCount = articleData.commentCount;
                            article.Content = articleData.content;
                            article.IsTop = articleData.isTop;
                            article.PublishDate = string.IsNullOrWhiteSpace(articleData.publishDateStr)
                                ? (DateTime?)null
                                : DateTime.Parse(articleData.publishDateStr);
                            article.LikeCount = articleData.likeCount;
                            article.ViewCount = articleData.viewCount;
                            article.Title = articleData.title;
                            media.MediaArticles.Add(article);
                            addCount++;
                        }
                    }
                }
            }
            var msg = "采集成功！新增：" + addCount + "篇，更新：" + updateCount + "篇";
            _dbContext.SaveChanges();
            return msg;
        }
    }
}
