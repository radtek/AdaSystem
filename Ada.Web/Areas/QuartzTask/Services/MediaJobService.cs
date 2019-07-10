using System;
using System.Collections.Generic;
using System.Data.Entity.SqlServer;
using System.Linq;
using System.Threading;
using System.Web;
using Ada.Core;
using Ada.Core.Domain;
using Ada.Core.Domain.Admin;
using Ada.Core.Domain.QuartzTask;
using Ada.Core.Domain.Resource;
using Ada.Core.Tools;
using Ada.Core.ViewModel.API.iDataAPI;
using Ada.Services.Cache;
using Newtonsoft.Json;
using Quartz;

namespace QuartzTask.Services
{
    public class MediaJobService : IMediaJobService
    {
        private readonly IDbContext _dbContext;
        private readonly IRepository<Job> _repository;
        private readonly IRepository<Media> _mediaRepository;
        private readonly IRepository<MediaArticle> _mediaArticleRepository;
        private readonly IRepository<Field> _fieldRepository;
        private readonly ICacheService _cacheService;
        public MediaJobService(IDbContext dbContext,
            IRepository<Job> repository,
            IRepository<Media> mediaRepository,
            IRepository<Field> fieldRepository,
            ICacheService cacheService,
            IRepository<MediaArticle> mediaArticleRepository)
        {
            _dbContext = dbContext;
            _repository = repository;
            _mediaRepository = mediaRepository;
            _fieldRepository = fieldRepository;
            _cacheService = cacheService;
            _mediaArticleRepository = mediaArticleRepository;
        }

        public void WeixinInfo(IJobExecutionContext context)
        {
            var name = context.JobDetail.Key.Name;
            var group = context.JobDetail.Key.Group;
            var job = _repository.LoadEntities(d => d.JobName == name && d.GroupName == group).FirstOrDefault();
            var times = job.Repetitions ?? 3;
            var hour = job.Taxis ?? 360;
            var media = _mediaRepository.LoadEntities(d =>
                    d.IsDelete == false &&
                    d.IsSlide == true &&
                    d.MediaType.CallIndex == "weixin" &&
                    d.Status == Consts.StateNormal &&
                    (d.CollectionDate == null || SqlFunctions.DateDiff("hour", d.CollectionDate, DateTime.Now) > hour))
                .FirstOrDefault();
            if (media != null)
            {
                media.CollectionDate = DateTime.Now;
                bool isSuccess = true;
                string errMsg = null;
                if (!string.IsNullOrWhiteSpace(media.MediaID))
                {
                    var url = job.ApiUrl + media.MediaID;
                    for (int i = 0; i < times; i++)
                    {
                        try
                        {
                            var rhtml = Ada.Core.Tools.HttpUtility.Get(url);
                            var result = JsonConvert.DeserializeObject<WeiXinInfosJSON>(rhtml);
                            if (result.retcode == ReturnCode.请求成功)
                            {
                                if (result.data.Any())
                                {
                                    var weixinInfo = result.data.FirstOrDefault();
                                    if (weixinInfo != null)
                                    {
                                        media.IsAuthenticate = weixinInfo.idVerified;
                                        media.MediaName = weixinInfo.screenName;
                                        media.MonthPostNum = weixinInfo.monthPostCount;
                                        media.MediaLogo = weixinInfo.avatarUrl;
                                        media.MediaQR = weixinInfo.qrcodeUrl;
                                        media.Content = weixinInfo.biography;
                                        media.Abstract = weixinInfo.idVerifiedInfo;
                                        if (!string.IsNullOrWhiteSpace(weixinInfo.biz))
                                        {
                                            media.MediaLink = weixinInfo.biz;
                                        }
                                    }
                                }
                                isSuccess = true;
                                break;
                            }

                            if (result.retcode == ReturnCode.目标参数搜索没结果 || result.retcode == ReturnCode.用户帐号不存在)
                            {
                                media.Content = result.message;
                                isSuccess = true;
                                break;
                            }
                        }
                        catch (Exception ex)
                        {
                            errMsg = ex.Message;
                            isSuccess = false;
                        }
                    }
                }
                if (isSuccess == false && job.IsLog == true)
                {
                    var detail = new JobDetail
                    {
                        Id = IdBuilder.CreateIdNum(),
                        RequestDate = DateTime.Now,
                        AddedDate = DateTime.Now,
                        IsSuccess = false,
                        Retcode = "502",
                        Retmsg = "采集微信用户信息:" + media.MediaID + "异常，" + errMsg
                    };
                    job.JobDetails.Add(detail);
                }
                job.Remark = media.MediaID + "-" + DateTime.Now;
                _mediaRepository.Update(media);

            }
            else
            {
                job.Remark = "暂无可采集数据";
            }
            if (context.NextFireTimeUtc != null)
            {
                job.NextTime = context.NextFireTimeUtc.Value.ToLocalTime().DateTime;
            }
            _repository.Update(job);
            _dbContext.SaveChanges();
        }

        public void WeixinArticle(IJobExecutionContext context)
        {
            var name = context.JobDetail.Key.Name;
            var group = context.JobDetail.Key.Group;
            var job = _repository.LoadEntities(d => d.JobName == name && d.GroupName == group).FirstOrDefault();
            var times = job.Repetitions ?? 3;
            var hour = job.Taxis ?? 24;
            var no = job.TimeOut ?? -6;
            var where = job.Params.Split(',');
            var noUpdate = DateTime.Now.AddMonths(no);//
            var media = _mediaRepository.LoadEntities(d =>
                    d.IsDelete == false &&
                    where.Contains(d.TransactorId) &&
                    d.IsSlide == true &&
                    d.MediaType.CallIndex == "weixin" &&
                    d.Status == Consts.StateNormal &&
                    (d.LastPushDate > noUpdate || d.LastPushDate == null) &&
                    (d.ApiUpDate == null || SqlFunctions.DateDiff("hour", d.ApiUpDate, DateTime.Now) > hour))
                .FirstOrDefault();

            if (media != null)
            {
                media.ApiUpDate = DateTime.Now;
                var detail = new JobDetail { Id = IdBuilder.CreateIdNum(), RequestDate = DateTime.Now, IsSuccess = true };
                if (!string.IsNullOrWhiteSpace(media.MediaLink))
                {
                    var url = job.ApiUrl + media.MediaLink.Trim();
                    for (int i = 0; i < times; i++)
                    {
                        try
                        {
                            var rhtml = Ada.Core.Tools.HttpUtility.Get(url);
                            var result = JsonConvert.DeserializeObject<WeiXinProJSON>(rhtml);
                            if (result.retcode == ReturnCode.请求成功)
                            {
                                if (result.data.Any())
                                {
                                    var cache = _cacheService.GetObject<List<string>>("WEIXINBRAND");
                                    if (cache == null)
                                    {
                                        var brandList = _fieldRepository.LoadEntities(d => d.FieldType.CallIndex == "Brand").Select(d => d.Text).ToList();
                                        cache = brandList;
                                        _cacheService.Put("WEIXINBRAND", brandList, TimeSpan.FromDays(180));
                                    }
                                    List<string> brands = cache as List<string>;
                                    DateTime? lastPush = null;
                                    int index = 1;
                                    foreach (var articleData in result.data.OrderByDescending(d => d.publishDate))
                                    {
                                        if (string.IsNullOrWhiteSpace(articleData.id))
                                        {
                                            continue;
                                        }
                                        if (articleData.id.Length > 128)
                                        {
                                            continue;
                                        }

                                        DateTime? pubDate = null;
                                        if (DateTime.TryParse(articleData.publishDateStr, out var publishDate))
                                        {
                                            if (index == 1)
                                            {
                                                lastPush = publishDate;
                                            }

                                            pubDate = publishDate;
                                        }
                                        var article = media.MediaArticles.FirstOrDefault(d => d.ArticleId == articleData.id);
                                        if (article == null)
                                        {
                                            article = new MediaArticle();
                                            article.Id = IdBuilder.CreateIdNum();
                                            article.ArticleId = articleData.id;
                                            article.ArticleIdx = articleData.idx;
                                            article.ArticleUrl = articleData.url;
                                            article.IsOriginal = articleData.origin;
                                            article.Biz = articleData.biz;
                                            article.CommentCount = articleData.commentCount;
                                            article.Content = GetBrands(articleData.content, brands);
                                            article.IsTop = articleData.isTop;
                                            article.PublishDate = pubDate;
                                            article.LikeCount = articleData.likeCount;
                                            article.ViewCount = articleData.viewCount;
                                            article.Title = articleData.title;
                                            if (!string.IsNullOrWhiteSpace(articleData.posterId))
                                            {
                                                media.MediaID = articleData.posterId;
                                            }
                                            if (!string.IsNullOrWhiteSpace(articleData.posterScreenName))
                                            {
                                                media.MediaName = articleData.posterScreenName;
                                            }
                                            media.MediaArticles.Add(article);
                                        }
                                        //else
                                        //{
                                        //    if (articleData.viewCount != null)
                                        //    {
                                        //        article.ViewCount = articleData.viewCount;
                                        //    }
                                        //    if (articleData.likeCount != null)
                                        //    {
                                        //        article.LikeCount = articleData.likeCount;
                                        //    }
                                        //    if (articleData.commentCount != null)
                                        //    {
                                        //        article.CommentCount = articleData.commentCount;
                                        //    }
                                        //}
                                        index++;
                                    }
                                    //更新文章统计
                                    var viewCount = media.MediaArticles.Where(d => d.IsTop == true).OrderByDescending(d => d.PublishDate).Take(10)
                                        .Average(d => d.ViewCount);
                                    media.AvgReadNum = Convert.ToInt32(viewCount);
                                    //media.LastReadNum = media.MediaArticles.Where(l => l.IsTop == true)
                                    //    .OrderByDescending(a => a.PublishDate).FirstOrDefault()?.ViewCount;
                                    var start30 = DateTime.Now.Date.AddDays(-30);
                                    var end30 = DateTime.Now.AddDays(1).Date;
                                    var count = media.MediaArticles.Where(d => d.PublishDate >= start30 && d.PublishDate < end30).Select(d => new
                                    {
                                        Date = d.PublishDate.Value.ToString("yyyy-MM-dd")
                                    }).GroupBy(d => d.Date).Count();
                                    media.PublishFrequency = count;
                                    media.LastPushDate = lastPush;
                                }

                                detail.IsSuccess = true;
                                break;
                            }

                            if (result.retcode == ReturnCode.目标参数搜索没结果)
                            {
                                detail.IsSuccess = true;
                                break;
                            }
                            if (result.retcode == ReturnCode.用户帐号不存在)
                            {
                                media.Content = result.message;
                                media.Status = Consts.StateLock;
                                detail.IsSuccess = true;
                                break;
                            }
                        }
                        catch (Exception ex)
                        {
                            detail.Retmsg = "采集微信文章列表:" + media.MediaID + "异常，" + ex.Message;
                            detail.Retcode = "502";
                            detail.IsSuccess = false;
                        }
                    }
                }
                if (detail.IsSuccess == false && job.IsLog == true)
                {
                    detail.AddedDate = DateTime.Now;
                    job.JobDetails.Add(detail);
                }
                job.Remark = media.MediaID + "-" + DateTime.Now;


                _mediaRepository.Update(media);

            }
            else
            {
                job.Remark = "暂无可采集数据";
            }
            if (context.NextFireTimeUtc != null)
            {
                job.NextTime = context.NextFireTimeUtc.Value.ToLocalTime().DateTime;
            }
            _repository.Update(job);
            _dbContext.SaveChanges();
        }
        public void WeixinArticleData(IJobExecutionContext context)
        {
            var name = context.JobDetail.Key.Name;
            var group = context.JobDetail.Key.Group;
            var job = _repository.LoadEntities(d => d.JobName == name && d.GroupName == group).FirstOrDefault();
            var times = job.Repetitions ?? 3;
            var hour = job.Taxis ?? 24;
            var no = job.TimeOut ?? 3;
            var index = job.Params;
            var article = _mediaArticleRepository.LoadEntities(d => d.ArticleIdx==index &&
                                                                     SqlFunctions.DateDiff("day", d.PublishDate,
                                                                         DateTime.Now) <= no &&
                                                                     (d.ModifiedDate == null ||
                                                                      SqlFunctions.DateDiff("hour", d.ModifiedDate,
                                                                          DateTime.Now) > hour))
                .OrderByDescending(d => d.Id).FirstOrDefault();

            
            if (article != null)
            {
                article.ModifiedDate = DateTime.Now;
                bool isSuccess = true;
                string errMsg = null;
                if (!string.IsNullOrWhiteSpace(article.ArticleUrl))
                {
                    byte[] b = System.Text.Encoding.Default.GetBytes(article.ArticleUrl);
                    var url = job.ApiUrl + Convert.ToBase64String(b);

                    for (int i = 0; i < times; i++)
                    {
                        try
                        {
                            var rhtml = Ada.Core.Tools.HttpUtility.Get(url);
                            var result = JsonConvert.DeserializeObject<WeiXinPro2JSON>(rhtml);
                            if (result.retcode == ReturnCode.请求成功)
                            {
                                if (result.data.Any())
                                {
                                    var data = result.data.FirstOrDefault();
                                    if (data != null)
                                    {
                                        article.ViewCount = data.viewCount ?? 0;
                                        article.LikeCount = data.likeCount ?? 0;
                                    }
                                }
                                isSuccess = true;
                                break;
                            }

                            if (result.retcode == ReturnCode.目标参数搜索没结果 || result.retcode == ReturnCode.用户帐号不存在)
                            {
                                isSuccess = true;
                                break;
                            }
                        }
                        catch (Exception ex)
                        {
                            errMsg = "采集微信文章数据:" + article.Id + "异常，" + ex.Message;
                            isSuccess = false;
                        }
                    }
                }
                if (isSuccess == false && job.IsLog == true)
                {
                    var detail = new JobDetail
                    {
                        Id = IdBuilder.CreateIdNum(),
                        RequestDate = DateTime.Now,
                        IsSuccess = false,
                        Retcode = "500",
                        AddedDate = DateTime.Now,
                        Retmsg = errMsg
                    };
                    job.JobDetails.Add(detail);
                }
                job.Remark = article.Id + "-" + DateTime.Now;
                _mediaArticleRepository.Update(article);

            }
            else
            {
                job.Remark = "暂无可采集数据";
            }
            if (context.NextFireTimeUtc != null)
            {
                job.NextTime = context.NextFireTimeUtc.Value.ToLocalTime().DateTime;
            }
            _repository.Update(job);
            _dbContext.SaveChanges();
        }

        public void WeixinArticleBySouhu(IJobExecutionContext context)
        {
            var name = context.JobDetail.Key.Name;
            var group = context.JobDetail.Key.Group;
            var job = _repository.LoadEntities(d => d.JobName == name && d.GroupName == group).FirstOrDefault();
            var times = job.Repetitions ?? 3;
            var hour = job.Taxis ?? 24;
            var no = job.TimeOut ?? -6;
            var where = job.Params.Split(',');
            var noUpdate = DateTime.Now.AddMonths(no);//
            var media = _mediaRepository.LoadEntities(d =>
                    d.IsDelete == false &&
                    where.Contains(d.TransactorId) &&
                    d.IsSlide == true &&
                    d.MediaType.CallIndex == "weixin" &&
                    d.Status == Consts.StateNormal &&
                    (d.LastPushDate > noUpdate || d.LastPushDate == null) &&
                    (d.ApiUpDate == null || SqlFunctions.DateDiff("hour", d.ApiUpDate, DateTime.Now) > hour))
                .FirstOrDefault();

            if (media != null)
            {
                media.ApiUpDate = DateTime.Now;
                var detail = new JobDetail { Id = IdBuilder.CreateIdNum(), RequestDate = DateTime.Now, IsSuccess = true };
                if (!string.IsNullOrWhiteSpace(media.MediaID))
                {
                    var url = job.ApiUrl + media.MediaID.Trim();
                    for (int i = 0; i < times; i++)
                    {
                        try
                        {
                            var rhtml = Ada.Core.Tools.HttpUtility.Get(url);
                            var result = JsonConvert.DeserializeObject<WeiXinProJSON>(rhtml);
                            if (result.retcode == ReturnCode.请求成功)
                            {
                                if (result.data.Any())
                                {
                                    var cache = _cacheService.GetObject<List<string>>("WEIXINBRAND");
                                    if (cache == null)
                                    {
                                        var brandList = _fieldRepository.LoadEntities(d => d.FieldType.CallIndex == "Brand").Select(d => d.Text).ToList();
                                        cache = brandList;
                                        _cacheService.Put("WEIXINBRAND", brandList, TimeSpan.FromDays(180));
                                    }
                                    List<string> brands = cache as List<string>;
                                    DateTime? lastPush = null;
                                    int index = 1;
                                    foreach (var articleData in result.data.OrderByDescending(d => d.publishDate))
                                    {
                                        if (string.IsNullOrWhiteSpace(articleData.id))
                                        {
                                            continue;
                                        }
                                        if (articleData.id.Length > 128)
                                        {
                                            continue;
                                        }

                                        DateTime? pubDate = null;
                                        if (DateTime.TryParse(articleData.publishDateStr, out var publishDate))
                                        {
                                            if (index == 1)
                                            {
                                                lastPush = publishDate;
                                            }

                                            pubDate = publishDate;
                                        }
                                        var article = media.MediaArticles.FirstOrDefault(d => d.ArticleId == articleData.id);
                                        if (article == null)
                                        {
                                            article = new MediaArticle();
                                            article.Id = IdBuilder.CreateIdNum();
                                            article.ArticleId = articleData.id;
                                            article.ArticleIdx = articleData.idx;
                                            article.ArticleUrl = articleData.url;
                                            article.IsOriginal = articleData.origin;
                                            article.Biz = articleData.biz;
                                            article.CommentCount = articleData.commentCount;
                                            article.Content = GetBrands(articleData.content, brands);
                                            article.IsTop = articleData.isTop;
                                            article.PublishDate = pubDate;
                                            article.LikeCount = articleData.likeCount;
                                            article.ViewCount = articleData.viewCount;
                                            article.Title = articleData.title;
                                            if (!string.IsNullOrWhiteSpace(articleData.posterId))
                                            {
                                                media.MediaID = articleData.posterId;
                                            }
                                            if (!string.IsNullOrWhiteSpace(articleData.posterScreenName))
                                            {
                                                media.MediaName = articleData.posterScreenName;
                                            }
                                            media.MediaArticles.Add(article);
                                        }
                                        //else
                                        //{
                                        //    if (articleData.viewCount != null)
                                        //    {
                                        //        article.ViewCount = articleData.viewCount;
                                        //    }
                                        //    if (articleData.likeCount != null)
                                        //    {
                                        //        article.LikeCount = articleData.likeCount;
                                        //    }
                                        //    if (articleData.commentCount != null)
                                        //    {
                                        //        article.CommentCount = articleData.commentCount;
                                        //    }
                                        //}
                                        index++;
                                    }
                                    //更新文章统计
                                    var viewCount = media.MediaArticles.Where(d => d.IsTop == true).OrderByDescending(d => d.PublishDate).Take(10)
                                        .Average(d => d.ViewCount);
                                    media.AvgReadNum = Convert.ToInt32(viewCount);
                                    //media.LastReadNum = media.MediaArticles.Where(l => l.IsTop == true)
                                    //    .OrderByDescending(a => a.PublishDate).FirstOrDefault()?.ViewCount;
                                    var start30 = DateTime.Now.Date.AddDays(-30);
                                    var end30 = DateTime.Now.AddDays(1).Date;
                                    var count = media.MediaArticles.Where(d => d.PublishDate >= start30 && d.PublishDate < end30).Select(d => new
                                    {
                                        Date = d.PublishDate.Value.ToString("yyyy-MM-dd")
                                    }).GroupBy(d => d.Date).Count();
                                    media.PublishFrequency = count;
                                    media.LastPushDate = lastPush;
                                }

                                detail.IsSuccess = true;
                                break;
                            }

                            if (result.retcode == ReturnCode.目标参数搜索没结果)
                            {
                                detail.IsSuccess = true;
                                break;
                            }
                            if (result.retcode == ReturnCode.用户帐号不存在)
                            {
                                //media.Content = result.message;
                                //media.Status = Consts.StateLock;
                                detail.IsSuccess = true;
                                break;
                            }
                        }
                        catch (Exception ex)
                        {
                            detail.Retmsg = "采集微信文章列表:" + media.MediaID + "异常，" + ex.Message;
                            detail.Retcode = "502";
                            detail.IsSuccess = false;
                        }
                    }
                }
                if (detail.IsSuccess == false && job.IsLog == true)
                {
                    detail.AddedDate = DateTime.Now;
                    job.JobDetails.Add(detail);
                }
                job.Remark = media.MediaID + "-" + DateTime.Now;


                _mediaRepository.Update(media);

            }
            else
            {
                job.Remark = "暂无可采集数据";
            }
            if (context.NextFireTimeUtc != null)
            {
                job.NextTime = context.NextFireTimeUtc.Value.ToLocalTime().DateTime;
            }
            _repository.Update(job);
            _dbContext.SaveChanges();
        }


        public void WeixinArticlePro4(IJobExecutionContext context)
        {
            var name = context.JobDetail.Key.Name;
            var group = context.JobDetail.Key.Group;
            var job = _repository.LoadEntities(d => d.JobName == name && d.GroupName == group).FirstOrDefault();
            var times = job.Repetitions ?? 3;
            var hour = job.Taxis ?? 24;
            var no = job.TimeOut ?? -6;
            List<string> where = new List<string>();
            if (!string.IsNullOrWhiteSpace(job.Params))
            {
                where = job.Params.Split(',').ToList();
            }
            
            var noUpdate = DateTime.Now.AddMonths(no);//
            Media media;
            if (where.Any())
            {
                media = _mediaRepository.LoadEntities(d =>
                        d.IsDelete == false &&
                        where.Contains(d.TransactorId) &&
                        d.IsSlide == true &&
                        d.MediaType.CallIndex == "weixin" &&
                        d.Status == Consts.StateNormal &&
                        (d.LastPushDate > noUpdate || d.LastPushDate == null) &&
                        (d.ApiUpDate == null || SqlFunctions.DateDiff("hour", d.ApiUpDate, DateTime.Now) > hour))
                    .FirstOrDefault();
            }
            else
            {
                media = _mediaRepository.LoadEntities(d =>
                        d.IsDelete == false &&
                        d.IsSlide == true &&
                        d.MediaType.CallIndex == "weixin" &&
                        d.Status == Consts.StateNormal &&
                        (d.LastPushDate > noUpdate || d.LastPushDate == null) &&
                        (d.ApiUpDate == null || SqlFunctions.DateDiff("hour", d.ApiUpDate, DateTime.Now) > hour))
                    .FirstOrDefault();
            }
            

            if (media != null)
            {
                media.ApiUpDate = DateTime.Now;
                var detail = new JobDetail { Id = IdBuilder.CreateIdNum(), RequestDate = DateTime.Now, IsSuccess = true };
                if (!string.IsNullOrWhiteSpace(media.MediaID))
                {
                    var url = job.ApiUrl + media.MediaID.Trim()+ "&beginDate="+ DateTime.Now.AddDays(-29).ToString("yyyy-MM-dd") + "&endDate="+DateTime.Now.AddDays(1).ToString("yyyy-MM-dd");
                    for (int i = 0; i < times; i++)
                    {
                        try
                        {
                            var rhtml = Ada.Core.Tools.HttpUtility.Get(url);
                            var result = JsonConvert.DeserializeObject<WeiXinProJSON>(rhtml);
                            if (result.retcode == ReturnCode.请求成功)
                            {
                                if (result.data.Any())
                                {
                                    var cache = _cacheService.GetObject<List<string>>("WEIXINBRAND");
                                    if (cache == null)
                                    {
                                        var brandList = _fieldRepository.LoadEntities(d => d.FieldType.CallIndex == "Brand").Select(d => d.Text).ToList();
                                        cache = brandList;
                                        _cacheService.Put("WEIXINBRAND", brandList, TimeSpan.FromDays(180));
                                    }
                                    List<string> brands = cache as List<string>;
                                    DateTime? lastPush = null;
                                    int index = 1;
                                    foreach (var articleData in result.data.OrderByDescending(d => d.publishDate))
                                    {
                                        if (string.IsNullOrWhiteSpace(articleData.id))
                                        {
                                            continue;
                                        }
                                        if (articleData.id.Length > 128)
                                        {
                                            continue;
                                        }

                                        DateTime? pubDate = null;
                                        if (DateTime.TryParse(articleData.publishDateStr, out var publishDate))
                                        {
                                            if (index == 1)
                                            {
                                                lastPush = publishDate;
                                            }

                                            pubDate = publishDate;
                                        }
                                        var article = media.MediaArticles.FirstOrDefault(d => d.ArticleId == articleData.id);
                                        if (article == null)
                                        {
                                            article = new MediaArticle();
                                            article.Id = IdBuilder.CreateIdNum();
                                            article.ArticleId = articleData.id;
                                            article.ArticleIdx = articleData.idx;
                                            article.ArticleUrl = articleData.url;
                                            article.IsOriginal = articleData.origin;
                                            article.Biz = articleData.biz;
                                            article.CommentCount = articleData.commentCount;
                                            article.Content = GetBrands(articleData.content, brands);
                                            article.IsTop = articleData.isTop;
                                            article.PublishDate = pubDate;
                                            article.LikeCount = articleData.likeCount;
                                            article.ViewCount = articleData.viewCount;
                                            article.Title = articleData.title;
                                            if (!string.IsNullOrWhiteSpace(articleData.posterId))
                                            {
                                                media.MediaID = articleData.posterId;
                                            }
                                            if (!string.IsNullOrWhiteSpace(articleData.posterScreenName))
                                            {
                                                media.MediaName = articleData.posterScreenName;
                                            }
                                            media.MediaArticles.Add(article);
                                        }
                                        else
                                        {
                                            if (articleData.viewCount != null)
                                            {
                                                article.ViewCount = articleData.viewCount;
                                            }
                                            if (articleData.likeCount != null)
                                            {
                                                article.LikeCount = articleData.likeCount;
                                            }
                                            if (articleData.commentCount != null)
                                            {
                                                article.CommentCount = articleData.commentCount;
                                            }
                                        }
                                        index++;
                                    }
                                    //更新文章统计
                                    var viewCount = media.MediaArticles.Where(d => d.IsTop == true).OrderByDescending(d => d.PublishDate).Take(10)
                                        .Average(d => d.ViewCount);
                                    media.AvgReadNum = Convert.ToInt32(viewCount);
                                    media.LastReadNum = media.MediaArticles.Where(l => l.IsTop == true)
                                        .OrderByDescending(a => a.PublishDate).FirstOrDefault()?.ViewCount;
                                    var start30 = DateTime.Now.Date.AddDays(-30);
                                    var end30 = DateTime.Now.AddDays(1).Date;
                                    var count = media.MediaArticles.Where(d => d.PublishDate >= start30 && d.PublishDate < end30).Select(d => new
                                    {
                                        Date = d.PublishDate.Value.ToString("yyyy-MM-dd")
                                    }).GroupBy(d => d.Date).Count();
                                    media.PublishFrequency = count;
                                    media.LastPushDate = lastPush;
                                }

                                detail.IsSuccess = true;
                                break;
                            }

                            if (result.retcode == ReturnCode.目标参数搜索没结果)
                            {
                                detail.IsSuccess = true;
                                break;
                            }
                            if (result.retcode == ReturnCode.用户帐号不存在)
                            {
                                //media.Content = result.message;
                                //media.Status = Consts.StateLock;
                                detail.IsSuccess = true;
                                break;
                            }
                        }
                        catch (Exception ex)
                        {
                            detail.Retmsg = "采集微信文章列表:" + media.MediaID + "异常，" + ex.Message;
                            detail.Retcode = "502";
                            detail.IsSuccess = false;
                        }
                    }
                }
                if (detail.IsSuccess == false && job.IsLog == true)
                {
                    detail.AddedDate = DateTime.Now;
                    job.JobDetails.Add(detail);
                }
                job.Remark = media.MediaID + "-" + DateTime.Now;


                _mediaRepository.Update(media);

            }
            else
            {
                job.Remark = "暂无可采集数据";
            }
            if (context.NextFireTimeUtc != null)
            {
                job.NextTime = context.NextFireTimeUtc.Value.ToLocalTime().DateTime;
            }
            _repository.Update(job);
            _dbContext.SaveChanges();
        }


        private string GetBrands(string content, List<string> brands)
        {
            string htmlstr = string.Empty;
            int request = 1;
            while (request <= 3)
            {
                try
                {
                    htmlstr = Ada.Core.Tools.HttpUtility.Post(
                        "http://api01.idataapi.cn:8000/nlp/segment/bitspaceman?apikey=aHkIQg6KZL5nKgqhcAbrT7AYq484DkAfmFzd8rBgYDrK6CItsvAAOWwz7BiFkoQx",
                        new List<KeyValuePair<string, string>>() { new KeyValuePair<string, string>("text", content) });
                    request = 9999;
                }
                catch (Exception)
                {
                    request++;
                }
            }
            if (!string.IsNullOrWhiteSpace(htmlstr))
            {
                var resultJson = JsonConvert.DeserializeObject<BitspacemanJSON>(htmlstr);
                if (resultJson.wordList.Any())
                {
                    var words = resultJson.wordList.Where(d => d.length > 1 && brands.Contains(d.word)).Select(d => d.word);
                    words = words.Distinct().ToList();
                    return string.Join(",", words);
                }
            }
            return null;
        }
        public void RedBookInfo(IJobExecutionContext context)
        {
            var name = context.JobDetail.Key.Name;
            var group = context.JobDetail.Key.Group;
            var job = _repository.LoadEntities(d => d.JobName == name && d.GroupName == group).FirstOrDefault();
            var times = job.Repetitions ?? 3;
            var hour = job.Taxis ?? 360;
            var media = _mediaRepository.LoadEntities(d =>
                    d.IsDelete == false &&
                    d.IsSlide == true &&
                    d.MediaType.CallIndex == "redbook" &&
                    d.Status == Consts.StateNormal &&
                    (d.CollectionDate == null || SqlFunctions.DateDiff("hour", d.CollectionDate, DateTime.Now) > hour))
                .FirstOrDefault();
            if (media != null)
            {
                media.CollectionDate = DateTime.Now;
                bool isSuccess = true;
                string errMsg = null;
                if (!string.IsNullOrWhiteSpace(media.MediaID))
                {
                    var url = job.ApiUrl + media.MediaID;
                    for (int i = 0; i < times; i++)
                    {
                        try
                        {
                            var rhtml = Ada.Core.Tools.HttpUtility.Get(url);
                            var result = JsonConvert.DeserializeObject<RedBookJSON>(rhtml);
                            if (result.retcode == ReturnCode.请求成功)
                            {
                                if (result.data.Any())
                                {
                                    var info = result.data.FirstOrDefault();
                                    if (info != null)
                                    {
                                        media.MediaName = Utils.FilterEmoji(info.screenName);
                                        media.Content = info.biography;
                                        media.FansNum = info.fansCount;
                                        media.PostNum = info.postCount;
                                        media.MediaLink = info.url;
                                        media.FriendNum = info.followCount;
                                        media.LikesNum = info.likeCount;
                                        media.MediaLogo = info.avatarUrl;
                                        media.AuthenticateType = info.idGrade;
                                        if (!string.IsNullOrWhiteSpace(info.location))
                                        {
                                            if (info.location.Length <= 30)
                                            {
                                                media.Area = info.location;
                                            }
                                        }
                                    }
                                }
                                isSuccess = true;
                                break;
                            }

                            if (result.retcode == ReturnCode.目标参数搜索没结果 || result.retcode == ReturnCode.用户帐号不存在)
                            {
                                media.Content = result.message;
                                isSuccess = true;
                                break;
                            }
                        }
                        catch (Exception ex)
                        {
                            errMsg = ex.Message;
                            isSuccess = false;
                        }
                    }
                }
                if (isSuccess == false && job.IsLog == true)
                {
                    var detail = new JobDetail
                    {
                        Id = IdBuilder.CreateIdNum(),
                        RequestDate = DateTime.Now,
                        AddedDate = DateTime.Now,
                        IsSuccess = false,
                        Retcode = "502",
                        Retmsg = "采集小红书用户信息:" + media.MediaID + "异常，" + errMsg
                    };
                    job.JobDetails.Add(detail);
                }
                job.Remark = media.MediaID + "-" + DateTime.Now;
                _mediaRepository.Update(media);

            }
            else
            {
                job.Remark = "暂无可采集数据";
            }
            if (context.NextFireTimeUtc != null)
            {
                job.NextTime = context.NextFireTimeUtc.Value.ToLocalTime().DateTime;
            }
            _repository.Update(job);
            _dbContext.SaveChanges();
        }
        public void RedBookArticle(IJobExecutionContext context)
        {
            var name = context.JobDetail.Key.Name;
            var group = context.JobDetail.Key.Group;
            var job = _repository.LoadEntities(d => d.JobName == name && d.GroupName == group).FirstOrDefault();
            var times = job.Repetitions ?? 3;
            var hour = job.Taxis ?? 48;
            var media = _mediaRepository.LoadEntities(d =>
                    d.IsDelete == false &&
                    d.IsSlide == true &&
                    d.MediaType.CallIndex == "redbook" &&
                    d.Status == Consts.StateNormal &&
                    (d.ApiUpDate == null || SqlFunctions.DateDiff("hour", d.ApiUpDate, DateTime.Now) > hour))
                .FirstOrDefault();
            if (media != null)
            {
                media.ApiUpDate = DateTime.Now;
                bool isSuccess = true;
                string errMsg = null;
                if (!string.IsNullOrWhiteSpace(media.MediaID))
                {
                    var url = job.ApiUrl + media.MediaID.Trim();
                    for (int i = 0; i < times; i++)
                    {
                        try
                        {
                            var rhtml = Ada.Core.Tools.HttpUtility.Get(url);
                            var result = JsonConvert.DeserializeObject<RedBookArticleJSON>(rhtml);
                            if (result.retcode == ReturnCode.请求成功)
                            {
                                if (result.data.Any())
                                {
                                    
                                    foreach (var articleData in result.data)
                                    {
                                        if (string.IsNullOrWhiteSpace(articleData.id))
                                        {
                                            continue;
                                        }
                                        if (articleData.id.Length > 128)
                                        {
                                            continue;
                                        }
                                        var article = media.MediaArticles.FirstOrDefault(d => d.ArticleId == articleData.id);
                                        if (article != null)
                                        {
                                            continue;
                                        }
                                        article = new MediaArticle();
                                        article.Id = IdBuilder.CreateIdNum();
                                        article.ArticleId = articleData.id;
                                        article.Title = articleData.title;
                                        GetRedBookArticle(article);
                                        media.MediaArticles.Add(article);
                                    }
                                }
                                isSuccess = true;
                                break;
                            }

                            if (result.retcode == ReturnCode.目标参数搜索没结果)
                            {
                                isSuccess = true;
                                break;
                            }
                            if (result.retcode == ReturnCode.用户帐号不存在)
                            {
                                media.Status = Consts.StateLock;
                                isSuccess = true;
                                break;
                            }
                        }
                        catch (Exception ex)
                        {
                            errMsg = ex.Message;
                            isSuccess = false;
                        }
                    }
                }
                if (isSuccess == false && job.IsLog == true)
                {
                    var detail = new JobDetail
                    {
                        Id = IdBuilder.CreateIdNum(),
                        RequestDate = DateTime.Now,
                        AddedDate = DateTime.Now,
                        IsSuccess = false,
                        Retcode = "502",
                        Retmsg = "采集小红书文章列表信息:" + media.MediaID + "异常，" + errMsg
                    };
                    job.JobDetails.Add(detail);
                }
                job.Remark = media.MediaID + "-" + DateTime.Now;
                //更新统计
                var viewCount = media.MediaArticles.OrderByDescending(d => d.PublishDate).Take(50)
                    .Average(d => d.ViewCount);
                media.AvgReadNum = Convert.ToInt32(viewCount);
                //平均评论数
                media.CommentNum = Convert.ToInt32(media.MediaArticles.OrderByDescending(a => a.PublishDate)
                    .Take(50).Average(aaa => aaa.CommentCount));
                //平均点赞数
                media.AvgReadNum = Convert.ToInt32(media.MediaArticles.OrderByDescending(a => a.PublishDate)
                    .Take(50).Average(aaa => aaa.LikeCount));
                //平均收藏数
                media.TransmitNum = Convert.ToInt32(media.MediaArticles.OrderByDescending(a => a.PublishDate)
                    .Take(50).Average(aaa => aaa.ShareCount));
                _mediaRepository.Update(media);

            }
            else
            {
                job.Remark = "暂无可采集数据";
            }
            if (context.NextFireTimeUtc != null)
            {
                job.NextTime = context.NextFireTimeUtc.Value.ToLocalTime().DateTime;
            }
            _repository.Update(job);
            _dbContext.SaveChanges();
        }

        private void GetRedBookArticle(MediaArticle article)
        {
            for (int i = 0; i < 3; i++)
            {
                try
                {
                    var url =
                                "http://api01.idataapi.cn:8000/post/xiaohongshu_ids?apikey=aHkIQg6KZL5nKgqhcAbrT7AYq484DkAfmFzd8rBgYDrK6CItsvAAOWwz7BiFkoQx&id=" +
                                article.ArticleId;
                    var rhtml = Ada.Core.Tools.HttpUtility.Get(url);
                    var result = JsonConvert.DeserializeObject<RedBookArticleJSON>(rhtml);
                    if (result.retcode != ReturnCode.请求成功) continue;
                    if (!result.data.Any()) continue;
                    var articleData = result.data[0];
                    article.LikeCount = articleData.likeCount;
                    if (DateTime.TryParse(articleData.publishDateStr, out var date))
                    {
                        article.PublishDate = date;
                    }
                    article.ShareCount = articleData.favoriteCount;
                    article.CommentCount = articleData.commentCount;
                    article.ArticleUrl = articleData.url;
                    article.Content = articleData.content;
                    break;
                }
                catch
                {
                    //
                }
            }
        }

        public void WeiboArticle(IJobExecutionContext context)
        {
            var name = context.JobDetail.Key.Name;
            var group = context.JobDetail.Key.Group;
            var job = _repository.LoadEntities(d => d.JobName == name && d.GroupName == group).FirstOrDefault();
            var times = job.Repetitions ?? 3;
            var hour = job.Taxis ?? 48;
            var media = _mediaRepository.LoadEntities(d =>
                    d.IsDelete == false &&
                    d.IsSlide == true &&
                    d.MediaType.CallIndex == "sinablog" &&
                    d.Status == Consts.StateNormal &&
                    (d.CollectionDate == null || SqlFunctions.DateDiff("hour", d.CollectionDate, DateTime.Now) > hour))
                .FirstOrDefault();
            if (media != null)
            {
                media.CollectionDate = DateTime.Now;
                bool isSuccess = true;
                string errMsg = null;
                if (!string.IsNullOrWhiteSpace(media.MediaID))
                {
                    var url = job.ApiUrl + media.MediaID.Trim();
                    for (int i = 0; i < times; i++)
                    {
                        try
                        {
                            var rhtml = Ada.Core.Tools.HttpUtility.Get(url);
                            var result = JsonConvert.DeserializeObject<WeiBoJSON>(rhtml);
                            if (result.retcode == ReturnCode.请求成功)
                            {
                                if (result.data.Any())
                                {
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
                                        if (string.IsNullOrWhiteSpace(articleData.id))
                                        {
                                            continue;
                                        }
                                        if (articleData.id.Length > 128)
                                        {
                                            continue;
                                        }
                                        var article = media.MediaArticles.FirstOrDefault(d => d.ArticleId == articleData.id);
                                        if (article != null)
                                        {
                                            article.ArticleUrl = articleData.url;
                                            article.CommentCount = articleData.commentCount;
                                            article.Content = articleData.content;
                                            article.ShareCount = articleData.shareCount;
                                            if (DateTime.TryParse(articleData.publishDateStr, out var date))
                                            {
                                                article.PublishDate = date;
                                            }
                                            article.LikeCount = articleData.likeCount;
                                            article.ViewCount = articleData.viewCount;
                                            article.Title = articleData.title;
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
                                            if (DateTime.TryParse(articleData.publishDateStr, out var date))
                                            {
                                                article.PublishDate = date;
                                            }
                                            article.LikeCount = articleData.likeCount;
                                            article.ViewCount = articleData.viewCount;
                                            article.Title = articleData.title;
                                            media.MediaArticles.Add(article);
                                        }
                                    }
                                }
                                isSuccess = true;
                                media.TransmitNum = Convert.ToInt32(media.MediaArticles.OrderByDescending(a => a.PublishDate).Take(50)
                                                    .Average(aaa => aaa.ShareCount));
                                media.CommentNum = Convert.ToInt32(media.MediaArticles.OrderByDescending(a => a.PublishDate)
                                    .Take(50).Average(aaa => aaa.CommentCount));
                                media.LikesNum = Convert.ToInt32(media.MediaArticles.OrderByDescending(a => a.PublishDate).Take(50)
                                    .Average(aaa => aaa.LikeCount));
                                media.LastPushDate = media.MediaArticles.OrderByDescending(a => a.PublishDate).FirstOrDefault()?.PublishDate;
                                var weekDate = DateTime.Now.Date.AddDays(-7);
                                media.MonthPostNum = media.MediaArticles.OrderByDescending(a => a.PublishDate)
                                    .Count(l => l.PublishDate > weekDate);
                                break;
                            }

                            if (result.retcode == ReturnCode.目标参数搜索没结果)
                            {
                                isSuccess = true;
                                break;
                            }
                            if (result.retcode == ReturnCode.用户帐号不存在)
                            {
                                media.Status = Consts.StateLock;
                                isSuccess = true;
                                break;
                            }
                        }
                        catch (Exception ex)
                        {
                            errMsg = ex.Message;
                            isSuccess = false;
                        }
                    }
                }
                if (isSuccess == false && job.IsLog == true)
                {
                    var detail = new JobDetail
                    {
                        Id = IdBuilder.CreateIdNum(),
                        RequestDate = DateTime.Now,
                        AddedDate = DateTime.Now,
                        IsSuccess = false,
                        Retcode = "502",
                        Retmsg = "采集微博信息:" + media.MediaID + "异常，" + errMsg
                    };
                    job.JobDetails.Add(detail);
                }
                job.Remark = media.MediaID + "-" + DateTime.Now;
                _mediaRepository.Update(media);

            }
            else
            {
                job.Remark = "暂无可采集数据";
            }
            if (context.NextFireTimeUtc != null)
            {
                job.NextTime = context.NextFireTimeUtc.Value.ToLocalTime().DateTime;
            }
            _repository.Update(job);
            _dbContext.SaveChanges();
        }
    }
}