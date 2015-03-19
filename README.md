# ESP8266_memory_analyzer
A .net tool for analyzing memory consumption for compiled ELF file for ESP8266

## Usage
memanalyzer <path_to_objdump> <path_to_app_out>

## Sample output
```
   Section|                   Description| Start (hex)|   End (hex)|Used space
------------------------------------------------------------------------------
      data|        Initialized Data (RAM)|    3FFE8000|    3FFE89F0|    2544
    rodata|           ReadOnly Data (RAM)|    3FFE89F0|    3FFE8E44|    1108
       bss|      Uninitialized Data (RAM)|    3FFE8E48|    3FFF0A28|   31712
      text|            Cached Code (IRAM)|    40100000|    401062CE|   25294
irom0_text|           Uncached Code (SPI)|    40240000|    402633EC|  144364
Total Used RAM : 35364
Free RAM : 46556
Free IRam : 7492
```
