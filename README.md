## TodoList gRPC (Recruiting Edition)

## About This Fork

This repository is a streamlined fork of a student proof-of-concept originally created during a gRPC and ASP.NET learning exercise. The codebase has been cleaned up and kept intentionally small so that it can serve as a **neutral, well-defined backend** for technical interviews at Swisspension.

### Why We Use It

During our three-hour coding assessment each candidate is asked to build a simple front-end that consumes the gRPC API exposed by this project. The task helps us verify practical knowledge claimed on a résumé.

### Technology Snapshot

| Layer             | Tech                                        |
| ----------------- | ------------------------------------------- |
| Backend           | C#, .NET, ASP.NET Core, gRPC                |
| Front-end options | Any framework agreed **before** the session |
| Transport         | HTTP/2 (gRPC) or HTTP/1.1 (gRPC-Web)        |

### Your Mission

1. **Clone** the branch prepared for you.
2. **Generate client stubs** for your chosen language or framework by invoking `protoc` with the `protos/todo.proto` file. Detailed flags are intentionally omitted—consult the official docs for your stack.
3. Build a UI that can:
    - Display existing to-do items.
    - Add, update, and delete items through the gRPC service.
4. If you pick a client-side rendered framework, configure a **reverse proxy** so that your UI and the gRPC backend share the same origin. A pre-built _Caddyfile_ is provided to speed this up `caddy run --config Caddyfile`.

### Certificates & HTTPS

Self-signed certificates for `localhost` live under `cert/`. You may:

-   Import `localhost.pfx` into your browser or operating system trust store, or
-   Reference the `.crt` and `.key` directly from the reverse proxy.

**Note:** The backend is already configured to present this certificate; no extra work is required unless you decide to host the front-end over HTTPS as well.

### Branching Model

Every interviewee works on a dedicated branch named `candidate/<your-name>` that is created before the session starts. Feel free to commit as often as you like—history is part of the evaluation.

### Project Conventions

-   **Commit Messages:** Write each commit message as an English sentence in the past tense (single sentence).
-   **Comments and Variable Naming:** Use English for all code comments and variable names.
-   **Framework and Language Naming Conventions:** Follow the official naming conventions and best practices for the programming language(s) and framework(s) you use in your frontend implementation. This helps maintain readability, consistency, and professionalism in the codebase.

### What We Evaluate

-   Understanding of your chosen front-end stack
-   Ability to read protocol buffers and integrate gRPC or gRPC-Web
-   Code clarity, project structure, and commit hygiene
-   Problem-solving speed

## Prerequisites & Quick Setup

### Backend (.NET)

```sh
# Install .NET 8 SDK for the gRPC backend
curl -sSL https://dot.net/v1/dotnet-install.sh | bash -s -- --channel 8.0 --install-dir ~/.dotnet

# Add .NET tools and SDK path to environment variables
cat <<EOF >> ~/.bashrc

export DOTNET_ROOT="\$HOME/.dotnet"
export PATH="\$HOME/.dotnet:\$HOME/.dotnet/tools:\$PATH"
EOF

# Start the backend server in TodoServer/
dotnet run
```

### CLI Client (.NET)

```sh
# Run inside TodoCliClient/
dotnet run
```

### C++ Client (`todo_qt_client/`)

```sh
# Install build-essential C++ tools and Python for Conan
sudo apt install build-essential cmake python3-pip python3-jinja2

# For isolated Python CLI tools
pip3 install --user pipx
pipx ensurepath

# Install Conan using pipx
pipx install conan

# Make sure your Conan profile (~/.conan2/profiles/default) specifies:
# compiler=gcc (or clang), compiler.cppstd=20, and compiler.version matches your system

# Install C++ dependencies for both debug and release (or pass build type, e.g. build-debug)
./conan_install.sh

# Generate C++ gRPC stubs
./generate_grpc_stubs.sh

# Build (Debug)
cmake --build build-debug
# Or use the deployment script
./publish.sh
```

### Python Flask Client (`todo_flask_client/`)

```sh
# Python virtualenv and pipenv
sudo apt install python3-pip python3-venv

# Setup virtual environment and dependencies
pipenv install
pipenv shell
pipenv sync

# Generate Python gRPC stubs
./generate_grpc_stubs.sh

# Start Flask app
flask run
```

### Vue Client (`todo-vue-client/`)

```sh
# Install NVM for proper Node version handling
curl -o- https://raw.githubusercontent.com/nvm-sh/nvm/v0.39.0/install.sh | bash
source ~/.bashrc

# Install Node.js and Yarn
nvm install 22.17.0
npm install -g yarn

# If you are developing the Vue client, install Caddy for reverse proxy/same-origin dev
sudo apt install caddy
caddy run --config Caddyfile

cd todo-vue-client
yarn install

# Generate gRPC-Web stubs (required before dev/build)
yarn generate:grpc-stubs

# Start dev server or build for production
yarn dev
yarn build
```
