Imports System.Data
Imports cmd = Microsoft.VisualBasic
Imports System.Globalization
Imports System.Net.Mail
Imports System.Text
Imports System.IO
Imports System.Threading
Imports System.Data.SqlClient
Imports System.IO.Compression
Imports Ionic.Zip
Public Class ClassLibrarySendMail
#Region "Send mail"

    Private Function RenderDataTableToHtml(ByVal dsInfo As DataSet) As String
        Dim tableStr As StringBuilder = New StringBuilder()

        For M As Integer = 0 To dsInfo.Tables.Count - 1

            If dsInfo.Tables(M).Rows IsNot Nothing AndAlso dsInfo.Tables(M).Rows.Count > 0 Then
                Dim columnsQty As Integer = dsInfo.Tables(M).Columns.Count
                Dim rowsQty As Integer = dsInfo.Tables(M).Rows.Count
                tableStr.Append("<TABLE BORDER=""1"">")
                tableStr.Append("<TR bgcolor=""#FF0000"">")

                For j As Integer = 0 To columnsQty - 1
                    tableStr.Append("<TH>" & dsInfo.Tables(M).Columns(j).ColumnName & "</TH>")
                Next

                tableStr.Append("</TR>")

                For i As Integer = 0 To rowsQty - 1
                    tableStr.Append("<TR>")

                    For k As Integer = 0 To columnsQty - 1
                        tableStr.Append("<TD>")
                        tableStr.Append(dsInfo.Tables(M).Rows(i)(k).ToString())
                        tableStr.Append("</TD>")
                    Next

                    tableStr.Append("</TR>")
                Next

                tableStr.Append("</TABLE>")
                tableStr.Append("<BR>")
                'tableStr.Append("<BR>")
            End If

        Next

        Return tableStr.ToString()
    End Function
    Private Sub DeleteDirectory(ByVal path As String, bDelete As Boolean)
        Try
            If bDelete Then
                If Directory.Exists(path) Then
                    'Delete all files from the Directory
                    For Each filepath As String In Directory.GetFiles(path)
                        File.Delete(filepath)
                    Next
                End If
            End If
        Catch ex As Exception
            Throw
        End Try

    End Sub

    Private Sub MoveFile(ByVal path As String, ByVal sPath_BAK As String)
        Try
            sPath_BAK = sPath_BAK + "\" + DateTime.Now.ToString("yyyyMMdd")
            Dim exists As Boolean = System.IO.Directory.Exists(sPath_BAK)
            If exists = False Then
                System.IO.Directory.CreateDirectory(sPath_BAK)
            End If
            If Directory.Exists(path) Then
                For Each filepath As String In Directory.GetFiles(path)
                    'File.Delete(filepath)
                    Dim result As String
                    result = System.IO.Path.GetFileName(filepath)
                    System.IO.File.Move(filepath, sPath_BAK + "\" + result)
                    File.Delete(filepath)
                Next
            End If

        Catch ex As Exception
            Throw
        End Try

    End Sub
    Public Function Sendmail(MailTo As String, smtp As String, sDetail As String, MailFrom As String, sSubjectmail As String, ByVal ds_detail As DataSet) As Boolean

        'Dim strSubject As String = My.Settings.MailSubject
        Dim strBody As String
        Dim strSendTo As String = Trim(MailTo)
        Dim smtpClient As New SmtpClient(Trim(smtp))
        'Dim attachment As System.Net.Mail.Attachment
        'SaleDate

        Try
            strBody = sDetail
            Dim mailMsg As New System.Net.Mail.MailMessage '(Trim(sendfrom), Trim(strSendTo)) 
            Dim Mailfroms As MailAddress = New MailAddress(Trim(MailFrom))


            Dim EmailTo As String = ""
            Dim pos As Integer = InStr(Trim(strSendTo), ";")
            Dim ToMailAddress As System.Net.Mail.MailAddress
            If pos <> 0 Then
                Do Until pos = 0
                    EmailTo = cmd.Left(Trim(strSendTo), pos - 1)
                    ToMailAddress = New System.Net.Mail.MailAddress(Trim(EmailTo), "")
                    mailMsg.To.Add(ToMailAddress)
                    strSendTo = cmd.Right(Trim(strSendTo), Len(Trim(strSendTo)) - pos)
                    pos = InStr(Trim(strSendTo), ";")
                Loop
                ToMailAddress = New System.Net.Mail.MailAddress(Trim(strSendTo), "")
                mailMsg.To.Add(ToMailAddress)
            Else
                mailMsg = New System.Net.Mail.MailMessage(Trim(MailFrom), Trim(strSendTo))
            End If
            '---------------

            Try
                If ds_detail.Tables.Count <> 0 Then
                    strBody = RenderDataTableToHtml(ds_detail)
                End If

                With mailMsg
                    .Subject = Trim(sSubjectmail)
                    .From = Mailfroms
                    .IsBodyHtml = True
                    .Body = strBody
                    .Priority = Net.Mail.MailPriority.Normal


                End With
                smtpClient.Send(mailMsg)  'send mail

                Return True
            Catch ex As Exception
                'MessageBox.Show("mail error" & ex.ToString)
                Return False
            Finally
                mailMsg.Dispose()
            End Try
        Catch ex As Exception
            'MessageBox.Show(ex.ToString)
            Return False
        Finally

        End Try

    End Function

    Public Function Sendmail(MailTo As String, smtp As String, sDetail As String, MailFrom As String, sSubjectmail As String, bAttachmentFile As Boolean, DirectoryToZip As String, ZipToCreate As String, ByVal ds_detail As DataSet) As Boolean

        'Dim strSubject As String = My.Settings.MailSubject
        Dim strBody As String
        Dim strSendTo As String = Trim(MailTo)
        Dim smtpClient As New SmtpClient(Trim(smtp))
        Dim attachment As System.Net.Mail.Attachment
        'SaleDate

        Try
            strBody = sDetail
            Dim mailMsg As New System.Net.Mail.MailMessage '(Trim(sendfrom), Trim(strSendTo)) 
            Dim Mailfroms As MailAddress = New MailAddress(Trim(MailFrom))


            Dim EmailTo As String = ""
            Dim pos As Integer = InStr(Trim(strSendTo), ";")
            Dim ToMailAddress As System.Net.Mail.MailAddress
            If pos <> 0 Then
                Do Until pos = 0
                    EmailTo = cmd.Left(Trim(strSendTo), pos - 1)
                    ToMailAddress = New System.Net.Mail.MailAddress(Trim(EmailTo), "")
                    mailMsg.To.Add(ToMailAddress)
                    strSendTo = cmd.Right(Trim(strSendTo), Len(Trim(strSendTo)) - pos)
                    pos = InStr(Trim(strSendTo), ";")
                Loop
                ToMailAddress = New System.Net.Mail.MailAddress(Trim(strSendTo), "")
                mailMsg.To.Add(ToMailAddress)
            Else
                mailMsg = New System.Net.Mail.MailMessage(Trim(MailFrom), Trim(strSendTo))
            End If
            '---------------

            Try
                If ds_detail.Tables.Count <> 0 Then
                    strBody = RenderDataTableToHtml(ds_detail)
                End If
                With mailMsg
                    .Subject = Trim(sSubjectmail)
                    .From = Mailfroms
                    .IsBodyHtml = True
                    .Body = strBody
                    'If strFile <> "" Then
                    '    Dim AttachFile As New Attachment(Trim(strFile))
                    '    .Attachments.Add(AttachFile)
                    'End If
                    .Priority = Net.Mail.MailPriority.Normal

                    If bAttachmentFile Then
                        ZipToCreate = DirectoryToZip & "\" & ZipToCreate
                        Using zip As ZipFile = New ZipFile
                            zip.AddDirectory(DirectoryToZip)
                            zip.Save(ZipToCreate)
                        End Using

                        If File.Exists(ZipToCreate) Then
                            '.Attachments.Add(sAttachmentFile)
                            attachment = New System.Net.Mail.Attachment(ZipToCreate, "application/zip")

                            .Attachments.Add(attachment) 'attachment
                        End If

                    End If

                End With
                smtpClient.Send(mailMsg)  'send mail

                Return True
            Catch ex As Exception
                'MessageBox.Show("mail error" & ex.ToString)
                Return False
            Finally

                mailMsg.Dispose()
                'DeleteDirectory(DirectoryToZip, True)
                File.Delete(ZipToCreate)
            End Try
        Catch ex As Exception
            'MessageBox.Show(ex.ToString)
            Return False
        Finally

        End Try

    End Function

    Public Function Sendmail(MailTo As String, smtp As String, sDetail As String, MailFrom As String, sSubjectmail As String, bAttachmentFile As Boolean, DirectoryToZip As String, ZipToCreate As String, bDeleteFile As Boolean, ByVal sPath_BAK As String, ByVal ds_detail As DataSet) As Boolean

        'Dim strSubject As String = My.Settings.MailSubject
        Dim strBody As String
        Dim strSendTo As String = Trim(MailTo)
        Dim smtpClient As New SmtpClient(Trim(smtp))
        Dim attachment As System.Net.Mail.Attachment
        'SaleDate

        Try
            strBody = sDetail
            Dim mailMsg As New System.Net.Mail.MailMessage '(Trim(sendfrom), Trim(strSendTo)) 
            Dim Mailfroms As MailAddress = New MailAddress(Trim(MailFrom))


            Dim EmailTo As String = ""
            Dim pos As Integer = InStr(Trim(strSendTo), ";")
            Dim ToMailAddress As System.Net.Mail.MailAddress
            If pos <> 0 Then
                Do Until pos = 0
                    EmailTo = cmd.Left(Trim(strSendTo), pos - 1)
                    ToMailAddress = New System.Net.Mail.MailAddress(Trim(EmailTo), "")
                    mailMsg.To.Add(ToMailAddress)
                    strSendTo = cmd.Right(Trim(strSendTo), Len(Trim(strSendTo)) - pos)
                    pos = InStr(Trim(strSendTo), ";")
                Loop
                ToMailAddress = New System.Net.Mail.MailAddress(Trim(strSendTo), "")
                mailMsg.To.Add(ToMailAddress)
            Else
                mailMsg = New System.Net.Mail.MailMessage(Trim(MailFrom), Trim(strSendTo))
            End If
            '---------------

            Try
                If bAttachmentFile = False Then
                    If ds_detail.Tables.Count <> 0 Then
                        strBody = RenderDataTableToHtml(ds_detail)
                    End If
                End If
                With mailMsg
                    .Subject = Trim(sSubjectmail)
                    .From = Mailfroms
                    .IsBodyHtml = True
                    .Body = strBody
                    'If strFile <> "" Then
                    '    Dim AttachFile As New Attachment(Trim(strFile))
                    '    .Attachments.Add(AttachFile)
                    'End If
                    .Priority = Net.Mail.MailPriority.Normal

                    If bAttachmentFile Then
                        ZipToCreate = DirectoryToZip & "\" & ZipToCreate
                        Using zip As ZipFile = New ZipFile
                            zip.AddDirectory(DirectoryToZip)
                            zip.Save(ZipToCreate)
                        End Using

                        If File.Exists(ZipToCreate) Then
                            '.Attachments.Add(sAttachmentFile)
                            attachment = New System.Net.Mail.Attachment(ZipToCreate, "application/zip")

                            .Attachments.Add(attachment) 'attachment
                        End If

                    End If

                End With
                smtpClient.Send(mailMsg)  'send mail

                Return True
            Catch ex As Exception
                'MessageBox.Show("mail error" & ex.ToString)
                Return False
            Finally
                mailMsg.Dispose()
                If bDeleteFile Then
                    DeleteDirectory(DirectoryToZip, bDeleteFile)
                Else
                    MoveFile(DirectoryToZip, sPath_BAK)
                End If
            End Try
        Catch ex As Exception
            'MessageBox.Show(ex.ToString)
            Return False
        Finally

        End Try

    End Function

    Public Function Sendmail(MailTo As String, smtp As String, sDetail As String, MailFrom As String, sSubjectmail As String) As Boolean

        'Dim strSubject As String = My.Settings.MailSubject
        Dim strBody As String
        Dim strSendTo As String = Trim(MailTo)
        Dim smtpClient As New SmtpClient(Trim(smtp))
        'SaleDate

        Try
            strBody = sDetail
            Dim mailMsg As New System.Net.Mail.MailMessage '(Trim(sendfrom), Trim(strSendTo)) 
            Dim Mailfroms As MailAddress = New MailAddress(Trim(MailFrom))


            Dim EmailTo As String = ""
            Dim pos As Integer = InStr(Trim(strSendTo), ";")
            Dim ToMailAddress As System.Net.Mail.MailAddress
            If pos <> 0 Then
                Do Until pos = 0
                    EmailTo = cmd.Left(Trim(strSendTo), pos - 1)
                    ToMailAddress = New System.Net.Mail.MailAddress(Trim(EmailTo), "")
                    mailMsg.To.Add(ToMailAddress)
                    strSendTo = cmd.Right(Trim(strSendTo), Len(Trim(strSendTo)) - pos)
                    pos = InStr(Trim(strSendTo), ";")
                Loop
                ToMailAddress = New System.Net.Mail.MailAddress(Trim(strSendTo), "")
                mailMsg.To.Add(ToMailAddress)
            Else
                mailMsg = New System.Net.Mail.MailMessage(Trim(MailFrom), Trim(strSendTo))
            End If
            '---------------

            Try

                With mailMsg
                    .Subject = Trim(sSubjectmail)
                    .From = Mailfroms
                    .IsBodyHtml = True
                    .Body = strBody
                    'If strFile <> "" Then
                    '    Dim AttachFile As New Attachment(Trim(strFile))
                    '    .Attachments.Add(AttachFile)
                    'End If
                    .Priority = Net.Mail.MailPriority.Normal

                End With
                smtpClient.Send(mailMsg)  'send mail

                Return True
            Catch ex As Exception
                'MessageBox.Show("mail error" & ex.ToString)
                Return False
            Finally
                mailMsg.Dispose()
            End Try
        Catch ex As Exception
            'MessageBox.Show(ex.ToString)
            Return False
        Finally

        End Try

    End Function

    Public Function Sendmail_Bak(strSubject As String, MailTo As String, smtp As String, sDetail As String, MailFrom As String, sSubjectmail As String, bAttachmentFile As Boolean, DirectoryToZip As String, ZipToCreate As String, bDeleteFile As Boolean) As Boolean

        'Dim strSubject As String = My.Settings.MailSubject
        Dim strBody As String
        Dim strSendTo As String = Trim(MailTo)
        Dim smtpClient As New SmtpClient(Trim(smtp))
        Dim attachment As System.Net.Mail.Attachment
        'SaleDate

        Try
            strBody = "<table>" 'Header
            strBody += "<tr style='background-color: lightsteelblue'>"
            strBody += "<td align='center' style='width: 100px; height: 21px'>"
            strBody += "Subject</td>"
            strBody += "<td align='center' style='width: 450px; height: 21px'>"
            strBody += "Detail</td>"
            strBody += "</tr>"
            strBody += "<tr>" 'Detail 
            strBody += "<td align='center' style='width: 100px; height: 21px'>"
            'strBody += "Sale Event</td>"
            strBody += Trim(strSubject) & "</td>"
            strBody += "<td align='center' style='width: 450px; height: 21px'>"
            strBody += Trim(sDetail) & "</td>"
            strBody += "</tr>"
            strBody += "</table>"

            Dim mailMsg As New System.Net.Mail.MailMessage '(Trim(sendfrom), Trim(strSendTo)) 
            Dim Mailfroms As MailAddress = New MailAddress(Trim(MailFrom))


            Dim EmailTo As String = ""
            Dim pos As Integer = InStr(Trim(strSendTo), ";")
            Dim ToMailAddress As System.Net.Mail.MailAddress
            If pos <> 0 Then
                Do Until pos = 0
                    EmailTo = cmd.Left(Trim(strSendTo), pos - 1)
                    ToMailAddress = New System.Net.Mail.MailAddress(Trim(EmailTo), "")
                    mailMsg.To.Add(ToMailAddress)
                    strSendTo = cmd.Right(Trim(strSendTo), Len(Trim(strSendTo)) - pos)
                    pos = InStr(Trim(strSendTo), ";")
                Loop
                ToMailAddress = New System.Net.Mail.MailAddress(Trim(strSendTo), "")
                mailMsg.To.Add(ToMailAddress)
            Else
                mailMsg = New System.Net.Mail.MailMessage(Trim(MailFrom), Trim(strSendTo))
            End If
            '---------------

            Try

                With mailMsg
                    .Subject = Trim(sSubjectmail)
                    .From = Mailfroms
                    .IsBodyHtml = True
                    .Body = strBody
                    'If strFile <> "" Then
                    '    Dim AttachFile As New Attachment(Trim(strFile))
                    '    .Attachments.Add(AttachFile)
                    'End If
                    .Priority = Net.Mail.MailPriority.Normal

                    If bAttachmentFile Then
                        ZipToCreate = DirectoryToZip & "\" & ZipToCreate
                        Using zip As ZipFile = New ZipFile
                            zip.AddDirectory(DirectoryToZip)
                            zip.Save(ZipToCreate)
                        End Using

                        If File.Exists(ZipToCreate) Then
                            '.Attachments.Add(sAttachmentFile)
                            attachment = New System.Net.Mail.Attachment(ZipToCreate, "application/zip")
                            .Attachments.Add(attachment) 'attachment
                        End If

                    End If

                End With
                smtpClient.Send(mailMsg)  'send mail

                Return True
            Catch ex As Exception
                'MessageBox.Show("mail error" & ex.ToString)
                Return False
            Finally
                mailMsg.Dispose()
                If bDeleteFile Then
                    DeleteDirectory(DirectoryToZip, bDeleteFile)
                End If
            End Try
        Catch ex As Exception
            'MessageBox.Show(ex.ToString)
            Return False
        Finally

        End Try

    End Function
#End Region
End Class
