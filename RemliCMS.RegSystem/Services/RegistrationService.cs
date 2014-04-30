using System;
using System.Collections.Generic;
using System.Linq;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using RemliCMS.RegSystem.Entities;

namespace RemliCMS.RegSystem.Services
{
    public class RegistrationService : EntityService<Registration>
    {
        public bool AllowRegistration()
        {
            var regOpenDate = new DateTime(2014,5,4);
            var regCloseDate = new DateTime(2014, 5, 26);

            if (DateTime.Today >= regOpenDate && DateTime.Today <= regCloseDate)
            {
                return true;                
            }

            return false;
        }

        public bool IsExistEmail(string submittedEmail)
        {
            // checks whether Email exist and return false otherwise.
            var emailQuery = Query<Registration>.EQ(g => g.RegEmail, submittedEmail.ToLower());
            var foundEmail = MongoConnectionHandler.MongoCollection.FindOne(emailQuery);

            if (foundEmail != null)
            {
                return true;
            }
            return false;
        }

        public bool IsExistPhone(string submittedPhone)
        {
            // checks whether Phone exist and return false otherwise.
            var phoneQuery = Query<Registration>.EQ(g => g.RegPhone, submittedPhone);
            var foundPhone = MongoConnectionHandler.MongoCollection.FindOne(phoneQuery);

            if (foundPhone != null)
            {
                return true;
            }
            return false;
        }

        public int GetLastId()
        {
            var foundRegistration = MongoConnectionHandler.MongoCollection.FindAll()
                                                          .SetSortOrder(SortBy<Registration>.Ascending(g => g.RegId))
                                                          .LastOrDefault();

            if (foundRegistration == null)
            {
                return 0;
            }
            return foundRegistration.RegId;

        }

        public Registration GetByRegId(int regId)
        {
            var registrationQuery = Query<Registration>.EQ(g => g.RegId, regId);
            var foundRegistration = MongoConnectionHandler.MongoCollection.FindOne(registrationQuery);

            return foundRegistration;
        }

        public void UpdateLastOpened(string regObjectId)
        {
            var queryObjectId = new ObjectId(regObjectId);
            var registrationQuery = Query<Registration>.EQ(g => g.Id, queryObjectId);
            var foundRegistration = MongoConnectionHandler.MongoCollection.FindOne(registrationQuery);

            if (foundRegistration != null)
            {
                foundRegistration.DateOpened = DateTime.Now.AddHours(-5);
                Update(foundRegistration);
            }

        }

        public Registration OpenReg(string regEmail, string regPhone)
        {
            var registrationQuery = Query.And(
                Query<Registration>.EQ(g => g.RegEmail, regEmail),
                Query<Registration>.EQ(g => g.RegPhone, regPhone)
                );

            var foundRegistration = MongoConnectionHandler.MongoCollection.FindOne(registrationQuery);

            return foundRegistration;
        }

        public decimal getTotalPrice(int regId)
        {
            var participantService = new ParticipantService();
            var foundParticipantList = participantService.GetParticipantList(regId);

            if (foundParticipantList.Count == 0)
            {
                return 0;
            }

            var totalCost = foundParticipantList.Where(p => p.StatusId == 1 || p.StatusId == 2 || p.StatusId == 3 || p.StatusId == 5).
                                                 Aggregate((decimal) 0, (c, p) => c + p.PartPrice);

            return totalCost;
        }

        public List<Registration> ListAllRegistrations()
        {
            //var registrationQuery = Query<Registration>.EQ(g => g.IsDeleted, false);

            //var foundRegistrationList = MongoConnectionHandler.MongoCollection.Find(registrationQuery).ToList();

            var foundRegistrationList = MongoConnectionHandler.MongoCollection.FindAll()
                .SetSortOrder(SortBy<Registration>.Ascending(g => g.RegId)).ToList();

            return foundRegistrationList;
        }

    }
}
