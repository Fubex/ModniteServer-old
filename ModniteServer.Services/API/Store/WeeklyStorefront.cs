using System.Collections.Generic;

namespace ModniteServer.API.Store
{
    internal sealed class WeeklyStorefront : Storefront
    {
        public WeeklyStorefront()
        {
            Name = "BRWeeklyStorefront";
            IsWeeklyStore = true;
            Catalog = new List<StoreItem>();
            foreach (var i in ApiConfig.Current.FeaturedShopItems)
            {
                Catalog.Add(new StoreItem
                {
                    TemplateId = i.Key,
                    Priority = i.Value
                });
            }
        }
    }
}