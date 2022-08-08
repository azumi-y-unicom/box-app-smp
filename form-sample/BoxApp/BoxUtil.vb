
Imports System.IO
Imports Box.V2
Imports Box.V2.Auth
Imports Box.V2.JWTAuth
Imports Box.V2.Config
Imports Box.V2.Models


Public Class BoxUtil
    Public Property mAppConfig As BoxAppConfig

    ''' <summary>boxトークン（60分まで利用可能）</summary>
    ''' <returns></returns>
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


    ''' <summary> </summary>
    ''' <returns></returns>
    Public Async Function Authentication() As Task(Of String)

        'valid For 60 minutes so should be cached And re-used
        Dim adminToken = Await session.AdminTokenAsync()

        Return adminToken.ToString
    End Function

    Public Async Function GetUseInfo() As Task(Of BoxUser)
        Dim client As BoxClient = session.AdminClient(mToken)
        Return Await client.UsersManager.GetCurrentUserInformationAsync()
    End Function


    ''' <summary>boxフォルダ情報を取得する </summary>
    ''' <param name="folderId">フォルダを表す一意の識別子</param>
    ''' <returns>boxフォルダ情報オブジェクト</returns>
    Public Async Function GetFolderInfo(ByVal folderId As String) As Task(Of BoxFolder)
        Dim client As BoxClient = session.AdminClient(mToken)
        Return Await client.FoldersManager.GetInformationAsync(folderId)

    End Function

    ''' <summary> </summary>
    ''' <param name="folderId">検索対象のフォルダを表す一意の識別子</param>
    ''' <param name="targetName">ファイル・フォルダ名</param>
    ''' <returns></returns>
    Public Async Function GetFolderIdByName(ByVal folderId As String, ByVal targetName As String) As Task(Of BoxFolder)
        Dim client As BoxClient = session.AdminClient(mToken)

        Dim fInfo As BoxFolder = Await client.FoldersManager.GetInformationAsync(folderId)
        '' fInfo.
        For Each item In fInfo.ItemCollection.Entries
            ' item.Type Box.V2.
        Next

        Return fInfo

    End Function

    ''' <summary>指定のフォルダをコピーする</summary>
    ''' <param name="sourceId">コピー元のフォルダID</param>
    ''' <param name="targetParentId">コピー先の親フォルダ</param>
    Public Async Sub CopyFolder(ByVal sourceId As String, ByVal targetParentId As String)
        Dim client As BoxClient = session.AdminClient(mToken)

        ' リクエストパラメタを記載
        Dim requestParams = New BoxFolderRequest()
        With requestParams
            .Id = sourceId
            .Parent = New BoxRequestEntity()
            With .Parent
                .Id = targetParentId
            End With
        End With
        ' フォルダをコピーして、コピー後のフォルダ情報を取得
        Dim FolderCopy As BoxFolder = Await client.FoldersManager.CopyAsync(requestParams)

    End Sub

End Class

