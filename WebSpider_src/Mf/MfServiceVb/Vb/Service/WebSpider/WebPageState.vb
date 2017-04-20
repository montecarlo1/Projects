Imports System
Imports System.Net

Imports Mf.Util

Namespace Mf.Service.WebSpider

   Public Class WebPageState

      Private Sub New()
      End Sub

      Public Sub New(ByVal uri As Uri)
         m_uri = uri
      End Sub

      Public Sub New(ByVal uri As String)
         MyClass.New(New Uri(uri))
      End Sub

#Region "properties"
      Private m_uri As Uri
      Private m_content As String
      Private m_processInstructions As String = ""
      Private m_processStarted As Boolean = False
      Private m_processSuccessfull As Boolean = False
      Private m_statusCode As String
      Private m_statusDescription As String

      Public ReadOnly Property Uri() As Uri
         Get
            Return m_uri
         End Get
      End Property

      Public Property ProcessStarted() As Boolean
         Get
            Return m_processStarted
         End Get
         Set(ByVal Value As Boolean)
            m_processStarted = Value
         End Set
      End Property
      Public Property ProcessSuccessfull() As Boolean
         Get
            Return m_processSuccessfull
         End Get
         Set(ByVal Value As Boolean)
            m_processSuccessfull = Value
         End Set
      End Property

      Public Property ProcessInstructions() As String
         Get
            Return m_processInstructions
         End Get
         Set(ByVal Value As String)
            m_processInstructions = Value
         End Set
      End Property

      Public Property Content() As String
         Get
            Return m_content
         End Get
         Set(ByVal Value As String)
            m_content = Value
         End Set
      End Property

      Public Property StatusCode() As String
         Get
            Return m_statusCode
         End Get
         Set(ByVal Value As String)
            m_statusCode = Value
         End Set
      End Property

      Public Property StatusDescription() As String
         Get
            Return m_statusDescription
         End Get
         Set(ByVal Value As String)
            m_statusDescription = Value
         End Set
      End Property
#End Region

   End Class

End Namespace
