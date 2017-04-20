using System;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading;

using Mf.Util;

namespace Mf.Service.WebSpider
{
	/// <summary>
	/// Summary description for WebPageProcessor.
	/// </summary>
	public class WebPageProcessor
	{
      private  WebSpider         m_spider;

      public WebPageProcessor( WebSpider spider )
		{
         m_spider          = spider;
      }

      public void Process( Object s )
      {
         try
         {
            Log( "Process Uri: {0}, HashCode: {1}", ((WebPageState)s).Uri.AbsoluteUri, Thread.CurrentThread.GetHashCode( ) );

            WebPageState   state = (WebPageState)s;
            WebRequest     req   = WebRequest.Create( state.Uri );
            WebResponse    res   = null;
            Thread.Sleep( 1 );

            try
            {
               res = req.GetResponse( );
               Thread.Sleep( 1 );

               if ( res is HttpWebResponse )
               {
                  state.StatusCode        = ((HttpWebResponse)res).StatusCode.ToString( );
                  state.StatusDescription = ((HttpWebResponse)res).StatusDescription;
               }
               if ( res is FileWebResponse /* && FILE EXIST - TODO */ )
               {
                  state.StatusCode        = "OK";
                  state.StatusDescription = "OK";
               }

               if ( state.StatusCode.Equals( "OK" ) )
               {
                  StreamReader   sr    = new StreamReader( res.GetResponseStream( ) );
            
                  state.Content        = sr.ReadToEnd( );
                  Thread.Sleep( 1 );

                  Log( "Handle Links for Uri: {0}", state.Uri.AbsoluteUri );
                  HandleLinks( state );
               }
            }
            catch( Exception ex )
            {
               if ( ex.ToString( ).IndexOf( "404" ) != -1 )
               {
                  state.StatusCode        = "404";
                  state.StatusDescription = "(404) Not Found";
               }
               else
               {
                  state.StatusDescription = ex.ToString( );
               }
            }
            finally
            {
               if ( res != null )
               {
                  res.Close( );
               }
            }

            state.Content = null;

            Log( "Process Uri: {0} - Success", state.Uri.AbsoluteUri );

         }
         catch (Exception ex)
         {
            Log( "Process Uri: {0} - Failure", ((WebPageState)s).Uri.AbsoluteUri );
            Log( ex.ToString( ) );
         }

         lock ( m_spider )
         {
            m_spider.ThreadCount--;
            if ( m_spider.ThreadCount == 0 )
            {
               _.P( "ResetEvent.Set" );

               m_spider.ResetEvent.Set( );
            }
         }
      }

      private void HandleLinks( WebPageState state )
      {
         string   html     = state.Content;
         Match    m        = RegExUtil.GetMatchRegEx( RegularExpression.UrlExtractor, html );
         
         while( m.Success )
         {
            m_spider.AddWebPage( state.Uri, m.Groups["url"].ToString( ) );

            m = m.NextMatch( );
         }
      }

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

   }
}
