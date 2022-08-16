
Public Class BoxAppConfig
    ''' <summary>
    ''' JWT認証ファイルパス
    ''' </summary>
    Public Property AuthJsonFilePath As String

    ''' <summary>
    ''' アプリケーションルートフォルダID
    ''' </summary>
    Public Property AppRootFolderId As String

    ''' <summary>
    ''' ファイルアップロード[一括／分割]判定の閾値
    ''' </summary>
    Public Property UploadFileSizeThreshold As Long

    ''' <summary>
    ''' ファイルアップロード等box通信時のタイムアウト
    ''' </summary>
    ''' <returns></returns>
    Public Property Timeout As TimeSpan = Nothing

    ''' <summary>
    ''' フォルダパスの区切り文字
    ''' </summary>
    ''' <returns></returns>
    Public Property ForderSeparateString As String = "/"

End Class
