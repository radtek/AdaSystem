﻿using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Ada.Core;
using Ada.Core.Domain.API;
using Ada.Core.Domain.Resource;
using Ada.Core.Tools;
using Ada.Core.ViewModel.API;
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
            for (int i = 1; i <= page; i++)
            {
                var apiUrl = urlparams + "&pageToken=" + i;
                string htmlstr = string.Empty;
                int request = 1;
                while (request <= apiInfo.TimeOut)
                {
                    try
                    {
                        htmlstr = HttpUtility.Get(url + apiUrl);
                        request = 9999;
                    }
                    catch (Exception ex)
                    {
                        if (request == apiInfo.TimeOut)
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
            for (int i = 1; i <= page; i++)
            {
                var apiUrl = urlparams + "&pageToken=" + i;
                string htmlstr = string.Empty;
                int request = 1;
                while (request <= apiInfo.TimeOut)
                {
                    try
                    {
                        htmlstr = HttpUtility.Get(url + apiUrl);
                        request = 9999;
                    }
                    catch (Exception ex)
                    {
                        if (request == apiInfo.TimeOut)
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
                    var authType = GetAuthenticateType(mediaInfo.extend?.verified_type);
                    if (!string.IsNullOrWhiteSpace(authType))
                    {
                        media.AuthenticateType = authType;
                    }
                    var sex = GetSex(mediaInfo.extend?.gender);
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

        private string GetAuthenticateType(int? verifiedtype)
        {
            if (verifiedtype != null)
            {
                string type = null;
                switch (verifiedtype)
                {
                    case 0:
                        type = "黄V";
                        break;
                    case 1:
                    case 2:
                    case 3:
                    case 4:
                    case 5:
                    case 6:
                    case 7:
                    case 8:
                        type = "蓝V";
                        break;
                    case 200:
                    case 220:
                    case 400:
                        type = "达人";
                        break;
                }

                return type;
            }

            return null;
        }
        private string GetSex(string sex)
        {
            if (string.IsNullOrWhiteSpace(sex)) return null;
            string temp = null;
            switch (sex)
            {
                case "m":
                    temp = "男";
                    break;
                case "f":
                    temp = "女";
                    break;
            }
            return temp;

        }
    }
}
