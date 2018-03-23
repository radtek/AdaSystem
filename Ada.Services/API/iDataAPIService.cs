using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Ada.Core;
using Ada.Core.Domain.API;
using Ada.Core.Domain.Resource;
using Ada.Core.Tools;
using Ada.Core.ViewModel.API;
using Ada.Core.ViewModel.API.iDataAPI;
using log4net;
using Newtonsoft.Json;

namespace Ada.Services.API
{
    public class iDataAPIService : IiDataAPIService
    {
        private readonly IDbContext _dbContext;
        private readonly IAPIInterfacesService _apiInterfacesService;
        private readonly IRepository<Media> _mediaRepository;
        public ILog Log { get; set; }
        public iDataAPIService(IAPIInterfacesService apiInterfacesService, IDbContext dbContext, IRepository<Media> mediaRepository)
        {
            _apiInterfacesService = apiInterfacesService;
            _dbContext = dbContext;
            _mediaRepository = mediaRepository;
        }

        public RequestResult GetWeinXinInfo(BaseParams baseParams)
        {
            var apiInfo = _apiInterfacesService.GetAPIInterfacesByCallIndex(baseParams.CallIndex);
            string url = string.Format(apiInfo.APIUrl + "?apikey={0}", apiInfo.Token);
            int times = apiInfo.TimeOut ?? 3;
            var ids = baseParams.UID.Split(',');
            int updateCount = 0;
            foreach (var id in ids)
            {
                int request = 1;
                string urlparams = "&id=" + id;
                var apiUrl = url + urlparams;
                string htmlstr = string.Empty;
                while (request <= times)
                {
                    try
                    {
                        htmlstr = HttpUtility.Get(apiUrl);
                        request = 9999;
                    }
                    catch (Exception ex)
                    {
                        if (request == times)
                        {
                            APIRequestRecord exrecord = new APIRequestRecord();
                            exrecord.Id = IdBuilder.CreateIdNum();
                            exrecord.RequestParameters = urlparams;
                            exrecord.IsSuccess = false;
                            exrecord.Retcode = "500";
                            exrecord.ReponseContent = ex.Message;
                            exrecord.Retmsg = "请求异常";
                            exrecord.ReponseDate = DateTime.Now;
                            apiInfo.APIRequestRecords.Add(exrecord);
                        }
                        request++;
                    }
                }
                if (!string.IsNullOrWhiteSpace(htmlstr))
                {
                    var result = JsonConvert.DeserializeObject<WeiXinInfosJSON>(htmlstr);
                    //失败日志
                    if (result.retcode != ReturnCode.请求成功)
                    {
                        APIRequestRecord record = new APIRequestRecord();
                        record.Id = IdBuilder.CreateIdNum();
                        record.IsSuccess = false;
                        record.RequestParameters = urlparams;
                        record.Retcode = result.retcode.GetHashCode().ToString();
                        record.Retmsg = result.message;
                        record.ReponseContent = htmlstr;
                        record.ReponseDate = DateTime.Now;
                        record.AddedById = baseParams.TransactorId;
                        record.AddedBy = baseParams.Transactor;
                        apiInfo.APIRequestRecords.Add(record);
                    }
                    if (result.retcode == ReturnCode.请求成功)
                    {
                        //成功日志
                        if (baseParams.IsLog)
                        {
                            APIRequestRecord record = new APIRequestRecord();
                            record.Id = IdBuilder.CreateIdNum();
                            record.IsSuccess = true;
                            record.RequestParameters = urlparams;
                            record.Retcode = result.retcode.GetHashCode().ToString();
                            record.Retmsg = result.message;
                            //record.ReponseContent = "当前采集文章数：" + result.data.Count;
                            record.ReponseDate = DateTime.Now;
                            record.AddedById = baseParams.TransactorId;
                            record.AddedBy = baseParams.Transactor;
                            apiInfo.APIRequestRecords.Add(record);
                        }
                        if (result.data.Count > 0)
                        {
                            var media = _mediaRepository.LoadEntities(d => d.MediaID == id && d.IsDelete == false).FirstOrDefault();
                            if (media != null)
                            {
                                var weixinInfo = result.data[0];
                                media.IsAuthenticate = weixinInfo.idVerified;
                                media.MediaName = weixinInfo.screenName;
                                media.MonthPostNum = weixinInfo.monthPostCount;
                                media.MediaLogo = weixinInfo.avatarUrl;
                                media.MediaQR = weixinInfo.qrcodeUrl;
                                media.Content = weixinInfo.biography;
                                media.Abstract = weixinInfo.idVerifiedInfo;
                                media.CollectionDate = DateTime.Now;
                                if (DateTime.TryParse(weixinInfo.lastPost?.date, out var date))
                                {
                                    media.LastPushDate = date;
                                }
                                updateCount++;
                            }
                        }
                    }
                }
                
                _dbContext.SaveChanges();
            }
            RequestResult requestResult = new RequestResult();
            requestResult.UpdateCount = updateCount;
            requestResult.Message = "采集成功！更新：" + updateCount + "个微信号";
            return requestResult;
        }
        public RequestResult GetWeiXinArticles(WeiXinProParams wxparams)
        {
            var apiInfo = _apiInterfacesService.GetAPIInterfacesByCallIndex(wxparams.CallIndex);

            string url = string.Format(apiInfo.APIUrl + "?apikey={0}", apiInfo.Token);
            string urlparams = string.Empty;
            if (!string.IsNullOrWhiteSpace(wxparams.UID))
            {
                urlparams = "&uid=" + wxparams.UID;
            }
            if (!string.IsNullOrWhiteSpace(wxparams.ArticleLinks))
            {
                var base64 = Convert.ToBase64String(Encoding.Default.GetBytes(wxparams.ArticleLinks));
                urlparams = "&link=" + base64;
            }
            if (!string.IsNullOrWhiteSpace(wxparams.Range))
            {
                urlparams += "&range=" + wxparams.Range;
            }
            int page = wxparams.PageNum ?? 1;
            int addCount = 0;
            int updateCount = 0;
            int times = apiInfo.TimeOut ?? 3;
            for (int i = 1; i <= page; i++)
            {
                var apiUrl = urlparams + "&pageToken=" + i;
                string htmlstr = string.Empty;
                int request = 1;
                while (request <= times)
                {
                    try
                    {
                        htmlstr = HttpUtility.Get(url + apiUrl);
                        request = 9999;
                    }
                    catch (Exception ex)
                    {
                        if (request == times)
                        {
                            APIRequestRecord exrecord = new APIRequestRecord();
                            exrecord.Id = IdBuilder.CreateIdNum();
                            exrecord.RequestParameters = apiUrl;
                            exrecord.IsSuccess = false;
                            exrecord.Retcode = "500";
                            exrecord.ReponseContent = ex.Message;
                            exrecord.Retmsg = "请求异常";
                            exrecord.ReponseDate = DateTime.Now;
                            apiInfo.APIRequestRecords.Add(exrecord);
                        }
                        request++;
                    }
                }
                if (string.IsNullOrWhiteSpace(htmlstr))
                {
                    break;
                }
                var result = JsonConvert.DeserializeObject<WeiXinProJSON>(htmlstr);
                //记录日志
                //成功日志
                if (result.retcode == ReturnCode.请求成功 && wxparams.IsLog)
                {
                    APIRequestRecord record = new APIRequestRecord();
                    record.Id = IdBuilder.CreateIdNum();
                    record.IsSuccess = true;
                    record.RequestParameters = apiUrl;
                    record.Retcode = result.retcode.GetHashCode().ToString();
                    record.Retmsg = result.message;
                    record.ReponseContent = "当前采集文章数：" + result.data.Count;
                    record.ReponseDate = DateTime.Now;
                    record.AddedById = wxparams.TransactorId;
                    record.AddedBy = wxparams.Transactor;
                    apiInfo.APIRequestRecords.Add(record);
                }
                //失败日志
                if (result.retcode != ReturnCode.请求成功)
                {
                    APIRequestRecord record = new APIRequestRecord();
                    record.Id = IdBuilder.CreateIdNum();
                    record.IsSuccess = false;
                    record.RequestParameters = apiUrl;
                    record.Retcode = result.retcode.GetHashCode().ToString();
                    record.Retmsg = result.message;
                    record.ReponseContent = htmlstr;
                    record.ReponseDate = DateTime.Now;
                    record.AddedById = wxparams.TransactorId;
                    record.AddedBy = wxparams.Transactor;
                    apiInfo.APIRequestRecords.Add(record);
                    break;
                }
                if (result.data.Count > 0)
                {
                    var media = _mediaRepository.LoadEntities(d => d.MediaID == wxparams.UID && d.IsDelete == false).FirstOrDefault();
                    if (media == null)
                    {
                        break;
                    }
                    media.CollectionDate = DateTime.Now;
                    foreach (var articleData in result.data)
                    {
                        var article = media.MediaArticles.FirstOrDefault(d => d.ArticleId == articleData.id);
                        if (article != null)
                        {
                            article.ArticleIdx = articleData.idx;
                            article.ArticleUrl = articleData.url;
                            article.IsOriginal = articleData.original;
                            //article.OriginUrl = articleData.originUrl;
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
                            //article.OriginUrl = articleData.originUrl;
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
                else
                {

                    //请求成功，但记录为空的
                    if (!string.IsNullOrWhiteSpace(wxparams.UID))
                    {
                        var media = _mediaRepository.LoadEntities(d => d.MediaID == wxparams.UID && d.IsDelete == false).FirstOrDefault();
                        if (media != null) media.IsSlide = false;
                    }
                    APIRequestRecord norecord = new APIRequestRecord();
                    norecord.Id = IdBuilder.CreateIdNum();
                    norecord.RequestParameters = apiUrl;
                    norecord.IsSuccess = result.retcode == ReturnCode.请求成功;
                    norecord.Retcode = result.retcode.GetHashCode().ToString();
                    norecord.ReponseContent = htmlstr;
                    norecord.Retmsg = "请求成功，返回记录为空";
                    norecord.ReponseDate = DateTime.Now;
                    apiInfo.APIRequestRecords.Add(norecord);
                }
                //如果没有下一页就退出
                if (result.hasNext == false)
                {
                    break;
                }
            }
            RequestResult requestResult = new RequestResult();
            requestResult.AddCount = addCount;
            requestResult.UpdateCount = updateCount;
            requestResult.Message = "采集成功！新增：" + addCount + "篇，更新：" + updateCount + "篇";

            _dbContext.SaveChanges();
            //try
            //{
            //    _dbContext.SaveChanges();
            //}
            //catch (DbEntityValidationException ex)
            //{
            //    StringBuilder errors = new StringBuilder();
            //    IEnumerable<DbEntityValidationResult> validationResult = ex.EntityValidationErrors;
            //    foreach (DbEntityValidationResult result in validationResult)
            //    {
            //        ICollection<DbValidationError> validationError = result.ValidationErrors;
            //        foreach (DbValidationError err in validationError)
            //        {
            //            errors.Append(err.PropertyName + ":" + err.ErrorMessage + "\r\n");
            //        }
            //    }
            //    requestResult.Message = errors.ToString();
            //}
            return requestResult;
        }
        /// <summary>
        /// 先更新微信信息，再更新文章内容
        /// </summary>
        /// <param name="wxparams"></param>
        /// <returns></returns>
        public RequestResult GetWeiXinInfoPro(WeiXinProParams wxparams)
        {
            var apiInfo = _apiInterfacesService.GetAPIInterfacesByCallIndex(wxparams.CallIndex);
            string url = string.Format(apiInfo.APIUrl + "?apikey={0}", apiInfo.Token);
            int times = apiInfo.TimeOut ?? 3;
            int request = 1;
            string urlparams = "&id=" + wxparams.UID;
            var apiUrl = url + urlparams;
            string htmlstr = string.Empty;
            int addCount = 0;
            int updateCount = 0;
            while (request <= times)
            {
                try
                {
                    htmlstr = HttpUtility.Get(apiUrl);
                    request = 9999;
                }
                catch (Exception ex)
                {
                    if (request == times)
                    {
                        APIRequestRecord exrecord = new APIRequestRecord();
                        exrecord.Id = IdBuilder.CreateIdNum();
                        exrecord.RequestParameters = urlparams;
                        exrecord.IsSuccess = false;
                        exrecord.Retcode = "500";
                        exrecord.ReponseContent = ex.Message;
                        exrecord.Retmsg = "请求异常";
                        exrecord.ReponseDate = DateTime.Now;
                        apiInfo.APIRequestRecords.Add(exrecord);
                    }
                    request++;
                }
            }
            if (!string.IsNullOrWhiteSpace(htmlstr))
            {
                var result = JsonConvert.DeserializeObject<WeiXinInfosJSON>(htmlstr);
                //失败日志
                if (result.retcode != ReturnCode.请求成功)
                {
                    APIRequestRecord record = new APIRequestRecord();
                    record.Id = IdBuilder.CreateIdNum();
                    record.IsSuccess = false;
                    record.RequestParameters = urlparams;
                    record.Retcode = result.retcode.GetHashCode().ToString();
                    record.Retmsg = result.message;
                    record.ReponseContent = htmlstr;
                    record.ReponseDate = DateTime.Now;
                    record.AddedById = wxparams.TransactorId;
                    record.AddedBy = wxparams.Transactor;
                    apiInfo.APIRequestRecords.Add(record);
                }
                if (result.retcode == ReturnCode.请求成功)
                {
                    //成功日志
                    if (wxparams.IsLog)
                    {
                        APIRequestRecord record = new APIRequestRecord();
                        record.Id = IdBuilder.CreateIdNum();
                        record.IsSuccess = true;
                        record.RequestParameters = urlparams;
                        record.Retcode = result.retcode.GetHashCode().ToString();
                        record.Retmsg = result.message;
                        //record.ReponseContent = "当前采集文章数：" + result.data.Count;
                        record.ReponseDate = DateTime.Now;
                        record.AddedById = wxparams.TransactorId;
                        record.AddedBy = wxparams.Transactor;
                        apiInfo.APIRequestRecords.Add(record);
                    }
                    if (result.data.Count > 0)
                    {
                        var media = _mediaRepository.LoadEntities(d => d.MediaID == wxparams.UID && d.IsDelete == false).FirstOrDefault();
                        //阅读数
                        
                        if (media != null)
                        {
                            var viewCount = media.MediaArticles.Where(d=>d.IsTop==true).OrderByDescending(d => d.PublishDate).Take(10)
                                .Average(d => d.ViewCount);
                            var lastPost = media.LastPushDate;
                            //是否要更新文章
                            var isUpdateArticle = lastPost == null;

                            var weixinInfo = result.data[0];
                            media.IsAuthenticate = weixinInfo.idVerified;
                            media.MediaName = weixinInfo.screenName;
                            media.MonthPostNum = weixinInfo.monthPostCount;
                            media.MediaLogo = weixinInfo.avatarUrl;
                            media.MediaQR = weixinInfo.qrcodeUrl;
                            media.Abstract = weixinInfo.idVerifiedInfo;
                            media.Content = weixinInfo.biography;
                            media.AvgReadNum = Convert.ToInt32(viewCount);
                            media.CollectionDate = DateTime.Now;
                            if (DateTime.TryParse(weixinInfo.lastPost?.date, out var date))
                            {
                                media.LastPushDate = date;
                            }
                            
                            if (!isUpdateArticle)
                            {
                                if (date != lastPost.Value)
                                {
                                    isUpdateArticle = true;
                                }
                            }
                            //更新文章
                            if (isUpdateArticle&& weixinInfo.lastPost!=null)
                            {
                                var apiArticle = _apiInterfacesService.GetAPIInterfacesByCallIndex(wxparams.CallIndexWeiXinInfo);
                                //更新日期范围
                                var start = DateTime.Now.AddDays(-2).ToString("yyyy-MM-dd");
                                var end = DateTime.Now.ToString("yyyy-MM-dd");
                                var timeSpan = DateTime.Now - date;
                                if (timeSpan.TotalDays > 2)
                                {
                                    start = date.AddDays(-2).ToString("yyyy-MM-dd");
                                    end = date.ToString("yyyy-MM-dd");
                                }
                                string urlArticle = string.Format(apiArticle.APIUrl + "?apikey={0}&uid={1}&pageToken=1&beginDate={2}&endDate={3}", apiArticle.Token,
                                    wxparams.UID, start, end);
                                string htmlstrArticle = string.Empty;
                                int requestArticle = 1;
                                int timesArticle = apiArticle.TimeOut ?? 3;
                                while (requestArticle <= timesArticle)
                                {
                                    try
                                    {
                                        htmlstrArticle = HttpUtility.Get(urlArticle);
                                        requestArticle = 9999;
                                    }
                                    catch (Exception ex)
                                    {
                                        if (requestArticle == timesArticle)
                                        {
                                            APIRequestRecord exrecord = new APIRequestRecord();
                                            exrecord.Id = IdBuilder.CreateIdNum();
                                            exrecord.RequestParameters = wxparams.UID;
                                            exrecord.IsSuccess = false;
                                            exrecord.Retcode = "500";
                                            exrecord.ReponseContent = ex.Message;
                                            exrecord.Retmsg = "请求异常";
                                            exrecord.ReponseDate = DateTime.Now;
                                            apiArticle.APIRequestRecords.Add(exrecord);
                                        }
                                        requestArticle++;
                                    }
                                }
                                if (!string.IsNullOrWhiteSpace(htmlstrArticle))
                                {
                                    var resultArticle = JsonConvert.DeserializeObject<WeiXinProJSON>(htmlstrArticle);
                                    if (resultArticle.data.Count > 0)
                                    {
                                        foreach (var articleData in resultArticle.data)
                                        {
                                            var article = media.MediaArticles.FirstOrDefault(d => d.ArticleId == articleData.id);
                                            if (article != null)
                                            {
                                                article.ArticleIdx = articleData.idx;
                                                article.ArticleUrl = articleData.url;
                                                article.IsOriginal = articleData.original;
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

                            }
                        }
                    }
                }
            }
            RequestResult requestResult = new RequestResult();
            requestResult.AddCount = addCount;
            requestResult.UpdateCount = updateCount;
            requestResult.Message = "采集成功！新增：" + addCount + "篇，更新：" + updateCount + "篇";
            _dbContext.SaveChanges();
            return requestResult;
        }
        public RequestResult GetWeiBoArticles(WeiBoParams wbparams)
        {
            var apiInfo = _apiInterfacesService.GetAPIInterfacesByCallIndex(wbparams.CallIndex);

            string url = string.Format(apiInfo.APIUrl + "?apikey={0}", apiInfo.Token);
            string urlparams = string.Empty;
            if (!string.IsNullOrWhiteSpace(wbparams.UID))
            {
                urlparams = "&uid=" + wbparams.UID;
            }

            if (!string.IsNullOrWhiteSpace(wbparams.Date))
            {
                urlparams += "&date=" + wbparams.Date;
            }
            int page = wbparams.PageNum ?? 1;
            int addCount = 0;
            int updateCount = 0;
            int times = apiInfo.TimeOut ?? 3;
            for (int i = 1; i <= page; i++)
            {
                var apiUrl = urlparams + "&pageToken=" + i;
                string htmlstr = string.Empty;
                int request = 1;
                while (request <= times)
                {
                    try
                    {
                        htmlstr = HttpUtility.Get(url + apiUrl);
                        request = 9999;
                    }
                    catch (Exception ex)
                    {
                        if (request == times)
                        {
                            //异常日期
                            APIRequestRecord exrecord = new APIRequestRecord();
                            exrecord.Id = IdBuilder.CreateIdNum();
                            exrecord.RequestParameters = apiUrl;
                            exrecord.IsSuccess = false;
                            exrecord.Retcode = "500";
                            exrecord.ReponseContent = ex.Message;
                            exrecord.Retmsg = "请求异常";
                            exrecord.ReponseDate = DateTime.Now;
                            apiInfo.APIRequestRecords.Add(exrecord);
                        }
                        request++;
                    }
                }
                if (string.IsNullOrWhiteSpace(htmlstr))
                {
                    break;
                }
                var result = JsonConvert.DeserializeObject<WeiBoJSON>(htmlstr);
                //记录日志
                //成功日志
                if (result.retcode == ReturnCode.请求成功 && wbparams.IsLog)
                {
                    APIRequestRecord record = new APIRequestRecord();
                    record.Id = IdBuilder.CreateIdNum();
                    record.IsSuccess = true;
                    record.RequestParameters = apiUrl;
                    record.Retcode = result.retcode.GetHashCode().ToString();
                    record.Retmsg = result.message;
                    record.ReponseContent = "当前采集文章数：" + result.data.Count;
                    record.ReponseDate = DateTime.Now;
                    record.AddedById = wbparams.TransactorId;
                    record.AddedBy = wbparams.Transactor;
                    apiInfo.APIRequestRecords.Add(record);
                }
                //失败日志
                if (result.retcode != ReturnCode.请求成功)
                {
                    APIRequestRecord record = new APIRequestRecord();
                    record.Id = IdBuilder.CreateIdNum();
                    record.IsSuccess = false;
                    record.RequestParameters = apiUrl;
                    record.Retcode = result.retcode.GetHashCode().ToString();
                    record.Retmsg = result.message;
                    record.ReponseContent = htmlstr;
                    record.ReponseDate = DateTime.Now;
                    record.AddedById = wbparams.TransactorId;
                    record.AddedBy = wbparams.Transactor;
                    apiInfo.APIRequestRecords.Add(record);
                    break;
                }
                if (result.data.Count > 0)
                {
                    var media = _mediaRepository.LoadEntities(d => d.MediaID == wbparams.UID && d.IsDelete == false).FirstOrDefault();
                    if (media == null)
                    {
                        break;
                    }

                    //根据第一个数据更新微博信息
                    var mediaInfo = result.data[0].from;
                    media.MediaName = mediaInfo.name;
                    media.Content = mediaInfo.description;
                    media.FansNum = mediaInfo.fansCount;
                    media.PostNum = mediaInfo.postCount;
                    media.MediaLink = mediaInfo.url;
                    media.FriendNum = mediaInfo.friendCount;
                    media.MediaLogo = mediaInfo.extend?.avatar_large;
                    media.Area = mediaInfo.extend?.location;
                    media.IsAuthenticate = mediaInfo.extend?.verified;
                    media.Abstract = mediaInfo.extend?.verified_reason;
                    media.CollectionDate = DateTime.Now;
                    var authType = Utils.BlogAuthenticateType(mediaInfo.extend?.verified_type);
                    if (!string.IsNullOrWhiteSpace(authType))
                    {
                        media.AuthenticateType = authType;
                    }
                    var sex = Utils.BlogSex(mediaInfo.extend?.gender);
                    if (!string.IsNullOrWhiteSpace(sex))
                    {
                        media.Sex = sex;
                    }
                    foreach (var articleData in result.data)
                    {

                        var article = media.MediaArticles.FirstOrDefault(d => d.ArticleId == articleData.id);
                        if (article != null)
                        {
                            //article.ArticleId = articleData.id;
                            article.ArticleUrl = articleData.url;
                            article.CommentCount = articleData.commentCount;
                            article.Content = articleData.content;
                            article.ShareCount = articleData.shareCount;
                            article.PublishDate = string.IsNullOrWhiteSpace(articleData.pDate)
                                ? (DateTime?)null
                                : DateTime.Parse(articleData.pDate);
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
                            article.ArticleUrl = articleData.url;
                            article.CommentCount = articleData.commentCount;
                            article.Content = articleData.content;
                            article.ShareCount = articleData.shareCount;
                            article.PublishDate = string.IsNullOrWhiteSpace(articleData.pDate)
                                ? (DateTime?)null
                                : DateTime.Parse(articleData.pDate);
                            article.LikeCount = articleData.likeCount;
                            article.ViewCount = articleData.viewCount;
                            article.Title = articleData.title;
                            media.MediaArticles.Add(article);
                            addCount++;
                        }
                    }
                }
                else
                {
                    //第一页请求成功，但记录为空的
                    if (!string.IsNullOrWhiteSpace(wbparams.UID) && i == 1)
                    {
                        var media = _mediaRepository.LoadEntities(d => d.MediaID == wbparams.UID && d.IsDelete == false).FirstOrDefault();
                        if (media != null) media.IsSlide = false;
                    }
                    APIRequestRecord norecord = new APIRequestRecord();
                    norecord.Id = IdBuilder.CreateIdNum();
                    norecord.RequestParameters = apiUrl;
                    norecord.IsSuccess = result.retcode == ReturnCode.请求成功;
                    norecord.Retcode = result.retcode.GetHashCode().ToString();
                    norecord.ReponseContent = htmlstr;
                    norecord.Retmsg = "请求成功，返回记录为空";
                    norecord.ReponseDate = DateTime.Now;
                    apiInfo.APIRequestRecords.Add(norecord);
                }
                //如果没有下一页就退出
                if (result.hasNext == false)
                {
                    break;
                }
            }
            RequestResult requestResult = new RequestResult();
            requestResult.AddCount = addCount;
            requestResult.UpdateCount = updateCount;
            requestResult.Message = "采集成功！新增：" + addCount + "篇，更新：" + updateCount + "篇";
            _dbContext.SaveChanges();
            return requestResult;
        }

        public string TestApi(TestParams testParams)
        {
            var apiInfo = _apiInterfacesService.GetAPIInterfacesByCallIndex(testParams.CallIndex);
            IEnumerable<KeyValuePair<string, string>> postdata = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("region_id",testParams.ReginId==null?"1":testParams.ReginId.ToString()),
                new KeyValuePair<string, string>("api_id",testParams.ApiId.ToString()),
                new KeyValuePair<string, string>("apitype",testParams.Apiype.ToString()),
                new KeyValuePair<string, string>("id",testParams.UID)
            };
            return HttpUtility.Post(apiInfo.APIUrl, postdata);
            
        }
        
    }
}
