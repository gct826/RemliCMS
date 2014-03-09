using System.Collections.Generic;
using System.Linq;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using RemliCMS.WebData.Entities;

namespace RemliCMS.WebData.Services
{
    public class PageHeaderService : EntityService<PageHeader>
    {
        public string GetDefaultPermalink()
        {
            // returns the Permalink of the default page header.
            var pageHeaderQuery = Query<PageHeader>.EQ(g => g.IsDefault, true);
            var foundPageHeader = MongoConnectionHandler.MongoCollection.FindOne(pageHeaderQuery);

            if (foundPageHeader == null)
            {
                return "";
            }

            return foundPageHeader.Permalink;
        }

        public bool IsExistPermalink(string permalink)
        {
            // checks whether Page Header exist and return false otherwise.
            var pageHeaderQuery = Query<PageHeader>.EQ(g => g.Permalink, permalink.ToLower());
            var foundPageHeader = MongoConnectionHandler.MongoCollection.FindOne(pageHeaderQuery);

            return foundPageHeader != null;
        }

        public List<PageHeader> ListAllChildren(ObjectId parentObjectId)
        {
            // returns a list of all Page Headers, including active and none active ones for a given parent.
            var parentHeaderQuery = Query<PageHeader>.EQ(g => g.ParentId, parentObjectId);

            var pageHeaderList = MongoConnectionHandler.MongoCollection.Find(parentHeaderQuery)
                .SetSortOrder(SortBy<PageHeader>.Ascending(g => g.Order))
                .ToList();

            return pageHeaderList;
        }
        
        public PageHeader Details(string permalink)
        {
            // returns one specific Page Header based on Code. 
            var pageHeaderQuery = Query<PageHeader>.EQ(g => g.Permalink, permalink.ToLower());
            
            var pageHeader = MongoConnectionHandler.MongoCollection.Find(pageHeaderQuery)
                .SetFields(Fields<PageHeader>.Include(g => g.Id, g => g.Name, g => g.Permalink, g => g.IsDefault, g => g.Order, g => g.ParentId))
                .FirstOrDefault();

            return pageHeader;
        }

        public void UpdateDetails(PageHeader modifiedPageHeader)
        {
            // updates only Name, Permalink, and IsDefault flag    
            var pageHeaderQuery = Query<PageHeader>.EQ(g => g.Id, modifiedPageHeader.Id);
            
            var pageHeaderUpdate = Update<PageHeader>
                .Set(g => g.Name, modifiedPageHeader.Name)
                .Set(g => g.Permalink, modifiedPageHeader.Permalink)
                .Set(g => g.IsDefault, modifiedPageHeader.IsDefault);

            var result = this.MongoConnectionHandler.MongoCollection.Update(
                pageHeaderQuery,
                pageHeaderUpdate,
                WriteConcern.Acknowledged);

            if (!result.Ok)
            {
                //// Something went wrong
            }
        }


        public void UpdateParentId(PageHeader modifiedPageHeader)
        {
            // updates only Name, Permalink, and IsDefault flag    
            var pageHeaderQuery = Query<PageHeader>.EQ(g => g.Id, modifiedPageHeader.Id);

            var pageHeaderUpdate = Update<PageHeader>
                .Set(g => g.ParentId, modifiedPageHeader.ParentId);

            var result = this.MongoConnectionHandler.MongoCollection.Update(
                pageHeaderQuery,
                pageHeaderUpdate,
                WriteConcern.Acknowledged);

            if (!result.Ok)
            {
                //// Something went wrong
            }
        }

        public void UpdateOrder(PageHeader modifiedPageHeader)
        {
            // updates only Name, Permalink, and IsDefault flag    
            var pageHeaderQuery = Query<PageHeader>.EQ(g => g.Id, modifiedPageHeader.Id);

            var pageHeaderUpdate = Update<PageHeader>
                .Set(g => g.Order, modifiedPageHeader.Order);

            var result = this.MongoConnectionHandler.MongoCollection.Update(
                pageHeaderQuery,
                pageHeaderUpdate,
                WriteConcern.Acknowledged);

            if (!result.Ok)
            {
                //// Something went wrong
            }
        }

        public string ReturnPath(ObjectId pageHeaderObjectId)
        {

            // returns the location of the Page
            var pageHeaderQuery = Query<PageHeader>.EQ(g => g.Id, pageHeaderObjectId);
            var foundPageHeader = MongoConnectionHandler.MongoCollection.FindOne(pageHeaderQuery);

            string pageHeaderLocation = foundPageHeader.Name;
            while (foundPageHeader.ParentId != ObjectId.Empty)
            {
                pageHeaderQuery = Query<PageHeader>.EQ(g => g.Id, foundPageHeader.ParentId);
                foundPageHeader = MongoConnectionHandler.MongoCollection.FindOne(pageHeaderQuery);

                pageHeaderLocation = foundPageHeader.Name + " / " + pageHeaderLocation;
            }

            pageHeaderLocation = "/ " + pageHeaderLocation;
            return pageHeaderLocation;
        }

        public List<PageTitle> ListPageTitles(ObjectId pageHeaderObjectId)
        {
            var pageHeaderQuery = Query<PageHeader>.EQ(g => g.Id, pageHeaderObjectId);

            var foundPageHeader = MongoConnectionHandler.MongoCollection.FindOne(pageHeaderQuery);

            return foundPageHeader.PageTitles;
        }

        public PageTitle ReturnPageTitle(ObjectId pageHeaderObjectId, ObjectId translationObjectId)
        {
            var pageHeaderQuery = Query<PageHeader>.EQ(g => g.Id, pageHeaderObjectId);

            var foundPageHeader = MongoConnectionHandler.MongoCollection.FindOne(pageHeaderQuery);

            var title = foundPageHeader.PageTitles.FindLast(q => q.TranslationId == translationObjectId);

            return title;            

        }

        public void AddTitle(ObjectId pageHeaderObjectId, PageTitle pageTitle)
        {
            var updateResult = MongoConnectionHandler.MongoCollection.Update(
                Query<PageHeader>.EQ(p => p.Id, pageHeaderObjectId),
                Update<PageHeader>.Push(p => p.PageTitles, pageTitle),
                new MongoUpdateOptions
                    {
                        WriteConcern = WriteConcern.Acknowledged
                    });
        }

        public int GetLastOrder(ObjectId parentHeaderObjectId)
        {
            var parentHeaderQuery = Query<PageHeader>.EQ(g => g.ParentId, parentHeaderObjectId);

            var pageHeaderList = MongoConnectionHandler.MongoCollection.Find(parentHeaderQuery)
                .SetSortOrder(SortBy<PageHeader>.Ascending(g => g.Order))
                .ToList();

            if (pageHeaderList.Count == 0)
            {
                return 0;
            }
                
            return pageHeaderList.Last().Order;
 
        }

        public void ReOrderChildren(ObjectId parentHeaderObjectId)
        {
            var parentHeaderQuery = Query<PageHeader>.EQ(g => g.ParentId, parentHeaderObjectId);

            var pageHeaderList = MongoConnectionHandler.MongoCollection.Find(parentHeaderQuery)
                .SetSortOrder(SortBy<PageHeader>.Ascending(g => g.Order))
                .ToList();

            if (pageHeaderList.Count == 0)
            {
                return;
            }

            var orderNum = 0;

            foreach (var pageHeader in pageHeaderList)
            {
                pageHeader.Order = orderNum;
                UpdateOrder(pageHeader);
                orderNum++;
            }
        }

        public void MoveOrderDown(ObjectId pageHeaderObjectId)
        {
            var pageHeaderQuery = Query<PageHeader>.EQ(g => g.Id, pageHeaderObjectId);

            var currentPageHeader = MongoConnectionHandler.MongoCollection.FindOne(pageHeaderQuery);

            int currentOrderNum = currentPageHeader.Order;

            int lastOrderNum = GetLastOrder(currentPageHeader.ParentId);

            if (currentOrderNum == lastOrderNum)
            {
                return;
            }

            var nextPageHeaderQuery = Query.And(
                    Query<PageHeader>.EQ(g => g.ParentId, currentPageHeader.ParentId),
                    Query<PageHeader>.EQ(g => g.Order, currentPageHeader.Order + 1)
                    );

            var nextPageHeader = MongoConnectionHandler.MongoCollection.FindOne(nextPageHeaderQuery);

            currentPageHeader.Order = currentPageHeader.Order + 1;
            nextPageHeader.Order = nextPageHeader.Order - 1;

            UpdateOrder(currentPageHeader);
            UpdateOrder(nextPageHeader);
        }

        public void MoveOrderUp(ObjectId pageHeaderObjectId)
        {
            var pageHeaderQuery = Query<PageHeader>.EQ(g => g.Id, pageHeaderObjectId);

            var currentPageHeader = MongoConnectionHandler.MongoCollection.FindOne(pageHeaderQuery);

            if (currentPageHeader.Order == 0)
            {
                return;
            }

            var prevPageHeaderQuery = Query.And(
                    Query<PageHeader>.EQ(g => g.ParentId, currentPageHeader.ParentId),
                    Query<PageHeader>.EQ(g => g.Order, currentPageHeader.Order - 1)
                    );

            var prevPageHeader = MongoConnectionHandler.MongoCollection.FindOne(prevPageHeaderQuery);

            currentPageHeader.Order = currentPageHeader.Order - 1;
            prevPageHeader.Order = prevPageHeader.Order + 1;

            UpdateOrder(currentPageHeader);
            UpdateOrder(prevPageHeader);
        }

    }
}
