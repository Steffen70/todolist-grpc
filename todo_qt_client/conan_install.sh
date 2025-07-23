#!/usr/bin/env bash
set -e

PRESETS_FILE="CMakePresets.json"

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

    # Handle variable substitution in binaryDir (${sourceDir} -> current directory)
    local SOURCE_DIR
    SOURCE_DIR="$(pwd)"
    BUILD_DIR="${BUILD_DIR//\$\{sourceDir\}/$SOURCE_DIR}"

    echo "Found preset: $PRESET_NAME"
    echo "BUILD_DIR: $BUILD_DIR"
    echo "BUILD_TYPE: $BUILD_TYPE"

    # Clean up any existing build directory
    rm -rf "$BUILD_DIR"

    # Install Conan dependencies and generate toolchain
    echo "Installing Conan dependencies..."
    conan install . --output-folder="$BUILD_DIR" --build=missing -s build_type="$BUILD_TYPE" --lockfile-out="conan.lock"

    # Use the CMake preset for configuration
    echo "Configuring with CMake preset..."
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
