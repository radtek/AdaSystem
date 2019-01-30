using System.Linq;
using Ada.Core;
using Ada.Core.Domain.Vote;
using Ada.Core.ViewModel.Vote;


namespace Ada.Services.Vote
{
   public interface IVoteItemService : IDependency
   {
       void Add(VoteItem entity);
       void Update(VoteItem entity);
       void Delete(VoteItem entity);
       VoteItem GetById(string id);
        IQueryable<VoteItem> LoadEntitiesFilter(VoteItemView viewModel);
   }
}
