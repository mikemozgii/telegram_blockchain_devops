#!/bin/bash
apt update
apt -y upgrade
apt -y install postgresql postgresql-contrib
sed -i "s/host    all             all             127.0.0.1\\/32/host    all             all             0.0.0.0\\/0/g" /etc/postgresql/13/main/pg_hba.conf
sed -i "s/#listen_addresses = 'localhost'/listen_addresses = '*'/g" /etc/postgresql/13/main/postgresql.conf
su - postgres -c "psql -U postgres -d postgres -c \"alter user postgres with password 'temp3232';\""
systemctl start postgresql
systemctl enable postgresql

#install PostGis
apt -y install gnupg2
wget --quiet -O - https://www.postgresql.org/media/keys/ACCC4CF8.asc | sudo apt-key add -
echo "deb http://apt.postgresql.org/pub/repos/apt/ `lsb_release -cs`-pgdg main" | sudo tee /etc/apt/sources.list.d/pgdg.list
apt -y install postgis postgresql-12-postgis-3

systemctl restart postgresql


#https://computingforgeeks.com/how-to-install-postgresql-13-on-ubuntu/
