all: build

build: main_executable_module.o data_io.o data_stat.o data_process.o decision.o sort.o
	gcc -Wall -Wextra -Werror main_executable_module.o data_io.o data_stat.o data_process.o decision.o sort.o -o ../../build/Quest_5

main_executable_module.o:
	gcc -c -Wall -Wextra -Werror ../main_executable_module/*.c

data_io.o:
	gcc -c -Wall -Wextra -Werror ../data_libs/data_io.c

sort.o:
	gcc -c -Wall -Wextra -Werror ../main_executable_module/sort.c

data_stat.o:
	gcc -c -Wall -Wextra -Werror ../data_libs/data_stat.c

data_process.o:
	gcc -c -Wall -Wextra -Werror ../data_module/data_process.c

decision.o:
	gcc -c -Wall -Wextra -Werror ../yet_another_decision_module/decision.c

libdata_stat.a: ../data_libs/data_stat.o
	ar rc ../main_executable_module/data_stat.a ../data_libs/data_stat.o
 
build_with_static: main_executable_module.o data_io.o libdata_stat.a data_process.o decision.o sort.o 
	gcc -Wall -Wextra -Werror main_executable_module.c  ../main_executable_module/data_stat.a ../data_libs/data_io.c ../data_module/data_process.c ../yet_another_decision_module/decision.c sort.c  -o ../../build/Quest_3

rebuild: clean build

clean:
	rm -rf *.o *.a build