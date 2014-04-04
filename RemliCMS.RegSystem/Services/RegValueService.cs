using System;
using System.Collections.Generic;
using System.Linq;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using RemliCMS.RegSystem.Entities;

namespace RemliCMS.RegSystem.Services
{
    public class RegValueService : EntityService<RegValue>
    {
        
        public int GetLastValue(ObjectId regFieldObjectId)
        {
            // returns the last created value for the Reg Field
            var regFieldQuery = Query<RegValue>.EQ(g => g.RegFieldObjectId, regFieldObjectId);
            var foundRegField = MongoConnectionHandler.MongoCollection.Find(regFieldQuery);

            var lastValue = foundRegField.ToList().OrderBy(g => g.Value);

            if (lastValue.Any())
            {
                return lastValue.Last().Value; 
            }

            return 0;
        }

        public void AddText(ObjectId regFieldObjectId, int value, RegText updateText)
        {
            var valueQuery = Query.And(
                Query<RegValue>.EQ(g => g.RegFieldObjectId, regFieldObjectId),
                Query<RegValue>.EQ(g => g.Value, value)
                );

            var updateBuilder = Update<RegValue>.Push(g => g.Translation, updateText);

            var updateResult = MongoConnectionHandler.MongoCollection.Update(
                valueQuery,
                updateBuilder,
                new MongoUpdateOptions
                    {
                        WriteConcern = WriteConcern.Acknowledged
                    });

        }

        public string GetValueText(ObjectId translationObjectId, string regFieldkey, int regValue)
        {
            var regFeildService = new RegFieldService();
            var regFieldObjectId = regFeildService.FindRegFieldObjectId(regFieldkey);

            if (regFieldObjectId == ObjectId.Empty)
            {
                return "Reg Field Not Found";
            }
            
            var textList = ListValueText(regFieldObjectId, regValue);

            if (textList.Count == 0)
            {
                return "Reg Value Not Found";
            }

            var text = textList.FindLast(g => g.TranslationId == translationObjectId).Text;

            return text;

        }

        public List<RegValue> GetAllValues(ObjectId regFieldObjectId)
        {
            var valueQuery = Query<RegValue>.EQ(g => g.RegFieldObjectId, regFieldObjectId);

            var foundValues = MongoConnectionHandler.MongoCollection.Find(valueQuery)
                .SetSortOrder(SortBy<RegValue>.Ascending(g => g.Value));

            return foundValues.ToList();
        }

        public List<RegText> ListValueText(ObjectId regFieldObjectId, int value)
        {
            var valueQuery = Query.And(
                Query<RegValue>.EQ(g => g.RegFieldObjectId, regFieldObjectId),
                Query<RegValue>.EQ(g => g.Value, value)
                );

            var foundValue = MongoConnectionHandler.MongoCollection.FindOne(valueQuery);

            return foundValue.Translation.ToList();
        }

        public List<ValueText> GetValueTextList(string regFieldkey, ObjectId translationObjectId)
        {
            var regFeildService = new RegFieldService();
            var regFieldObjectId = regFeildService.FindRegFieldObjectId(regFieldkey);

            var returnValueTextList = new List<ValueText>();

            var valueQuery = Query<RegValue>.EQ(g => g.RegFieldObjectId, regFieldObjectId);

            var foundValueList = MongoConnectionHandler.MongoCollection.Find(valueQuery)
                                .SetSortOrder(SortBy<RegValue>.Ascending(g => g.Value)).ToList();

            if (foundValueList.Count == 0)
            {
                return returnValueTextList;
            }

            foreach (var value in foundValueList)
            {
                var addValueText = new ValueText();
                addValueText.Value = value.Value;

                var translationList = value.Translation.FindAll(g => g.TranslationId == translationObjectId).ToList();
                
                if (translationList.Count != 0)
                {
                    addValueText.Text = translationList.Last().Text;
                    returnValueTextList.Add(addValueText);
                }
            }
            
            return returnValueTextList;

        }
    }
}
