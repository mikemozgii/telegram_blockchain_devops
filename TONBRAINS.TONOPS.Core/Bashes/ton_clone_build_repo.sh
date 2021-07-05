#!/bin/bash
#Clone
git clone --recursive "${TON_GITHUB_REPO}" "${TON_SRC_DIR}"
cd "${TON_SRC_DIR}"
git checkout "${TON_STABLE_GITHUB_COMMIT_ID}"
#Build
mkdir -p "${TON_BUILD_DIR}"   
cd "${TON_BUILD_DIR}"
cmake .. -G "Ninja" -DCMAKE_BUILD_TYPE=Release -DPORTABLE=ON
ninja



#git clone --recursive  "https://tonbrains@dev.azure.com/tonbrains/ton/_git/ton" "/root/ton"

#mkdir -p  cd "/root/ton/build"