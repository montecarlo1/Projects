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
         m_uri             = uri;
		}

      public WebPageState( string uri ) 
         : this( new Uri( uri ) ) { }

      #region properties
      Uri            m_uri;
      string         m_content;
      string         m_processInstructions   = "";
      bool           m_processStarted        = false;
      bool           m_processSuccessfull    = false;
      string         m_statusCode;
      string         m_statusDescription;

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
      public bool ProcessStarted
      {
         get
         {
            return m_processStarted;
         }
         set
         {
            m_processStarted = value;
         }
      }
      public bool ProcessSuccessfull
      {
         get
         {
            return m_processSuccessfull;
         }
         set
         {
            m_processSuccessfull = value;
         }
      }
      public string ProcessInstructions
      {
         get
         {
            return ( m_processInstructions == null ? "" : m_processInstructions );
         }
         set
         {
            m_processInstructions = value;
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
