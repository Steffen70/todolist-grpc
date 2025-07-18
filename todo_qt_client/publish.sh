#!/usr/bin/env bash
set -e

PRESET_NAME="${1:-build-release}"

# Capture build dir output from conan_install.sh
BUILD_DIR=$(./conan_install.sh "$PRESET_NAME" | tail -n 1)

cmake --build "$BUILD_DIR"

PUBLISH_DIR="publish"
BIN_NAME="$(basename "$PWD")"

rm -rf "$PUBLISH_DIR"
mkdir -p "$PUBLISH_DIR"

# Copy only the executable (add more files as needed)
cp "$BUILD_DIR/$BIN_NAME" "$PUBLISH_DIR/"

echo "Published $BIN_NAME to $PUBLISH_DIR/"
