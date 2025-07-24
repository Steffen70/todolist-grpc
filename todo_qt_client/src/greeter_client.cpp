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

    channel_ = grpc::CreateChannel(target, grpc::SslCredentials(ssl_opts));
    stub_ = swisspension::Greeter::NewStub(channel_);
}

std::string GreeterClient::sayHello(const std::string& name) const
{
    swisspension::HelloRequest req;
    req.set_name(name);

    swisspension::HelloReply rep;
    grpc::ClientContext ctx;

    if (const grpc::Status st = stub_->SayHello(&ctx, req, &rep); !st.ok())
        throw std::runtime_error("RPC failed: " + st.error_message());

    return rep.message();
}
