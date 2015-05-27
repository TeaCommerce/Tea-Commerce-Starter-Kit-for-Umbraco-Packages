using System.Collections.Generic;
using System.Xml.Serialization;

namespace ZeroCostPayment.Models.Settings {
  public class ZeroCostPaymentConfig {
    [XmlElement( "zeroCostPayment" )]
    public List<ZeroCostPaymentSetting> Settings = new List<ZeroCostPaymentSetting>();
  }

  public class ZeroCostPaymentSetting {
    [XmlAttribute( "storeId" )]
    public long StoreId { get; set; }

    [XmlAttribute( "methodId" )]
    public long PaymentMethodId { get; set; }
  }
}