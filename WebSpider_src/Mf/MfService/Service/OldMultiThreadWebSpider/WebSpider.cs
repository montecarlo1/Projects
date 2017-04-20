using System;
using System.Collections;
using System.Net;
using System.Threading;
using System.Windows.Forms;
using Mf.Util;
//using Mf._Test.Util;

namespace Mf.Service.WebSpider
{
   /// <summary>
   /// Summary description for Spider.
   /// </summary>
   public class WebSpider
   {
      public  const  int               MAX_THREADS = 20;

      private        int               m_threadCount;
      private        int               m_threadCountMax;

      private        Uri      	      m_startUri;
      private        Uri   		      m_baseUri;
      private        int	            m_uriProcessedCountMax;
      private        int			      m_uriProcessedCount;
      private        bool              m_saveWebContent;

      private        Queue    	      m_webPagesPending;
      private        Hashtable	      m_webPages;
      private        WebPageProcessor  m_processor;
      private        ManualResetEvent  m_resetEvent;

      public WebSpider( string startUri, string baseUri, int uriProcessedCountMax ) 
         : this ( startUri, baseUri, uriProcessedCountMax, 10, false )
      {
      }

      public WebSpider( string startUri, string baseUri, int uriProcessedCountMax, int threadCountMax, bool saveWebContent )
      {
         StartUri 	               = new Uri( startUri );
         BaseUri  	               = new Uri( Is.EmptyString( baseUri ) ? m_startUri.GetLeftPart( UriPartial.Authority ) : baseUri );

         UriProcessedCountMax       = uriProcessedCountMax;
         SaveWebContent             = saveWebContent;

         ThreadCountMax             = threadCountMax;
         
         // This should move down to execute, will need to wait for interface
         ThreadCount                = 0;  
         m_processor                = new WebPageProcessor( this );

         m_webPagesPending          = new Queue( );
         m_webPages                 = new Hashtable( );
      }

      #region public interface
      public void Execute( )
      {
         m_uriProcessedCount = 0;

         DateTime start = DateTime.Now;

         Log( "Start At: " + start );

         AddWebPage( m_startUri, m_startUri.AbsoluteUri );

         try
         {
            while ( m_webPagesPending.Count > 0 && m_uriProcessedCount < m_uriProcessedCountMax )
            {
//               Log( "MainLoop - Thread Count: {0}, PendingPages: {1}", m_threadCount, m_webPagesPending.Count );

               ResetEvent = new ManualResetEvent( false );

               while ( m_threadCount < m_threadCountMax && m_webPagesPending.Count > 0 )
               {
                  Log( "Inner-MainLoop - Thread Count: {0}, PendingPages: {1}, MAX: {2}", m_threadCount, m_webPagesPending.Count, m_threadCountMax );

                  ThreadPool.QueueUserWorkItem( 
                     new WaitCallback(m_processor.Process), m_webPagesPending.Dequeue( ) );

                  m_uriProcessedCount++;
                  m_threadCount++;

//                  Log( "Inner-MainLoopEnd - Thread Count: {0}, PendingPages: {1}", m_threadCount, m_webPagesPending.Count );
               }

               ResetEvent.WaitOne( Timeout.Infinite, true );

//               Log( "MainLoopEnd - Thread Count: {0}, PendingPages: {1}", m_threadCount, m_webPagesPending.Count );
            }

         }
         catch (NotSupportedException)
         {
            Log( "This class will fail on systems with limited thread support" );
         }

         DateTime end = DateTime.Now;

         float elasped = (end.Ticks - start.Ticks)/10000000;

         Log( "End At: " + end );
         Log( "Elasped Time: {0} seconds", elasped );

         Log( "Pages Processed: "   + UriProcessedCount );
         Log( "Pages Pending: "     + m_webPagesPending.Count );
      }

      public void AddWebPage( Uri baseUri, string newUri )
      {
         // Remove any anchors
         string url = StrUtil.LeftIndexOf( newUri, "#" );   

         // Construct a Uri, using the current page Uri as a base reference
         Uri    uri = new Uri( baseUri, url );

         if ( ! ValidPageExtension( uri.LocalPath ) )
         {
//            Log( "Uri not processed: " + uri + " - Invalid Extension" );

            return;
         }

         if ( m_webPages.Contains( uri ) )
         {
            // Log( "Add WebPage: " + uri + " - Allready exists" );
         }
         else
         {
//            Log( "Add WebPage: " + uri );

            WebPageState state = new WebPageState( uri );

            // Only process pages that are with in the same location as this site

            if ( uri.AbsoluteUri.StartsWith( BaseUri.AbsoluteUri ) )
            {
               m_webPagesPending.Enqueue  ( state );
            }

            m_webPages.Add             ( uri, state );
         }
      }
      #endregion

      #region local interface

      private static readonly string[] m_validPageExtensions = new string[]
            {"html", "php", "asp", "htm", "jsp", "shtml", "php3", "aspx", "pl", "cfm" };

      private bool ValidPageExtension( string path ) 
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

      #region Log
      private void Log( string v ) 
      { 
         _.P( v );
      }
      private void Log( string format, object o1 ) 
      { 
         _.P( format, o1 );
      }
      private void Log( string format, object o1, object o2 ) 
      { 
         _.P( format, o1, o2 );
      }
      private void Log( string format, object o1, object o2, object o3 ) 
      { 
         _.P( format, o1, o2, o3 );
      }
      #endregion

      #region properties
      public int ThreadCount
      {
         get
         {
            return m_threadCount;
         }
         set
         {
            m_threadCount = value;
         }
      }
      public int ThreadCountMax
      {
         get
         {
            return m_threadCountMax;
         }
         set
         {  // Must be between 1 && MAX_THREADS
            m_threadCountMax = ( value<1 ? 1 : ( value>MAX_THREADS ? MAX_THREADS : value ) );
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
      public int UriProcessedCount
      {
         get
         {
            return m_uriProcessedCount;
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
      public bool SaveWebContent
      {
         get
         {
            return m_saveWebContent;
         }
         set
         {  
            m_saveWebContent = value;
         }
      }
      public Hashtable WebPages
      {
         get
         {
            return m_webPages;
         }
      }
      internal ManualResetEvent ResetEvent
      {
         get
         {
            return m_resetEvent;
         }
         set
         {
            m_resetEvent = value;
         }
      }
      #endregion
   }

}
