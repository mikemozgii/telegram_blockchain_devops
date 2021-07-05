#!/bin/bash
#apt update
#apt -y upgrade
#apt -y install ntp 
#systemctl enable ntp
#systemctl start ntp
# no need in ntp on ubuntu
#http://www.geoffstratton.com/expand-hard-disk-ubuntu-lvm
timedatectl set-timezone UTC
timedatectl set-ntp on

