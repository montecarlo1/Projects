using System;

namespace Mf.Service.WebSpider
{
	/// <summary>
	/// Summary description for WebPage.
	/// </summary>
	public class WebPage
	{
      private WebPage( ) {}

		public WebPage( string url )
		{
         m_status = PageStatus.Pending;
         m_url    = url;
		}

      #region properties
      WebPage.PageStatus   m_status;
      string               m_url;
      string               m_content;

      public WebPage.PageStatus Status
      {
         get
         {
            return m_status;
         }
         set
         {
            m_status = value;
         }
      }
      public string Url
      {
         get
         {
            return m_url;
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
      #endregion

      public enum PageStatus
      {
         Pending,
         Success,
         Fail,
         Skip
      }
	}
}
