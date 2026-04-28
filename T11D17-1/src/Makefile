all:  door_struct stack_test list_test 

door_struct: dmanager_module.c door_struct.h
	gcc dmanager_module.c -o ../build/Quest_1
list_test: list_test.c
	gcc list.c list_test.c -o ../build/Quest_2

stack_test: stack_test.c stack.c
	gcc -c stack_test.c stack.c
	gcc stack_test.o stack.o -o ../build/Quest_3


clean:
	rm -rf *.o 
	rm ../build/*

rebuild:
	clean all