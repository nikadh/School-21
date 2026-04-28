#include <stdio.h>
#ifndef DOOR_STRUCT_H
#define DOOR_STRUCT_H

struct door {
    int id;
    int status;
};

void initialize_doors(struct door *doors);
void output(struct door *doors, int n);
void sort(struct door *a, int size);
#endif