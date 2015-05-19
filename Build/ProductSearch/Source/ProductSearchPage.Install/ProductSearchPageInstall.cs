using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Xml;
using TeaCommerce.Api.Models;
using TeaCommerce.Api.Services;
using umbraco.cms.businesslogic.packager.standardPackageActions;
using umbraco.interfaces;
using Umbraco.Core;
using Umbraco.Core.Models;
using Umbraco.Core.Services;
using Umbraco.Web;
using ContentExtensions = umbraco.ContentExtensions;
using File = System.IO.File;

namespace ProductSearchPage.Install {
  public class ProductSearchPageInstall : IPackageAction {
    public string Alias() {
      return "ProductSearchPageInstaller";
    }

    public bool Execute( string packageName, XmlNode xmlData ) {
      IMediaService mediaService = ApplicationContext.Current.Services.MediaService;
      IContentService contentService = UmbracoContext.Current.Application.Services.ContentService;
      IContentTypeService contentTypeService = ApplicationContext.Current.Services.ContentTypeService;

      //Create ProductSearchPage
      IContent langContent = contentService.GetByLevel( 1 ).FirstOrDefault();
      var searchPage = contentService.CreateContent( "Search", langContent, "ProductSearchPage" );
      contentService.SaveAndPublishWithStatus( searchPage );


      return true;

    }

    public XmlNode SampleXml() {
      return helper.parseStringToXmlNode( string.Format( @"<Action runat=""install"" alias=""{0}"" />", Alias() ) );
    }

    public bool Undo( string packageName, XmlNode xmlData ) {
      //Remove stuff
      return true;
    }
  }
}
