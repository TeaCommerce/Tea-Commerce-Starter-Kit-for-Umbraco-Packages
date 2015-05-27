using System.IO;
using System.Web;
using System.Xml.Serialization;

namespace ZeroCostPayment.Services {
  public class ConfigReaderService {
    private static ConfigReaderService _instance;

    public static ConfigReaderService Instance {
      get { return _instance ?? ( _instance = new ConfigReaderService() ); }
    }

    public T GenericDeSerialize<T>( string path ) {
      XmlSerializer serializer = new XmlSerializer( typeof( T ) );

      TextReader tr = new StreamReader( HttpContext.Current.Server.MapPath( path ) );

      T b = (T)serializer.Deserialize( tr );
      tr.Close();
      return b;
    }
  }
}