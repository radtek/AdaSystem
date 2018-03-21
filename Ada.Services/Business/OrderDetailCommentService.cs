using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ada.Core;
using Ada.Core.Domain;
using Ada.Core.Domain.Admin;
using Ada.Core.Domain.Business;
using Ada.Core.Domain.Purchase;
using Ada.Core.ViewModel.Business;
using Ada.Core.ViewModel.Resource;

namespace Ada.Services.Business
{
    public class OrderDetailCommentService : IOrderDetailCommentService
    {
        private readonly IDbContext _dbContext;
        private readonly IRepository<Manager> _managerRepository;
        private readonly IRepository<OrderDetailComment> _repository;
        public OrderDetailCommentService(IDbContext dbContext,
            IRepository<OrderDetailComment> repository,
            IRepository<Manager> managerRepository)
        {
            _dbContext = dbContext;
            _repository = repository;
            _managerRepository = managerRepository;
        }
        public void Add(OrderDetailComment entity)
        {
            _repository.Add(entity);
            _dbContext.SaveChanges();
        }

        public void Add(IEnumerable<OrderDetailComment> entities)
        {
            _repository.Add(entities);
            _dbContext.SaveChanges();
        }

        public void Remove(OrderDetailComment entity)
        {
            _repository.Remove(entity);
            _dbContext.SaveChanges();
        }
        public void Remove(IEnumerable<OrderDetailComment> entities)
        {
            _repository.Remove(entities);
            _dbContext.SaveChanges();
        }
        public IQueryable<OrderDetailCommentView> LoadComments(string id, int pageindex, int pagesize, out int total)
        {
            var mediaComments = _repository.LoadEntities(d => d.BusinessOrderDetail.MediaPrice.Media.Id==id);
            var managers = _managerRepository.LoadEntities(d => true);
            var allList = from c in mediaComments
                from m in managers
                where c.TransactorId == m.Id
                select new OrderDetailCommentView()
                {
                    Transactor = c.Transactor,
                    Content = c.Content,
                    Score = c.Score,
                    CommentDate = c.CommentDate,
                    TransactorImage = m.Image,
                    Organization = m.Organizations.FirstOrDefault().OrganizationName
                };
            total=mediaComments.Count();
            return allList.OrderByDescending(d => d.CommentDate).Skip(0).Take(pageindex * pagesize);
        }
        public IQueryable<MediaCommentView> LoadComments(MediaCommentView viewModel)
        {
            var mediaComments = _repository.LoadEntities(d => d.IsDelete == false);
            if (!string.IsNullOrWhiteSpace(viewModel.MediaId))
            {
                mediaComments = mediaComments.Where(d => d.BusinessOrderDetail.MediaPrice.MediaId == viewModel.MediaId);
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
