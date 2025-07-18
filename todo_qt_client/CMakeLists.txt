# Minimum CMake version and project definition
cmake_minimum_required(VERSION 3.22)

# Resolve the directory name.
get_filename_component(PROJECT_DIR_NAME "${CMAKE_CURRENT_SOURCE_DIR}" NAME)

project(${PROJECT_DIR_NAME} LANGUAGES CXX)

# Set C++ standard explicitly
set(CMAKE_CXX_STANDARD 20)
set(CMAKE_CXX_STANDARD_REQUIRED ON)
set(CMAKE_CXX_EXTENSIONS OFF)

add_executable(${PROJECT_DIR_NAME} main.cpp)

find_package(fmt REQUIRED CONFIG)
target_link_libraries(${PROJECT_DIR_NAME} PRIVATE fmt::fmt)
