

Imports System.Configuration
Imports Box.V2.Models

Public Class Form1
    Public Const MSG_AUTH_ON As String = "ON"
    Public Const MSG_AUTH_OFF As String = "OFF"

    Private boxUtil As BoxUtil
    Private mBoxConf = New BoxAppConfig()


    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        ' サンプル：格納先設定
        mBoxConf.AuthJsonFilePath = ConfigurationManager.AppSettings("auth-json-file-path")
        mBoxConf.AppRootFolderId = ConfigurationManager.AppSettings("box-root-folder-id")
        mBoxConf.UploadFileSizeThreshold = Long.Parse(ConfigurationManager.AppSettings("upload-file-size-threshold"))
        lb_approotid.Text = mBoxConf.AppRootFolderId

    End Sub

    Private Async Sub btnJWTAuth_Click(sender As Object, e As EventArgs) Handles btnJWTAuth.Click

        Try
            ' JWT認証準備
            boxUtil = New BoxUtil(mBoxConf)

            ' 認証とトークン取得
            Dim token = Await boxUtil.Authentication()

            ' 認証確認
            If String.IsNullOrEmpty(token) Then
                SetAuthState(False)
            Else

                boxUtil.mToken = token
                SetAuthState(True)
            End If
        Catch ex As Exception
            SetAuthState(False)
            TbMsg.Text = ex.ToString
        End Try

    End Sub

    Private Async Sub BtnGetFolderId_Click(sender As Object, e As EventArgs) Handles BtnGetFolderId.Click
        TbMsg.Text = ""
        Dim fid As String
        Try

            fid = Await boxUtil.GetFolderIdByPath("exp1/exp2")

            If fid = "" Then
                TbMsg.Text = "対象のフォルダはありません。"
            End If
            TbMsg.Text = fid
        Catch ex As Exception

            TbMsg.Text = ex.ToString


        End Try
    End Sub

    Private Async Sub hogeAsync()
        TbMsg.Text = ""
        Dim fid As String
        fid = TbRootFolderId.Text
        '' 未認証の時は終了
        If Not IsAuthenticated() Then
            Exit Sub
        End If

        Try

            Dim fInfo As BoxFolder = Await boxUtil.GetFolderInfo(fid)

            ' ID=0(ルート)の場合はParentはNothing
            If fInfo.Parent IsNot Nothing Then
                TbMsg.Text += fInfo.Parent.Id & " : " & fInfo.Parent.Name & vbCrLf
                TbMsg.Text += "------" & vbCrLf
            End If


            For Each item In fInfo.ItemCollection.Entries
                TbMsg.Text += item.Id & " : " & item.Type & " : " & item.Name & vbCrLf

            Next
        Catch ex As Exception
            TbMsg.Text = ex.ToString & vbCrLf
        End Try
    End Sub

#Region "GUI操作"

    ''' <summary>
    ''' 認証チェック
    ''' </summary>
    ''' <returns></returns>
    Private Function IsAuthenticated() As Boolean
        If String.Equals(LbAuthState.Text, MSG_AUTH_ON) Then
            Return True
        Else
            MessageBox.Show("認証ボタンを押してください。")
            Return False
        End If
    End Function

    ''' <summary>
    ''' 認証状況設定
    ''' </summary>
    ''' <param name="isAuthOn"></param>
    Private Sub SetAuthState(ByVal isAuthOn)
        If isAuthOn Then
            LbAuthState.Text = MSG_AUTH_ON
        Else
            LbAuthState.Text = MSG_AUTH_OFF
        End If
    End Sub

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        TbRootFolderId.Text = mBoxConf.AppRootFolderId
    End Sub


    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        TbMsg.Text = ""
        Dim fid As String
        fid = TbRootFolderId.Text
        '' 未認証の時は終了
        If Not IsAuthenticated() Then
            Exit Sub
        End If

        Try

            ' boxUtil.CopyFolder("", "")
        Catch ex As Exception
            TbMsg.Text = ex.ToString & vbCrLf
        End Try

    End Sub

    Private Sub Button3_Click(sender As Object, e As EventArgs) Handles Button3.Click
    End Sub
#End Region


End Class
