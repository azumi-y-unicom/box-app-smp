<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class Form1
    Inherits System.Windows.Forms.Form

    'フォームがコンポーネントの一覧をクリーンアップするために dispose をオーバーライドします。
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Windows フォーム デザイナーで必要です。
    Private components As System.ComponentModel.IContainer

    'メモ: 以下のプロシージャは Windows フォーム デザイナーで必要です。
    'Windows フォーム デザイナーを使用して変更できます。  
    'コード エディターを使って変更しないでください。
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Me.btnJWTAuth = New System.Windows.Forms.Button()
        Me.TbMsg = New System.Windows.Forms.TextBox()
        Me.BtnGetFolderId = New System.Windows.Forms.Button()
        Me.TbRootFolderId = New System.Windows.Forms.TextBox()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.TbTargetFolderName = New System.Windows.Forms.TextBox()
        Me.LbAuthState = New System.Windows.Forms.Label()
        Me.Label3 = New System.Windows.Forms.Label()
        Me.Button1 = New System.Windows.Forms.Button()
        Me.SuspendLayout()
        '
        'btnJWTAuth
        '
        Me.btnJWTAuth.Location = New System.Drawing.Point(12, 28)
        Me.btnJWTAuth.Name = "btnJWTAuth"
        Me.btnJWTAuth.Size = New System.Drawing.Size(82, 23)
        Me.btnJWTAuth.TabIndex = 0
        Me.btnJWTAuth.Text = "JWT認証"
        Me.btnJWTAuth.UseVisualStyleBackColor = True
        '
        'TbMsg
        '
        Me.TbMsg.Location = New System.Drawing.Point(12, 177)
        Me.TbMsg.Multiline = True
        Me.TbMsg.Name = "TbMsg"
        Me.TbMsg.Size = New System.Drawing.Size(587, 173)
        Me.TbMsg.TabIndex = 1
        '
        'BtnGetFolderId
        '
        Me.BtnGetFolderId.Location = New System.Drawing.Point(506, 96)
        Me.BtnGetFolderId.Name = "BtnGetFolderId"
        Me.BtnGetFolderId.Size = New System.Drawing.Size(93, 23)
        Me.BtnGetFolderId.TabIndex = 2
        Me.BtnGetFolderId.Text = "フォルダID取得"
        Me.BtnGetFolderId.UseVisualStyleBackColor = True
        '
        'TbRootFolderId
        '
        Me.TbRootFolderId.Location = New System.Drawing.Point(117, 100)
        Me.TbRootFolderId.Name = "TbRootFolderId"
        Me.TbRootFolderId.Size = New System.Drawing.Size(100, 19)
        Me.TbRootFolderId.TabIndex = 3
        Me.TbRootFolderId.Text = "0"
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Location = New System.Drawing.Point(12, 103)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(85, 12)
        Me.Label1.TabIndex = 4
        Me.Label1.Text = "対象のフォルダID"
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.Location = New System.Drawing.Point(223, 103)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(76, 12)
        Me.Label2.TabIndex = 5
        Me.Label2.Text = "対象フォルダ名"
        '
        'TbTargetFolderName
        '
        Me.TbTargetFolderName.Location = New System.Drawing.Point(305, 100)
        Me.TbTargetFolderName.Name = "TbTargetFolderName"
        Me.TbTargetFolderName.Size = New System.Drawing.Size(157, 19)
        Me.TbTargetFolderName.TabIndex = 6
        Me.TbTargetFolderName.Text = "0"
        '
        'LbAuthState
        '
        Me.LbAuthState.AutoSize = True
        Me.LbAuthState.Location = New System.Drawing.Point(127, 38)
        Me.LbAuthState.Name = "LbAuthState"
        Me.LbAuthState.Size = New System.Drawing.Size(27, 12)
        Me.LbAuthState.TabIndex = 7
        Me.LbAuthState.Text = "OFF"
        '
        'Label3
        '
        Me.Label3.AutoSize = True
        Me.Label3.Location = New System.Drawing.Point(12, 75)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(108, 12)
        Me.Label3.TabIndex = 8
        Me.Label3.Text = "アプリ初期位置に戻る"
        '
        'Button1
        '
        Me.Button1.Location = New System.Drawing.Point(126, 70)
        Me.Button1.Name = "Button1"
        Me.Button1.Size = New System.Drawing.Size(54, 23)
        Me.Button1.TabIndex = 9
        Me.Button1.Text = "戻る"
        Me.Button1.UseVisualStyleBackColor = True
        '
        'Form1
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 12.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(611, 362)
        Me.Controls.Add(Me.Button1)
        Me.Controls.Add(Me.Label3)
        Me.Controls.Add(Me.LbAuthState)
        Me.Controls.Add(Me.TbTargetFolderName)
        Me.Controls.Add(Me.Label2)
        Me.Controls.Add(Me.Label1)
        Me.Controls.Add(Me.TbRootFolderId)
        Me.Controls.Add(Me.BtnGetFolderId)
        Me.Controls.Add(Me.TbMsg)
        Me.Controls.Add(Me.btnJWTAuth)
        Me.Name = "Form1"
        Me.Text = "Form1"
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    Friend WithEvents btnJWTAuth As Button
    Friend WithEvents TbMsg As TextBox
    Friend WithEvents BtnGetFolderId As Button
    Friend WithEvents TbRootFolderId As TextBox
    Friend WithEvents Label1 As Label
    Friend WithEvents Label2 As Label
    Friend WithEvents TbTargetFolderName As TextBox
    Friend WithEvents LbAuthState As Label
    Friend WithEvents Label3 As Label
    Friend WithEvents Button1 As Button
End Class
