pragma solidity >= 0.6.0;

interface IWorker {
    function work() external;
}

contract Worker is IWorker {
    uint counter;

    function work() external override {
        counter++;
    }
}

