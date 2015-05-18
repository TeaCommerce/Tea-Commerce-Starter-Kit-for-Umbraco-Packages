using System;
using System.Collections.Generic;
using System.Data;
using Umbraco.Core;
using Umbraco.Core.Persistence;

namespace FollowUpMail.Repositories {
  public class AutomatedMaillingRepository {

    /// <summary>
    /// Returns all orders which are finalized within the store denoted by storeId, have the orderproperty "followUpProcessedDate" set to "" and where finalized longer then dateTime ago.
    /// </summary>
    /// <param name="storeId">The store to query.</param>
    /// <param name="dateTime">The threshold for the query. Only orders which were finalized before this date will be considered.</param>
    /// <returns></returns>
    public IEnumerable<Guid> GetFollowUpOrdersOlderThenDateTime( long storeId, DateTime dateTime ) {
      UmbracoDatabase db = ApplicationContext.Current.DatabaseContext.Database;
      string sql = "SELECT * FROM TeaCommerce_Order as O LEFT JOIN TeaCommerce_CustomOrderProperty as OPFollow on O.Id = OPFollow.OrderId AND OPFollow.Alias = 'followUpProcessedDate' ";
      sql += "LEFT JOIN TeaCommerce_CustomOrderProperty as OPShipped on O.Id = OPShipped.OrderId AND OPShipped.Alias = 'shippedDate' WHERE O.DateFinalized IS NOT NULL AND O.StoreId = @0 ";
      sql += "AND O.DateFinalized < @1 AND OPFollow.Value = ''";
      return db.Fetch<Guid>( sql, storeId, dateTime.ToString( "s" ) );
    }
  }
}