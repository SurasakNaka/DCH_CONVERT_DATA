move C:\owh_itf\dropbox\Useroutbox\*.txt C:\owh_itf\inbound
timeout /t 60
move C:\OWH_ITF\INBOUND\*.txt C:\OWH_ITF\INBOUND\Transfer
timeout /t 60
cd\
cd ftpsync
ScpSync SendPKTToSAP
