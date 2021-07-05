
#!/bin/bash
# nano /etc/netplan/00-installer-config.yaml
configfile=$(ls /etc/netplan | grep .yaml | head -1)
echo "network:
  version: 2
  ethernets:
    eth0:
      addresses: [${ip_address}/24]
      gateway4: ${gateway_ip_address}
      nameservers:
        addresses: [${dns_ip_address_1}, ${dns_ip_address_2}]" > "/etc/netplan/$configfile"
netplan apply
reboot