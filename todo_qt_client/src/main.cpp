#include "todo_qt_client/my_header.hpp"

int main() {
    // stack allocation, automatic cleanup
    std::string s = "StackObject";
    Test stackTest(s, 'A', 'B');
    stackTest.say_hello();
    s = "MyStackObject";
    stackTest.say_hello();

    auto* heapTest = new Test("HeapObject", 'C', 'D');
    heapTest->say_hello();
    
    // manual cleanup
    delete heapTest;
    return 0;
}
