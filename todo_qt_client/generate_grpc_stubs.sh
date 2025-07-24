#!/usr/bin/env bash
set -e

DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
OUTDIR="$DIR/generated"
CONAN_LOCK="$DIR/conan.lock"

# Get recipe id from conan.lock
RECIPE_ID=$(jq -r '.requires[] | select(startswith("grpc/")) | split("%")[0]' "$CONAN_LOCK")
echo "Found recipe ID: $RECIPE_ID"

# Get first package id from conan list output
CONAN_LIST_OUTPUT=$(conan list "$RECIPE_ID:*" --cache 2>/dev/null)

# Extract the first package ID
PACKAGE_ID=$(echo "$CONAN_LIST_OUTPUT" | grep -E '^[[:space:]]*[a-f0-9]{40}[[:space:]]*$' | head -n1 | tr -d '[:space:]')
echo "Found package ID: $PACKAGE_ID"

# Run find_conan_package.py and get the package directory
FIND_SCRIPT="$DIR/find_conan_package.py"
PACKAGE_DIR=$(~/.local/share/pipx/venvs/conan/bin/python "$FIND_SCRIPT" "$RECIPE_ID" "$PACKAGE_ID")

echo "Using package directory: $PACKAGE_DIR"

# Search for grpc_cpp_plugin in that directory
PLUGIN_PATH=$(find "$PACKAGE_DIR" -type f -name grpc_cpp_plugin -executable | head -n1)
echo "Found grpc_cpp_plugin at: $PLUGIN_PATH"

# Create or clean the output directory
if [ -d "$OUTDIR" ]; then
    rm -rf "$OUTDIR"/*
else
    mkdir -p "$OUTDIR"
fi

BIN_NAME="$(basename "$DIR")"
OUTDIR="$OUTDIR/$BIN_NAME"
mkdir -p "$OUTDIR"

# Generate C++ gRPC stubs
protoc -I "$DIR/../protos" \
    --cpp_out="$OUTDIR" \
    --grpc_out="$OUTDIR" \
    --plugin=protoc-gen-grpc="$PLUGIN_PATH" \
    "$DIR/../protos/greet.proto"

echo "Generated C++ gRPC stubs in $OUTDIR"
