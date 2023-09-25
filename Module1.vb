Imports System.IO
Imports Npgsql
Module Module1
    Public pCONECTION_ID As String = "Server=localhost;Port=5432;User Id=postgres;Password=1234;Database=zap;"
    Public ID_LoginUser As String = ""



    Public Sub SQL_executeNonQuery(cCommand As String)
        Dim conn As NpgsqlConnection = New NpgsqlConnection(pCONECTION_ID)
        Dim objReturn As Object = Nothing
        Dim Command = New NpgsqlCommand
        Command.CommandText = "set session ""myapp.user"" = '" & ID_LoginUser & "'; "
        Command.CommandText += cCommand
        Dim mytrans As NpgsqlTransaction = Nothing
        Try
            conn.Open()
            mytrans = conn.BeginTransaction
            Command.Transaction = mytrans
            Command.Connection = conn
            objReturn = Command.ExecuteNonQuery()
            mytrans.Commit()
            conn.Close()
        Catch ex As Exception
            If mytrans IsNot Nothing Then mytrans.Rollback()
            If conn.State = ConnectionState.Open Then conn.Close()
            MessageBox.Show("Holve um erro ao atualizar a base de dados")
            objReturn = Nothing
        End Try
    End Sub
    Public Function SQL_escalar(cCommand As String) As Object
        Dim conn As NpgsqlConnection = New NpgsqlConnection(pCONECTION_ID)
        Dim objReturn As Object = Nothing
        Dim Command = New NpgsqlCommand
        Command.CommandText = "set session ""myapp.user"" = '" & ID_LoginUser & "'; "
        Command.CommandText += cCommand
        Dim mytrans As NpgsqlTransaction = Nothing
        Try
            conn.Open()
            mytrans = conn.BeginTransaction
            Command.Transaction = mytrans
            Command.Connection = conn
            objReturn = Command.ExecuteScalar()
            mytrans.Commit()
            conn.Close()
        Catch ex As Exception
            If mytrans IsNot Nothing Then mytrans.Rollback()
            If conn.State = ConnectionState.Open Then conn.Close()
            MessageBox.Show(ex.Message)
            objReturn = Nothing
        End Try
        Return objReturn
    End Function
    Public Function zipstring(ByVal text As String)
        Dim buffer As Byte() = System.Text.Encoding.Unicode.GetBytes(text)
        Dim ms As New MemoryStream()
        Using zipStream As New System.IO.Compression.GZipStream(ms, System.IO.Compression.CompressionMode.Compress, True)
            zipStream.Write(buffer, 0, buffer.Length)
        End Using

        ms.Position = 0
        Dim outStream As New MemoryStream()

        Dim compressed As Byte() = New Byte(ms.Length - 1) {}
        ms.Read(compressed, 0, compressed.Length)

        Dim gzBuffer As Byte() = New Byte(compressed.Length + 3) {}
        System.Buffer.BlockCopy(compressed, 0, gzBuffer, 4, compressed.Length)
        System.Buffer.BlockCopy(BitConverter.GetBytes(buffer.Length), 0, gzBuffer, 0, 4)
        Return Convert.ToBase64String(gzBuffer)
    End Function
    Function isNULL(cObject, cDefault)
        If cObject Is Nothing Then
            Return cDefault
        ElseIf String.IsNullOrEmpty(cObject.ToString) Then
            Return cDefault
        ElseIf String.IsNullOrWhiteSpace(cObject.ToString) Then
            Return cDefault
        ElseIf cObject.ToString = "  /  /    " Then
            Return cDefault
        ElseIf cObject.ToString = "  /  /  " Then
            Return cDefault
        Else
            Return cObject
        End If
    End Function

    Public Function SQL_GetDatatable(cSelectCommand As String) As DataTable
        Dim conn As NpgsqlConnection = New NpgsqlConnection(pCONECTION_ID)
        Dim objReturn As DataTable = New DataTable
        Dim Command As String = "set session ""myapp.user"" = '" & ID_LoginUser & "'; "
        Command += cSelectCommand
        Dim da As NpgsqlDataAdapter = New NpgsqlDataAdapter(Command, conn)
        da.Fill(objReturn)
        Return objReturn
    End Function
    Public Function SQL_executeHeader(cCommand As String) As NpgsqlDataReader
        Dim conn As NpgsqlConnection = New NpgsqlConnection(pCONECTION_ID)
        Dim objReturn As NpgsqlDataReader = Nothing
        Dim Command = New NpgsqlCommand
        Command.CommandText = "set session ""myapp.user"" = '" & ID_LoginUser & "'; "
        Command.CommandText += cCommand
        Dim mytrans As NpgsqlTransaction = Nothing
        Try
            conn.Open()
            mytrans = conn.BeginTransaction
            Command.Transaction = mytrans
            Command.Connection = conn
            objReturn = Command.ExecuteReader()
            mytrans.Commit()
            conn.Close()
        Catch ex As Exception
            If mytrans IsNot Nothing Then mytrans.Rollback()
            If conn.State = ConnectionState.Open Then conn.Close()
            MessageBox.Show("Holve um erro ao atualizar a base de dados")
            objReturn = Nothing
        End Try
        Return objReturn
    End Function
    Public Function UnZipString(compressedText As String) As String
        Dim gzBuffer As Byte() = Convert.FromBase64String(compressedText)
        Using ms As New MemoryStream()
            Dim msgLength As Integer = BitConverter.ToInt32(gzBuffer, 0)
            ms.Write(gzBuffer, 4, gzBuffer.Length - 4)

            Dim buffer As Byte() = New Byte(msgLength - 1) {}

            ms.Position = 0
            Using zipStream As New System.IO.Compression.GZipStream(ms, System.IO.Compression.CompressionMode.Decompress)
                zipStream.Read(buffer, 0, buffer.Length)
            End Using

            Return System.Text.Encoding.Unicode.GetString(buffer, 0, buffer.Length)
        End Using
    End Function


End Module
