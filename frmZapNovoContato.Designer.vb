<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmZapNovoContato
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
        txtNome = New TextBox()
        lblNome = New Label()
        lblTelefone = New Label()
        btnSalvar = New Button()
        txtTelefone = New MaskedTextBox()
        SuspendLayout()
        ' 
        ' txtNome
        ' 
        txtNome.Location = New Point(12, 106)
        txtNome.Name = "txtNome"
        txtNome.Size = New Size(206, 27)
        txtNome.TabIndex = 1
        ' 
        ' lblNome
        ' 
        lblNome.AutoSize = True
        lblNome.Location = New Point(12, 77)
        lblNome.Name = "lblNome"
        lblNome.Size = New Size(50, 20)
        lblNome.TabIndex = 2
        lblNome.Text = "Nome"
        ' 
        ' lblTelefone
        ' 
        lblTelefone.AutoSize = True
        lblTelefone.Location = New Point(12, 9)
        lblTelefone.Name = "lblTelefone"
        lblTelefone.Size = New Size(66, 20)
        lblTelefone.TabIndex = 3
        lblTelefone.Text = "Telefone"
        ' 
        ' btnSalvar
        ' 
        btnSalvar.Location = New Point(124, 158)
        btnSalvar.Name = "btnSalvar"
        btnSalvar.Size = New Size(94, 29)
        btnSalvar.TabIndex = 4
        btnSalvar.Text = "Salvar"
        btnSalvar.UseVisualStyleBackColor = True
        ' 
        ' txtTelefone
        ' 
        txtTelefone.Location = New Point(12, 32)
        txtTelefone.Name = "txtTelefone"
        txtTelefone.Size = New Size(206, 27)
        txtTelefone.TabIndex = 5
        ' 
        ' frmZapNovoContato
        ' 
        AutoScaleDimensions = New SizeF(8.0F, 20.0F)
        AutoScaleMode = AutoScaleMode.Font
        ClientSize = New Size(230, 197)
        Controls.Add(txtTelefone)
        Controls.Add(btnSalvar)
        Controls.Add(lblTelefone)
        Controls.Add(lblNome)
        Controls.Add(txtNome)
        Name = "frmZapNovoContato"
        Text = "frmZapNovoContato"
        ResumeLayout(False)
        PerformLayout()
    End Sub
    Friend WithEvents txtNome As TextBox
    Friend WithEvents lblNome As Label
    Friend WithEvents lblTelefone As Label
    Friend WithEvents btnSalvar As Button
    Friend WithEvents txtTelefone As MaskedTextBox
End Class
