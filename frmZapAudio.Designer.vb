<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmZapAudio
    Inherits System.Windows.Forms.Form

    'Descartar substituições de formulário para limpar a lista de componentes.
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

    'Exigido pelo Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'OBSERVAÇÃO: o procedimento a seguir é exigido pelo Windows Form Designer
    'Pode ser modificado usando o Windows Form Designer.  
    'Não o modifique usando o editor de códigos.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        btnSalvar = New Button()
        btnGravar = New Button()
        btnEnviar = New Button()
        btnPlay = New Button()
        PictureBox1 = New PictureBox()
        CType(PictureBox1, ComponentModel.ISupportInitialize).BeginInit()
        SuspendLayout()
        ' 
        ' btnSalvar
        ' 
        btnSalvar.Location = New Point(188, 12)
        btnSalvar.Name = "btnSalvar"
        btnSalvar.Size = New Size(94, 29)
        btnSalvar.TabIndex = 0
        btnSalvar.Text = "Salvar"
        btnSalvar.UseVisualStyleBackColor = True
        ' 
        ' btnGravar
        ' 
        btnGravar.Location = New Point(88, 12)
        btnGravar.Name = "btnGravar"
        btnGravar.Size = New Size(94, 29)
        btnGravar.TabIndex = 1
        btnGravar.Text = "Gravar"
        btnGravar.UseVisualStyleBackColor = True
        ' 
        ' btnEnviar
        ' 
        btnEnviar.Location = New Point(88, 47)
        btnEnviar.Name = "btnEnviar"
        btnEnviar.Size = New Size(94, 29)
        btnEnviar.TabIndex = 2
        btnEnviar.Text = "Enviar"
        btnEnviar.UseVisualStyleBackColor = True
        ' 
        ' btnPlay
        ' 
        btnPlay.Location = New Point(188, 47)
        btnPlay.Name = "btnPlay"
        btnPlay.Size = New Size(94, 29)
        btnPlay.TabIndex = 3
        btnPlay.Text = "Play"
        btnPlay.UseVisualStyleBackColor = True
        ' 
        ' PictureBox1
        ' 
        PictureBox1.Location = New Point(12, 12)
        PictureBox1.Name = "PictureBox1"
        PictureBox1.Size = New Size(59, 64)
        PictureBox1.TabIndex = 4
        PictureBox1.TabStop = False
        ' 
        ' frmZapAudio
        ' 
        AutoScaleDimensions = New SizeF(8F, 20F)
        AutoScaleMode = AutoScaleMode.Font
        ClientSize = New Size(295, 88)
        Controls.Add(PictureBox1)
        Controls.Add(btnPlay)
        Controls.Add(btnEnviar)
        Controls.Add(btnGravar)
        Controls.Add(btnSalvar)
        Name = "frmZapAudio"
        Text = "frmZapAudio"
        CType(PictureBox1, ComponentModel.ISupportInitialize).EndInit()
        ResumeLayout(False)
    End Sub

    Friend WithEvents btnSalvar As Button
    Friend WithEvents btnGravar As Button
    Friend WithEvents btnEnviar As Button
    Friend WithEvents btnPlay As Button
    Friend WithEvents PictureBox1 As PictureBox
End Class
