﻿using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using Ada.Core;
using Ada.Core.Domain;
using Ada.Core.Domain.Admin;
using Ada.Core.Domain.Resource;
using Ada.Core.ViewModel.Resource;

namespace Ada.Services.Resource
{
    public class MediaService : IMediaService
    {
        private readonly IDbContext _dbContext;
        private readonly IRepository<Media> _repository;
        private readonly IRepository<Manager> _managerRepository;
        private readonly IRepository<MediaComment> _mediaCommentRepository;
        private readonly IRepository<MediaReferencePrice> _mediaReferenceRepository;
        public MediaService(IRepository<Media> repository,
            IDbContext dbContext,
            IRepository<Manager> managerRepository,
            IRepository<MediaComment> mediaCommentRepository,
            IRepository<MediaReferencePrice> mediaReferenceRepository)
        {
            _repository = repository;
            _dbContext = dbContext;
            _managerRepository = managerRepository;
            _mediaCommentRepository = mediaCommentRepository;
            _mediaReferenceRepository = mediaReferenceRepository;
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
            //var isInclud = false;
            if (viewModel.Managers != null && viewModel.Managers.Count > 0)
            {
                allList = allList.Where(d => viewModel.Managers.Contains(d.TransactorId));
            }
            if (!string.IsNullOrWhiteSpace(viewModel.MediaTypeIndex))
            {
                allList = allList.Where(d => d.MediaType.CallIndex == viewModel.MediaTypeIndex.Trim());
            }
            if (!string.IsNullOrWhiteSpace(viewModel.MediaTypeId))
            {
                allList = allList.Where(d => d.MediaTypeId == viewModel.MediaTypeId.Trim());
            }
            if (!string.IsNullOrWhiteSpace(viewModel.GroupId))
            {
                allList = allList.Where(d => d.MediaGroups.Any(g=>viewModel.GroupId.Contains(g.Id)));
            }
            if (!string.IsNullOrWhiteSpace(viewModel.Sex))
            {
                allList = allList.Where(d => d.Sex == viewModel.Sex);
            }
            if (!string.IsNullOrWhiteSpace(viewModel.RetentionTime))
            {
                allList = allList.Where(d => d.RetentionTime.Contains(viewModel.RetentionTime));
            }
            if (viewModel.Cooperation != null)
            {
                allList = allList.Where(d => d.Cooperation == viewModel.Cooperation);
            }
            if (!string.IsNullOrWhiteSpace(viewModel.search))
            {
                viewModel.search = viewModel.search.Trim();
                allList = allList.Where(d => d.MediaName.Contains(viewModel.search) || 
                                             d.MediaID.Contains(viewModel.search)
                                             //d.Abstract.Contains(viewModel.search)||
                                             //d.Transactor.Contains(viewModel.search)||
                                             //d.LinkMan.Name.Contains(viewModel.search)
                                             );
            }
            if (!string.IsNullOrWhiteSpace(viewModel.MediaName))
            {
                viewModel.MediaName = viewModel.MediaName.Trim();
                allList = allList.Where(d => d.MediaName.Contains(viewModel.MediaName));
            }
            if (!string.IsNullOrWhiteSpace(viewModel.ADKey))
            {
                viewModel.ADKey = viewModel.ADKey.Trim();
                allList = allList.Where(d => d.MediaArticles.Any(a=>a.Content.Contains(viewModel.ADKey)));
            }
            if (!string.IsNullOrWhiteSpace(viewModel.Content))
            {
                allList = allList.Where(d => d.Content.Contains(viewModel.Content.Trim()));
            }
            if (!string.IsNullOrWhiteSpace(viewModel.Remark))
            {
                allList = allList.Where(d => d.Remark.Contains(viewModel.Remark.Trim()));
            }
            if (!string.IsNullOrWhiteSpace(viewModel.Abstract))
            {
                allList = allList.Where(d => d.Abstract.Contains(viewModel.Abstract.Trim()));
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
                allList = viewModel.IsGroup.Value ? allList.Where(d => d.MediaGroups.Any(g=>g.GroupType== Consts.StateNormal)) : allList.Where(d => !d.MediaGroups.Any(g => g.GroupType == Consts.StateNormal));
            }
            if (viewModel.HaveTag != null)
            {
                allList = viewModel.HaveTag.Value ? allList.Where(d => d.MediaTags.Any()) : allList.Where(d => !d.MediaTags.Any());
            }
            if (viewModel.AvgReadNumStart != null)
            {
                allList = allList.Where(d => d.AvgReadNum >= viewModel.AvgReadNumStart);
            }
            if (viewModel.AvgReadNumEnd != null)
            {
                allList = allList.Where(d => d.AvgReadNum <= viewModel.AvgReadNumEnd);
            }
            if (viewModel.LikesNumMin != null)
            {
                allList = allList.Where(d => d.LikesNum >= viewModel.LikesNumMin);
            }
            if (viewModel.LikesNumMax != null)
            {
                allList = allList.Where(d => d.LikesNum <= viewModel.LikesNumMax);
            }
            if (viewModel.CommentNumMin != null)
            {
                allList = allList.Where(d => d.CommentNum >= viewModel.CommentNumMin);
            }
            if (viewModel.CommentNumMax != null)
            {
                allList = allList.Where(d => d.CommentNum <= viewModel.CommentNumMax);
            }
            if (viewModel.TransmitNumMin != null)
            {
                allList = allList.Where(d => d.TransmitNum >= viewModel.TransmitNumMin);
            }
            if (viewModel.TransmitNumMax != null)
            {
                allList = allList.Where(d => d.TransmitNum <= viewModel.TransmitNumMax);
            }
            if (viewModel.PostNumMin != null)
            {
                allList = allList.Where(d => d.PostNum >= viewModel.PostNumMin);
            }
            if (viewModel.PostNumMax != null)
            {
                allList = allList.Where(d => d.PostNum <= viewModel.PostNumMax);
            }
            if (viewModel.PublishFrequencyMin != null)
            {
                allList = allList.Where(d => d.PublishFrequency >= viewModel.PublishFrequencyMin);
            }
            if (viewModel.PublishFrequencyMax != null)
            {
                allList = allList.Where(d => d.PublishFrequency <= viewModel.PublishFrequencyMax);
            }
            if (!string.IsNullOrWhiteSpace(viewModel.Areas))
            {
                allList = allList.Where(d => d.Area.Contains(viewModel.Areas.Trim()));
            }
            if (!string.IsNullOrWhiteSpace(viewModel.SEO))
            {
                allList = allList.Where(d => d.SEO.Contains(viewModel.SEO.Trim()));
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
                allList = allList.Where(d => d.Platform.Contains(viewModel.Platform.Trim()));
            }
            if (!string.IsNullOrWhiteSpace(viewModel.Channel))
            {
                allList = allList.Where(d => d.Channel.Contains(viewModel.Channel));
            }
            if (!string.IsNullOrWhiteSpace(viewModel.ChannelType))
            {
                allList = allList.Where(d => d.ChannelType.Contains(viewModel.ChannelType));
            }
            if (!string.IsNullOrWhiteSpace(viewModel.MediaID))
            {
                viewModel.MediaID = viewModel.MediaID.Trim();
                allList = allList.Where(d => d.MediaID.Contains(viewModel.MediaID));
            }
            if (!string.IsNullOrWhiteSpace(viewModel.LinkManId))
            {
                allList = allList.Where(d => d.LinkManId == viewModel.LinkManId.Trim());
            }
            if (!string.IsNullOrWhiteSpace(viewModel.LinkManName))
            {
                viewModel.LinkManName = viewModel.LinkManName.Trim();
                allList = allList.Where(d => d.LinkMan.Name.Contains(viewModel.LinkManName));
            }
            if (!string.IsNullOrWhiteSpace(viewModel.Transactor))
            {
                viewModel.Transactor = viewModel.Transactor.Trim();
                allList = allList.Where(d => d.Transactor.Contains(viewModel.Transactor));
            }
            if (!string.IsNullOrWhiteSpace(viewModel.AuthenticateType))
            {
                allList = allList.Where(d => d.AuthenticateType == viewModel.AuthenticateType);
            }
            if (!string.IsNullOrWhiteSpace(viewModel.MediaBatch))
            {
                viewModel.MediaBatch = viewModel.MediaBatch.Trim().Replace("\r\n", ",").Replace("\n", ",").Replace("\t", ",").Replace("，", ",");
                var mediaNames = SplitStr(viewModel.MediaBatch);
                allList = allList.Where(d => mediaNames.Contains(d.MediaName) || mediaNames.Contains(d.MediaID));
            }
            if (!string.IsNullOrWhiteSpace(viewModel.MediaNames))
            {
                viewModel.MediaNames = viewModel.MediaNames.Trim().Replace("\r\n", ",").Replace("\n", ",").Replace("\t", ",").Replace("，", ",");
                var mediaNames = SplitStr(viewModel.MediaNames);
                allList = allList.Where(d => mediaNames.Contains(d.MediaName));
            }
            if (!string.IsNullOrWhiteSpace(viewModel.MediaIDs))
            {
                viewModel.MediaIDs = viewModel.MediaIDs.Trim().Replace("\r\n", ",").Replace("\n", ",").Replace("\t", ",").Replace("，", ",");
                var mediaIDs = SplitStr(viewModel.MediaIDs);
                allList = allList.Where(d => mediaIDs.Contains(d.MediaID));
            }
            if (!string.IsNullOrWhiteSpace(viewModel.DouYinIDs))
            {
                viewModel.DouYinIDs = viewModel.DouYinIDs.Trim().Replace("\r\n", ",").Replace("\n", ",").Replace("\t", ",").Replace("，", ",");
                var mediaIDs = SplitStr(viewModel.DouYinIDs);
                allList = allList.Where(d => mediaIDs.Contains(d.Abstract));
            }
            if (viewModel.MediaTagIds != null)
            {
                allList = allList.Include(d => d.MediaTags).Where(d => d.MediaTags.Any(t => viewModel.MediaTagIds.Contains(t.Id)));
            }
            if (!string.IsNullOrWhiteSpace(viewModel.AddedDateRange))
            {
                var temp = viewModel.AddedDateRange.Trim().Replace("至", "#").Split('#');
                var min = Convert.ToDateTime(temp[0].Trim());
                var max = Convert.ToDateTime(temp[1].Trim()).AddDays(1);
                allList = allList.Where(d => d.AddedDate >= min && d.AddedDate < max);
            }

            if (viewModel.FansNumStart != null)
            {
                viewModel.FansNumStart = viewModel.FansNumStart >= 10000
                    ? viewModel.FansNumStart
                    : viewModel.FansNumStart * 10000;
                allList = allList.Where(d => d.FansNum >= viewModel.FansNumStart);
            }
            if (viewModel.FansNumEnd != null)
            {
                viewModel.FansNumEnd = viewModel.FansNumEnd >= 10000
                    ? viewModel.FansNumEnd
                    : viewModel.FansNumEnd * 10000;
                allList = allList.Where(d => d.FansNum <= viewModel.FansNumEnd);
            }
            if (!string.IsNullOrWhiteSpace(viewModel.FansNumRange))
            {
                var temp = viewModel.FansNumRange.Trim().Replace("至", "-").Split('-');
                var min = Convert.ToInt32(temp[0].Trim()) > 10000 ? Convert.ToInt32(temp[0].Trim()) : Convert.ToInt32(temp[0].Trim()) * 10000;
                var max = Convert.ToInt32(temp[1].Trim()) > 10000 ? Convert.ToInt32(temp[1].Trim()) : Convert.ToInt32(temp[1].Trim()) * 10000;
                allList = allList.Where(d => d.FansNum >= min && d.FansNum <= max);
            }
            if (!string.IsNullOrWhiteSpace(viewModel.AvgReadNumRange))
            {
                var temp = viewModel.AvgReadNumRange.Trim().Replace("至", "-").Split('-');
                var min = Convert.ToInt32(temp[0].Trim());
                var max = Convert.ToInt32(temp[1].Trim());
                allList = allList.Where(d => d.AvgReadNum >= min && d.AvgReadNum <= max);
            }
            if (!string.IsNullOrWhiteSpace(viewModel.SellPriceRange))
            {

                if (!string.IsNullOrWhiteSpace(viewModel.AdPositionName))
                {
                    var temp = viewModel.SellPriceRange.Trim().Replace("至", "-").Split('-');
                    var min = Convert.ToDecimal(temp[0].Trim());
                    var max = Convert.ToDecimal(temp[1].Trim());
                    allList = allList.Include(d => d.MediaPrices)
                        .Where(d => d.MediaPrices.Any(p => p.SellPrice >= min && p.SellPrice <= max && p.AdPositionName == viewModel.AdPositionName));
                }
            }
            if (!string.IsNullOrWhiteSpace(viewModel.PriceRange))
            {

                if (!string.IsNullOrWhiteSpace(viewModel.AdPositionName))
                {
                    var temp = viewModel.PriceRange.Trim().Replace("至", "-").Split('-');
                    var min = Convert.ToDecimal(temp[0].Trim());
                    var max = Convert.ToDecimal(temp[1].Trim());
                    allList = allList.Include(d => d.MediaPrices)
                        .Where(d => d.MediaPrices.Any(p => p.PurchasePrice >= min && p.PurchasePrice <= max && p.AdPositionName == viewModel.AdPositionName));
                }
            }
            if (viewModel.PriceStart != null)
            {
                if (!string.IsNullOrWhiteSpace(viewModel.AdPositionName))
                {
                    allList = allList.Include(d => d.MediaPrices)
                        .Where(d => d.MediaPrices.Any(p => p.PurchasePrice >= viewModel.PriceStart && p.AdPositionName == viewModel.AdPositionName));
                }
                else
                {
                    allList = allList.Include(d => d.MediaPrices)
                        .Where(d => d.MediaPrices.Any(p => p.PurchasePrice >= viewModel.PriceStart));
                }
            }
            if (viewModel.PriceEnd != null)
            {
                if (!string.IsNullOrWhiteSpace(viewModel.AdPositionName))
                {
                    allList = allList.Include(d => d.MediaPrices)
                        .Where(d => d.MediaPrices.Any(p => p.PurchasePrice <= viewModel.PriceEnd && p.AdPositionName == viewModel.AdPositionName));
                }
                else
                {
                    allList = allList.Include(d => d.MediaPrices)
                        .Where(d => d.MediaPrices.Any(p => p.PurchasePrice <= viewModel.PriceEnd));
                }
            }

            if (viewModel.SellPriceStart != null)
            {
                if (!string.IsNullOrWhiteSpace(viewModel.AdPositionName))
                {
                    allList = allList.Include(d => d.MediaPrices)
                        .Where(d => d.MediaPrices.Any(p => p.SellPrice >= viewModel.SellPriceStart && p.AdPositionName == viewModel.AdPositionName));
                }
                else
                {
                    allList = allList.Include(d => d.MediaPrices)
                        .Where(d => d.MediaPrices.Any(p => p.SellPrice >= viewModel.SellPriceStart));
                }
            }
            if (viewModel.SellPriceEnd != null)
            {
                if (!string.IsNullOrWhiteSpace(viewModel.AdPositionName))
                {
                    allList = allList.Include(d => d.MediaPrices)
                        .Where(d => d.MediaPrices.Any(p => p.SellPrice <= viewModel.SellPriceEnd && p.AdPositionName == viewModel.AdPositionName));
                }
                else
                {
                    allList = allList.Include(d => d.MediaPrices)
                        .Where(d => d.MediaPrices.Any(p => p.SellPrice <= viewModel.SellPriceEnd));
                }
            }
            if (viewModel.PriceInvalidDate != null)
            {
                var endDate = viewModel.PriceInvalidDate.Value.AddDays(1);
                allList = allList.Include(d => d.MediaPrices).Where(d =>
                      d.MediaPrices.Where(t => t.IsDelete == false).Any(p => p.InvalidDate < endDate));
            }

            if (viewModel.PriceProtectionDate != null)
            {
                allList = allList.Where(d => d.PriceProtectionDate == viewModel.PriceProtectionDate);
            }
            if (viewModel.PriceProtectionIsBrand != null)
            {
                allList = allList.Where(d => d.PriceProtectionIsBrand == viewModel.PriceProtectionIsBrand);
            }
            if (viewModel.PriceProtectionIsPrePay != null)
            {
                allList = allList.Where(d => d.PriceProtectionIsPrePay == viewModel.PriceProtectionIsPrePay);
            }
            viewModel.total = allList.Count();
            //找出不存在的
            if (!string.IsNullOrWhiteSpace(viewModel.MediaBatch))
            {
                var searchMedias = viewModel.MediaBatch.Split(',').Distinct().Where(d => !string.IsNullOrWhiteSpace(d)).ToList();
                var searchCount = searchMedias.Count;
                var resultCount = viewModel.total;
                if (searchCount != resultCount)
                {
                    foreach (var item in searchMedias)
                    {
                        if (!allList.Any(d => d.MediaName.Contains(item) || d.MediaID.Contains(item)))
                        {
                            viewModel.NoExistent.Add(item);
                        }
                    }
                }
            }
            int offset = viewModel.offset ?? 0;
            int rows = viewModel.limit ?? 10;
            if (!string.IsNullOrWhiteSpace(viewModel.PriceSortOrder))
            {
                var arr = viewModel.PriceSortOrder.Split('|');
                var sort = arr[0];
                var order = arr[1];
                if (order.ToLower()=="desc")
                {
                    return allList
                        .OrderByDescending(d => d.MediaPrices.FirstOrDefault(p => p.AdPositionName == sort).SellPrice)
                        .Skip(offset).Take(rows);
                }
                return allList
                    .OrderBy(d => d.MediaPrices.FirstOrDefault(p => p.AdPositionName == sort).SellPrice)
                    .Skip(offset).Take(rows);
            }
            else
            {
                string order = string.IsNullOrWhiteSpace(viewModel.order) ? "desc" : viewModel.order;
                if (order == "desc")
                {
                    if (viewModel.sort== "Transactor")
                    {
                        return allList.OrderByDescending(d => d.Transactor).Skip(offset).Take(rows);
                    }
                    else
                    {
                        return allList.OrderByDescending(d => d.IsTop).ThenByDescending(d => d.IsHot).ThenByDescending(d => d.IsRecommend).ThenByDescending(d => d.MediaName).ThenByDescending(d => d.Id).Skip(offset).Take(rows);
                    }
                    
                }

                if (viewModel.sort == "Transactor")
                {
                    return allList.OrderBy(d => d.Transactor).Skip(offset).Take(rows);
                }
                else
                {
                    return allList.OrderByDescending(d => d.IsTop).ThenByDescending(d => d.IsHot).ThenByDescending(d => d.IsRecommend).ThenBy(d => d.MediaName).ThenBy(d => d.Id).Skip(offset).Take(rows);
                }

            }

        }
        public IQueryable<MediaView> LoadEntitiesFilters(MediaView viewModel)
        {

            var allList = _repository.LoadEntities(d => d.IsDelete == false);
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
            //if (!string.IsNullOrWhiteSpace(viewModel.Content))
            //{
            //    allList = allList.Where(d => d.Content.Contains(viewModel.Content));
            //}
            //if (!string.IsNullOrWhiteSpace(viewModel.Remark))
            //{
            //    allList = allList.Where(d => d.Remark.Contains(viewModel.Remark));
            //}
            //if (viewModel.HasArticles != null)
            //{
            //    allList = !viewModel.HasArticles.Value ? allList.Where(d => d.MediaArticles.Count == 0) : allList.Where(d => d.MediaArticles.Count > 0);
            //}
            //if (viewModel.IsSlide != null)
            //{
            //    allList = allList.Where(d => d.IsSlide == viewModel.IsSlide);
            //}
            if (!string.IsNullOrWhiteSpace(viewModel.Areas))
            {
                allList = allList.Where(d => d.Area.Contains(viewModel.Areas));
            }
            //if (!string.IsNullOrWhiteSpace(viewModel.SEO))
            //{
            //    allList = allList.Where(d => d.SEO.Contains(viewModel.SEO));
            //}
            //if (!string.IsNullOrWhiteSpace(viewModel.Efficiency))
            //{
            //    allList = allList.Where(d => d.Efficiency.Contains(viewModel.Efficiency));
            //}
            //if (!string.IsNullOrWhiteSpace(viewModel.ResourceType))
            //{
            //    allList = allList.Where(d => d.ResourceType.Contains(viewModel.ResourceType));
            //}
            if (!string.IsNullOrWhiteSpace(viewModel.Platform))
            {
                allList = allList.Where(d => d.Platform.Contains(viewModel.Platform));
            }
            //if (!string.IsNullOrWhiteSpace(viewModel.Channel))
            //{
            //    allList = allList.Where(d => d.Channel.Contains(viewModel.Channel));
            //}
            if (!string.IsNullOrWhiteSpace(viewModel.MediaID))
            {
                allList = allList.Where(d => d.MediaID.Contains(viewModel.MediaID));
            }
            //if (!string.IsNullOrWhiteSpace(viewModel.LinkManId))
            //{
            //    allList = allList.Where(d => d.LinkManId == viewModel.LinkManId);
            //}
            //if (!string.IsNullOrWhiteSpace(viewModel.LinkManName))
            //{
            //    allList = allList.Where(d => d.LinkMan.Name.Contains(viewModel.LinkManName));
            //}
            //if (!string.IsNullOrWhiteSpace(viewModel.Transactor))
            //{
            //    allList = allList.Where(d => d.Transactor.Contains(viewModel.Transactor));
            //}
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
                allList = allList.Include(d => d.MediaTags).Where(d => d.MediaTags.Any(t => viewModel.MediaTagIds.Contains(t.Id)));
            }
            //if (viewModel.PriceStart != null)
            //{
            //    if (!string.IsNullOrWhiteSpace(viewModel.AdPositionName))
            //    {
            //        allList = allList.Include(d => d.MediaPrices)
            //            .Where(d => d.MediaPrices.Any(p => p.PurchasePrice >= viewModel.PriceStart && p.AdPositionName == viewModel.AdPositionName));
            //    }
            //    else
            //    {
            //        allList = allList.Include(d => d.MediaPrices)
            //            .Where(d => d.MediaPrices.Any(p => p.PurchasePrice >= viewModel.PriceStart));
            //    }
            //}
            //if (viewModel.PriceEnd != null)
            //{
            //    if (!string.IsNullOrWhiteSpace(viewModel.AdPositionName))
            //    {
            //        allList = allList.Include(d => d.MediaPrices)
            //            .Where(d => d.MediaPrices.Any(p => p.PurchasePrice <= viewModel.PriceEnd && p.AdPositionName == viewModel.AdPositionName));
            //    }
            //    else
            //    {
            //        allList = allList.Include(d => d.MediaPrices)
            //            .Where(d => d.MediaPrices.Any(p => p.PurchasePrice <= viewModel.PriceEnd));
            //    }
            //}
            //if (viewModel.PriceInvalidDate != null)
            //{
            //    var endDate = viewModel.PriceInvalidDate.Value.AddDays(1);
            //    allList = allList.Include(d => d.MediaPrices).Where(d =>
            //        d.MediaPrices.Any(p => p.InvalidDate < endDate));
            //}

            //var result = allList.Select(d => new MediaView
            //{
            //    Id = d.Id,
            //    MediaName = d.MediaName,
            //    MediaID = d.MediaID,
            //    MediaTypeIndex = d.MediaType.CallIndex,
            //    MediaTypeName = d.MediaType.TypeName,
            //    IsAuthenticate = d.IsAuthenticate,
            //    IsOriginal = d.IsOriginal,
            //    IsComment = d.IsComment,
            //    FansNum = d.FansNum,
            //    ChannelType = d.ChannelType,
            //    LastReadNum = d.MediaArticles.Where(l => l.IsTop == true).OrderByDescending(a => a.PublishDate).FirstOrDefault().ViewCount,
            //    AvgReadNum = (int?)d.MediaArticles.OrderByDescending(a => a.PublishDate).Take(10).Average(a => a.ViewCount),
            //    PublishFrequency = d.PublishFrequency,
            //    Areas = d.Area,
            //    Sex = d.Sex,
            //    Client = d.Client,
            //    SEO = d.SEO,
            //    Abstract = d.Abstract,
            //    PostNum = d.PostNum,
            //    MonthPostNum = d.MonthPostNum,
            //    FriendNum = d.FriendNum,
            //    Efficiency = d.Efficiency,
            //    ResourceType = d.ResourceType,
            //    Channel = d.Channel,
            //    LastPushDate = d.LastPushDate,
            //    AuthenticateType = d.AuthenticateType,
            //    Platform = d.Platform,
            //    TransmitNum = (int?)d.MediaArticles.OrderByDescending(a => a.PublishDate).Take(50).Average(aaa => aaa.ShareCount),
            //    CommentNum = (int?)d.MediaArticles.OrderByDescending(a => a.PublishDate).Take(50).Average(aaa => aaa.CommentCount),
            //    LikesNum = (int?)d.MediaArticles.OrderByDescending(a => a.PublishDate).Take(50).Average(aaa => aaa.LikeCount),
            //    BlogLastPushDate = d.MediaArticles.OrderByDescending(a => a.PublishDate).FirstOrDefault().PublishDate,
            //    WeekArticleCount = d.MediaArticles.OrderByDescending(a => a.PublishDate).Count(l => SqlFunctions.DateDiff("day", l.PublishDate, DateTime.Now) <= 7),
            //    Content = d.Content,
            //    Remark = d.Remark,
            //    Status = d.Status,
            //    ApiUpDate = d.ApiUpDate,
            //    MediaLink = d.MediaLink,
            //    MediaLogo = d.MediaLogo,
            //    MediaQR = d.MediaQR,
            //    LinkManId = d.LinkManId,
            //    LinkManName = d.LinkMan.Name,
            //    Transactor = d.Transactor,
            //    MediaGroups = d.MediaGroups.Select(g => new MediaGroupView() { Id = g.Id, GroupName = g.GroupName }).ToList(),
            //    MediaTags = d.MediaTags.Select(t => new MediaTagView() { Id = t.Id, TagName = t.TagName }).ToList(),
            //    MediaPrices = d.MediaPrices.Select(p => new MediaPriceView() { AdPositionName = p.AdPositionName, PriceDate = p.PriceDate, InvalidDate = p.InvalidDate, PurchasePrice = p.PurchasePrice }).ToList()
            //});
            var result = allList.SelectMany(d => d.MediaArticles.Select(a => new
            {
                a.Media,
                a.CommentCount,
                a.ViewCount,
                a.LikeCount,
                a.IsTop,
                a.IsOriginal,
                a.PublishDate,
                a.ShareCount
            }))
                .GroupBy(d => d.Media).Select(d => new MediaView()
                {
                    MediaName = d.Key.MediaName,
                    MediaID = d.Key.MediaID,
                    Id = d.Key.Id,
                    LastReadNum = d.Where(dd => dd.IsTop == true).OrderByDescending(ddd => ddd.PublishDate).FirstOrDefault().ViewCount,
                    AvgReadNums = d.OrderByDescending(dd => dd.PublishDate).Take(10).Average(ddd => ddd.ViewCount)
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
        public void Update(Expression<Func<Media, bool>> whereLambda, Expression<Func<Media, Media>> updateLambda)
        {
            _repository.Update(whereLambda,updateLambda);
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
        public IQueryable<Ada.Core.ViewModel.Statistics.MediaUpdate> GetMediaUpdatedInfo(MediaView view)
        {
            var medias = _repository.LoadEntities(d => d.IsDelete == false && d.Status == Consts.StateNormal);
            if (!string.IsNullOrWhiteSpace(view.TransactorId))
            {
                medias = medias.Where(d => d.TransactorId == view.TransactorId);
            }

            if (view.PriceInvalidDate==null)
            {
                view.PriceInvalidDate= new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            }

            return medias.GroupBy(d => d.MediaType).Select(d => new Core.ViewModel.Statistics.MediaUpdate
            {
                TypeName = d.Key.TypeName,
                Total = d.Count(),
                Updated = d.Count(m => m.MediaPrices.Where(t=>t.IsDelete==false).Any(p => p.InvalidDate >= view.PriceInvalidDate)),
                NoUpdated = d.Count(m => m.MediaPrices.Where(t => t.IsDelete == false).Any(p => p.InvalidDate < view.PriceInvalidDate))
            });
        }
        private List<string> SplitStr(string str)
        {
            var arr = str.Split(',').Distinct().Where(d => !string.IsNullOrWhiteSpace(d)).ToList();
            List<string> list=new List<string>();
            foreach (var item in arr)
            {
                var temp = item.Trim();
                list.Add(temp);
            }

            return list;
        }

        public void ClearMediaReferencePrices(string id,string platform = null)
        {
            var list = _mediaReferenceRepository.LoadEntities(d => d.MediaId == id);
            if (!string.IsNullOrWhiteSpace(platform))
            {
                list = list.Where(d => d.Platform == platform);
            }
            foreach (var item in list)
            {
                _mediaReferenceRepository.Remove(item);
            }
            
        }
    }

}
