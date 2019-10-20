firewall-cmd --zone=public --remove-port=5050/tcp --permanent

systemctl stop    mystique.service
systemctl disable mystique.service

systemctl restart rsyslog

rm -rf /var/www/mystique

