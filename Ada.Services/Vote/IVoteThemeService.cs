using System.Linq;
using Ada.Core;
using Ada.Core.Domain.Vote;
using Ada.Core.ViewModel.Vote;


namespace Ada.Services.Vote
{
   public interface IVoteThemeService : IDependency
   {
       void Add(VoteTheme entity);
       void Update(VoteTheme entity);
       void Delete(VoteTheme entity);
       VoteTheme GetById(string id);
        IQueryable<VoteTheme> LoadEntitiesFilter(VoteThemeView viewModel);
   }
}
