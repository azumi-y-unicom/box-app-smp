

Imports System.Configuration
Imports Box.V2.Models

Public Class Form1
    Public Const MSG_AUTH_ON As String = "ON"
    Public Const MSG_AUTH_OFF As String = "OFF"

    Private boxUtil As BoxUtil
    Private mBoxConf = New BoxAppConfig()
    Private appRootId As String


    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        ' サンプル：格納先設定
        mBoxConf.ConfigJson = "./json/config.json"
        appRootId = ConfigurationManager.AppSettings("box-root-folder-id")

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
        TbRootFolderId.Text = ""
    End Sub

    Private Sub TbRootFolderId_TextChanged(sender As Object, e As EventArgs) Handles TbRootFolderId.TextChanged

    End Sub
#End Region

End Class
