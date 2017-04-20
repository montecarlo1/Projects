Imports System

Namespace Mf.Service.WebSpider

   Public Interface IWebPageProcessor
      Function Process(ByVal state As WebPageState) As Boolean

      Property HandleContent() As WebPageContentDelegate
   End Interface

   Public Delegate Sub WebPageContentDelegate(ByVal state As WebPageState)

End Namespace