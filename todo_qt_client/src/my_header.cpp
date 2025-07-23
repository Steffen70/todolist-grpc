#include "todo_qt_client/my_header.hpp"
#include <fmt/core.h>

Test::Test(const std::string& name, const char& resourceA, const char& resourceB): name_{name}, resourceA_{resourceA}, resourceB_{resourceB}
{
    fmt::print("Constructing Test: {}\n", name_);
}

Test::~Test() {
    fmt::print("Destructing Test: {}\n", name_);
}

void Test::say_hello() const {
    fmt::print("Hello from Test ({})! ResourceA={} ResourceB={}\n", name_, resourceA_, resourceB_);
}
