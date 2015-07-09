#!/bin/sh

yum install -y gcc gcc-c++ gettext zlib-devel

wget http://origin-download.mono-project.com/sources/mono/mono-4.0.2.5.tar.bz2
tar -jxf mono-4.0.2.5.tar.bz2
cd mono-4.0.2
./configure --prefix=/usr
make
make install

yum install -y libpcap-devel
