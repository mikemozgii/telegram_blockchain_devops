#!/bin/bash
sed -i "s/Hostname=/Hostname=${host_name}\n#/g" /etc/zabbix/zabbix_agentd.conf

systemctl restart zabbix-agent