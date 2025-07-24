#include <todo_qt_client/greeter_client.hpp>
#include <fstream>
#include <sstream>
#include <stdexcept>

namespace
{
    std::string readFile(const std::string& path)
    {
        std::ifstream in(path, std::ios::binary);
        if (!in) throw std::runtime_error("Cannot open " + path);
        std::ostringstream oss;
        oss << in.rdbuf();
        return oss.str();
    }
}

GreeterClient::GreeterClient(const std::string& target, const std::string& root_ca_path)
{
    grpc::SslCredentialsOptions ssl_opts;
    ssl_opts.pem_root_certs = readFile(root_ca_path);

    _channel = grpc::CreateChannel(target, grpc::SslCredentials(ssl_opts));
    _stub = swisspension::Greeter::NewStub(_channel);
}

std::string GreeterClient::sayHello(const std::string& name)
{
    swisspension::HelloRequest req;
    req.set_name(name);

    swisspension::HelloReply rep;
    grpc::ClientContext ctx;

    const grpc::Status st = _stub->SayHello(&ctx, req, &rep);
    if (!st.ok())
        throw std::runtime_error("RPC failed: " + st.error_message());

    return rep.message();
}
