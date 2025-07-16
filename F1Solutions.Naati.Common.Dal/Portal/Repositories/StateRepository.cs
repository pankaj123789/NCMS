using System.Collections.Generic;
using System.Linq;
using F1Solutions.Naati.Common.Contracts.Dal.Portal;
using F1Solutions.Naati.Common.Dal.Domain;
using NHibernate.Linq;
using State = F1Solutions.Naati.Common.Dal.Domain.State;

namespace F1Solutions.Naati.Common.Dal.Portal.Repositories
{
    //Like this one, all repositories in this assembly should probably be in terms of the ServiceContracts rather than the SAM domain
    //Unfortunately, most of the methods on the base class won't work, but this is just for lookup so that's OK.
    public interface IStateRepository : IRepository<Contracts.Dal.Portal.State>
    {
        IList<Contracts.Dal.Portal.State> GetAll();

        State GetState(string state);

        Suburb GetSuburb(State state, string suburb);
    }

    public class StateRepository : Repository<Contracts.Dal.Portal.State>, IStateRepository
    {
        private ISystemValuesTranslator mTranslator;

        public StateRepository(ICustomSessionManager sessionManager, ISystemValuesTranslator translator)
            : base(sessionManager)
        {
            mTranslator = translator;
        }

        public IList<Contracts.Dal.Portal.State> GetAll()
        {
            var allStates = Session.Query<Region>()
                .Where(r => r.State != null)
                .Where(r => r.Country != null)
                .Fetch(r => r.State)
                .Fetch(r => r.Country)
                .ToList();

            return allStates.Select(r => new Contracts.Dal.Portal.State()
            {
                Abbreviation = r.State.Abbreviation,
                DisplayText = r.State.Name,
                IsAustralian = r.Country.Id == mTranslator.AustraliaCountryId,
                SamId = r.State.Id,
            }).ToList();
        }

        public State GetState(string state)
        {
            var stateQuery = Session.Query<State>();
            var stateObject = stateQuery
                .FirstOrDefault(r => r.Abbreviation == state);

            return stateObject;
        }

        public Suburb GetSuburb(State state, string suburb)
        {
            var databaseSuburb = Session.Query<Suburb>().FirstOrDefault(s => s.State == state && s.Name == suburb);

            return databaseSuburb;
        }
    }
}
