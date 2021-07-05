#!/bin/bash
echo "Step 1: Install Java"
apt install default-jre -y
apt install default-jdk -y
echo "Step 2: Install ElasticSearch"
wget -qO - https://artifacts.elastic.co/GPG-KEY-elasticsearch | apt-key add -
echo "deb https://artifacts.elastic.co/packages/7.x/apt stable main" | tee -a /etc/apt/sources.list.d/elastic-7.x.list
apt update
apt install elasticsearch -y
sed -i 's/#network.host: 192.168.0.1/network.host: localhost/' /etc/elasticsearch/elasticsearch.yml
sed -i 's/-Xmslg/-Xms128m/' /etc/elasticsearch/jvm.options
sed -i 's/-Xmxlg/-Xmx128m/' /etc/elasticsearch/jvm.options
systemctl start elasticsearch
systemctl enable elasticsearch
echo "Step 3: Install Kibana"
apt install kibana -y
systemctl start kibana
systemctl enable kibana
echo "Step 4: Add rules ufw"
ufw allow 9200/tcp
ufw allow 5601/tcp
