using System.Collections.Generic;
using System.Linq;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using RemliCMS.WebData.Entities;

namespace RemliCMS.WebData.Services
{
    public class PageIndexService : EntityService<PageIndex>
    {
        public List<PageIndex> ListIndexs(ObjectId pageHeaderObjectId)
        {
            var pageHeaderQuery = Query<PageIndex>.EQ(g => g.PageHeaderId, pageHeaderObjectId);
            
            var foundPageIndexs = MongoConnectionHandler.MongoCollection.Find(pageHeaderQuery)
                .SetSortOrder(SortBy<PageIndex>.Ascending(g => g.Order))
                .SetFields(Fields<PageIndex>.Include(g => g.Id, g => g.CreatedDate, g => g.Order))
                .ToList();

            return foundPageIndexs;
        }

        public int GetLastOrderNum(ObjectId pageHeaderObjectId)
        {
            var pageHeaderQuery = Query<PageIndex>.EQ(g => g.PageHeaderId, pageHeaderObjectId);

            var foundPageIndex = MongoConnectionHandler.MongoCollection.Find(pageHeaderQuery)
                .SetSortOrder(SortBy<PageIndex>.Ascending(g => g.Order))
                .Last();

            if (foundPageIndex == null)
            {
                return 0;
            }

            return foundPageIndex.Order;
        }

        public void MoveOrderDown(ObjectId pageIndexObjectId)
        {
            var pageIndexQuery = Query<PageIndex>.EQ(g => g.Id, pageIndexObjectId);

            var currentPageIndex = MongoConnectionHandler.MongoCollection.FindOne(pageIndexQuery);

            int currentOrderNum = currentPageIndex.Order;

            int lastOrderNum = GetLastOrderNum(currentPageIndex.PageHeaderId);

            if (currentOrderNum == lastOrderNum)
            {
                return;
            }

            var nextPageIndexQuery = Query.And(
                    Query<PageIndex>.EQ(g => g.PageHeaderId, currentPageIndex.PageHeaderId),
                    Query<PageIndex>.EQ(g => g.Order,currentPageIndex.Order+1)
                    );

            var nextPageIndex = MongoConnectionHandler.MongoCollection.FindOne(nextPageIndexQuery);

            currentPageIndex.Order = currentPageIndex.Order + 1;
            nextPageIndex.Order = nextPageIndex.Order - 1;

            Update(currentPageIndex);
            Update(nextPageIndex);

        }

        public void MoveOrderUp(ObjectId pageIndexObjectId)
        {
            var pageIndexQuery = Query<PageIndex>.EQ(g => g.Id, pageIndexObjectId);

            var currentPageIndex = MongoConnectionHandler.MongoCollection.FindOne(pageIndexQuery);

            if (currentPageIndex.Order == 0)
            {
                return;
            }

            var prevPageIndexQuery = Query.And(
                    Query<PageIndex>.EQ(g => g.PageHeaderId, currentPageIndex.PageHeaderId),
                    Query<PageIndex>.EQ(g => g.Order, currentPageIndex.Order - 1)
                    );

            var prevPageIndex = MongoConnectionHandler.MongoCollection.FindOne(prevPageIndexQuery);

            currentPageIndex.Order = currentPageIndex.Order - 1;
            prevPageIndex.Order = prevPageIndex.Order + 1;

            Update(currentPageIndex);
            Update(prevPageIndex);
        }
    
        public List<PageContent> ListContents(ObjectId pageIndexObjectId)
        {
            var pageIndexQuery = Query<PageIndex>.EQ(g => g.Id, pageIndexObjectId);

            var foundPageIndex = MongoConnectionHandler.MongoCollection.FindOne(pageIndexQuery);

            return foundPageIndex.PageContents.ToList();
        }

        public PageContent GetLastContent(ObjectId pageIndexObjectId, ObjectId translationObjectId)
        {
            var pageIndexQuery = Query<PageIndex>.EQ(g => g.Id, pageIndexObjectId);
            var foundPageIndex = MongoConnectionHandler.MongoCollection.FindOne(pageIndexQuery);

            var pageContent = foundPageIndex.PageContents.FindLast(q => q.TranslationId == translationObjectId);

            return pageContent;
        }

        public string GetContentString(ObjectId pageIndexObjectId, ObjectId translationObjectId)
        {
            var pageIndexQuery = Query<PageIndex>.EQ(g => g.Id, pageIndexObjectId);
            var foundPageIndex = MongoConnectionHandler.MongoCollection.FindOne(pageIndexQuery);

            var pageContent = foundPageIndex.PageContents.FindLast(q => q.TranslationId == translationObjectId);
            
            if (pageContent == null)
            {
                return null;
            }

            return pageContent.Content;
        }

        public void AddContent(ObjectId pageIndexObjectId, PageContent pageContent)
        {
            var updateResult = MongoConnectionHandler.MongoCollection.Update(
                Query<PageIndex>.EQ(p => p.Id, pageIndexObjectId),
                Update<PageIndex>.Push(p => p.PageContents, pageContent),
                new MongoUpdateOptions
                {
                    WriteConcern = WriteConcern.Acknowledged
                });
        }


    }
}
