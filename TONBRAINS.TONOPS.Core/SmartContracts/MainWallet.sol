pragma solidity >=0.6.0;
pragma AbiHeader time;
pragma AbiHeader expire;

abstract contract Upgradable {
    /*
     * Set code
     */

    function upgrade(TvmCell newcode) public virtual {
        require(msg.pubkey() == tvm.pubkey(), 100);
        tvm.accept();
        tvm.commit();
        tvm.setcode(newcode);
        tvm.setCurrentCode(newcode);
        onCodeUpgrade();
    }

    function onCodeUpgrade() internal virtual;
}

contract MainWallet is Upgradable {

    uint32 walletId = 0xffffffff;

    modifier owner {
        require(msg.pubkey() == tvm.pubkey(), 100);
        tvm.accept(); 
        _;
	}

    constructor() public owner {}
    
    /*
     * Publics
     */

    /// @notice Transfers grams to other contracts.
    function transfer(address dest, uint128 value, bool bounce, TvmCell payload) public view owner {
        dest.transfer(value, bounce, 3, payload);
    }

    function onCodeUpgrade() internal override {}
}