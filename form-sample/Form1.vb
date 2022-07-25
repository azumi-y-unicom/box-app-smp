
Public Class Form1
    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load

    End Sub

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        Dim conf = New BoxAppConfig()
        conf.ConfigJson = "./json/config.json"
        Dim hoge = New BoxUtil
        hoge.mAppConfig = conf
        hoge.Authentication()

    End Sub
End Class
