#!/usr/bin/env bash
set -e

DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
OUTDIR="$DIR/generated"

echo "Using package directory: $PACKAGE_DIR"

# TODO: Get recepie id from conan.lock

# TODO: Get first package id from `conan list conan list $RECEPIE_ID:*`

# TODO: Run find_conan_package.py and provide the recepie id and package id - the last print line will be the $PACKAGE_DIR

# Search for grpc_cpp_plugin in that directory
PLUGIN_PATH=$(find "$PACKAGE_DIR" -type f -name grpc_cpp_plugin -executable | head -n1)

if [ -z "$PLUGIN_PATH" ]; then
  echo "Error: grpc_cpp_plugin not found in $BUILD_DIR"
  exit 1
fi

echo "Found grpc_cpp_plugin at: $PLUGIN_PATH"

# Create or clean the output directory
if [ -d "$OUTDIR" ]; then
    rm -rf "$OUTDIR"/*
else
    mkdir -p "$OUTDIR"
fi

# Generate C++ gRPC stubs
protoc -I proto \
    --cpp_out=generated \
    --grpc_out=generated \
    --plugin=protoc-gen-grpc="$PLUGIN_PATH" \
    $DIR/../protos/greet.proto
