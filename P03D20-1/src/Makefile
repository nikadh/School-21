CC=gcc
CFLAGS=-c -Wall -Wextra -Werror -std=c11
# CFLAGS=-c -std=c11
BUILD_PATH=../build

all: graph
	rm -rf *.o

graph: clean graph.o prepare.o io.o polish.o parser.o operations.o calc.o
	$(CC) *.o -o $(BUILD_PATH)/graph

graph.o:
	$(CC) $(CFLAGS) ./graph.c

prepare.o:
	$(CC) $(CFLAGS) ./helpers/prepare.c

io.o:
	$(CC) $(CFLAGS) ./helpers/io.c

polish.o:
	$(CC) $(CFLAGS) ./polish/polish.c

parser.o:
	$(CC) $(CFLAGS) ./polish/parser.c

operations.o:
	$(CC) $(CFLAGS) ./polish/operations.c

calc.o:
	$(CC) $(CFLAGS) ./polish/calc.c

clean:
	rm -rf *.o
