using System.Linq;
using TeaCommerce.Api.Models;
using TeaCommerce.Api.Notifications;
using TeaCommerce.Api.Web;
using Umbraco.Core;
using Zero_cost_payment_method_management.Models.Settings;
using Zero_cost_payment_method_management.Services;

namespace Zero_cost_payment_method_management.EventHandlers {
  public class ZeroCostPaymentHandler : ApplicationEventHandler {

    protected override void ApplicationStarted( UmbracoApplicationBase umbracoApplication, ApplicationContext applicationContext ) {
      NotificationCenter.Order.Updated += Order_Updated;
    }

    /*
     * Use a special payment method if the total price of the order i 0.
     * Set the payment method to the default payment method for the country if the price of the order goes above 0.
     */
    private void Order_Updated( Order order, OrderUpdatedEventArgs orderUpdatedEventArgs ) {
      ZeroCostPaymentConfig config = ConfigReaderService.Instance.GenericDeSerialize<ZeroCostPaymentConfig>( "~/Config/ZeroCostPaymentMethodHandler.config" );
      ZeroCostPaymentSetting storeSetting = config.Settings.FirstOrDefault( setting => setting.StoreId == order.StoreId );

      long zeroCostPaymentId = storeSetting != null ? storeSetting.PaymentMethodId : 0;

      if ( order.PaymentInformation.PaymentMethodId == zeroCostPaymentId && order.TotalPrice.Value.WithVat > 0 ) {
        Country orderCountry = TeaCommerceHelper.GetCurrentPaymentCountry( order.StoreId );
        order.PaymentInformation.PaymentMethodId = orderCountry.DefaultPaymentMethodId;
        order.Save();
      } else if ( order.TotalPrice.Value.WithVat <= 0 && order.PaymentInformation.PaymentMethodId != zeroCostPaymentId ) {
        order.PaymentInformation.PaymentMethodId = zeroCostPaymentId;
        order.Save();
      }
    }
  }
}