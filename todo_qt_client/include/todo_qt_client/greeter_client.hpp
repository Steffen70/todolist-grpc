#pragma once
#include <grpcpp/grpcpp.h>
#include <todo_qt_client/greet.grpc.pb.h>
#include <string>
#include <memory>

class GreeterClient
{
public:
    // Construct with host:port and path to root CA file
    GreeterClient(const std::string& target,
                  const std::string& root_ca_path);
    // RAII ⇒ automatic clean-up
    ~GreeterClient() = default;

    // Non-copyable, movable
    GreeterClient(const GreeterClient&) = delete;
    GreeterClient& operator=(const GreeterClient&) = delete;
    GreeterClient(GreeterClient&&) = default;
    GreeterClient& operator=(GreeterClient&&) = default;

    // Perform the RPC and return the reply or throw on error
    std::string sayHello(const std::string& name);

private:
    std::shared_ptr<grpc::Channel> _channel;
    std::unique_ptr<swisspension::Greeter::Stub> _stub;
};
