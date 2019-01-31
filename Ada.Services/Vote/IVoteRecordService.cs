using System.Linq;
using Ada.Core;
using Ada.Core.Domain.Vote;
using Ada.Core.ViewModel.Vote;


namespace Ada.Services.Vote
{
   public interface IVoteRecordService : IDependency
   {
      
       void Delete(VoteItemRecord entity);
        VoteItemRecord GetById(string id);
        IQueryable<VoteItemRecord> LoadEntitiesFilter(VoteItemRecordView viewModel);
   }
}
