# Minimum CMake version and project definition
cmake_minimum_required(VERSION 3.22)

# Resolve the directory name.
get_filename_component(PROJECT_DIR_NAME "${CMAKE_CURRENT_SOURCE_DIR}" NAME)

project(${PROJECT_DIR_NAME} LANGUAGES CXX)

# Set C++ standard explicitly
set(CMAKE_CXX_STANDARD 20)
set(CMAKE_CXX_STANDARD_REQUIRED ON)
set(CMAKE_CXX_EXTENSIONS OFF)

file(GLOB_RECURSE SRC_FILES
    CONFIGURE_DEPENDS
    src/*.cpp
    generated/**/*.pb.cc
)

add_executable(${PROJECT_NAME} ${SRC_FILES})

target_include_directories(${PROJECT_NAME}
    PUBLIC
    $<BUILD_INTERFACE:${PROJECT_SOURCE_DIR}/include>
    $<BUILD_INTERFACE:${PROJECT_SOURCE_DIR}/generated>
    $<INSTALL_INTERFACE:include>
)

find_package(gRPC REQUIRED CONFIG)
find_package(fmt REQUIRED CONFIG)

target_link_libraries(${PROJECT_NAME} PRIVATE gRPC::grpc++ fmt::fmt)
