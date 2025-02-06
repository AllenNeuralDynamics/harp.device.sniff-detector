cmake_minimum_required(VERSION 3.13)
find_package(Git REQUIRED)
execute_process(COMMAND "${GIT_EXECUTABLE}" rev-parse --short HEAD OUTPUT_VARIABLE COMMIT_ID OUTPUT_STRIP_TRAILING_WHITESPACE)
message(STATUS "Computed Git Hash: ${COMMIT_ID}")
add_definitions(-DGIT_HASH="${COMMIT_ID}") # Usable in source code.

add_definitions(-DUSBD_MANUFACTURER="Allen Institute")
add_definitions(-DUSBD_PRODUCT="sniff-detector")


include(${PICO_SDK_PATH}/pico_sdk_init.cmake)

project(sniff-detector-${COMMIT_ID})

set(CMAKE_CXX_STANDARD 17)

# Compile for profiling/debugging/etc. Default: none enabled
add_definitions(-DDEBUG)
add_definitions(-DDEBUG_HARP_MSG_IN)
add_definitions(-DDEBUG_HARP_MSG_OUT)

# initialize the Raspberry Pi Pico SDK
pico_sdk_init()

# include the harp core
add_subdirectory(lib/harp.core.rp2040/firmware)
add_subdirectory(lib/rp2040.pio-ads7049)

add_executable(${PROJECT_NAME}
    src/main.cpp
)

# Where to look for header files.
include_directories(inc)

# Link libraries to the targets that need them.
#target_link_libraries(analog_load_cell pico_stdlib hardware_adc)
target_link_libraries(${PROJECT_NAME}
    pico_stdlib hardware_dma pio_ads7049 harp_core harp_sync harp_c_app)

# create map/bin/hex/uf2 file in addition to ELF.
pico_add_extra_outputs(${PROJECT_NAME})

if (DEFINED DEBUG)
    message(WARNING "Debug printf() messages enabled from harp core UART \
            with baud rate 921600.")
    pico_enable_stdio_uart(${PROJECT_NAME} 1)
    # Additional libraries need to have stdio init also.
endif()

