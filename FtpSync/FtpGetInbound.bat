@Echo off
Echo =====
Echo Batch start on %DATE% %TIME%
cd\
cd FtpSync
winscp.exe  /script=ScriptTransferPKT.txt  /log=C:\FtpSync\PKTFTP.log
if %ERRORLEVEL% neq 0 goto error
 
exit /b 0
 
:error
echo Upload failed, keeping local files
Echo End of Batch
Echo =====
exit /b 1
