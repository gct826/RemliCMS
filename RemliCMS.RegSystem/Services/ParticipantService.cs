using System.Collections.Generic;
using System.Linq;
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

    }
}
