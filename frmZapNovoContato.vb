Imports System.Text
Imports System.Text.RegularExpressions

Public Class frmZapNovoContato
    Friend editar As Boolean
    Friend telefone As String
    Friend nome As String
    Friend contatoId As Integer
    Friend cpf As String
    Friend meuPai As frmZap
    Private Sub frmZapNovoContato_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        AddHandler txtTelefone.Validated, AddressOf txtTelefone_Validated


        If editar Then
            txtTelefone.Text = telefone
            txtNome.Text = nome
        End If



        If editar Then
            validaTelefone(txtTelefone)
        End If
    End Sub

    Private Sub validaTudo()
        If Not validaNome() Then
            Exit Sub
        End If

        If validaTelefone() Then
            If editar Then
                updateZapContato()
                MessageBox.Show("Contato Editado com sucesso!")
                txtNome.Clear()
                txtTelefone.Clear()
            Else
                insertZapContato()
                MessageBox.Show("Contato adicionado com sucesso!")
                txtNome.Clear()
                txtTelefone.Clear()
            End If

        Else
            Exit Sub
        End If
        meuPai.lstContatos.Items.Clear()
        meuPai.preencheListaContatos()
        Me.Close()
    End Sub

    Private Sub btnConfirmar_Click(sender As Object, e As EventArgs) Handles btnSalvar.Click
        validaTudo()
    End Sub
    Private Sub updateZapContato()
        Dim strsql As StringBuilder = New StringBuilder

        strsql.Append("update ")
        strsql.Append("zap.zap_contato ")
        strsql.Append("set ")
        strsql.Append($"numerocli = '{txtTelefone.Text.Replace("(", "").Replace(")", "").Replace("-", "").Replace(" ", "")}',")
        strsql.Append($"nomecli = '{isNULL(txtNome.Text, "")}' ")
        strsql.Append($"where ")
        strsql.Append($"numerocli = '{telefone}'")
        SQL_executeNonQuery(strsql.ToString)


        If meuPai.telefoneCLI <> txtTelefone.Text.Replace("(", "").Replace(")", "").Replace("-", "").Replace(" ", "") Then
            meuPai.telefoneCLI = txtTelefone.Text.Replace("(", "").Replace(")", "").Replace("-", "").Replace(" ", "")
            meuPai.reload()
        End If
    End Sub
    Private Sub insertZapContato()
        Dim strsql As StringBuilder = New StringBuilder

        strsql.Append("insert ")
        strsql.Append("into ")
        strsql.Append("zap.zap_contato ")
        strsql.Append("(numerocli,")
        strsql.Append("nomecli) ")
        strsql.Append("values(")
        strsql.Append($"'{txtTelefone.Text.Replace("(", "").Replace(")", "").Replace("-", "").Replace(" ", "")}',")
        strsql.Append($"'{isNULL(txtNome.Text, "")}')")
        SQL_executeNonQuery(strsql.ToString)
    End Sub

    Private Function validaTelefone()
        Dim temTelefone As Boolean
        If txtTelefone.Text <> "" Then
            temTelefone = retornaSeTemTelefone()
        Else
            MessageBox.Show("Este telefone já pertence a um dos contatos")
            temTelefone = False
        End If

        If temTelefone <> False Then
            If txtTelefone.Text.Replace("(", "").Replace(")", "").Replace("-", "").Replace(" ", "").Length < 10 OrElse txtTelefone.Text.Replace("(", "").Replace(")", "").Replace("-", "").Replace(" ", "").Length > 11 Then
                MessageBox.Show("Telefone incorreto")
                temTelefone = False
            End If
        End If

        Return temTelefone
    End Function

    Private Function retornaSeTemTelefone() As Boolean
        Dim strsql As StringBuilder = New StringBuilder
        Dim temTelefone As Boolean

        strsql.Append("select ")
        strsql.Append("numerocli ")
        strsql.Append("from ")
        strsql.Append("zap.zap_contato ")
        strsql.Append("where ")
        strsql.Append($"numerocli = '{txtTelefone.Text.Replace("(", "").Replace(")", "").Replace("-", "").Replace(" ", "")}'")

        If isNULL(SQL_escalar(strsql.ToString), "") <> "" Then
            temTelefone = True
        Else
            temTelefone = False
        End If

        Return temTelefone
    End Function



    Private Function validaNome() As Boolean
        If txtNome.Text = "" Then
            MessageBox.Show("Nome inválido")
            txtNome.Clear()
            Return False
        End If
        Return True
    End Function

    Private Sub txtTelefone_Enter(sender As Object, e As EventArgs)
        txtTelefone.SelectionStart = 0
        txtTelefone.Mask = ""
    End Sub


    Private Function validaTelefone(ByRef fone As MaskedTextBox) As Boolean
        Dim celular As Boolean = False
        Dim tamanhoErrado As Boolean = False
        Dim valido As Boolean = True
        fone.Mask = ""


        If fone.Text <> "" Then

            If fone.Text.Replace("(", "").Replace(")", "").Replace("-", "").Length = 11 Then
                celular = True
            ElseIf fone.Text.Replace("(", "").Replace(")", "").Replace("-", "").Length = 10 Then
                celular = False
            Else
                tamanhoErrado = True
            End If


            If tamanhoErrado Then
                MessageBox.Show("Telefone errado deve ter 10 ou 11 digitos<br/>com o DDD (2 digitos)<br/>e não inciado com 0 (zero)")
                valido = False
            End If

            If valido AndAlso fone.Text.Substring(0, 1) = "0" Then
                MessageBox.Show("O numero não pode iniciar com ""0"" (zero)")
                valido = False
            End If

            If valido AndAlso celular Then

                If fone.Text.Substring(2, 1) = "9" Then
                    fone.Mask = "(99) 9 9999-9999"
                Else
                    MessageBox.Show("Celulares devem começar com ""9""")
                    valido = False
                End If
            Else
                fone.Mask = "(99) 9999-9999"
            End If

            If Not valido Then
                fone.Mask = ""
            End If
        End If

        Return valido
    End Function
    Private Sub txtTelefone_Validated(sender As Object, e As EventArgs)
        validaTelefone(txtTelefone)
    End Sub

    Private Sub txtTelefone_Validating(sender As Object, e As System.ComponentModel.CancelEventArgs)
        txtTelefone.Mask = ""
    End Sub
End Class