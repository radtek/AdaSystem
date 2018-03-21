using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ada.Core;
using Ada.Core.Domain.Business;
using Ada.Core.ViewModel.Business;
using Ada.Core.ViewModel.Resource;

namespace Ada.Services.Business
{
    public interface IOrderDetailCommentService : IDependency
    {
        void Add(OrderDetailComment entity);
        void Add(IEnumerable<OrderDetailComment> entities);
        void Remove(OrderDetailComment entity);
        void Remove(IEnumerable<OrderDetailComment> entities);
        IQueryable<OrderDetailCommentView> LoadComments(string id, int pageindex, int pagesize, out int total);
        IQueryable<MediaCommentView> LoadComments(MediaCommentView viewModel);
    }
}
