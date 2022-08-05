
Imports System.IO
Imports Box.V2
Imports Box.V2.Auth
Imports Box.V2.JWTAuth
Imports Box.V2.Config
Imports Box.V2.Models


Public Class BoxUtil
    Public Property mAppConfig As BoxAppConfig
    Public Property mToken As String

    Public Property config
    Public Property session

    Sub New(conf As BoxAppConfig)
        mAppConfig = conf
        Dim reader = New StreamReader(mAppConfig.ConfigJson)
        Dim json = reader.ReadToEnd()

        config = BoxConfig.CreateFromJsonString(json)
        session = New BoxJWTAuth(config)

    End Sub



    Public Async Function Authentication() As Task(Of String)

        'valid For 60 minutes so should be cached And re-used
        Dim adminToken = Await session.AdminTokenAsync()

        Return adminToken.ToString
    End Function

    Public Async Function GetUseInfo() As Task(Of BoxUser)
        Dim client As BoxClient = session.AdminClient(mToken)
        Return Await client.UsersManager.GetCurrentUserInformationAsync()
    End Function


    ''' <summary> boxフォルダ情報を取得する </summary>
    ''' <param name="folderId">フォルダを表す一意の識別子</param>
    ''' <returns></returns>
    Public Async Function GetFolderInfo(ByVal folderId As String) As Task(Of BoxFolder)
        Dim client As BoxClient = session.AdminClient(mToken)
        Return Await client.FoldersManager.GetInformationAsync(folderId)

    End Function

    ''' <summary>  </summary>
    ''' <param name="folderId">フォルダを表す一意の識別子</param>
    ''' <param name="targetName"></param>
    ''' <returns></returns>
    Public Async Function GetFolderId(ByVal folderId As String, ByVal targetName As String) As Task(Of BoxFolder)
        Dim client As BoxClient = session.AdminClient(mToken)

        Dim fInfo As BoxFolder = Await client.FoldersManager.GetInformationAsync(folderId)
        ' For Each item In fInfo.ItemCollection.Entries
        ' item.Type Box.V2.
        ' Next

        Return fInfo

    End Function


End Class

