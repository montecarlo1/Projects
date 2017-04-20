Imports System
Imports System.IO
Imports System.Net
Imports System.Text.RegularExpressions

Namespace Mf.Service.WebSpider

   Public Class WebPageProcessor
      Implements IWebPageProcessor

      Public Function Process(ByVal state As WebPageState) As Boolean Implements IWebPageProcessor.Process
         state.ProcessStarted = True
         state.ProcessSuccessfull = False

         Try
            Console.WriteLine("Process Uri: {0}", state.Uri.AbsoluteUri)

            Dim req As WebRequest = WebRequest.Create(state.Uri)
            Dim res As WebResponse = Nothing

            Try
               res = req.GetResponse()

               If TypeOf res Is HttpWebResponse Then
                  state.StatusCode = CType(res, HttpWebResponse).StatusCode.ToString()
                  state.StatusDescription = CType(res, HttpWebResponse).StatusDescription
               End If

               If TypeOf res Is FileWebResponse Then
                  state.StatusCode = "OK"
                  state.StatusDescription = "OK"
               End If

               If state.StatusCode.Equals("OK") Then
                  Dim sr As New StreamReader(res.GetResponseStream())

                  state.Content = sr.ReadToEnd()

                  If Not (ContentHandler Is Nothing) Then
                     Dim doIt As WebPageContentDelegate = ContentHandler
                     doIt(state)
                  End If
               End If

               state.ProcessSuccessfull = True
            Catch ex As Exception
               HandleException(ex, state)
            Finally
               If Not (res Is Nothing) Then
                  res.Close()
               End If
            End Try
         Catch ex As Exception
            Console.WriteLine(ex.ToString())
         End Try

         Console.WriteLine("Successfull: {0}", state.ProcessSuccessfull)

         Return state.ProcessSuccessfull
      End Function 'Process

#Region "local interface"

      Private Sub HandleException(ByVal ex As Exception, ByRef state As WebPageState) '
         If ex.ToString().IndexOf("(404)") <> -1 Then
            state.StatusCode = "404"
            state.StatusDescription = "(404) Not Found"
         ElseIf ex.ToString().IndexOf("(403)") <> -1 Then
            state.StatusDescription = "(403) Forbidden"

         ElseIf ex.ToString().IndexOf("(502)") <> -1 Then
            state.StatusCode = "502"
            state.StatusDescription = "(502) Bad Gateway"
         ElseIf ex.ToString().IndexOf("(503)") <> -1 Then
            state.StatusCode = "503"
            state.StatusDescription = "(503) Server Unavailable"
         ElseIf ex.ToString().IndexOf("(504)") <> -1 Then
            state.StatusCode = "504"
            state.StatusDescription = "(504) Gateway Timeout"
         ElseIf Not (ex.InnerException Is Nothing) And TypeOf ex.InnerException Is FileNotFoundException Then
            state.StatusCode = "FileNotFound"
            state.StatusDescription = ex.InnerException.Message
         Else
            state.StatusDescription = ex.ToString()
         End If
      End Sub 'HandleException
#End Region

#Region "properties"
      Private m_contentHandler As WebPageContentDelegate = Nothing

      Public Property ContentHandler() As WebPageContentDelegate Implements IWebPageProcessor.HandleContent
         Get
            Return m_contentHandler
         End Get
         Set(ByVal Value As WebPageContentDelegate)
            m_contentHandler = Value
         End Set
      End Property

#End Region

   End Class

End Namespace 'Mf.Service.WebSpider
