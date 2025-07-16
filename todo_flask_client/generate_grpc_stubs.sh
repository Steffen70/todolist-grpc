#!/usr/bin/env bash
set -e

DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
OUTDIR="$DIR/generated"

# Create or clean the output directory
if [ -d "$OUTDIR" ]; then
    rm -rf "$OUTDIR"/*
else
    mkdir -p "$OUTDIR"
fi

# Generate Python gRPC stubs
pipenv run python -m grpc_tools.protoc \
  -I "$DIR/../protos" \
  --python_out="$OUTDIR" \
  --grpc_python_out="$OUTDIR" \
  "$DIR/../protos/greet.proto"

# Fix imports using protoletariat
pipenv run protol \
  --create-package \
  --in-place \
  --python-out "$OUTDIR" \
  protoc --proto-path="$DIR/../protos" "$DIR/../protos/greet.proto"
