set webdev="%programfiles%\Common Files\microsoft shared\DevServer\9.0\WebDev.WebServer"
start http://localhost:8085/Portal
%webdev% /port:8085 /path:"%cd%" /vpath:"/Portal"
