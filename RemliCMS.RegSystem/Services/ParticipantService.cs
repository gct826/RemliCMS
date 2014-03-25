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
     
    }
}
