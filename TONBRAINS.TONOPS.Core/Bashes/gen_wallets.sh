#!/bin/bash -eE

URL="https://github.com/tonlabs/ton-labs-contracts/raw/master/solidity/setcodemultisig"
rm -f SetcodeMultisigWallet.tvc
rm -f SetcodeMultisigWallet.abi.json
wget "$URL/SetcodeMultisigWallet.tvc"
wget "$URL/SetcodeMultisigWallet.abi.json"

ZERO="0000000000000000000000000000000000000000000000000000000000000000"

WALLETS_FILE=ValidatorWallets.fif

while IFS= read -r line; do
    if [ "$(echo "$line" | grep ^entry)" != "" ]; then
        PUBKEY=$(echo "$line" | cut -d ' ' -f 4)
        NAME=$(echo "$line" | cut -d ' ' -f 2)
        NAME="${NAME%\"}"
        NAME="${NAME#\"}"
        echo "{ \"public\": ${PUBKEY}, \"secret\": \"${ZERO}\" }" >deploy.keys.json
        cp SetcodeMultisigWallet.tvc "./${NAME}SetcodeMultisigWallet.tvc"
        ./tonos-cli genaddr "./${NAME}SetcodeMultisigWallet.tvc" SetcodeMultisigWallet.abi.json --setkey deploy.keys.json --wc -1 --save
    fi

done <"./${WALLETS_FILE}"

rm -f SetcodeMultisigWallet.tvc
rm -f SetcodeMultisigWallet.abi.json
rm -f deploy.keys.json
