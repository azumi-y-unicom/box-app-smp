
Imports System.IO
Imports Box.V2
Imports Box.V2.JWTAuth
Imports Box.V2.Config
Imports Box.V2.Models
Imports Box.V2.Utility

Public Class BoxUtil

    ''' <summary>アプリ設定</summary>
    Public Property mAppConfig As BoxAppConfig

    ''' <summary>boxトークン（同一のトークンは60分まで利用可能）</summary>
    Public Property mToken As String

    Public Property boxConf
    Public Property session

#Region "定数"
    Public Const BOX_ITEM_TYPE_FOLDER As String = "folder"
    Public Const BOX_ITEM_TYPE_FILE As String = "file"

#End Region


    Sub New(conf As BoxAppConfig)
        mAppConfig = conf
        Dim reader = New StreamReader(mAppConfig.AuthJsonFilePath)
        Dim json = reader.ReadToEnd()

        boxConf = BoxConfig.CreateFromJsonString(json)
        session = New BoxJWTAuth(boxConf)

    End Sub

#Region "ロジック部"

    ''' <summary>
    ''' フォルダパス形式で指定したフォルダ上に指定した名前のフォルダを新規で作成する
    ''' </summary>
    ''' <param name="folderParentPath">配置先のフォルダパス</param>
    ''' <param name="newFolderName"></param>
    Public Sub CreateFolderByTreePath(ByVal folderParentPath As String, ByVal newFolderName As String)
        Dim folderParentId As String = "0"

        ' フォルダパス存在しない場合はエラーとする。
        CreateFolder(folderParentId, newFolderName)
    End Sub

    ''' <summary>
    ''' Boxにファイルアップロードする
    ''' </summary>
    ''' <param name="folderParentPath">配置先のフォルダパス</param>
    ''' <param name="upLoadFileName"></param>
    ''' <param name="localFilePath"></param>
    Public Sub UploadFileByTreePath(ByVal folderParentPath As String, ByVal upLoadFileName As String, ByVal localFilePath As String)
        Dim folderParentId As String = "0"

        ' フォルダパス存在しない場合はエラーとする。

        UploadFileById(folderParentId, upLoadFileName, localFilePath)
    End Sub

    ''' <summary>
    ''' Boxにファイルアップロードする
    ''' </summary>
    ''' <param name="folderParentId">配置先のフォルダID</param>
    ''' <param name="upLoadFileName"></param>
    ''' <param name="localFilePath"></param>
    Public Sub UploadFileById(ByVal folderParentId As String, ByVal upLoadFileName As String, ByVal localFilePath As String)
        ' バリデーションチェック

        ' 一括アップロードか分割アップロードか分岐
        If isSplitUploadFileSize(localFilePath, mAppConfig.UploadFileSizeThreshold) Then
            SplitedUploadFile(folderParentId, upLoadFileName, localFilePath)
        Else
            UploadFile(folderParentId, upLoadFileName, localFilePath)
        End If
    End Sub

    ''' <summary>対象フォルダ内でファイル・フォルダを検索し、IDを返す</summary>
    ''' <param name="folderPath">検索するフォルダパス</param>
    ''' <returns>フォルダID、対象フォルダが存在しない場合は空文字を返す</returns>
    Public Async Function GetFolderIdByPath(ByVal folderPath As String) As Task(Of String)
        Dim client As BoxClient = session.AdminClient(mToken)
        Dim targetId As String = ""

        ' フォルダーパスからフォルダ名を取り出す
        Dim searchFolderNames As String() = folderPath.Split(mAppConfig.ForderSeparateString)

        ' アプリケーションルートからフォルダを検索
        Dim rootFolderItems = Await client.FoldersManager.GetFolderItemsAsync(mAppConfig.AppRootFolderId, 1000, autoPaginate:=True)

        Dim boxitems = rootFolderItems
        Dim nextId As String = ""

        For Each tagetFolderName In searchFolderNames
            nextId = ""
            For Each boxItem As BoxItem In boxitems.Entries
                If boxItem.Type = BOX_ITEM_TYPE_FOLDER And boxItem.Name = tagetFolderName Then
                    nextId = boxItem.Id
                    Exit For
                End If
            Next

            ' 次のIDが空の場合は中断、対象なしで返す
            If nextId = "" Then
                Exit For
            End If
            ' 次のフォルダ情報を取得する。　最終フォルダの場合は無駄
            Dim nextolderItems = Await client.FoldersManager.GetFolderItemsAsync(nextId, 1000, autoPaginate:=True)
            boxitems = nextolderItems
        Next
        targetId = nextId


        Return targetId

    End Function


    Private Function SearchIDFolderName(ByVal boxitems As BoxCollection(Of BoxItem), ByVal folderName As String) As String

    End Function

#End Region

#Region "Box通信機能"
    ''' <summary>
    ''' トークンを取得する。
    ''' </summary>
    ''' <returns></returns>
    Public Async Function Authentication() As Task(Of String)

        'valid For 60 minutes so should be cached And re-used
        Dim adminToken = Await session.AdminTokenAsync()

        Return adminToken.ToString
    End Function
#Region "グループ・ユーザー・権限設定"
    ''' <summary>
    ''' ユーザー情報を取得する
    ''' </summary>
    ''' <returns></returns>
    Public Async Function GetUseInfo() As Task(Of BoxUser)
        Dim client As BoxClient = session.AdminClient(mToken)
        Return Await client.UsersManager.GetCurrentUserInformationAsync()
    End Function

    ' 指定したフォルダのコラボレーション情報を取得する
    Public Function GetCollaboration()
        Return ""
    End Function
    ' 指定したフォルダのコラボレーションを追加・作成する
    ' 指定したフォルダのコラボレーションを変更する
    ' 指定したフォルダのコラボレーションを削除する


#End Region
#Region "ファイル・フォルダ操作"

    ''' <summary>
    ''' boxフォルダ情報を取得する
    ''' </summary>
    ''' <param name="folderId">フォルダを表す一意の識別子</param>
    ''' <returns>boxフォルダ情報オブジェクト</returns>
    Public Async Function GetFolderInfo(ByVal folderId As String) As Task(Of BoxFolder)
        Dim client As BoxClient = session.AdminClient(mToken)
        Return Await client.FoldersManager.GetInformationAsync(folderId)

    End Function

    ''' <summary>
    ''' 指定のフォルダをコピーする
    ''' </summary>
    ''' <param name="sourceId">コピー元のフォルダID</param>
    ''' <param name="targetParentId">コピー先の親フォルダ</param>
    ''' <remarks>boxアプリケーションスコープをファイル・フォルダへの書き込み権限が必要になります。</remarks>
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

    ''' <summary>
    ''' フォルダを新規作成する
    ''' </summary>
    ''' <param name="folderParentId"></param>
    ''' <param name="newFolderName"></param>
    Public Async Sub CreateFolder(ByVal folderParentId As String, ByVal newFolderName As String)
        Dim client As BoxClient = session.AdminClient(mToken)
        ' リクエストパラメタを記載
        Dim requestParams = New BoxFolderRequest()
        With requestParams
            .Name = newFolderName
            .Parent = New BoxRequestEntity()
            With .Parent
                .Id = folderParentId
            End With
        End With
        ' フォルダをコピーして、コピー後のフォルダ情報を取得
        Dim FolderCopy As BoxFolder = Await client.FoldersManager.CreateAsync(requestParams)
    End Sub

    Public Async Sub UploadFile(ByVal folderParentId As String, ByVal upLoadFileName As String, ByVal localFilePath As String)
        Dim client As BoxClient = session.AdminClient(mToken)
        Using fileStream As FileStream = New FileStream(localFilePath, FileMode.Open)
            Dim requestParams As BoxFileRequest = New BoxFileRequest()
            With requestParams
                .Name = upLoadFileName
                .Parent = New BoxRequestEntity()
                With .Parent
                    .Id = folderParentId
                End With
            End With

            Dim boxFile As BoxFile = Await client.FilesManager.UploadAsync(requestParams, fileStream, timeout:=mAppConfig.timeout)
        End Using
    End Sub

    Public Async Sub SplitedUploadFile(ByVal folderParentId As String, ByVal upLoadFileName As String, ByVal localFilePath As String)
        Dim client As BoxClient = session.AdminClient(mToken)

        Using fileStream As FileStream = New FileStream(localFilePath, FileMode.Open)
            Dim bFile = Await client.FilesManager.UploadUsingSessionAsync(fileStream, upLoadFileName, folderParentId, timeout:=mAppConfig.timeout)
        End Using

    End Sub

    ' 指定のフォルダ名を変更する

    ' 指定のファイル名を変更する

#End Region

#End Region

#Region "小機能"
    Private Function isSplitUploadFileSize(ByVal filePath As String, ByVal thresholdByteSize As Long) As Boolean
        ' FileInfo の新しいインスタンスを生成する
        Dim cFileInfo As New System.IO.FileInfo(filePath)
        Return cFileInfo.Length > thresholdByteSize
    End Function
#End Region

End Class

