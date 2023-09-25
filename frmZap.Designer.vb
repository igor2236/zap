<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class frmZap
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()>
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()>
    Private Sub InitializeComponent()
        WebView21 = New Microsoft.Web.WebView2.WinForms.WebView2()
        btnAtualizar = New Button()
        lstContatos = New ListBox()
        txtMsg = New RichTextBox()
        btnEnviar = New Button()
        btnAudio = New Button()
        btnSplit = New Button()
        btnNovoContato = New Button()
        btnEditarContato = New Button()
        CType(WebView21, ComponentModel.ISupportInitialize).BeginInit()
        SuspendLayout()
        ' 
        ' WebView21
        ' 
        WebView21.AllowExternalDrop = True
        WebView21.CreationProperties = Nothing
        WebView21.DefaultBackgroundColor = Color.White
        WebView21.Location = New Point(227, 12)
        WebView21.Name = "WebView21"
        WebView21.Size = New Size(745, 623)
        WebView21.TabIndex = 0
        WebView21.ZoomFactor = 1R
        ' 
        ' btnAtualizar
        ' 
        btnAtualizar.Location = New Point(12, 12)
        btnAtualizar.Name = "btnAtualizar"
        btnAtualizar.Size = New Size(192, 29)
        btnAtualizar.TabIndex = 1
        btnAtualizar.Text = "Atualizar"
        btnAtualizar.UseVisualStyleBackColor = True
        ' 
        ' lstContatos
        ' 
        lstContatos.FormattingEnabled = True
        lstContatos.ItemHeight = 20
        lstContatos.Location = New Point(12, 87)
        lstContatos.Name = "lstContatos"
        lstContatos.Size = New Size(192, 644)
        lstContatos.TabIndex = 2
        ' 
        ' txtMsg
        ' 
        txtMsg.Location = New Point(227, 641)
        txtMsg.Name = "txtMsg"
        txtMsg.Size = New Size(549, 90)
        txtMsg.TabIndex = 3
        txtMsg.Text = ""
        ' 
        ' btnEnviar
        ' 
        btnEnviar.Location = New Point(877, 641)
        btnEnviar.Name = "btnEnviar"
        btnEnviar.Size = New Size(95, 90)
        btnEnviar.TabIndex = 4
        btnEnviar.Text = "Enviar"
        btnEnviar.UseVisualStyleBackColor = True
        ' 
        ' btnAudio
        ' 
        btnAudio.Location = New Point(782, 641)
        btnAudio.Name = "btnAudio"
        btnAudio.Size = New Size(89, 41)
        btnAudio.TabIndex = 5
        btnAudio.Text = "Audio"
        btnAudio.UseVisualStyleBackColor = True
        ' 
        ' btnSplit
        ' 
        btnSplit.Location = New Point(782, 690)
        btnSplit.Name = "btnSplit"
        btnSplit.Size = New Size(89, 41)
        btnSplit.TabIndex = 6
        btnSplit.Text = "Split"
        btnSplit.UseVisualStyleBackColor = True
        ' 
        ' btnNovoContato
        ' 
        btnNovoContato.Location = New Point(12, 47)
        btnNovoContato.Name = "btnNovoContato"
        btnNovoContato.Size = New Size(94, 29)
        btnNovoContato.TabIndex = 7
        btnNovoContato.Text = "Novo"
        btnNovoContato.UseVisualStyleBackColor = True
        ' 
        ' btnEditarContato
        ' 
        btnEditarContato.Location = New Point(112, 47)
        btnEditarContato.Name = "btnEditarContato"
        btnEditarContato.Size = New Size(92, 29)
        btnEditarContato.TabIndex = 8
        btnEditarContato.Text = "Editar"
        btnEditarContato.UseVisualStyleBackColor = True
        ' 
        ' frmZap
        ' 
        AutoScaleDimensions = New SizeF(8F, 20F)
        AutoScaleMode = AutoScaleMode.Font
        ClientSize = New Size(995, 751)
        Controls.Add(btnEditarContato)
        Controls.Add(btnNovoContato)
        Controls.Add(btnSplit)
        Controls.Add(btnAudio)
        Controls.Add(btnEnviar)
        Controls.Add(txtMsg)
        Controls.Add(lstContatos)
        Controls.Add(btnAtualizar)
        Controls.Add(WebView21)
        Name = "frmZap"
        Text = "Zap"
        CType(WebView21, ComponentModel.ISupportInitialize).EndInit()
        ResumeLayout(False)
    End Sub

    Friend WithEvents WebView21 As Microsoft.Web.WebView2.WinForms.WebView2
    Friend WithEvents btnAtualizar As Button
    Friend WithEvents lstContatos As ListBox
    Friend WithEvents txtMsg As RichTextBox
    Friend WithEvents btnNovoContato As Button
    Friend WithEvents btnAudio As Button
    Friend WithEvents btnSplit As Button
    Friend WithEvents btnEnviar As Button
    Friend WithEvents btnEditarContato As Button
End Class
