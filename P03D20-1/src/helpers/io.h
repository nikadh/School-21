#ifndef IO_H
#define IO_H

#include <math.h>
#include <stdio.h>

#include "../polish/calc.h"
#include "../polish/polish.h"

void input(char *str);
void init_matrix(int matrix[25][80]);
void fill_matrix(struct polish *polish, int matrix[25][80]);
void draw_matrix(int matrix[25][80]);

#endif
