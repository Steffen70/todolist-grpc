#include <todo_qt_client/greeter_client.hpp>
#include <fmt/core.h>
#include <iostream>

int main()
{
    try
    {
        GreeterClient client("localhost:8445", "../cert/root_ca.crt");
        const std::string reply = client.sayHello("Qt client");
        fmt::print("Server replied: {}\n", reply);
    }
    catch (const std::exception& ex)
    {
        std::cerr << "Error: " << ex.what() << '\n';
        return EXIT_FAILURE;
    }
}
