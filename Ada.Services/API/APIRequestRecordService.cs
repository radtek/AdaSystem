﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ada.Core;
using Ada.Core.Domain.API;
using Ada.Core.Domain.Log;
using Ada.Core.ViewModel.Admin;
using Ada.Core.ViewModel.API;

namespace Ada.Services.API
{
    public class APIRequestRecordService : IAPIRequestRecordService
    {
        private readonly IDbContext _dbContext;
        private readonly IRepository<APIRequestRecord> _repository;
        public APIRequestRecordService(IRepository<APIRequestRecord> repository, IDbContext dbContext)
        {
            _repository = repository;
            _dbContext = dbContext;
        }
        public void Delete(params string[] ids)
        {
            var list = new List<APIRequestRecord>();
            foreach (var id in ids)
            {
                var log = _repository.LoadEntities(d => d.Id == id).FirstOrDefault();
                list.Add(log);
            }
            _repository.Remove(list);
            _dbContext.SaveChanges();
        }

        public IQueryable<APIRequestRecord> LoadEntitiesFilter(APIRequestRecordView viewModel)
        {
            var allList = _repository.LoadEntities(d => d.IsDelete == false);
            //条件过滤

            if (!string.IsNullOrWhiteSpace(viewModel.search))
            {
                allList = allList.Where(d => d.RequestParameters.Contains(viewModel.search));
            }
            if (!string.IsNullOrWhiteSpace(viewModel.Retmsg))
            {
                allList = allList.Where(d => d.Retmsg.Contains(viewModel.Retmsg));
            }
            if (!string.IsNullOrWhiteSpace(viewModel.APIName))
            {
                allList = allList.Where(d => d.APIInterfaces.APIName.Contains(viewModel.APIName));
            }
            if (!string.IsNullOrWhiteSpace(viewModel.Retcode))
            {
                allList = allList.Where(d => d.Retcode == viewModel.Retcode);
            }
            if (!string.IsNullOrWhiteSpace(viewModel.DateRange))
            {
                var dateRanges = viewModel.DateRange.Split('至');
                if (dateRanges.Length == 2)
                {
                    DateTime.TryParse(dateRanges[0], out var start);
                    DateTime.TryParse(dateRanges[1], out var end);
                    end = end.AddDays(1);
                    allList = allList.Where(d => d.ReponseDate >= start && d.ReponseDate < end);
                }
            }
            if (viewModel.IsSuccess != null)
            {
                allList = allList.Where(d => d.IsSuccess == viewModel.IsSuccess);
            }
            viewModel.total = allList.Count();
            int offset = viewModel.offset ?? 0;
            int rows = viewModel.limit ?? 10;
            string order = string.IsNullOrWhiteSpace(viewModel.order) ? "desc" : viewModel.order;
            if (order == "desc")
            {
                return allList.OrderByDescending(d => d.Id).Skip(offset).Take(rows);
            }
            return allList.OrderBy(d => d.Id).Skip(offset).Take(rows);
        }
    }
}
