Imports System.IO
Imports System.Text
Imports System.Text.RegularExpressions
Imports System.Windows.Forms
Imports System.Threading
Imports Microsoft.Web.WebView2.Core
Imports Npgsql.Replication.PgOutput.Messages.RelationMessage
Imports Microsoft.UI.Xaml.Controls
Imports System.Text.Encodings.Web
Imports System.Net

Public Class frmZap
    Private html2 As String
    Private threadAtualiza As Thread = New Thread(AddressOf atualizarConversa)
    Friend telefoneCLI As String
    Private telefoneNosso As String = "Telefone da instancia"
    Private instanceId As String = "instancia"
    Private token As String = "token"
    Private resourceCheck As Image = My.Resources.check_2
    Private resourceUnread As Image = My.Resources.check_1
    Private resourceDocument As Image = My.Resources.black_doc
    Private resourceRelogin As Image = My.Resources.relogin_zap_128
    Private resourceImage As Image = My.Resources.image_zap
    Private image = ImageToBase64(resourceImage)
    Private resourceAudio As Image = My.Resources.audio_zap
    Friend audioZap = ImageToBase64(resourceAudio)
    Private resourceVideo As Image = My.Resources.video_zap
    Private videoZap = ImageToBase64(resourceVideo)
    Private resourceSticker As Image = My.Resources.stiker_zap
    Private stickerZap = ImageToBase64(resourceSticker)
    Private document = ImageToBase64(resourceDocument)
    Friend relogin_zap = ImageToBase64(resourceRelogin)
    Private read = ImageToBase64(resourceCheck)
    Private unread = ImageToBase64(resourceUnread)
    Private dtContatos As DataTable = New DataTable
    Private contatoGlobal As String = ""
    Private resourcePath As String = "C:\zap_resources\"


    Private Async Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Dim options As CoreWebView2EnvironmentOptions = New CoreWebView2EnvironmentOptions("--allow-file-access-from-files")
        Dim environment As CoreWebView2Environment = Await CoreWebView2Environment.CreateAsync(Nothing, Nothing, options)

        Await WebView21.EnsureCoreWebView2Async(environment)
        threadAtualiza.IsBackground = True
        threadAtualiza.Start()
        preencheListaContatos()

        AddHandler WebView21.CoreWebView2.NewWindowRequested, AddressOf CoreWebView2_NewWindowRequested
        reload()
    End Sub

    Private Sub doc_click()
        Dim mensagem As String = isNULL(txtMsg.Text, "")
        sendFile(mensagem)
    End Sub
    Private Sub img_click()
        Dim mensagem As String = isNULL(txtMsg.Text, "")
        sendImage(mensagem)
    End Sub
    Private Sub vid_click()
        Dim mensagem As String = isNULL(txtMsg.Text, "")
        sendVideo(mensagem)
    End Sub
    Private Sub sendFile(ByVal mensagem As String)
        Dim ret As String
        Dim novaConversa As String = ""
        Dim idLocal As String = ""

        ret = EnviarArquivoZap(mensagem)

        If isNULL(ret, "") <> "" AndAlso ret.Contains("message"":""ok") Then
            Dim arquivoServer As String
            Dim retArray = ret.Split("|")
            arquivoServer = retArray(1)
            idLocal = retArray(2) 'idLocal

            Try
                novaConversa += $"<div class='chat-r' id='{idLocal}'>
					<div class=sp></div>
					<div class=""mess mess-r"">"
                'novaConversa += $"<h2><span class='atendente'>{ID_LoginUser}:</span></h2>"
                novaConversa += $"<a href='{arquivoServer}' class='document'><img width='50px' src='data:image/png;base64,{document}'></a>"
                novaConversa += $"<p>{mensagem}</p>
                <div Class='check'> 
							<span>{Date.Now}</span>"
                novaConversa += $"<img class='checkImg' src='data:image/png;base64,{relogin_zap}'>"
                novaConversa += $"</div>
                   <div class='mess-reaction'><span class='reaction-r'></span></div>
					</div>
				</div>"


                Dim javascripto As String = $"
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

			colocaLinks()
			AddListenerAnchor()

            "
                invokeExecuteScriptAsync(javascripto)
                txtMsg.Text = ""
            Catch ex As Exception
                MessageBox.Show(ex.Message)
            End Try
        Else
            If ret = "" Then
                ret = "Formato incompatível"
            End If
            MessageBox.Show(ret)
        End If
        'Reload()
    End Sub

    Private Function EnviarArquivoZap(ByVal mensagem As String) As String
        Dim retorno As String
        Dim WebRequest As HttpWebRequest
        Dim postdata As String
        Dim enc As UTF8Encoding = New System.Text.UTF8Encoding()
        Dim ret As System.IO.StreamReader
        Dim postdatabytes As Byte()
        Dim sqlLocalFile As String
        Dim idLocal As String
        Dim nomeTratadoSemEspaco As String
        Dim byteData() As Byte
        Dim webUtility As UrlEncoder
        Dim fileBytes As Byte()
        Dim base64string As String
        Dim fileName As String
        Dim encod As New System.Text.UTF8Encoding


        Dim ofd As New OpenFileDialog
        ofd.Filter = "All Files|*.txt;*.docx;*.doc;*.pdf*;.xls;*.xlsx;*.pptx;*.ppt|Text File (.txt)|*.txt|Word File (.docx ,.doc)|*.docx;*.doc|PDF (.pdf)|*.pdf|Spreadsheet (.xls ,.xlsx)|  *.xls ;*.xlsx|Presentation (.pptx ,.ppt)|*.pptx;*.ppt" 'If you like file type filters you can add them here
        If ofd.ShowDialog = DialogResult.OK Then

            byteData = File.ReadAllBytes(ofd.FileName)
            fileBytes = File.ReadAllBytes(ofd.FileName)

            fileName = (ofd.SafeFileName)
            Try
                base64string = Convert.ToBase64String(byteData)
            Catch ex As Exception
                MessageBox.Show(ex.Message)
            End Try

            webUtility = UrlEncoder.Create()
            base64string = webUtility.Encode(base64string)
            nomeTratadoSemEspaco = ofd.SafeFileName.Replace(" ", "").Trim()

            Try
                System.IO.File.Copy(ofd.FileName, $"{resourcePath}documents\{nomeTratadoSemEspaco}", True)
            Catch ex As Exception
                MessageBox.Show(ex.Message)
            End Try

            sqlLocalFile = $"http:file://C:/zap_resources/documents/{nomeTratadoSemEspaco}"

            idLocal = SQL_escalar($"insert into zap.msgzap (event_type, dataack, visto, atendente, datamedia) values('message_create','server',true, '{ID_LoginUser}','{sqlLocalFile}') returning id")

            mensagem = System.Net.WebUtility.UrlEncode(mensagem)
            WebRequest = HttpWebRequest.Create($"https://api.ultramsg.com/{instanceId}/messages/document")
            postdata = $"token={token}&to={telefoneCLI}&filename={fileName}&document={base64string}&caption={mensagem}&referenceId={idLocal}&teste=10"
            postdatabytes = enc.GetBytes(postdata)
            WebRequest.Method = "POST"
            WebRequest.ContentType = "application/x-www-form-urlencoded"
            'WebRequest.GetRequestStream().Write(postdatabytes)
            WebRequest.GetRequestStream().Write(postdatabytes, 0, postdatabytes.Length)

            Try
                ret = New System.IO.StreamReader(WebRequest.GetResponse().GetResponseStream())
                retorno = ret.ReadToEnd() + $"|{sqlLocalFile}" + $"|{idLocal}"
                ret.Close()
            Catch ex As Exception
                MessageBox.Show(ex.Message)
                Exit Function
            End Try
        End If

        Return retorno
    End Function
    Private Function EnviarImagemZap(ByVal mensagem As String, ByVal idLocal As String) As String
        Dim base64string As String
        Dim WebRequest As HttpWebRequest
        Dim postdata As String
        Dim enc As UTF8Encoding
        Dim postdatabytes As Byte()
        Dim ret As System.IO.StreamReader
        Dim sqlLocalFile As String
        Dim nomeTratadoSemEspaco As String



        Dim ofd As New OpenFileDialog
        ofd.Filter = "All Images Files (*.png;*.jpeg;*.gif;*.jpg;*.bmp;*.tiff;*.tif)|*.png;*.jpeg;*.gif;*.jpg;*.bmp;*.tiff;*.tif" +
            "|PNG Portable Network Graphics (*.png)|*.png" +
            "|JPEG File Interchange Format (*.jpg *.jpeg *jfif)|*.jpg;*.jpeg;*.jfif" +
            "|BMP Windows Bitmap (*.bmp)|*.bmp" +
            "|TIF Tagged Imaged File Format (*.tif *.tiff)|*.tif;*.tiff" +
            "|GIF Graphics Interchange Format (*.gif)|*.gif"
        If ofd.ShowDialog = DialogResult.OK Then

            Try
                Dim encod As New System.Text.UTF8Encoding
                Dim byteData() As Byte = File.ReadAllBytes(ofd.FileName)
                base64string = Convert.ToBase64String(byteData)
                nomeTratadoSemEspaco = ofd.SafeFileName.Replace(" ", "").Trim()
            Catch ex As Exception
                MessageBox.Show(ex.Message)
                Exit Function
            End Try


            Try
                System.IO.File.Copy(ofd.FileName, $"{resourcePath}images\{nomeTratadoSemEspaco}", True)
            Catch ex As Exception
                MessageBox.Show(ex.Message)
            End Try

            Try
                sqlLocalFile = $"http:file://C:/zap_resources/images/{nomeTratadoSemEspaco}"
                Dim updateImageInSql As String = $"UPDATE zap.msgzap SET datamedia = '{sqlLocalFile}' WHERE id = {idLocal}"
                SQL_executeNonQuery(updateImageInSql)

                Dim webUtility = UrlEncoder.Create()
                base64string = webUtility.Encode(base64string)

                mensagem = System.Net.WebUtility.UrlEncode(mensagem)
                WebRequest = HttpWebRequest.Create($"https://api.ultramsg.com/{instanceId}/messages/image")
                postdata = $"token={token}&to={telefoneCLI}&image={base64string}&caption={mensagem}&priority=&referenceId={idLocal}&nocache=&msgId=&mentions="
                enc = New System.Text.UTF8Encoding()
                postdatabytes = enc.GetBytes(postdata)
                WebRequest.Method = "POST"
                WebRequest.ContentType = "application/x-www-form-urlencoded"
                WebRequest.GetRequestStream().Write(postdatabytes, 0, postdatabytes.Length)
                ret = New System.IO.StreamReader(WebRequest.GetResponse().GetResponseStream())

                Return ret.ReadToEnd + $"|{sqlLocalFile}"

            Catch ex As Exception
                MessageBox.Show(ex.Message)
                Exit Function
            End Try
        End If

    End Function
    Private Sub sendImage(ByVal mensagem As String)
        Dim ret As String

        Dim novaConversa As String = ""
        Dim idLocal = SQL_escalar($"insert into zap.msgzap (event_type,dataack,visto, atendente,datamedia) values('message_create','server',true, '{ID_LoginUser}','') returning id")


        ret = EnviarImagemZap(mensagem, idLocal)

        If isNULL(ret, "") <> "" AndAlso ret.Contains("message"":""ok") Then
            Dim retArray = ret.Split("|")
            Dim imagem64OuUriRet As String = retArray(1) '1 da imagem retornada
            Try
                novaConversa += $"<div class='chat-r' id='{idLocal}'>
					<div class=sp></div>
					<div class=""mess mess-r"">"
                'novaConversa += $"<h2><span class='atendente'>{ID_LoginUser}:</span></h2>"
                novaConversa += $"<a href='{imagem64OuUriRet}' class='myImage'><img width='65px' src='data:image/png;base64,{image}'></a>"
                novaConversa += $"<p>{mensagem}</p>
                <div Class='check'> 
							<span>{Date.Now}</span>"
                novaConversa += $"<img class='checkImg' src='data:image/png;base64,{relogin_zap}'>"
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

			colocaLinks()
			AddListenerAnchor()

            "
                invokeExecuteScriptAsync(javascriptola)
                txtMsg.Text = ""
            Catch ex As Exception
                MessageBox.Show(ex.Message)
            End Try
        Else
            If ret = "" Then
                ret = "Formato incompatível"
            End If
            MessageBox.Show(ret)
        End If
    End Sub

    Private Sub sendVideo(ByVal mensagem As String)
        Dim ret As String
        Dim novaConversa As String = ""
        Dim idLocal As String = ""

        ret = EnviarVideoZap(mensagem)
        If isNULL(ret, "") <> "" AndAlso ret.Contains("message"":""ok") Then
            Dim arquivoServer As String
            Dim retArray = ret.Split("|")
            arquivoServer = retArray(1)
            idLocal = retArray(2) 'idLocal
            Try
                novaConversa += $"<div class='chat-r' id='{idLocal}'>
					<div class=sp></div>
					<div class=""mess mess-r"">"
                'novaConversa += $"<h2><span class='atendente'>{ID_LoginUser}:</span></h2>"
                novaConversa += $"<a href='{arquivoServer}' class='document'><img width='50px' src='data:image/png;base64,{videoZap}'></a>"
                novaConversa += $"<p>{mensagem}</p>
                <div Class='check'> 
							<span>{Date.Now}</span>"
                novaConversa += $"<img class='checkImg' src='data:image/png;base64,{relogin_zap}'>"
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
			
			colocaLinks()
			AddListenerAnchor()

            "
                invokeExecuteScriptAsync(javascriptola)
                txtMsg.Text = ""
            Catch ex As Exception
                MessageBox.Show(ex.Message)
            End Try



        Else
            If ret = "" Then
                ret = "Formato incompatível"
            End If
            MessageBox.Show(ret)
        End If
        'Reload()
    End Sub
    Private Function EnviarVideoZap(ByVal mensagem As String) As String

        Dim ofd As New OpenFileDialog
        ofd.Filter = "Video and Music Files (*.mov, *.mp4, *.3gp, |*.mov; *.mp4; *.3gp;|All Files (*.*)|*.*"
        If ofd.ShowDialog = DialogResult.OK Then
            Dim encod As New System.Text.UTF8Encoding

            Dim byteData() As Byte = File.ReadAllBytes(ofd.FileName)
            Dim webUtility As UrlEncoder
            Dim fileBytes As Byte() = File.ReadAllBytes(ofd.FileName)
            Dim base64string As String
            Dim fileName As String

            fileName = (ofd.SafeFileName)
            Try
                base64string = Convert.ToBase64String(byteData)
            Catch ex As Exception
                MessageBox.Show(ex.Message)
            End Try

            webUtility = UrlEncoder.Create()
            base64string = webUtility.Encode(base64string)
            Dim base64Compactada As String = zipstring(base64string)
            Dim nomeTratadoSemEspaco = ofd.SafeFileName.Replace(" ", "").Trim()

            Try
                System.IO.File.Copy(ofd.FileName, $"{resourcePath}videos\{nomeTratadoSemEspaco}", True)
            Catch ex As Exception
                MessageBox.Show(ex.Message)
            End Try


            Dim sqlLocalFile As String = $"http:file://C:/zap_resources/videos/{nomeTratadoSemEspaco}"


            Dim idLocal = SQL_escalar($"insert into zap.msgzap (event_type, dataack, visto, atendente, datamedia) values('message_create','server',true, '{ID_LoginUser}','{sqlLocalFile}') returning id")

            mensagem = System.Net.WebUtility.UrlEncode(mensagem)
            Dim WebRequest As HttpWebRequest
            WebRequest = HttpWebRequest.Create($"https://api.ultramsg.com/{instanceId}/messages/video")
            Dim postdata As String = $"token={token}&to={telefoneCLI}&video={base64string}&caption={mensagem}&priority=&referenceId={idLocal}&nocache=&msgId=&mentions="
            Dim enc As UTF8Encoding = New System.Text.UTF8Encoding()
            Dim postdatabytes As Byte() = enc.GetBytes(postdata)
            WebRequest.Method = "POST"
            WebRequest.ContentType = "application/x-www-form-urlencoded"
            'WebRequest.GetRequestStream().Write(postdatabytes)
            WebRequest.GetRequestStream().Write(postdatabytes, 0, postdatabytes.Length)
            Dim ret As System.IO.StreamReader
            Dim retorno As String

            Try
                ret = New System.IO.StreamReader(WebRequest.GetResponse().GetResponseStream())
                retorno = ret.ReadToEnd() + $"|{sqlLocalFile}" + $"|{idLocal}"
                ret.Close()
            Catch ex As Exception
                MessageBox.Show(ex.Message)
                Exit Function
            End Try
            Return retorno
        End If
    End Function
    Private Sub CoreWebView2_NewWindowRequested(ByVal sender As Object, ByVal e As CoreWebView2NewWindowRequestedEventArgs)
        Try
            Dim caminho As String = ""
            e.Handled = True
            Dim p As Process = New Process()
            p.StartInfo.UseShellExecute = True
            If e.Uri.Contains("https") Then
                caminho = e.Uri
            ElseIf e.Uri.Contains("about:blank#blocked") Then
                Exit Sub

            Else
                'caminho = "\\" + e.Uri.Substring(e.Uri.IndexOf("file//") + 6)
                caminho = e.Uri.Substring(e.Uri.IndexOf("file//") + 6)
                caminho = caminho.Replace("/", "\")

            End If

            p.StartInfo.FileName = caminho

            p.Start()
        Catch ex As Exception
            MessageBox.Show(ex.Message)
        End Try

    End Sub
    Friend Async Sub reload()
        preencheListaContatos()
        If telefoneCLI <> "" Then
            GetHtml()
            UpdateHTML()
            BringToFront()
            atualizaStatusVisto()
            WebView21.NavigateToString(html2)
            btnEditarContato.Visible = True
        Else
            btnEditarContato.Visible = False
        End If



    End Sub
    Private Sub atualizaStatusVisto()
        Dim strsql As StringBuilder = New StringBuilder
        strsql.Append("update ")
        strsql.Append("zap.msgzap ")
        strsql.Append("set ")
        strsql.Append("visto = true ")
        strsql.Append($"where numerocli = '{telefoneCLI}' ")
        SQL_executeNonQuery(strsql.ToString) 'Comentar depois pra testar o grid
    End Sub
    Friend Sub preencheListaContatos()
        lstContatos.Items.Clear()
        dtContatos = SQL_GetDatatable("select id, numerocli,nomecli,dataultimamsg,3 as prioridade from zap.zap_contato")
        'dtContatos.DefaultView.Sort = "prioridade"

        For i As Integer = 0 To dtContatos.Rows.Count - 1
            If isNULL(dtContatos.Rows(i)("nomecli"), "") <> "" Then
                lstContatos.Items.Add(dtContatos.Rows(i)("nomecli") + ";" + dtContatos.Rows(i)("numerocli"))
            Else
                lstContatos.Items.Add("Sem contato" + ";" + dtContatos.Rows(i)("numerocli"))
            End If
        Next
    End Sub
    Private Sub GetHtml()
        Dim cliente As String = "Contato"
        html2 = "<!DOCTYPE html>" +
    "<html lang=en>" +
    "<head>" +
    "<meta charset=UTF-8>" +
    "<title>Whatsapp UI</title>" +
    "<style>" +
    "*{padding:  0;margin:0;box-sizing:border-box;font-family:sans-serif;}" +
    ".container {width:612px;margin: auto;}" +
    ".chat {display:flex;flex-direction:column;height: 100vh;background:#f1f0e8;}" +
    ".chat-header {display: flex;cursor:pointer;}" +
    ".profile {width: 100%;background:#036055;display:flex;align-items:center;height:60px;padding: 0px 10px;position:relative;}" +
    ".profile .pp {width:50px;display:inline-block;border-radius:50%;margin-left:32px;}" +
    ".profile .arrow {display:inline-block;width:30px;position:absolute;top:19px;cursor:pointer;}" +
    ".profile h2 {display:inline-block;line-height: 60px;vertical-align: bottom;color:#fff;font-size:20px;}" +
    ".profile span {color:#ccc;position:absolute;top:42px;left:100px;font-size:14px;}" +
    ".right .icon {display: inline-block;width: 25px;margin-left: 10px;}" +
    ".profile .left {flex: 1;}" +
    ".chat-box {background: url('../img/bg.jpeg');background-attachment: fixed;padding-left: 20px;overflow-y: scroll;flex: 1;}" +
    ".chat-box .img_chat {width: 280px;}" +
    ".chat-r {display: flex;}" +
    ".chat-r .sp {flex: 1;}" +
    ".chat-l {display: flex;}" +
    ".chat-l .sp {flex: 1;}" +
    ".chat-box .mess {max-width: 300px;background: #F7FCF6;padding: 10px;border-radius: 10px;margin: 20px 0px;cursor: pointer;}" +
    ".chat-box .mess p {word-break: break-all;font-size: 18px;}" +
    ".chat-box .mess-r {background: #E2FFC7;}" +
    ".chat-box .emoji {width: 20px;}" +
    ".chat-box .check {float: right; position: relative}" + ' pos relative
    ".chat-box .check img {width: 20px; top: 5px; position: relative;}" + ' top e pos
    ".chat-box .check span {color: #888;font-size: 12px;font-weight: 700px;}" +
    "*::-webkit-scrollbar {width: 5px;}" +
    "*::-webkit-scrollbar-track {background: #f1f1f1;}" +
    "*::-webkit-scrollbar-thumb {background: #aaa;}" +
    "*::-webkit-scrollbar-thumb:hover {background: #555;}" +
    ".chat-footer {display: flex;justify-content: center;align-items: center;border-radius: 60px;position: relative;cursor: pointer;}" +
    ".chat-footer textarea {display: block;flex: 1;width: 100%;height: 50px;border-radius: 60px;margin: 5px;padding: 10px;outline: none;font-size: 19px;padding-left: 40px;padding-right: 90px;border: 2px solid #ccc;color: #555;resize: none;}" +
    ".chat-footer .mic {display: block;width: 55px;height: 55px;margin-right: 20px;}" +
    ".chat-footer .emo{display: block;width: 35px;height: 35px;position: absolute;left: 10px;top: 12px;}" +
    ".chat-footer .icons {position: absolute;right: 100px;top: 10px;}" +
    ".chat-footer .icons img{display: inline-block;width: 35px;height: 35px;margin-left: 5px;}" +
    ".mess-reaction {position: relative;}" +
    ".reaction-l {position: absolute;top: 25px;right: -15px;}" +
    ".reaction-r {position: absolute;top: 25px;left: -15px;}" +
    "span.atendente {font-size: 15px; background: yellow;}" +
    "#btnScrollDown{font-size: 30px;border-radius: 20px;width: 6%;background-color: #036055;cursor: pointer;color: #fff;position: absolute;top: 410px;left: 90%;border: solid #036055;}" +
    "</style>" +
    "</head>" +
    "<body>" +
    "<div class=container>" +
    "<div class=chat>" +
    "<div class=chat-header>" +
    "<div class=profile>" +
    "<div class=left>" +
    "<h2>" +
    contatoGlobal +
    "</h2>" +
    "</div>" +
    "<div class=right style=color:white;>" +
     "</div>" +
     "</div>" +
     "</div>" +
     "<div class=chat-box>"
    End Sub
    Private Sub UpdateHTML()
        Dim dt As DataTable = New DataTable
        dt = retornaMensagens()




        For Each chat As DataRow In dt.Rows
            Dim sqlId As String = chat("id")
            Dim mensagem As String = chat("databody")
            Dim horas As DateTime = chat("datatime")
            Dim dataFrom As String = chat("datafrom")
            Dim datamidia As String = chat("datamedia")
            Dim reaction As String = chat("reaction")
            Dim msgStatus As String = chat("dataack")
            Dim atendente As String = chat("atendente")
            Dim dataId As String = chat("dataid")


            Dim enviado As Boolean

            If dataFrom.Contains(telefoneNosso) Then
                enviado = True
            Else
                enviado = False
            End If

            mensagem = DecodeEncodedNonAsciiCharacters(mensagem)
            reaction = DecodeEncodedNonAsciiCharacters(reaction)


            If enviado Then
                html2 += $"<div class='chat-r' id='{sqlId}'>
					<div class=sp></div>
					<div class=""mess mess-r"">"
                'html2 += $"<h2><span class='atendente'>{atendente}:</span></h2>"
                If chat("datatype") = "image" Then
                    html2 += $"<a href='{datamidia}' class='myImage'><img width='65px' src='data:image/png;base64,{image}'></a>"
                ElseIf chat("datatype") = "ptt" OrElse chat("datatype") = "audio" Then
                    html2 += $"<a href='{datamidia}' class='document'><img width='50px' src='data:image/png;base64,{audioZap}'></a>"
                ElseIf chat("datatype") = "video" Then
                    html2 += $"<a href='{datamidia}' class='document'><img width='50px' src='data:image/png;base64,{videoZap}'></a>"
                ElseIf chat("datatype") = "sticker" Then
                    html2 += $"<a href='{datamidia}' class='document'><img width='50px' src='data:image/png;base64,{stickerZap}'></a>"
                ElseIf chat("datatype") = "document" Then

                    If Not datamidia.Contains("http://s3") Then

                    End If
                    Try
                        Dim string64 As String = UnZipString(datamidia)
                        Dim bytes64() = Convert.FromBase64String(string64)

                        File.WriteAllBytes("C:/wprgtemp/teste001.pdf", bytes64)

                    Catch ex As Exception
                        'MsgAlert(ex.Message, "Erro", MessageBoxIcon.Error)
                    End Try



                    html2 += $"<a href='{datamidia}' class='document'><img width='50px' src='data:image/png;base64,{document}'></a>"
                End If
                html2 += $"<p>{mensagem}</p>
                <div Class='check'> 
							<span>{horas}</span>"
                If msgStatus = "read" Then
                    html2 += $"<img class='checkImg' src='data:image/png;base64,{read}'>"
                ElseIf msgStatus <> "read" Then
                    html2 += $"<img class='checkImg' src='data:image/png;base64,{unread}'>"
                End If
                html2 += $"</div>
                   <div class='mess-reaction'><span class='reaction-r'>{reaction}</span></div>
					</div>
				</div>
"
            Else
                html2 += $"<div class=chat-l>
							<div class=mess>"
                If chat("datatype") = "image" Then
                    html2 += $"<a href='{datamidia}' class='myImage'><img width='65px' src='data:image/png;base64,{image}'></a>"
                ElseIf chat("datatype") = "ptt" OrElse chat("datatype") = "audio" Then
                    html2 += $"<a href='{datamidia}' class='document'><img width='50px' src='data:image/png;base64,{audioZap}'></a>"
                ElseIf chat("datatype") = "video" Then
                    html2 += $"<a href='{datamidia}' class='document'><img width='50px' src='data:image/png;base64,{videoZap}'></a>"
                ElseIf chat("datatype") = "sticker" Then
                    html2 += $"<a href='{datamidia}' class='document'><img width='50px' src='data:image/png;base64,{stickerZap}'></a>"
                ElseIf chat("datatype") = "document" Then
                    html2 += $"<a href='{datamidia}' class='document'><img width='50px' src='data:image/png;base64,{document}'></a>"
                End If
                html2 += $"<p>{mensagem}</p>
				<div class=check>
							<span>{horas}</span>
                        </div>
                   <div class='mess-reaction'><span class='reaction-l'>{reaction}</span></div>
						</div>
					<div class=sp></div>
			    </div>"
            End If
        Next
        If dt.Rows.Count > 0 Then
            html2 += "<div id='scrollDown'><button id='btnScrollDown'>&#8595;</button></div>"
            html2 += "<div id='enlargedImageContainer' style='display: none; position: fixed; top: 0; left: 0; width: 100%; height: 100%; background-color: rgba(0, 0, 0, 0.8);'>
  <img id='enlargedImage' style='width: 100%; height: 100%' />
<button id='closeButton' style='position: absolute; top: 10px; left: 10px; border-radius: 15px; background-color: #075E54; color: #fff; font-size: 20px; cursor: pointer; padding: 5px; '>Fechar ╳</button>
</div></div>"
            html2 +=
"<script>
			  // Get all the image elements
			  var myImages = document.querySelectorAll('.myImage');
			  var myDocuments = document.querySelectorAll('.document');
			
			  // Get the container, image, and close button elements
			  const container = document.getElementById('enlargedImageContainer');
			  const enlargedImage = document.getElementById('enlargedImage');
			  const closeButton = document.getElementById('closeButton');
			
			
			  // Faz a tela de carregamento antes de aparecer para o usuario
			  container.style.display = 'block';
			  enlargedImage.src = 'https://brveiculossorriso.com.br/fotos/carregando.gif'
			  closeButton.style.display = 'none';
			
			
			   
			  // Loop through the images
			  function AddListenerToImages_DEPRECATED()
			{
			  myImages.forEach(function(myImage) {
			    // Add a click event listener to the image
			    myImage.addEventListener('click', function() {
			      // Set the source of the enlarged image to be the same as the clicked image
			      enlargedImage.src = myImage.src;
			
			      // Show the container
			      container.style.display = 'block';
			    });
			  });
			}

		// Trocando Listener

		  function AddListenerToImages() {
			myImages.forEach(function(myImage) {
			  if (!myImage.hasAttribute('data-listener-added')) {
				myImage.addEventListener('click', function() {
				  event.preventDefault();
				  console.log(myImage.href);
				  window.open(myImage.href);
				});
				myImage.setAttribute('data-listener-added', 'true');
			  }
			});
		  }

		

			
			AddListenerToImages()
			
			function AddListenerToDocuments() {
			  myDocuments.forEach(function(myDocument) {
			    if (!myDocument.hasAttribute('data-listener-added')) {
			      myDocument.addEventListener('click', function() {
			        event.preventDefault();
			        window.open(myDocument.href);
			      });
			      myDocument.setAttribute('data-listener-added', 'true');
			    }
			  });
			}
			
			AddListenerToDocuments()
			
			  // Add a click event listener to the close button
			  closeButton.addEventListener('click', function() {
			    // Hide the container
			    container.style.display = 'none';
			  });
			
			     var elements = document.querySelectorAll('.mess');
			
			    // Get the last element
			    var lastElement = elements[elements.length - 1];
			
			    window.addEventListener('load', () => {
			        lastElement.scrollIntoView();
			        container.style.display = 'none';
			        closeButton.style.display = 'block'
			    });
			
			var btnScroll = document.getElementById(""btnScrollDown"")
			
			 btnScroll.addEventListener('click', function() {
			        lastElement.scrollIntoView();
			    });
			
			
			    var chatBox = document.getElementsByClassName('chat-box')[0];
			
			    chatBox.addEventListener('scroll', function() {
			        var isAtBottom = chatBox.scrollTop + chatBox.clientHeight >= chatBox.scrollHeight;
			        if (!isAtBottom) {
			            // The user has scrolled to the bottom of the chat box
			            btnScroll.style.display = ""block"";
			            
			        }
			        else{
			             btnScroll.style.display = ""none"";
			            }
			    });
			
			function RemoveRepeatedMsg() {
			    var elements = document.querySelectorAll('.chat-r');
			    var ids = {};
			
			    for (var i = 0; i < elements.length; i++) {
			        var id = elements[i].id;
			        if (ids[id]) {
			            ids[id].push(elements[i]);
			        } else {
			            ids[id] = [elements[i]];
			        }
			    }
			
			    for (var id in ids) {
			        if (ids[id].length > 1) {
			            for (var i = 0; i < ids[id].length - 1; i++) {
			                ids[id][i].parentNode.removeChild(ids[id][i]);
			            }
			        }
			    }
			}



// Adiciona <a> para links nas mensagens

const linkRegex = /(https?\:\/\/)?(www\.)?[^\s]+\.[^\s]+/g

function replacer(matched) {
    let withProtocol = matched
    
    if(!withProtocol.startsWith(""http"")) {
      withProtocol = ""http://"" + matched
    }
   
    const newStr = `<a onclick='event.preventDefault()' class=""text-link"" href=""${withProtocol}"">${matched}</a>`
    
    return newStr
  }



  function colocaLinks() {
    var mensagens = document.querySelectorAll('p')

    mensagens.forEach(function(mensagem) {
      if (!mensagem.hasAttribute('data-listener-added3')) {
        mensagem.innerHTML = mensagem.innerHTML.replace(linkRegex, replacer)
        mensagem.setAttribute('data-listener-added3', 'true');
      }
    });
  }

colocaLinks()

  function AddListenerAnchor() {
    var anchors = document.querySelectorAll('a');


    anchors.forEach(function(anchor) {
      if (!anchor.hasAttribute('data-listener-added')) {
        anchor.addEventListener('click', function() {
          event.preventDefault();
          window.open(anchor.href);
        });
        anchor.setAttribute('data-listener-added', 'true');
      }
    });
  }
 AddListenerAnchor()
	    
			</script>"
        End If
    End Sub
    Friend Sub ajustaTamanhoDoWebView()
        Dim ponto As Point
        WebView21.Height = 600
        WebView21.Width = 780
        ponto = New Point(210, 5)
        WebView21.Location = ponto
        WebView21.BringToFront()

        ponto.X = 500
        ponto.Y = 12
        btnAtualizar.BringToFront()

        ponto.X = 885
        ponto.Y = 12

        ponto.X = 768
        ponto.Y = 12

        txtMsg.BringToFront()
    End Sub
    Private Function GetVBdatetimeFromUnixTimestamp(UnixTimeStamp As Double) As DateTime
        Dim UnixEpoch As New DateTime(1970, 1, 1, 0, 0, 0, 0)
        Return UnixEpoch.AddSeconds(UnixTimeStamp - 10800)  '-3h gmt
    End Function
    Private Function SendMessage(Telefone As String, Mensagem As String, idMensagem As String) As StreamReader
        Dim WebRequest As HttpWebRequest
        WebRequest = HttpWebRequest.Create($"https://api.ultramsg.com/{instanceId}/messages/chat")
        Dim postdata As String = $"token={token}&to=+{Telefone}&body={Mensagem}&priority=5&referenceId={idMensagem}&msgId=&mentions=" '{ID_LoginUser}|
        Dim enc As UTF8Encoding = New System.Text.UTF8Encoding()
        Dim postdatabytes As Byte() = enc.GetBytes(postdata)
        WebRequest.Method = "POST"
        WebRequest.ContentType = "application/x-www-form-urlencoded"
        WebRequest.GetRequestStream().Write(postdatabytes, 0, postdatabytes.Length)
        Dim ret As New System.IO.StreamReader(WebRequest.GetResponse().GetResponseStream())
        Return ret
    End Function
    Private Function retornaMensagens() As DataTable
        Dim strsql As StringBuilder = New StringBuilder
        Dim dt As DataTable = New DataTable

        strsql.Append("select ")
        strsql.Append("id, ")
        strsql.Append("datafrom,")
        strsql.Append("databody,")
        strsql.Append("dataid,")
        strsql.Append("dataack,")
        strsql.Append("atendente,")
        strsql.Append("datatime,")
        strsql.Append("datamedia, ")
        strsql.Append("reaction, ")
        strsql.Append("datatype ")
        strsql.Append("from ")
        strsql.Append("zap.msgzap ")
        strsql.Append("where ")
        strsql.Append("datafrom like '55" + telefoneCLI + "%'")
        strsql.Append("or ")
        strsql.Append("datato like '55" + telefoneCLI + "%' ")
        strsql.Append("order by id desc limit 100")

        dt = SQL_GetDatatable(strsql.ToString)

        Dim reversedDt = dt.Clone()

        For i As Integer = dt.Rows.Count - 1 To 0 Step -1
            reversedDt.ImportRow(dt.Rows(i))
        Next

        Return reversedDt
    End Function

    Public Shared Function ImageToBase64(img As Image) As String
        Try
            Return Convert.ToBase64String(ImageToByteArray(img))
        Catch ex As Exception
            MessageBox.Show(ex.Message)
        End Try
    End Function

    Public Shared Function ImageToByteArray(img As Image) As Byte()
        Using strm As New MemoryStream
            Try
                img.Save(strm, img.RawFormat)
            Catch ex As Exception
                MessageBox.Show(ex.Message)
                Exit Function
            End Try
            Return strm.GetBuffer()
        End Using
    End Function
    Private Async Sub atualizarConversa()
        Do
            ' ver os unread
            Dim strsql As StringBuilder = New StringBuilder
            Dim dt As DataTable = New DataTable

            strsql.Append("select ")
            strsql.Append("id, ")
            strsql.Append("datafrom,")
            strsql.Append("databody,")
            strsql.Append("dataack,")
            strsql.Append("atendente,")
            strsql.Append("datatime,")
            strsql.Append("datamedia, ")
            strsql.Append("reaction, ")
            strsql.Append("datatype ")
            strsql.Append("from ")
            strsql.Append("zap.msgzap ")
            strsql.Append("where ")
            strsql.Append($"(numerocli = '{telefoneCLI}' ")
            strsql.Append("AND visto = false) ")
            strsql.Append("order by id asc")

            dt = SQL_GetDatatable(strsql.ToString)

            AtualizaVisualizado()

            If dt.Rows.Count = 0 Then

                threadAtualiza.Sleep(5000)
                Continue Do
            End If


            Dim novasConversas As String = ""

            Dim strsqlUpdateAck As StringBuilder = New StringBuilder

            For Each chat As DataRow In dt.Rows

                strsqlUpdateAck.Append("update ")
                strsqlUpdateAck.Append("zap.msgzap ")
                strsqlUpdateAck.Append("set ")
                strsqlUpdateAck.Append("visto = true ")
                strsqlUpdateAck.Append("where ")
                strsqlUpdateAck.Append($"id = {chat("id")};")


                Dim mensagem As String = chat("databody")
                Dim horas As DateTime = chat("datatime")
                Dim dataFrom As String = chat("datafrom")
                Dim datamidia As String = chat("datamedia")
                Dim reaction As String = chat("reaction")
                Dim msgStatus As String = chat("dataack")
                Dim atendente As String = chat("atendente")

                Dim enviado As Boolean

                If dataFrom.Contains(telefoneNosso) Then
                    enviado = True
                Else
                    enviado = False
                End If

                mensagem = DecodeEncodedNonAsciiCharacters(mensagem)
                reaction = DecodeEncodedNonAsciiCharacters(reaction)

                If enviado Then

                Else
                    novasConversas += $"<div class=chat-l>
							<div class=mess>
"
                    If chat("datatype") = "image" Then
                        novasConversas += $"<a href='{datamidia}' class='myImage'><img width='65px' src='data:image/png;base64,{image}'></a>"
                    ElseIf chat("datatype") = "ptt" OrElse chat("datatype") = "audio" Then
                        novasConversas += $"<a href='{datamidia}' class='document'><img width='50px' src='data:image/png;base64,{audioZap}'></a>"
                    ElseIf chat("datatype") = "video" Then
                        novasConversas += $"<a href='{datamidia}' class='document'><img width='50px' src='data:image/png;base64,{videoZap}'></a>"
                    ElseIf chat("datatype") = "sticker" Then
                        novasConversas += $"<a href='{datamidia}' class='document'><img width='50px' src='data:image/png;base64,{stickerZap}'></a>"
                    ElseIf chat("datatype") = "document" Then
                        novasConversas += $"<a href='{datamidia}' class='document'><img width='50px' src='data:image/png;base64,{document}'></a>"
                    End If
                    novasConversas += $"<p>{mensagem}</p>
				<div class=check>
							<span>{horas}</span>
                        </div>
                   <div class='mess-reaction'><span class='reaction-l'>{reaction}</span></div>
						</div>
					<div class=sp></div>
			    </div>"
                End If
                novasConversas = novasConversas.Trim()
            Next


            Dim stringJS = $"{novasConversas}"

            ' Seta no javascript ->
            Dim javascriptola As String = $"
        var node = document.querySelector('.chat-box')
        node.insertAdjacentHTML(""beforeend"", `{novasConversas}`)
        
        myImages = document.querySelectorAll('.myImage');
        AddListenerToImages()

        myDocuments = document.querySelectorAll('.document');
        AddListenerToDocuments()

        
        elements = document.querySelectorAll('.mess');

        // Get the last element
        lastElement = elements[elements.length - 1];

        lastElement.scrollIntoView();
  
		RemoveRepeatedMsg();

		colocaLinks()
		AddListenerAnchor()


        console.log(node)
        "
            GC.Collect()
            invokeExecuteScriptAsync(javascriptola)
            SQL_executeNonQuery(strsqlUpdateAck.ToString)
            threadAtualiza.Sleep(5000)
        Loop
    End Sub
    Function DecodeEncodedNonAsciiCharacters(value As String) As String
        Return Regex.Replace(value, "\\u(?<Value>[a-zA-Z0-9]{4})",
        Function(m)
            Return ChrW(Integer.Parse(m.Groups("Value").Value, System.Globalization.NumberStyles.HexNumber)).ToString()
        End Function)
    End Function
    Friend Async Sub invokeExecuteScriptAsync(javascriptola As String)
        Try
            BeginInvoke(Async Sub()
                            Await WebView21.ExecuteScriptAsync(javascriptola)
                        End Sub)
        Catch ex As Exception

        End Try
    End Sub
    Public Sub AtualizaVisualizado()
        Dim strsql As StringBuilder = New StringBuilder
        Dim dt As DataTable = New DataTable

        strsql.Append("select ")
        strsql.Append("id, ")
        strsql.Append("dataid,")
        strsql.Append("databody,")
        strsql.Append("datafromme,")
        strsql.Append("dataack,")
        strsql.Append("datatype ")
        strsql.Append("from ")
        strsql.Append("zap.msgzap ")
        strsql.Append("where ")
        strsql.Append("(datafrom like '55" + telefoneCLI + "%'")
        strsql.Append("or ")
        strsql.Append("datato like '55" + telefoneCLI + "%') ")
        strsql.Append("AND datafromme = true ")
        strsql.Append("order by id desc")

        dt = SQL_GetDatatable(strsql.ToString)


        Dim arrayChegou As String = "["
        Dim arrayVisualizou As String = "["

        For Each chat As DataRow In dt.Rows
            Dim sqlId = chat("id")
            Dim dataId = chat("dataid")
            Dim dataAck = chat("dataack")
            Dim dataType = chat("datatype")
            Dim fromMe As Boolean = chat("datafromme")


            If fromMe Then
                If dataAck <> "read" Then
                    arrayChegou += $"""{sqlId}"","
                Else
                    arrayVisualizou += $"""{sqlId}"","
                End If
            End If

        Next
        arrayChegou = arrayChegou.Substring(0, arrayChegou.Length - 1)
        arrayChegou += "]"

        arrayVisualizou = arrayVisualizou.Substring(0, arrayVisualizou.Length - 1)
        arrayVisualizou += "]"

        If arrayChegou.Equals("]") Then
            arrayChegou = "[]"
        End If

        If arrayVisualizou.Equals("]") Then
            arrayVisualizou = "[]"
        End If

        Dim javaScript As String = $"
            var idsChegou = 
{arrayChegou};
		
            var oneTick = ""data:image/png;base64,{unread}"";

            var idsVisualizado = {arrayVisualizou}
            var twoTick = ""data:image/png;base64,{read}"";
            "

        javaScript += "
            

                for (var i = 0; i < idsChegou.length; i++) {
                    var div = document.getElementById(idsChegou[i]);
                    var img = div.getElementsByClassName('checkImg')[0];
                    img.src = oneTick;
                }



                for (var i = 0; i < idsVisualizado.length; i++) {
                    var div = document.getElementById(idsVisualizado[i]);
                    var img = div.getElementsByClassName('checkImg')[0];
                    img.src = twoTick;
                }

            RemoveRepeatedMsg();
        "

        invokeExecuteScriptAsync(javaScript)

    End Sub

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles btnAtualizar.Click
        reload()
    End Sub


    Private Sub lstContatos_DoubleClick(sender As Object, e As EventArgs) Handles lstContatos.DoubleClick
        Dim contato As String = lstContatos.SelectedItem
        Dim contatoSplited = contato.Split(";")
        telefoneCLI = contatoSplited(1)
        contatoGlobal = contatoSplited(0)
        ajustaTamanhoDoWebView()
        reload()
    End Sub

    Private Sub btnSplit_Click(sender As Object, e As EventArgs) Handles btnSplit.Click
        Dim ctMenuStrip As ContextMenuStrip = New ContextMenuStrip

        ctMenuStrip.Items.Add("Doc", My.Resources.black_doc)
        ctMenuStrip.Items.Add("Img", My.Resources.image_zap)
        ctMenuStrip.Items.Add("Vid", My.Resources.video_zap)



        AddHandler ctMenuStrip.Items.Item(0).Click, AddressOf doc_click
        AddHandler ctMenuStrip.Items.Item(1).Click, AddressOf img_click
        AddHandler ctMenuStrip.Items.Item(2).Click, AddressOf vid_click


        Dim btnSender As Button = CType(sender, Button)
        Dim ptLowerLeft As Point = New Point(0, btnSender.Height)
        ptLowerLeft = btnSender.PointToScreen(ptLowerLeft)
        ctMenuStrip.Show(ptLowerLeft)
    End Sub

    Private Sub Button1_Click_1(sender As Object, e As EventArgs) Handles btnEnviar.Click
        Dim idLocal = SQL_escalar($"insert into zap.msgzap (event_type,dataack,visto, atendente, datamedia) values('message_create','server',true, '{ID_LoginUser}', '') returning id")

        If txtMsg.Text IsNot Nothing OrElse isNULL(txtMsg.Text, "") <> "" Then
            txtMsg.Text = Trim(txtMsg.Text)
            Dim mensagem = txtMsg.Text
            mensagem = System.Net.WebUtility.UrlEncode(mensagem)
            Dim ret As StreamReader = SendMessage(telefoneCLI, mensagem, idLocal)
            Dim retMessage = ret.ReadToEnd()

            If Not retMessage.Contains("ok") Then
                MessageBox.Show(retMessage)
            Else
                Dim novaConversa As String = ""


                novaConversa += $"<div class='chat-r' id='{idLocal}'>
					<div class=sp></div>
					<div class=""mess mess-r"">"
                'novaConversa += $"<h2><span class='atendente'>{ID_LoginUser}:</span></h2>"

                novaConversa += $"<p>{txtMsg.Text}</p>
                <div Class='check'> 
							<span>{Date.Now}</span>"
                novaConversa += $"<img class='checkImg' src='data:image/png;base64,{relogin_zap}'>"
                novaConversa += $"</div>
                   <div class='mess-reaction'><span class='reaction-r'></span></div>
					</div>
				</div>
"
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
			
			colocaLinks()
			AddListenerAnchor()

            "
                invokeExecuteScriptAsync(javascriptola)
            End If
            txtMsg.Text = ""
        End If

    End Sub

    Private Sub btnAudio_Click(sender As Object, e As EventArgs) Handles btnAudio.Click
        Dim frmAudio As frmZapAudio = New frmZapAudio
        frmAudio.instanceid = instanceId
        frmAudio.numerocli = telefoneCLI
        frmAudio.token = token
        frmAudio.meuPai = Me
        frmAudio.ShowDialog()
    End Sub
    Private Sub btnNovoContato_Click(sender As Object, e As EventArgs) Handles btnNovoContato.Click
        Dim form As frmZapNovoContato = New frmZapNovoContato()
        form.meuPai = Me
        form.editar = False
        form.Text = "Criar Novo Contato."
        form.ShowDialog()
    End Sub

    Private Sub btnEditarContato_Click(sender As Object, e As EventArgs) Handles btnEditarContato.Click
        Dim form As frmZapNovoContato = New frmZapNovoContato()
        form.editar = True
        form.nome = isNULL(SQL_escalar($"select nomecli from zap.zap_contato where numerocli = '{telefoneCLI}'"), "")
        form.telefone = telefoneCLI
        form.Text = "Editar Contato."
        form.meuPai = Me
        form.ShowDialog()
    End Sub

End Class
