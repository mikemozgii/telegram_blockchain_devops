#!/bin/bash
wget https://repo.zabbix.com/zabbix/5.0/ubuntu/pool/main/z/zabbix-release/zabbix-release_5.0-1+focal_all.deb
dpkg -i zabbix-release_5.0-1+focal_all.deb
apt update
apt install zabbix-agent
sed -i "s/Server=127.0.0.1/Server=0.0.0.0\/0/g" /etc/zabbix/zabbix_agentd.conf
sed -i "s/# Hostname=/Hostname=${host_name}/g" /etc/zabbix/zabbix_agentd.conf

systemctl restart zabbix-agent
systemctl enable zabbix-agent 
