#!/usr/bin/env bash
set -e

DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
PRESETS_FILE="$DIR/CMakePresets.json"

# Function to install/build one preset
run_for_preset() {
    local PRESET_NAME="$1"

    # Extract preset information using jq
    local preset_data
    preset_data=$(jq -r --arg preset_name "$PRESET_NAME" '
        .configurePresets[] |
        select(.name == $preset_name) |
        {
            binaryDir: .binaryDir,
            buildType: .cacheVariables.CMAKE_BUILD_TYPE
        }
    ' "$PRESETS_FILE")

    # Check if preset was found
    if [ -z "$preset_data" ] || [ "$preset_data" = "null" ]; then
        echo "Error: Preset '$PRESET_NAME' not found in $PRESETS_FILE"
        echo "Available presets:"
        jq -r '.configurePresets[].name' "$PRESETS_FILE" | sed 's/^/  - /'
        exit 1
    fi

    # Extract individual values
    local BUILD_DIR
    BUILD_DIR=$(echo "$preset_data" | jq -r '.binaryDir')
    local BUILD_TYPE
    BUILD_TYPE=$(echo "$preset_data" | jq -r '.buildType')

    # Handle variable substitution in binaryDir (${sourceDir} -> script directory)
    BUILD_DIR="${BUILD_DIR//\$\{sourceDir\}/$DIR}"

    echo "Found preset: $PRESET_NAME"
    echo "BUILD_DIR: $BUILD_DIR"

    # Clean up any existing build directory
    if [ -d "$BUILD_DIR" ]; then
        rm -rf "$BUILD_DIR"/*
    else
        mkdir -p "$BUILD_DIR"
    fi


    # Install Conan dependencies and generate toolchain
    echo "Installing Conan dependencies..."
    conan install . -c tools.system.package_manager:mode=install -c tools.system.package_manager:sudo=True --output-folder="$BUILD_DIR" --build=missing --lockfile-out="conan.lock" -s build_type="$BUILD_TYPE"
    rm "$DIR/CMakeUserPresets.json"

    # Use the CMake preset for configuration
    echo "Configuring with CMake preset $PRESET_NAME ..."
    cmake --preset "$PRESET_NAME"

    # Output the build directory for potential use by other scripts
    echo "$BUILD_DIR"
}

# Main logic
if [ $# -eq 0 ]; then
    # If no argument: loop through all presets defined in CMakePresets.json
    jq -r '.configurePresets[].name' "$PRESETS_FILE" | while read -r preset; do
        run_for_preset "$preset"
    done
else
    # If a preset name is provided, run only that one
    run_for_preset "$1"
fi
