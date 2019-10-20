./dotnet publish -r linux-x64 -o /var/www/mystique

cd /etc/systemd/system

echo "[Unit]
Description=Mystique

[Service]
WorkingDirectory=/var/www/mystique
ExecStart=/var/www/mystique/Mystique --urls=http://*:5050
Restart=always
RestartSec=12
KillSignal=SIGINT
StandardOutput=syslog
StandardError=syslog
SyslogIdentifier=Mystique
User=root
Environment=ASPNETCORE_ENVIRONMENT=Production
Environment=DOTNET_PRINT_TELEMETRY_MESSAGE=false

[Install]
WantedBy=multi-user.target" >> mystique.service

systemctl restart rsyslog

systemctl enable mystique.service
systemctl start  mystique.service

firewall-cmd --zone=public --add-port=5050/tcp --permanent
firewall-cmd --reload

