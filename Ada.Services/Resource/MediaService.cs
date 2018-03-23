using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.SqlServer;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ada.Core;
using Ada.Core.Domain;
using Ada.Core.Domain.Admin;
using Ada.Core.Domain.Resource;
using Ada.Core.Tools;
using Ada.Core.ViewModel.Resource;

namespace Ada.Services.Resource
{
    public class MediaService : IMediaService
    {
        private readonly IDbContext _dbContext;
        private readonly IRepository<Media> _repository;
        private readonly IRepository<MediaArticle> _mediaArticleRepository;
        private readonly IRepository<Manager> _managerRepository;
        private readonly IRepository<MediaComment> _mediaCommentRepository;
        public MediaService(IRepository<Media> repository,
            IDbContext dbContext,
            IRepository<Manager> managerRepository,
            IRepository<MediaComment> mediaCommentRepository,
            IRepository<MediaArticle> mediaArticleRepository)
        {
            _repository = repository;
            _dbContext = dbContext;
            _managerRepository = managerRepository;
            _mediaCommentRepository = mediaCommentRepository;
            _mediaArticleRepository = mediaArticleRepository;
        }

        public void Add(Media entity)
        {
            _repository.Add(entity);
            _dbContext.SaveChanges();
        }

        public void Delete(Media entity)
        {
            _repository.Delete(entity);
            _dbContext.SaveChanges();
        }

        public IQueryable<Media> LoadEntitiesFilter(MediaView viewModel)
        {
            var allList = _repository.LoadEntities(d => d.IsDelete == false);
            var isInclud = false;
            if (viewModel.Managers != null && viewModel.Managers.Count > 0)
            {
                allList = allList.Where(d => viewModel.Managers.Contains(d.TransactorId));
            }
            if (!string.IsNullOrWhiteSpace(viewModel.MediaTypeIndex))
            {
                allList = allList.Where(d => d.MediaType.CallIndex == viewModel.MediaTypeIndex);
            }
            if (!string.IsNullOrWhiteSpace(viewModel.MediaTypeId))
            {
                allList = allList.Where(d => d.MediaTypeId == viewModel.MediaTypeId);
            }
            if (!string.IsNullOrWhiteSpace(viewModel.search))
            {
                allList = allList.Where(d => d.MediaName.Contains(viewModel.search) || d.MediaID.Contains(viewModel.search));
            }
            if (!string.IsNullOrWhiteSpace(viewModel.MediaName))
            {
                allList = allList.Where(d => d.MediaName.Contains(viewModel.MediaName));
            }
            if (!string.IsNullOrWhiteSpace(viewModel.Content))
            {
                allList = allList.Where(d => d.Content.Contains(viewModel.Content));
            }
            if (!string.IsNullOrWhiteSpace(viewModel.Remark))
            {
                allList = allList.Where(d => d.Remark.Contains(viewModel.Remark));
            }
            if (viewModel.HasArticles != null)
            {
                allList = !viewModel.HasArticles.Value ? allList.Where(d => !d.MediaArticles.Any()) : allList.Where(d => d.MediaArticles.Any());
            }
            if (viewModel.IsTop != null)
            {
                allList = allList.Where(d => d.IsTop == viewModel.IsTop);
            }
            if (viewModel.IsHot != null)
            {
                allList = allList.Where(d => d.IsHot == viewModel.IsHot);
            }
            if (viewModel.IsRecommend != null)
            {
                allList = allList.Where(d => d.IsRecommend == viewModel.IsRecommend);
            }
            if (viewModel.IsSlide != null)
            {
                allList = allList.Where(d => d.IsSlide == viewModel.IsSlide);
            }
            if (viewModel.Status != null)
            {
                allList = allList.Where(d => d.Status == viewModel.Status);
            }
            if (viewModel.IsAuthenticate != null)
            {
                allList = allList.Where(d => d.IsAuthenticate == viewModel.IsAuthenticate);
            }
            if (viewModel.IsGroup != null)
            {
                allList = viewModel.IsGroup.Value ? allList.Where(d => d.MediaGroups.Any()) : allList.Where(d => !d.MediaGroups.Any());
            }

            if (viewModel.AvgReadNumStart != null)
            {
                allList = allList.Where(d =>d.AvgReadNum >=viewModel.AvgReadNumStart);
            }
            if (viewModel.AvgReadNumEnd != null)
            {
                allList = allList.Where(d =>d.AvgReadNum <=viewModel.AvgReadNumEnd);
            }
            if (!string.IsNullOrWhiteSpace(viewModel.Areas))
            {
                allList = allList.Where(d => d.Area.Contains(viewModel.Areas));
            }
            if (!string.IsNullOrWhiteSpace(viewModel.SEO))
            {
                allList = allList.Where(d => d.SEO.Contains(viewModel.SEO));
            }
            if (!string.IsNullOrWhiteSpace(viewModel.Efficiency))
            {
                allList = allList.Where(d => d.Efficiency.Contains(viewModel.Efficiency));
            }
            if (!string.IsNullOrWhiteSpace(viewModel.ResourceType))
            {
                allList = allList.Where(d => d.ResourceType.Contains(viewModel.ResourceType));
            }
            if (!string.IsNullOrWhiteSpace(viewModel.Platform))
            {
                allList = allList.Where(d => d.Platform.Contains(viewModel.Platform));
            }
            if (!string.IsNullOrWhiteSpace(viewModel.Channel))
            {
                allList = allList.Where(d => d.Channel.Contains(viewModel.Channel));
            }
            if (!string.IsNullOrWhiteSpace(viewModel.MediaID))
            {
                allList = allList.Where(d => d.MediaID.Contains(viewModel.MediaID));
            }
            if (!string.IsNullOrWhiteSpace(viewModel.LinkManId))
            {
                allList = allList.Where(d => d.LinkManId == viewModel.LinkManId);
            }
            if (!string.IsNullOrWhiteSpace(viewModel.LinkManName))
            {
                allList = allList.Where(d => d.LinkMan.Name.Contains(viewModel.LinkManName));
            }
            if (!string.IsNullOrWhiteSpace(viewModel.Transactor))
            {
                allList = allList.Where(d => d.Transactor.Contains(viewModel.Transactor));
            }
            if (!string.IsNullOrWhiteSpace(viewModel.MediaNames))
            {
                viewModel.MediaNames = viewModel.MediaNames.Trim().Replace("\r\n", ",").Replace("，", ",").Replace(" ", ",");
                var mediaNames = viewModel.MediaNames.Split(',').Distinct().Where(d => !string.IsNullOrWhiteSpace(d)).ToList();
                allList = allList.Where(d => mediaNames.Contains(d.MediaName));
            }
            if (!string.IsNullOrWhiteSpace(viewModel.MediaIDs))
            {
                viewModel.MediaIDs = viewModel.MediaIDs.Trim().Replace("\r\n", ",").Replace("，", ",").Replace(" ", ",");
                var mediaIDs = viewModel.MediaIDs.Split(',').Distinct().Where(d => !string.IsNullOrWhiteSpace(d)).ToList();
                allList = allList.Where(d => mediaIDs.Contains(d.MediaID));
            }
            if (viewModel.MediaTagIds != null)
            {
                allList = from m in allList
                          from t in m.MediaTags
                          where viewModel.MediaTagIds.Contains(t.Id)
                          select m;
            }
            if (!string.IsNullOrWhiteSpace(viewModel.AdPositionName))
            {
                if (!isInclud)
                {
                    allList = allList.Include(d => d.MediaPrices);
                    isInclud = true;
                }
                if (viewModel.PriceStart != null)
                {
                    allList = allList.Where(d =>
                        d.MediaPrices.FirstOrDefault(a => a.AdPositionName == viewModel.AdPositionName).PurchasePrice >=
                        viewModel.PriceStart);
                }
                if (viewModel.PriceEnd != null)
                {
                    allList = allList.Where(d =>
                        d.MediaPrices.FirstOrDefault(a => a.AdPositionName == viewModel.AdPositionName).PurchasePrice <=
                        viewModel.PriceEnd);
                }
            }
            if (viewModel.PriceInvalidDate != null)
            {
                if (!isInclud)
                {
                    allList = allList.Include(d => d.MediaPrices);
                }
                var endDate = viewModel.PriceInvalidDate.Value.AddDays(1);
                allList = allList.Where(d =>
                    d.MediaPrices.FirstOrDefault().InvalidDate <
                    endDate);
            }
            //allList = allList.Distinct();
            viewModel.total = allList.Count();
            int offset = viewModel.offset ?? 0;
            int rows = viewModel.limit ?? 10;
            string order = string.IsNullOrWhiteSpace(viewModel.order) ? "desc" : viewModel.order;
            if (order == "desc")
            {
                return allList.OrderByDescending(d=>d.IsTop).ThenByDescending(d => d.IsHot).ThenByDescending(d=>d.IsRecommend).ThenByDescending(d => d.Id).Skip(offset).Take(rows);
            }
            return allList.OrderByDescending(d => d.IsTop).ThenByDescending(d => d.IsHot).ThenByDescending(d => d.IsRecommend).ThenBy(d => d.Id).Skip(offset).Take(rows);
        }
        public IQueryable<MediaView> LoadEntitiesFilters(MediaView viewModel)
        {
            var allList = _repository.LoadEntities(d => d.IsDelete == false);
            if (viewModel.Managers != null && viewModel.Managers.Count > 0)
            {
                allList = allList.Where(d => viewModel.Managers.Contains(d.TransactorId));
            }
            if (!string.IsNullOrWhiteSpace(viewModel.MediaTypeIndex))
            {
                allList = allList.Where(d => d.MediaType.CallIndex == viewModel.MediaTypeIndex);
            }
            if (!string.IsNullOrWhiteSpace(viewModel.MediaTypeId))
            {
                allList = allList.Where(d => d.MediaTypeId == viewModel.MediaTypeId);
            }
            if (!string.IsNullOrWhiteSpace(viewModel.search))
            {
                allList = allList.Where(d => d.MediaName.Contains(viewModel.search) || d.MediaID.Contains(viewModel.search));
            }
            if (!string.IsNullOrWhiteSpace(viewModel.MediaName))
            {
                allList = allList.Where(d => d.MediaName.Contains(viewModel.MediaName));
            }
            if (!string.IsNullOrWhiteSpace(viewModel.Content))
            {
                allList = allList.Where(d => d.Content.Contains(viewModel.Content));
            }
            if (!string.IsNullOrWhiteSpace(viewModel.Remark))
            {
                allList = allList.Where(d => d.Remark.Contains(viewModel.Remark));
            }
            if (viewModel.HasArticles != null)
            {
                allList = !viewModel.HasArticles.Value ? allList.Where(d => d.MediaArticles.Count == 0) : allList.Where(d => d.MediaArticles.Count > 0);
            }
            if (viewModel.IsSlide != null)
            {
                allList = allList.Where(d => d.IsSlide == viewModel.IsSlide);
            }
            if (!string.IsNullOrWhiteSpace(viewModel.Areas))
            {
                allList = allList.Where(d => d.Area.Contains(viewModel.Areas));
            }
            if (!string.IsNullOrWhiteSpace(viewModel.SEO))
            {
                allList = allList.Where(d => d.SEO.Contains(viewModel.SEO));
            }
            if (!string.IsNullOrWhiteSpace(viewModel.Efficiency))
            {
                allList = allList.Where(d => d.Efficiency.Contains(viewModel.Efficiency));
            }
            if (!string.IsNullOrWhiteSpace(viewModel.ResourceType))
            {
                allList = allList.Where(d => d.ResourceType.Contains(viewModel.ResourceType));
            }
            if (!string.IsNullOrWhiteSpace(viewModel.Platform))
            {
                allList = allList.Where(d => d.Platform.Contains(viewModel.Platform));
            }
            if (!string.IsNullOrWhiteSpace(viewModel.Channel))
            {
                allList = allList.Where(d => d.Channel.Contains(viewModel.Channel));
            }
            if (!string.IsNullOrWhiteSpace(viewModel.MediaID))
            {
                allList = allList.Where(d => d.MediaID.Contains(viewModel.MediaID));
            }
            if (!string.IsNullOrWhiteSpace(viewModel.LinkManId))
            {
                allList = allList.Where(d => d.LinkManId == viewModel.LinkManId);
            }
            if (!string.IsNullOrWhiteSpace(viewModel.LinkManName))
            {
                allList = allList.Where(d => d.LinkMan.Name.Contains(viewModel.LinkManName));
            }
            if (!string.IsNullOrWhiteSpace(viewModel.Transactor))
            {
                allList = allList.Where(d => d.Transactor.Contains(viewModel.Transactor));
            }
            if (!string.IsNullOrWhiteSpace(viewModel.MediaNames))
            {
                viewModel.MediaNames = viewModel.MediaNames.Trim().Replace("\r\n", ",").Replace("，", ",").Replace(" ", ",");
                var mediaNames = viewModel.MediaNames.Split(',').Distinct().Where(d => !string.IsNullOrWhiteSpace(d)).ToList();
                allList = allList.Where(d => mediaNames.Contains(d.MediaName));
            }
            if (!string.IsNullOrWhiteSpace(viewModel.MediaIDs))
            {
                viewModel.MediaIDs = viewModel.MediaIDs.Trim().Replace("\r\n", ",").Replace("，", ",").Replace(" ", ",");
                var mediaIDs = viewModel.MediaIDs.Split(',').Distinct().Where(d => !string.IsNullOrWhiteSpace(d)).ToList();
                allList = allList.Where(d => mediaIDs.Contains(d.MediaID));
            }
            if (viewModel.MediaTagIds != null)
            {
                allList = from m in allList
                          from t in m.MediaTags
                          where viewModel.MediaTagIds.Contains(t.Id)
                          select m;
            }
            if (!string.IsNullOrWhiteSpace(viewModel.AdPositionName))
            {
                if (viewModel.PriceStart != null)
                {
                    allList = allList.Where(d =>
                        d.MediaPrices.FirstOrDefault(a => a.AdPositionName == viewModel.AdPositionName).PurchasePrice >=
                        viewModel.PriceStart);
                }
                if (viewModel.PriceEnd != null)
                {
                    allList = allList.Where(d =>
                        d.MediaPrices.FirstOrDefault(a => a.AdPositionName == viewModel.AdPositionName).PurchasePrice <=
                        viewModel.PriceEnd);
                }
            }

            var result = allList.Select(d => new MediaView
            {
                Id = d.Id,
                MediaName = d.MediaName,
                MediaID = d.MediaID,
                MediaTypeIndex = d.MediaType.CallIndex,
                MediaTypeName = d.MediaType.TypeName,
                IsAuthenticate = d.IsAuthenticate,
                IsOriginal = d.IsOriginal,
                IsComment = d.IsComment,
                FansNum = d.FansNum,
                ChannelType = d.ChannelType,
                LastReadNum = d.MediaArticles.Where(l => l.IsTop == true).OrderByDescending(a => a.PublishDate).FirstOrDefault().ViewCount,
                AvgReadNum = (int?)d.MediaArticles.OrderByDescending(a => a.PublishDate).Take(10).Average(a => a.ViewCount),
                PublishFrequency = d.PublishFrequency,
                Areas = d.Area,
                Sex = d.Sex,
                Client = d.Client,
                SEO = d.SEO,
                Abstract = d.Abstract,
                PostNum = d.PostNum,
                MonthPostNum = d.MonthPostNum,
                FriendNum = d.FriendNum,
                Efficiency = d.Efficiency,
                ResourceType = d.ResourceType,
                Channel = d.Channel,
                LastPushDate = d.LastPushDate,
                AuthenticateType = d.AuthenticateType,
                Platform = d.Platform,
                TransmitNum = (int?)d.MediaArticles.OrderByDescending(a => a.PublishDate).Take(50).Average(aaa => aaa.ShareCount),
                CommentNum = (int?)d.MediaArticles.OrderByDescending(a => a.PublishDate).Take(50).Average(aaa => aaa.CommentCount),
                LikesNum = (int?)d.MediaArticles.OrderByDescending(a => a.PublishDate).Take(50).Average(aaa => aaa.LikeCount),
                BlogLastPushDate = d.MediaArticles.OrderByDescending(a => a.PublishDate).FirstOrDefault().PublishDate,
                WeekArticleCount = d.MediaArticles.OrderByDescending(a => a.PublishDate).Count(l => SqlFunctions.DateDiff("day", l.PublishDate, DateTime.Now) <= 7),
                Content = d.Content,
                Remark = d.Remark,
                Status = d.Status,
                ApiUpDate = d.ApiUpDate,
                MediaLink = d.MediaLink,
                MediaLogo = d.MediaLogo,
                MediaQR = d.MediaQR,
                LinkManId = d.LinkManId,
                LinkManName = d.LinkMan.Name,
                Transactor = d.Transactor,
                MediaGroups = d.MediaGroups.Select(g => new MediaGroupView() { Id = g.Id, GroupName = g.GroupName }).ToList(),
                MediaTags = d.MediaTags.Select(t => new MediaTagView() { Id = t.Id, TagName = t.TagName }).ToList(),
                MediaPrices = d.MediaPrices.Select(p => new MediaPriceView() { AdPositionName = p.AdPositionName, PriceDate = p.PriceDate, InvalidDate = p.InvalidDate, PurchasePrice = p.PurchasePrice }).ToList()
            });

            viewModel.total = result.Count();
            int offset = viewModel.offset ?? 0;
            int rows = viewModel.limit ?? 10;
            string order = string.IsNullOrWhiteSpace(viewModel.order) ? "desc" : viewModel.order;
            if (order == "desc")
            {
                return result.OrderByDescending(d => d.Id).Skip(offset).Take(rows);
            }
            return result.OrderBy(d => d.Id).Skip(offset).Take(rows);
        }
        public void Update(Media entity)
        {
            _repository.Update(entity);
            _dbContext.SaveChanges();
        }

        public IQueryable<MediaCommentView> LoadComments(string id, int pageindex, int pagesize, out int total)
        {
            var mediaComments = _mediaCommentRepository.LoadEntities(d => d.MediaId == id);
            var managers = _managerRepository.LoadEntities(d => true);

            var allList = from c in mediaComments
                          from m in managers
                          where c.TransactorId == m.Id
                          select new MediaCommentView()
                          {
                              Transactor = c.Transactor,
                              Content = c.Content,
                              Score = c.Score,
                              CommentDate = c.CommentDate,
                              TransactorImage = m.Image,
                              Organization = m.Organizations.FirstOrDefault().OrganizationName
                          };
            total = mediaComments.Count();
            return allList.OrderByDescending(d => d.CommentDate).Skip(0).Take(pageindex * pagesize);
        }
        public IQueryable<MediaCommentView> LoadComments(MediaCommentView viewModel)
        {
            var mediaComments = _mediaCommentRepository.LoadEntities(d => d.IsDelete == false);
            if (!string.IsNullOrWhiteSpace(viewModel.MediaId))
            {
                mediaComments = mediaComments.Where(d => d.MediaId == viewModel.MediaId);
            }
            var managers = _managerRepository.LoadEntities(d => true);

            var allList = from c in mediaComments
                          from m in managers
                          where c.TransactorId == m.Id
                          select new MediaCommentView()
                          {
                              Transactor = c.Transactor,
                              Content = c.Content,
                              Score = c.Score,
                              CommentDate = c.CommentDate,
                              TransactorImage = m.Image,
                              Organization = m.Organizations.FirstOrDefault().OrganizationName
                          };
            viewModel.total = mediaComments.Count();
            viewModel.AvgScore = mediaComments.Average(d => d.Score);
            int offset = viewModel.offset ?? 0;
            int rows = viewModel.limit ?? 10;
            string order = string.IsNullOrWhiteSpace(viewModel.order) ? "desc" : viewModel.order;
            if (order == "desc")
            {
                return allList.OrderByDescending(d => d.CommentDate).Skip(offset).Take(rows);
            }
            return allList.OrderBy(d => d.CommentDate).Skip(offset).Take(rows);
        }

    }
}
