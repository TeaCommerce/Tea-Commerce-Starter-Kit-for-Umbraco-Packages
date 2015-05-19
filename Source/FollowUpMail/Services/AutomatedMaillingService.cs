using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using FollowUpMail.Models.Settings;
using FollowUpMail.Repositories;
using TeaCommerce.Api.Models;
using TeaCommerce.Api.Services;

namespace FollowUpMail.Services {
  public class AutomatedMaillingService {
    private static AutomatedMaillingService _instance;
    public static AutomatedMaillingService Instance {
      get { return _instance ?? ( _instance = new AutomatedMaillingService() ); }
    }

    private DateTime GetFinalizedOrderFollowUpDateTime( int days ) {
      return DateTime.Today.Subtract( TimeSpan.FromDays( days ) );
    }

    /// <summary>
    /// Pulls all finalized orders which have reached the threshhold where we may need to send followup emails.
    /// Sends follow up email on orders which contains the value "1" in the order property "followUp". Flags all 
    /// the pulled orders with the order property "followUpProcessedDate" with the current time.
    /// </summary>
    public void SendFollowupMails() {
      FollowUpMailConfig config = ConfigReaderService.Instance.GenericDeSerialize<FollowUpMailConfig>( "~/Config/FollowUpMail.config" );

      AutomatedMaillingRepository automatedMaillingRepository = new AutomatedMaillingRepository();
      Dictionary<long, IEnumerable<Order>> ordersToFollowUp = new Dictionary<long, IEnumerable<Order>>();
      IEnumerable<Store> stores = StoreService.Instance.GetAll();
      foreach ( Store store in stores ) {
        FollowUpMailSetting storeSetting = config.Settings.FirstOrDefault( setting => setting.StoreId == store.Id );
        if ( storeSetting != null ) {
          //use OrderService to get cached element, the Tea Commerce cache is used to prevent concurrency issues
          ordersToFollowUp[ store.Id ] = OrderService.Instance.Get( store.Id, automatedMaillingRepository.GetFollowUpOrdersOlderThenDateTime( store.Id, GetFinalizedOrderFollowUpDateTime( storeSetting.Days ) ) );
        }
      }

      foreach ( KeyValuePair<long, IEnumerable<Order>> storeOrders in ordersToFollowUp ) {
        FollowUpMailSetting storeSetting = config.Settings.FirstOrDefault( setting => setting.StoreId == storeOrders.Key );

        if ( storeSetting != null ) {
          EmailTemplate storeEmailTemplate = EmailTemplateService.Instance.GetAll( storeOrders.Key ).FirstOrDefault( emailTemplate => emailTemplate.Alias == storeSetting.TemplateAlias );
          if ( storeEmailTemplate != null ) {
            foreach ( Order order in storeOrders.Value ) {
              order.Properties.AddOrUpdate( "followUpProcessedDate", DateTime.Now.ToString( DateTimeFormatInfo.InvariantInfo ) );
              if ( order.Properties[ "followUp" ] == "1" ) {
                try {
                  //try in case the email doesn't send correct, could happen if email isn't formatted correctly.
                  storeEmailTemplate.Send( order, order.PaymentInformation.Email, order.LanguageId );
                } catch ( Exception e ) {
                  //The above can throw exceptions, this should not block execution so we fail silently.
                }
              }
              order.Save();
            }
          }
        }
      }
    }
  }
}