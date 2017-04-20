using System;
using System.Collections;
using System.Text.RegularExpressions;
using System.IO;
using System.Net;
using System.Windows.Forms;

using Mf.Util;

namespace Mf.Service.WebSpider
{
   /// <summary>
   /// Summary description for Spider.
   /// </summary>
   public class WebSpider
   {
      private        Uri      	      m_startUri;
      private        Uri   		      m_baseUri;
      private        int	            m_uriProcessedCountMax;
      private        int			      m_uriProcessedCount;
      private        bool              m_keepWebContent;

      private        Queue    	      m_webPagesPending;
      private        Hashtable	      m_webPages;
      private        IWebPageProcessor m_webPageProcessor;

      public WebSpider( string startUri ) : 
         this ( startUri, -1 ) { }

      public WebSpider( string startUri, int uriProcessedCountMax ) : 
         this ( startUri, "", uriProcessedCountMax, false, new WebPageProcessor( ) ) { }

      public WebSpider( string startUri, string baseUri, int uriProcessedCountMax ) : 
         this ( startUri, baseUri, uriProcessedCountMax, false, new WebPageProcessor( ) ) { }

      public WebSpider( string startUri, string baseUri, int uriProcessedCountMax, bool keepWebContent, IWebPageProcessor webPageProcessor )
      {
         StartUri 	               = new Uri( startUri );
         
         // In future this could be null and will process cross-site, but for now must exist
         BaseUri  	               = new Uri( Is.EmptyString( baseUri ) ? m_startUri.GetLeftPart( UriPartial.Authority ) : baseUri );

         UriProcessedCountMax       = uriProcessedCountMax;
         KeepWebContent             = keepWebContent;

         m_webPagesPending          = new Queue( );
         m_webPages                 = new Hashtable( );

         m_webPageProcessor         = webPageProcessor;

         m_webPageProcessor.ContentHandler += new WebPageContentDelegate( this.HandleLinks );
      }

      #region public interface
      public void Execute( )
      {
         UriProcessedCount = 0;

         DateTime start = DateTime.Now;

         Console.WriteLine( "======================================================================================================" );
         Console.WriteLine( "Proccess URI: " + m_startUri.AbsoluteUri );
         Console.WriteLine( "Start At    : " + start );
         Console.WriteLine( "------------------------------------------------------------------------------------------------------" );

         AddWebPage( StartUri, StartUri.AbsoluteUri );

         try
         {
            while ( WebPagesPending.Count > 0 && 
               ( UriProcessedCountMax == -1 || UriProcessedCount < UriProcessedCountMax ) )
            {
               Console.WriteLine( "Max URI's: {0}, Processed URI's: {1}, Pending URI's: {2}", UriProcessedCountMax, UriProcessedCount, WebPagesPending.Count );

               WebPageState state = (WebPageState)m_webPagesPending.Dequeue( );

               m_webPageProcessor.Process( state );

               if ( ! KeepWebContent )
               {
                  state.Content = null;
               }

               UriProcessedCount++;
            }
         }
         catch (Exception ex)
         {
            Console.WriteLine( "Failure while running web spider: " + ex.ToString( ) );
         }

         DateTime end = DateTime.Now;
         float elasped = (end.Ticks - start.Ticks)/10000000;

         Console.WriteLine( "------------------------------------------------------------------------------------------------------" );
         Console.WriteLine( "URI Finished   : " + m_startUri.AbsoluteUri );
         Console.WriteLine( "Pages Processed: " + UriProcessedCount );
         Console.WriteLine( "Pages Pending  : " + WebPagesPending.Count );
         Console.WriteLine( "End At         : " + end );
         Console.WriteLine( "Elasped Time   : {0} seconds", elasped );
         Console.WriteLine( "======================================================================================================" );
      }

      public void HandleLinks( WebPageState state )
      {
         if ( state.ProcessInstructions.IndexOf( "Handle Links" ) != -1 )
         {
            int   counter  = 0;
            Match m        = RegExUtil.GetMatchRegEx( RegularExpression.UrlExtractor, state.Content );

            while( m.Success )
            {
               if ( AddWebPage( state.Uri, m.Groups["url"].ToString( ) ) )
               {
                  counter++;
               }

               m = m.NextMatch( );
            }

            Console.WriteLine( "           : {0} new links were added", counter );
         }
      }

      private bool AddWebPage( Uri baseUri, string newUri )
      {
         // Remove any anchors
         string   url      = StrUtil.LeftIndexOf( newUri, "#" );   

         // Construct a Uri, using the current page Uri as a base reference
         Uri      uri      = new Uri( baseUri, url );

         if ( ! ValidPage( uri.LocalPath ) || m_webPages.Contains( uri ) )
         {
            return false;
         }
         WebPageState state = new WebPageState( uri );

         // Only process links for pages within the same site. 
         if ( uri.AbsoluteUri.StartsWith( BaseUri.AbsoluteUri ) )
         {
            state.ProcessInstructions += "Handle Links";
         }

         m_webPagesPending.Enqueue  ( state );
         m_webPages.Add             ( uri, state );
         
         return true;
      }
      #endregion

      #region local interface
      private static readonly string[] m_validPageExtensions = new string[]
            {"html", "php", "asp", "htm", "jsp", "shtml", "php3", "aspx", "pl", "cfm" };

      // A page is considered valid if there is no extension or if the
      // last character is a /.  If there is an extension then the page
      // is considered valid if that extension is classed as valid.
      private bool ValidPage( string path ) 
      { 
         int   pos      = path.IndexOf( "." );

         if ( pos == -1 || path[ path.Length-1 ] == 47 ) /*.ToString( ).Equals( "/" )*/ 
         {
            return true;
         }

         string uriExt = StrUtil.RightOf( path, pos ).ToLower( );

         // Uri ends in an extension
         foreach ( string ext in m_validPageExtensions )
         {
            if ( uriExt.Equals( ext ) )
            {
               return true;
            }
         }

         return false;
      }
      #endregion

      #region properties
      public IWebPageProcessor WebPageProcessor
      {
         get
         {
            return m_webPageProcessor;
         }
         set
         {  
            m_webPageProcessor = value;
         }
      }
      public Uri StartUri
      {
         get
         {
            return m_startUri;
         }
         set
         {  
            m_startUri = value;
         }
      }
      public Uri BaseUri
      {
         get
         {
            return m_baseUri;
         }
         set
         {  
            m_baseUri = value;
         }
      }
      private int UriProcessedCount
      {
         get
         {
            return m_uriProcessedCount;
         }
         set
         {
            m_uriProcessedCount = value;
         }
      }
      public int UriProcessedCountMax
      {
         get
         {
            return m_uriProcessedCountMax;
         }
         set
         {  
            m_uriProcessedCountMax = value;
         }
      }
      public bool KeepWebContent
      {
         get
         {
            return m_keepWebContent;
         }
         set
         {  
            m_keepWebContent = value;
         }
      }
      public Hashtable WebPages
      {
         get
         {
            return m_webPages;
         }
         set
         {
            m_webPages = value;
         }
      }
      private Queue WebPagesPending
      {
         get
         {
            return m_webPagesPending;
         }
         set
         {
            m_webPagesPending = value;
         }
      }
      #endregion
   }
}
