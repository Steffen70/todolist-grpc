#include <todo_qt_client/greeter_client.hpp>
#include <fmt/core.h>
#include <iostream>
#include <unistd.h>

std::string getExecutableDir()
{
    char buf[PATH_MAX];
    if (const ssize_t len = readlink("/proc/self/exe", buf, sizeof(buf) - 1); len != -1)
    {
        buf[len] = '\0';
        const std::string exePath(buf);
        const auto lastSlash = exePath.find_last_of('/');
        return exePath.substr(0, lastSlash);
    }
    throw std::runtime_error("Cannot determine executable path");
}

int main()
{
    try
    {
        const std::string binDir = getExecutableDir();
        const std::string caPath = binDir + "/root_ca.crt";
        const GreeterClient client("localhost:8445", caPath);
        const std::string reply = client.sayHello("Qt client");
        fmt::print("Server replied: {}\n", reply);
    }
    catch (const std::exception& ex)
    {
        std::cerr << "Error: " << ex.what() << '\n';
        return EXIT_FAILURE;
    }
}
