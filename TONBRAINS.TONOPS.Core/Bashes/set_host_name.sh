#!/bin/bash

#https://linuxize.com/post/how-to-change-hostname-on-ubuntu-18-04/
#https://linuxize.com/post/how-to-change-hostname-on-ubuntu-20-04/
sudo hostnamectl set-hostname ${new_host_name}
sudo sed -i "s/127.0.1.1 $HOSTNAME/127.0.1.1 ${new_host_name}/g" /etc/hosts
echo "${new_host_name}" > /etc/hostname
