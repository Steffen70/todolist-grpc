#!/usr/bin/env bash
set -e

# Get the directory where the script resides
DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
OUTDIR="$DIR/node_modules/grpc-stubs"

export PATH="$DIR/node_modules/.bin:$PATH"

# Create or clean the output directory
if [ -d "$OUTDIR" ]; then
    rm -rf "$OUTDIR"/*
else
    mkdir -p "$OUTDIR"
fi

protoc -I="$DIR/../protos" \
  --js_out=import_style=commonjs:"$OUTDIR" \
  --grpc-web_out=import_style=commonjs,mode=grpcwebtext:"$OUTDIR" \
  "$DIR/../protos/todo.proto"

cat > "$OUTDIR/package.json" << EOF
{
  "name": "grpc-stubs"
}
EOF
