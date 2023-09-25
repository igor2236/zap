Imports Microsoft.VisualBasic.Devices
Imports Microsoft.VisualBasic
Imports System.Runtime.InteropServices
Imports NAudio
Imports NAudio.Wave
Imports System.IO
Imports System.Text.Encodings.Web
Imports System.Net
Imports System.Text
Imports System.Management.Instrumentation
Imports C1.Win.C1Editor.Internal.HTML


Public Class frmZapAudio
    Private caminho As String = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "temp\zap_audio\")


    Private caminhoSaida As String

    Private waveIn As WaveInEvent = New WaveInEvent()
    Private writer As WaveFileWriter = Nothing
    Private nomeArquivo As String = ""
    Private closing As Boolean = False
    Private localSalvoMp3 As String = ""
    Friend token As String = ""
    Friend numerocli As String = ""
    Friend instanceid As String = ""
    Private resourcePath As String = "C:\zap_resources\audios\"

    Friend meuPai As frmZap

    Private Sub frmZapAudio_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Directory.CreateDirectory(caminho)
        btnEnviar.Enabled = False
        btnPlay.Enabled = False
        btnSalvar.Enabled = False
        PictureBox1.Image = My.Resources.mic_zap_vermelho

        '

        AddHandler waveIn.DataAvailable, Sub(s, a)
                                             writer.Write(a.Buffer, 0, a.BytesRecorded)
                                             If writer.Position > waveIn.WaveFormat.AverageBytesPerSecond * 30 Then
                                                 waveIn.StopRecording()
                                             End If
                                         End Sub


        AddHandler btnGravar.Click, Sub(s, a)
                                        nomeArquivo = $"{ID_LoginUser}{DateTime.Now.ToString("s").Replace("-", "").Replace(":", "")}"
                                        caminhoSaida = Path.Combine(caminho, nomeArquivo + ".wav")
                                        writer = New WaveFileWriter(caminhoSaida, waveIn.WaveFormat)
                                        waveIn.StartRecording()
                                        btnGravar.Enabled = False
                                        btnPlay.Enabled = False
                                        btnEnviar.Enabled = False
                                        btnSalvar.Enabled = True
                                        PictureBox1.Image = My.Resources.mic_zap_verde
                                    End Sub



        AddHandler btnSalvar.Click, Sub(s, a)
                                        waveIn.StopRecording()
                                        PictureBox1.Image = My.Resources.mic_zap_vermelho
                                        btnEnviar.Enabled = True
                                        btnGravar.Enabled = True
                                        btnPlay.Enabled = True
                                        btnSalvar.Enabled = False


                                    End Sub





        AddHandler Me.FormClosing, Sub(s, a)
                                       closing = True
                                       waveIn.StopRecording()

                                       ' mexi
                                       waveIn.Dispose()



                                   End Sub


        AddHandler waveIn.RecordingStopped, Sub(s, a)
                                                If writer IsNot Nothing Then
                                                    writer.Dispose()
                                                End If
                                                writer = Nothing
                                                If closing Then
                                                    waveIn.Dispose()
                                                    ' Limpar a pasta
                                                    Try
                                                        If Directory.Exists(caminho) Then
                                                            For Each arquivo As String In Directory.GetFiles(caminho)
                                                                File.Delete(arquivo)
                                                            Next
                                                        End If
                                                        Exit Sub
                                                    Catch ex As Exception
                                                        MessageBox.Show(ex.Message)
                                                        Exit Sub
                                                    End Try
                                                End If
                                                localSalvoMp3 = TransformaMp3()
                                                btnGravar.Enabled = True
                                                btnSalvar.Enabled = False

                                            End Sub
    End Sub



    Private Function TransformaMp3()
        Try
            Dim wavFile() As Byte = File.ReadAllBytes(caminhoSaida)
            Dim localSaidaMp3 = caminho + nomeArquivo + ".mp3"

            Using ms As MemoryStream = New MemoryStream(wavFile)
                Using rdr As WaveFileReader = New WaveFileReader(ms)
                    Using wtr As Lame.LameMP3FileWriter = New Lame.LameMP3FileWriter(localSaidaMp3, rdr.WaveFormat, 128)
                        rdr.CopyTo(wtr)
                    End Using
                End Using
            End Using
            Return localSaidaMp3
        Catch ex As Exception
            MsgBox(ex.Message)
        End Try
        Return ""
    End Function


    Private Sub AbreUltimoAudio()
        Try
            Dim p As Process = New Process()
            p.StartInfo.UseShellExecute = True
            p.StartInfo.FileName = caminhoSaida
            p.Start()
        Catch ex As Exception
            MessageBox.Show(ex.Message)
        End Try

    End Sub

    Private Sub Enviar_Click(sender As Object, e As EventArgs) Handles btnEnviar.Click

        Dim base64string As String = ""


        Try
            System.IO.File.Copy(localSalvoMp3, $"{resourcePath}{nomeArquivo}.mp3", True)

        Catch ex As Exception
            MessageBox.Show(ex.Message)
        End Try


        Try
            Dim byteData() As Byte = File.ReadAllBytes(localSalvoMp3)
            base64string = Convert.ToBase64String(byteData)

            Dim webUtility As UrlEncoder

            webUtility = UrlEncoder.Create()
            base64string = webUtility.Encode(base64string)
        Catch ex As Exception
            MessageBox.Show(ex.Message)
        End Try


        Try
            Dim sqlLocalFile As String = $"http:file://C:/zap_resources/audios/{nomeArquivo}.mp3"

            Dim idLocal = SQL_escalar($"insert into zap.msgzap (event_type, dataack, visto, atendente, datamedia) values('message_create','server',true, '{ID_LoginUser}','{sqlLocalFile}') returning id")


            Dim WebRequest As HttpWebRequest = HttpWebRequest.Create($"https://api.ultramsg.com/{instanceid}/messages/audio")
            Dim postdata As String = $"token={token}&to={numerocli}&audio={base64string}&priority=&referenceId={idLocal}&nocache=&msgId="
            Dim enc As UTF8Encoding = New System.Text.UTF8Encoding()
            Dim postdatabytes As Byte() = enc.GetBytes(postdata)
            WebRequest.Method = "POST"
            WebRequest.ContentType = "application/x-www-form-urlencoded"
            WebRequest.GetRequestStream().Write(postdatabytes, 0, postdatabytes.Length)
            Dim ret As New System.IO.StreamReader(WebRequest.GetResponse().GetResponseStream())

            Dim retString As String = ret.ReadToEnd()

            If isNULL(retString, "") <> "" AndAlso retString.Contains("message"":""ok") Then
                Try


                    Dim novaConversa = ""

                    novaConversa += $"<div class='chat-r' id='{idLocal}'>
					<div class=sp></div>
					<div class=""mess mess-r"">"
                    novaConversa += $"<h2><span class='atendente'>{ID_LoginUser}:</span></h2>"
                    novaConversa += $"<a href='{sqlLocalFile}' class='document'><img width='50px' src='data:image/png;base64,{meuPai.audioZap}'></a>"
                    novaConversa += $"<p>{meuPai.txtMsg.Text}</p>
                <div Class='check'> 
							<span>{Date.Now}</span>"
                    novaConversa += $"<img class='checkImg' src='data:image/png;base64,{meuPai.relogin_zap}'>"
                    novaConversa += $"</div>
                   <div class='mess-reaction'><span class='reaction-r'></span></div>
					</div>
				</div>"


                    Dim javascriptola As String = $"
                    var node = document.querySelector('.chat-box')
                    node.insertAdjacentHTML(""beforeend"", `{novaConversa}`)

                    myImages = document.querySelectorAll('.myImage');
                    AddListenerToImages()

                    myDocuments = document.querySelectorAll('.document');
                    AddListenerToDocuments()


                    elements = document.querySelectorAll('.mess');

                    // Get the last element
                    lastElement = elements[elements.length - 1];

                    lastElement.scrollIntoView();

            "
                    meuPai.invokeExecuteScriptAsync(javascriptola)
                    meuPai.txtMsg.Text = ""
                    Me.Close()
                Catch ex As Exception
                    MessageBox.Show(ex.Message)
                End Try
            Else
                MessageBox.Show("Erro Audio1")

            End If


        Catch ex As Exception
            MessageBox.Show(ex.Message)
        End Try
    End Sub

    Private Sub btnPlay_Click(sender As Object, e As EventArgs) Handles btnPlay.Click
        AbreUltimoAudio()
    End Sub

    Private Sub frmZapAudio_FormClosed(sender As Object, e As FormClosedEventArgs) Handles MyBase.FormClosed

        If closing = True Then
            Try
                If writer IsNot Nothing Then
                    writer.Dispose()
                End If
                writer = Nothing
                If Directory.Exists(caminho) Then
                    For Each arquivo As String In Directory.GetFiles(caminho)
                        File.Delete(arquivo)
                    Next
                End If
            Catch ex As Exception
                MessageBox.Show(ex.Message)
            End Try
        End If

    End Sub
End Class