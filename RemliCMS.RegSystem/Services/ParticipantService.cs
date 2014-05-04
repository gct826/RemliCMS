using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using RemliCMS.RegSystem.Entities;

namespace RemliCMS.RegSystem.Services
{
    public class ParticipantService : EntityService<Participant>
    {
        public Participant GetByPartId(int partId)
        {
            // returns participant by partId. 
            var participantQuery = Query<Participant>.EQ(g => g.PartId, partId);

            var foundParticipant = MongoConnectionHandler.MongoCollection.FindOne(participantQuery);

            return foundParticipant;
        }

        public int GetLastId()
        {
            var foundParticipant = MongoConnectionHandler.MongoCollection.FindAll()
                .SetSortOrder(SortBy<Participant>.Ascending(g => g.PartId))
                .LastOrDefault();

            if (foundParticipant == null)
            {
                return 0;
            }
            return foundParticipant.PartId;

        }

        public List<Participant> GetParticipantList(int regId)
        {
            var participantsQuery = Query<Participant>.EQ(g => g.RegId, regId);

            var foundParticipantList = MongoConnectionHandler.MongoCollection.Find(participantsQuery)
                .SetSortOrder(SortBy<Participant>.Ascending(g => g.PartId))
                .ToList();

            return foundParticipantList;
        }

        public List<Participant> ListAllParticipants(string searchString = "")
        {
            if (searchString == "")
            {
                var foundParticipantList = MongoConnectionHandler.MongoCollection.FindAll()
                    .SetSortOrder(SortBy<Participant>.Ascending(g => g.RegId)).ToList();
                
                return foundParticipantList;
            }

            var regex = new Regex(searchString);

            var participantQuery = Query.Or(
                Query<Participant>.Where(g => regex.IsMatch(g.LastName)),
                Query<Participant>.Where(g => regex.IsMatch(g.FirstName)),
                Query<Participant>.Where(g => regex.IsMatch(g.ChineseName)));

            var foundRegistrationList = MongoConnectionHandler.MongoCollection.Find(participantQuery)
                .SetSortOrder(SortBy<Registration>.Ascending(g => g.RegId)).ToList();

            return foundRegistrationList;
        }

        public bool CompleteRegistration(int regId)
        {
            var participantQuery = Query.And(
                Query<Participant>.EQ(g => g.RegId, regId),
                Query<Participant>.EQ(g => g.StatusId, 6)
            );

            var foundRegistrationList = MongoConnectionHandler.MongoCollection.Find(participantQuery);

            if (foundRegistrationList.Count() == 0)
            {
                return true;
            }

            return false;
        }
    }
}
