
Imports System.IO
Imports Box.V2
Imports Box.V2.Auth
Imports Box.V2.JWTAuth
Imports Box.V2.Config
Imports Box.V2.Models


Public Class BoxUtil
    Public Property mAppConfig As BoxAppConfig
    Public Async Sub Authentication()
        Dim reader = New StreamReader(mAppConfig.ConfigJson)
        Dim json = reader.ReadToEnd()
        Dim config = BoxConfig.CreateFromJsonString(json)
        Dim session = New BoxJWTAuth(config)

        'valid For 60 minutes so should be cached And re-used
        Dim adminToken = Await session.AdminTokenAsync()
        Dim adminClient As BoxClient = session.AdminClient(adminToken)

        Dim user As BoxUser = Await adminClient.UsersManager.GetCurrentUserInformationAsync()
        Console.WriteLine("現在のユーザーID = {0}", user.Id)

    End Sub
End Class

