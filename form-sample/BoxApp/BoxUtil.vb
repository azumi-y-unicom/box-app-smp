
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

    Public Const BOX_ERROR_MSG_DIRECTORY_NOT_FOUND As String = "[%s]はBOXに存在しません。ルートフォルダの設定および、BOXのフォルダを確認してください。"
    Public Const BOX_ERROR_MSG_FILE_NOT_FOUND As String = "[%s]はBOXに存在しません。BOXのフォルダを確認してください。[フォルダID＝%s]"

#End Region


    Sub New(conf As BoxAppConfig)
        mAppConfig = conf
        Dim reader = New StreamReader(mAppConfig.AuthJsonFilePath)
        Dim json = reader.ReadToEnd()

        boxConf = BoxConfig.CreateFromJsonString(json)
        session = New BoxJWTAuth(boxConf)

    End Sub

#Region "ロジック部"

#Region "ファイル・フォルダ操作"

#Region "取得"

    ''' <summary>対象フォルダ内でフォルダを検索し、IDを取得する</summary>
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

        ' ToDo ロジック見直し
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


    ''' <summary>
    ''' 指定フォルダID直下のファイル・フォルダのIDを取得する。
    ''' </summary>
    ''' <param name="folderId">検索対象フォルダID</param>
    ''' <param name="targetName">検索ファイル・フォルダ名称</param>
    ''' <returns>ファイル・フォルダのID　見つからない場合は空文字を返す</returns>
    Public Async Function FindItemId(ByVal folderId As String, ByVal targetName As String) As Task(Of String)
        Dim targetId As String = ""
        Dim client As BoxClient = session.AdminClient(mToken)
        Dim boxitems = Await client.FoldersManager.GetFolderItemsAsync(folderId, 100)
        For Each boxItem As BoxItem In boxitems.Entries
            If boxItem.Name = targetName Then
                targetId = boxItem.Id
                Exit For
            End If
        Next
        Return targetId
    End Function

#End Region

#Region "追加・更新"
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
    Public Async Sub UploadFileByTreePath(ByVal folderParentPath As String, ByVal upLoadFileName As String, ByVal localFilePath As String)
        Dim folderParentId As String

        folderParentId = Await GetFolderIdByPath(folderParentPath)

        ' フォルダパス存在しない場合はエラーとする。
        If folderParentId = "" Then
            Throw New DirectoryNotFoundException(Format(BOX_ERROR_MSG_DIRECTORY_NOT_FOUND, folderParentPath))
        End If

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
            ' ファイル分割アップロード
            SplitedUploadFile(folderParentId, upLoadFileName, localFilePath)
        Else
            ' ファイル一括アップロード
            UploadFile(folderParentId, upLoadFileName, localFilePath)
        End If
    End Sub

    ''' <summary>
    ''' フォルダパス形式で指定したフォルダのフォルダ名を変更する
    ''' </summary>
    ''' <param name="targetFolderPath"></param>
    ''' <param name="newName"></param>
    Public Async Sub RenameFolderByName(ByVal targetFolderPath As String, ByVal newName As String)
        Dim targetFoldertId As String
        targetFoldertId = Await GetFolderIdByPath(targetFolderPath)
        ' フォルダパス存在しない場合はエラーとする。
        If targetFoldertId = "" Then
            Throw New DirectoryNotFoundException(Format(BOX_ERROR_MSG_DIRECTORY_NOT_FOUND, targetFolderPath))
        End If
        Dim renamedFolder As BoxFolder = Await RenameFolder(targetFoldertId, newName)

    End Sub


#End Region
#End Region


#Region "コラボレーション操作"
    ' 指定フォルダへの指定したアクセス権をユーザに追加
    Public Async Sub HOGE(ByVal targetFolderPath)

    End Sub
#End Region
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
    Public Async Function GetUseCurrentInfo() As Task(Of BoxUser)
        Dim client As BoxClient = session.AdminClient(mToken)
        Return Await client.UsersManager.GetCurrentUserInformationAsync()
    End Function

    ''' <summary>
    ''' ユーザー情報を取得する
    ''' </summary>
    ''' <returns></returns>
    Public Async Function GetUseInfo(ByVal userId As String) As Task(Of BoxUser)
        Dim client As BoxClient = session.AdminClient(mToken)
        Return Await client.UsersManager.GetCurrentUserInformationAsync()
    End Function

    Public Async Function GetUses() As Task(Of BoxCollection(Of BoxUser))
        Dim client As BoxClient = session.AdminClient(mToken)
        Return Await client.UsersManager.GetEnterpriseUsersAsync()
    End Function

    ' 指定したフォルダのコラボレーション情報を取得する
    Public Async Function GetCollaborations(ByVal folderId As String) As Task(Of BoxCollection)

        Dim client As BoxClient = session.AdminClient(mToken)
        Dim collaborations As BoxCollection(Of BoxCollaboration) = Await client.FoldersManager.GetCollaborationsAsync(folderId)

        Return collaborations
    End Function


    ''' <summary>
    ''' 指定したフォルダのコラボレーションを追加・作成する
    ''' </summary>
    ''' <param name="targetFolderId">登録するフォルダID</param>
    ''' <param name="targetUserId">登録するユーザーID</param>
    ''' <param name="newRole">設定するロール、Sees参照</param>
    ''' <see cref="BoxCollaborationRoles"/>
    ''' <returns></returns>
    Public Async Function AddCollaboration(ByVal targetFolderId As String, ByVal targetUserId As String, ByVal newRole As String) As Task
        Dim client As BoxClient = session.AdminClient(mToken)

        Dim requestParams = New BoxCollaborationRequest()
        With requestParams
            .Item = New BoxRequestEntity()
            With .Item
                .Type = BoxType.folder
                .Id = targetFolderId
            End With
            .Role = newRole
            .AccessibleBy = New BoxCollaborationUserRequest()
            With .AccessibleBy
                .Type = BoxType.user
                .Id = targetUserId
            End With


        End With
        Dim collab = Await client.CollaborationsManager.AddCollaborationAsync(requestParams)
    End Function


    ''' <summary>
    ''' 指定したコラボレーション情報を編集する
    ''' </summary>
    ''' <param name="targetCollaborateId"></param>
    ''' <param name="newRole">See参照</param>
    ''' <returns></returns>
    ''' <see cref="BoxCollaborationRoles"/>
    Public Async Function EditCollavorationInfo(ByVal targetCollaborateId As String, ByVal newRole As String) As Task
        Dim client As BoxClient = session.AdminClient(mToken)

        Dim requestParams As BoxCollaborationRequest = New BoxCollaborationRequest()
        With requestParams
            .Id = targetCollaborateId
            .Role = newRole
        End With
        Dim collab As BoxCollaboration = Await client.CollaborationsManager.EditCollaborationAsync(requestParams)
    End Function

    ''' <summary>
    ''' 指定したコラボレーション情報を削除する
    ''' </summary>
    ''' <param name="targetCollaborateId"></param>
    Public Async Sub RemoveCollavorationInfo(ByVal targetCollaborateId As String)
        Dim client As BoxClient = session.AdminClient(mToken)
        Await client.CollaborationsManager.RemoveCollaborationAsync(targetCollaborateId)
    End Sub

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
    Public Async Function CopyFolder(ByVal sourceId As String, ByVal targetParentId As String, ByVal newName As String) As Task
        Dim client As BoxClient = session.AdminClient(mToken)

        ' リクエストパラメタを記載
        Dim requestParams = New BoxFolderRequest()
        If (Not String.IsNullOrWhiteSpace(newName)) Then
            With requestParams
                .Id = sourceId
                .Name = newName
                .Parent = New BoxRequestEntity()
                With .Parent
                    .Id = targetParentId
                End With
            End With
        Else
            With requestParams
                .Id = sourceId
                .Parent = New BoxRequestEntity()
                With .Parent
                    .Id = targetParentId
                End With
            End With

        End If
        ' フォルダをコピーして、コピー後のフォルダ情報を取得
        Dim FolderCopy As BoxFolder = Await client.FoldersManager.CopyAsync(requestParams)
    End Function

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

    ''' <summary>
    ''' 指定のフォルダ名を変更する
    ''' </summary>
    ''' <param name="targetId"></param>
    ''' <param name="newName"></param>
    Public Async Function RenameFolder(ByVal targetId As String, ByVal newName As String) As Task(Of BoxFolder)
        Dim client As BoxClient = session.AdminClient(mToken)
        Dim requestParams = New BoxFolderRequest()
        With requestParams
            .Id = targetId
            .Name = newName
        End With
        Dim updatedFile As BoxFolder = Await client.FoldersManager.UpdateInformationAsync(requestParams)
        Return updatedFile
    End Function

    ''' <summary>
    ''' 指定のファイル名を変更する
    ''' </summary>
    ''' <param name="targetId"></param>
    ''' <param name="newName"></param>
    Public Async Function RenameFile(ByVal targetId As String, ByVal newName As String) As Task(Of BoxFile)
        Dim client As BoxClient = session.AdminClient(mToken)
        Dim requestParams = New BoxFileRequest()
        With requestParams
            .Id = targetId
            .Name = newName
        End With
        Dim updatedFile As BoxFile = Await client.FilesManager.UpdateInformationAsync(requestParams)

    End Function

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

