using System;
using System.Net;

using Mf.Util;

namespace Mf.Service.WebSpider
{
	/// <summary>
	/// Summary description for WebPage.
	/// </summary>
	public class WebPageState
	{
      private WebPageState( ) {}

		public WebPageState( Uri uri )
		{
         m_webPageStatus   = WebPageStatus.Pending;
         m_uri             = uri;
		}

      #region properties
      WebPageStatus  m_webPageStatus;
      Uri            m_uri;
      string         m_content;
      string         m_statusCode;
      string         m_statusDescription;


      public WebPageStatus WebPageStatus
      {
         get
         {
            return m_webPageStatus;
         }
         set
         {
            m_webPageStatus = value;
         }
      }
      public Uri Uri
      {
         get
         {
            return m_uri;
         }
      }
      public string Content
      {
         get
         {
            return m_content;
         }
         set
         {
            m_content = value;
         }
      }
      public string StatusCode
      {
         get
         {
            return m_statusCode;
         }
         set
         {
            m_statusCode = value;
         }
      }
      public string StatusDescription
      {
         get
         {
            return m_statusDescription;
         }
         set
         {
            m_statusDescription = value;
         }
      }
      #endregion
	}
}
