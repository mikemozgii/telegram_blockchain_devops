#!/bin/bash
sudo apt update
apt -y upgrade
apt -y install openjdk-8-jdk
#java -version
wget -qO - https://artifacts.elastic.co/GPG-KEY-elasticsearch --no-check-certificate | sudo apt-key add -
echo "deb https://artifacts.elastic.co/packages/7.x/apt stable main" | sudo tee -a /etc/apt/sources.list.d/elastic-7.x.list
apt update
apt install elasticsearch
sed -i 's/#network.host: 192.168.0.1/network.host: 0.0.0.0/' /etc/elasticsearch/elasticsearch.yml
#was not accesable after install
#not sure about adding discovery.type: single-node
#not sure about uncomment port 9200 it's default'

systemctl restart elasticsearch.service
systemctl enable elasticsearch.service

apt -y install kibana
sed -i 's/#server.host: "localhost"/server.host: "0.0.0.0"/' /etc/kibana/kibana.yml
sed -i 's/#server.name: "your-hostname"/server.name: "Kibana Server"/' /etc/kibana/kibana.yml
systemctl enable kibana
systemctl start kibana