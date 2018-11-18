using System.Collections.Generic;

namespace ModniteServer.API.Store
{
    public sealed class DailyStorefront : Storefront
    {
        public DailyStorefront()
        {
            Name = "BRDailyStorefront";
            Catalog = new List<StoreItem>
            {
                new StoreItem
                {
                    TemplateId = "AthenaCharacter:CID_003_Athena_Commando_F_Default",
                },
                new StoreItem
                {
                    TemplateId = "AthenaCharacter:CID_004_Athena_Commando_F_Default"
                },
                new StoreItem
                {
                    TemplateId = "AthenaCharacter:CID_005_Athena_Commando_M_Default"
                },
                new StoreItem
                {
                    TemplateId = "AthenaCharacter:CID_006_Athena_Commando_M_Default"
                },
                new StoreItem
                {
                    TemplateId = "AthenaCharacter:CID_007_Athena_Commando_M_Default"
                },
                new StoreItem
                {
                    TemplateId = "AthenaCharacter:CID_008_Athena_Commando_M_Default"
                },
            };
        }
    }
}