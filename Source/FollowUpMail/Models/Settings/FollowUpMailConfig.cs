using System.Collections.Generic;
using System.Xml.Serialization;

namespace FollowUpMail.Models.Settings {
  public class FollowUpMailConfig {
    [XmlElement( "FollowUpMail" )]
    public List<FollowUpMailSetting> Settings = new List<FollowUpMailSetting>();
  }

  public class FollowUpMailSetting {
    [XmlAttribute( "storeId" )]
    public long StoreId { get; set; }

    [XmlAttribute( "templateAlias" )]
    public string TemplateAlias { get; set; }    
    
    [XmlAttribute( "days" )]
    public int Days { get; set; }
  }
}