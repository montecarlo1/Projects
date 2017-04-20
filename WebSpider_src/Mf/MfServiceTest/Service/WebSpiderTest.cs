using System;
using System.Collections;
using System.Data;
using System.IO;
using System.Text.RegularExpressions;
using NUnit.Framework;
using Mf.Service.WebSpider;
using Mf.Util;

namespace MfServiceTest.Service
{
   /// <summary>
   /// Summary description for TestWebSpider.
   /// </summary>
   [TestFixture]
   public class TestWebSpider
   {
      private RunSpider rs;
      private string    uri;
      private bool      isResultsOutput;
      private bool      isLogging;
      private bool      isLongRunning;
      private bool      is1st = true;
      private string    testLocation;

      [SetUp]
      public void Init()
      {
         isResultsOutput   = true;
         isLogging         = true;
         isLongRunning     = false;
         testLocation      = "C:/_AppDev/Mf/MfServiceTest/Service/TestWebSiteFileStructure/";

         if ( is1st )
         {
            is1st = false;
            Console.WriteLine( "Logging       : " + ( isLogging        ? "ON" : "OFF" ) );
            Console.WriteLine( "Results Output: " + ( isResultsOutput  ? "ON" : "OFF" ) );
            Console.WriteLine( "Long Running  : " + ( isLongRunning    ? "ON" : "OFF" ) );
         }
      }
  
      [Test]
      public void GoodUrl1( )
      {
         // Good url, 13 links on this url
         uri   = "http://www.holidaycoast.net.au/";
         rs    = new RunSpider( uri, 1 );

         PrintSpiderResults( rs );

         Assertion.AssertEquals( "Total Pages", rs.Pages.Length, 12 );
         SiteAssert( rs, uri, true, "OK" );
      }

      [Test]
      public void GoodUrl2( )
      {
         // Good url, 39 links on this url
         uri   = "http://www.gotdotnet.com/Community/Workspaces/workspace.aspx?id=c20d12b0-af52-402b-9b7c-aaeb21d1f431";
         rs    = new RunSpider( uri, 1 );

         PrintSpiderResults( rs );

         Assertion.AssertEquals( "Total Pages", rs.Pages.Length, 40 );
         SiteAssert( rs, uri, true, "OK" );
      }

      [Test]
      public void GoodUrlBaseUrl1( )
      {
         // Good url, Base Url is a path. Only do full parse in this path
         uri   = "http://www.holidaycoast.net.au/coffsharbour/";
         rs    = new RunSpider( uri, uri, 5 );

         PrintSpiderResults( rs );

         Assertion.AssertEquals( "Total Pages", rs.Pages.Length, 14 );
         SiteAssert( rs, uri, true, "OK" );
      }

      [Test]
      public void GoodUrlBaseUrl2( )
      {
         // Good url, Base Url is domain, Do full parse on any part of the domain
         uri   = "http://www.holidaycoast.net.au/coffsharbour/";
         rs    = new RunSpider( uri, "http://www.holidaycoast.net.au/", 5 );

         PrintSpiderResults( rs );

         Assertion.AssertEquals( "Total Pages", rs.Pages.Length, 46 );
         SiteAssert( rs, uri, true, "OK" );
      }

      [Test]
      public void GoodUrlBadRef( )
      {
         // TODO - Url works but the id value returns a bad page. Need to look into options here
         uri   = "http://www.gotdotnet.com/Community/Workspaces/workspace.aspx?id=c20d12b0-af52-402b-9b7c-aaeb21d1f43";
         rs    = new RunSpider( uri, 1 );

         PrintSpiderResults( rs );

         Assertion.AssertEquals( "Total Pages"  , rs.Pages.Length, 1 );
         SiteAssert( rs, uri, false, "404" );
      }

      [Test]
      public void E404BadUrl( )
      {
         // Bad Url
         uri   = "http://www.holidaycoast.net.au/nucklehead";
         rs    = new RunSpider( uri, 1 );

         PrintSpiderResults( rs );

         Assertion.AssertEquals( "Total Pages"  , rs.Pages.Length, 1 );
         SiteAssert( rs, uri, false, "404" );
      }

      [Test]
      public void E403Forbidden( )
      {
         // Bad Url
         uri   = "http://www.midcoast.com.au/~jpilgrim";
         rs    = new RunSpider( uri, 1 );

         PrintSpiderResults( rs );

         Assertion.AssertEquals( "Total Pages"  , rs.Pages.Length, 1 );
         SiteAssert( rs, uri, false, "403" );
      }

      [Test]
      public void E502BadHost( )
      {
         // Bad Host
         uri   = "http://www.invalidspidersite.net.au/";
         rs    = new RunSpider( uri, 1 );

         PrintSpiderResults( rs );

         Assertion.AssertEquals( "Total Pages"  , rs.Pages.Length, 1 );
         SiteAssert( rs, uri, false, "502" );
      }

      [Test]
      public void EntireSiteLongRunning( )
      {
         if ( isLongRunning )
         {
            uri   = "http://www.holidaycoast.net.au/";
            rs    = new RunSpider( uri, -1 );

            PrintSpiderResults( rs );

            Assertion.AssertEquals( "Total Pages", rs.Pages.Length, 224 );
            SiteAssert( rs, uri, true, "OK" );
         }
      }

      [Test]
      public void FileBaseUrl( )
      {
         uri   = testLocation + "index.html";
         rs    = new RunSpider( uri, "file:///" + testLocation, -1 );

         PrintSpiderResults( rs );

         Assertion.AssertEquals( "Total Pages", rs.Pages.Length, 12 );
         SiteAssert( rs, uri, true, "OK" );
      }

      [Test]
      public void ManualProcessor( )
      {
         uri   = "http://www.holidaycoast.net.au/";

         WebPageState         state       = new WebPageState( new Uri( uri ) );
         WebPageProcessor     processor   = new WebPageProcessor( );

         Assertion.Assert( "Process Page", processor.Process( state ) );
         Assertion.AssertEquals( "OK", state.StatusCode );
         Assertion.AssertEquals( true, state.ProcessSuccessfull );
      }

      [Test]
      public void ManualProcessorMultipleDelegates( )
      {
         ManualProcessorLinksCounter = 0;

         uri   = "http://www.holidaycoast.net.au/";

         WebPageState         state       = new WebPageState( new Uri( uri ) );
         WebPageProcessor     processor   = new WebPageProcessor( );

         processor.ContentHandler += new WebPageContentDelegate( HandleLinks );
         processor.ContentHandler += new WebPageContentDelegate( HandleContent );

         Assertion.Assert        ( "Process Page", processor.Process( state ) );
         Assertion.AssertEquals  ( "OK", state.StatusCode );
         Assertion.AssertEquals  ( true, state.ProcessSuccessfull );
         Assertion.AssertEquals  ( 16, ManualProcessorLinksCounter );
         Assertion.Assert        ( ManualProcessorContentFound );
      }

      private static int ManualProcessorLinksCounter = 0;
      private void HandleLinks( WebPageState state )
      {
         Match m = RegExUtil.GetMatchRegEx( RegularExpression.UrlExtractor, state.Content );
      
         while( m.Success )
         {
            ManualProcessorLinksCounter++;
            m = m.NextMatch( );
         }
      }

      private static bool ManualProcessorContentFound = false;
      private void HandleContent( WebPageState state )
      {
         ManualProcessorContentFound = state.Content.IndexOf( "Coffs" ) != -1;
      }

      private void SiteAssert( RunSpider rs, string uri, bool expProcessSuccesfull, string expStatusCode )
      {
         foreach ( WebPageState page in rs.Pages )
         {
            if ( uri.Equals( page.Uri ) )
            {
               Assertion.AssertEquals( "Page Status"  , expProcessSuccesfull  , page.ProcessSuccessfull  );
               Assertion.AssertEquals( "Page Code"    , expStatusCode         , page.StatusCode          );
            }
         }
      }

      private void PrintSpiderResults( RunSpider rs )
      {
         if ( isResultsOutput )
         {
            foreach( WebPageState state in rs.Pages )
            {
               Console.WriteLine( "Code: {0,4}, Process Status: {1}, Url: {2}, Desc: {3}", state.StatusCode, ProcessStatus( state ) ,state.Uri.AbsoluteUri, state.StatusDescription );
            }
         }
      }

      private string ProcessStatus( WebPageState state )
      {
         if ( state.ProcessInstructions.IndexOf( "Handle Links" ) == -1 )
         {
            return "Missed ";
         }
         return ( state.ProcessSuccessfull ? "Success" : "Failed " );
      }
   }

   public class RunSpider
   {
      public WebSpider      Spider;
      public WebPageState[] Pages;

      public RunSpider( string uri, int maxUri ) : this( uri, uri, maxUri ) { }

      public RunSpider( string uri, string baseUri, int maxUri )
      {
         Spider = new WebSpider( uri, baseUri, maxUri );
         Spider.Execute( );

         ICollection    webPages = Spider.WebPages.Values;

         Pages = new WebPageState[ webPages.Count ];

         int index = 0;
         foreach ( WebPageState webPage in webPages )
         {
            Pages[ index++ ] = webPage;
         }
      }
   }
}
