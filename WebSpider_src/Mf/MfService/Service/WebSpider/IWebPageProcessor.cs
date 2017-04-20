using System;
namespace Mf.Service.WebSpider
{
	/// <summary>
	/// Summary description for IWebPageProcessor.
	/// </summary>
   public interface IWebPageProcessor
   {
      bool Process( WebPageState state );

      WebPageContentDelegate ContentHandler { get; set; }
   }

   public delegate void WebPageContentDelegate( WebPageState state );
}