pragma solidity >= 0.6.0;
pragma AbiHeader expire;

import {IWorker} from "Worker.sol";

contract Caller {
    function call(address addr, uint128 tons) public pure {
        tvm.accept();
        IWorker(addr).work{value: tons * 1 ton, bounce: true}();
    }
}