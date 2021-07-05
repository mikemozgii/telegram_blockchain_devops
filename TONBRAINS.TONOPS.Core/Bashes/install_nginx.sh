#!/bin/bash
apt update
apt install nginx -y
systemctl start nginx
systemctl enable nginx
sudo ufw allow ‘NGINX Full’