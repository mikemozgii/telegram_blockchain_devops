#!/bin/bash
#https://bestmonitoringtools.com/how-to-install-zabbix-server-on-ubuntu/
wget https://repo.zabbix.com/zabbix/5.0/ubuntu/pool/main/z/zabbix-release/zabbix-release_5.0-1+focal_all.deb
dpkg -i zabbix-release_5.0-1+focal_all.deb
apt update
apt -y install zabbix-server-mysql zabbix-frontend-php zabbix-apache-conf zabbix-agent
apt -y install mariadb-common mariadb-server-10.3 mariadb-client-10.3

apt-get install -y expect

SECURE_MYSQL=$(expect -c "
set timeout 10
spawn mysql_secure_installation
expect \"Enter current password for root (enter for none):\"
send \"temp3232\r\"
expect \"Change the root password?\"
send \"y\r\"
expect \"New password:\"
send \"temp3232\r\"
expect \"Re-enter new password:\"
send \"temp3232\r\"
expect \"Remove anonymous users?\"
send \"y\r\"
expect \"Disallow root login remotely?\"
send \"y\r\"
expect \"Remove test database and access to it?\"
send \"y\r\"
expect \"Reload privilege tables now?\"
send \"y\r\"
expect eof
")
echo "$SECURE_MYSQL"

mysql -uroot -p'temp3232' -e "create database zabbix character set utf8 collate utf8_bin;"
mysql -uroot -p'temp3232' -e "grant all privileges on zabbix.* to zabbix@localhost identified by 'temp3232';"
mysql -uroot -p'temp3232' zabbix -e "set global innodb_strict_mode='OFF';"
zcat /usr/share/doc/zabbix-server-mysql*/create.sql.gz | mysql -uzabbix -p'temp3232' zabbix
mysql -uroot -p'temp3232' zabbix -e "set global innodb_strict_mode='ON';"
sed -i "s/# DBPassword=/DBPassword=temp3232/g" /etc/zabbix/zabbix_server.conf

sed -i "s/# php_value date.timezone/php_value date.timezone Etc\/UCT\n\t#/g" /etc/zabbix/apache.conf

systemctl restart zabbix-server zabbix-agent apache2
systemctl enable zabbix-server zabbix-agent apache2

