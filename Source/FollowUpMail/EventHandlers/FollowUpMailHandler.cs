using System;
using System.Globalization;
using TeaCommerce.Api.Models;
using TeaCommerce.Api.Notifications;
using Umbraco.Core;

namespace FollowUpMail.EventHandlers {
  public class FollowUpMailHandler : ApplicationEventHandler {
    protected override void ApplicationStarted( UmbracoApplicationBase umbracoApplication, ApplicationContext applicationContext ) {
      NotificationCenter.Order.Finalized += Order_Finalized;
      NotificationCenter.EmailTemplate.MailSend += EmailTemplate_MailSend;
    }

    private void EmailTemplate_MailSend( EmailTemplate emailTemplate, MailSendEventArgs e ) {
      if ( emailTemplate.Alias == "followupEmail" ) {
        //note that we have allready followed up on this mail. This is to prevent us sending more then one followup mail.
        e.Order.Properties.AddOrUpdate( "followUp", "0" );

        //note the date when a followup mail was last sent.
        e.Order.Properties.AddOrUpdate( "followUpMailSentDate", DateTime.Now.ToString( DateTimeFormatInfo.InvariantInfo ) );
        e.Order.Save();
      }
    }

    private void Order_Finalized( Order order, FinalizedEventArgs finalizedEventArgs ) {
      //Mark the order to be followed up in the followup flow.
      order.Properties.AddOrUpdate( "followUp", "1" );
      order.Properties.AddOrUpdate( "followUpProcessedDate", "" );
      order.Save();
    }
  }
}